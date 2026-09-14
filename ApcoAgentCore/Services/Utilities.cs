using ApcoAgentCore.Models;
using System.Text;
using System.Text.Json;

public static class Utilities
{
    public static int LastTrimmedCount { get; private set; } = 0;
    public static string BuildDump(
    LLM model,
    Preset preset,
    List<ChatMessage> history,
    List<ChatMessage> trimmed,
    string lastAnswer)
    {

        StringBuilder sb = new StringBuilder();

        // ---- هدر: مشخصات مدل و پریست ----
        sb.AppendLine("===== CONVERSATION DUMP =====");
        sb.AppendLine($"Model: {model?.Id}");
        sb.AppendLine($"Provider: {model?.Provider}");
        sb.AppendLine($"ContextWindowTokens: {model?.ContextWindowTokens}");
        sb.AppendLine($"Model.MaxTokens: {model?.MaxTokens}");
        sb.AppendLine($"Preset: {preset?.Name}");
        sb.AppendLine($"Preset.MaxTokens: {preset?.MaxTokens?.ToString() ?? "(null)"}");
        sb.AppendLine($"Preset.Temperature: {preset?.Temperature?.ToString() ?? "(null)"}");
        sb.AppendLine($"Total messages in history: {history.Count}");
        sb.AppendLine($"displayAnswer: {lastAnswer}");
        sb.AppendLine();

        // ---- محاسبه‌ی بودجه مثل TrimHistory ----
        int contextWindow = (model?.ContextWindowTokens ?? 0) > 0
            ? model.ContextWindowTokens
            : 4000;
        int maxOutput = preset?.MaxTokens ?? model?.MaxTokens ?? 500;
        int budget = contextWindow - maxOutput - 500;
        if (budget < 1000) budget = 1000;

        sb.AppendLine($"Trim budget (approx): {budget}");
        sb.AppendLine();

        // ---- بدنه‌ی گفتگو ----
        // هر پیام با ایندکس + نقش + تخمین توکن + متن.
        // متن بریده نمی‌شود تا اگر خواستی کامل بخوانی.
        int cumulative = 0;
        for (int i = 0; i < history.Count; i++)
        {
            ChatMessage m = history[i];
            int est = EstimateTokens(m.Content);
            cumulative += est;

            sb.AppendLine($"--- [{i}] {m.Role} | est {est} tokens | cumulative {cumulative} ---");
            sb.AppendLine(m.Content);
            sb.AppendLine();
        }

        // ---- وضعیت trim ----
        sb.AppendLine($"===== AFTER TRIM ===== (kept {trimmed.Count} of {history.Count})");

        int keptTokens = 0;
        foreach (ChatMessage m in trimmed)
            keptTokens += EstimateTokens(m.Content);

        sb.AppendLine($"Estimated tokens sent to model: {keptTokens}");
        sb.AppendLine();

        sb.AppendLine("Kept messages:");
        for (int i = 0; i < trimmed.Count; i++)
        {
            ChatMessage m = trimmed[i];
            string oneLine = m.Content.Replace("\r", " ").Replace("\n", " ");
            oneLine = oneLine.Length <= 200 ? oneLine : oneLine.Substring(0, 200) + "…";
            sb.AppendLine($"[{i}] {m.Role} | est {EstimateTokens(m.Content)} | {oneLine}");
        }

        return sb.ToString();
    }

    // ---------------------------------------------------------
    // تخمین تعداد توکن (تقریبی - ۴ کاراکتر = ۱ توکن)
    // این فقط fallback است وقتی provider «usage» نمی‌دهد.
    // ---------------------------------------------------------
    public static int EstimateTokens(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        return Math.Max(1, text.Length / 4);
    }

    // ---------------------------------------------------------
    // TrimHistory: مدیریت Context Window
    //
    // هدف: اگر تاریخچه از بودجه‌ی مجاز مدل بزرگ‌تر شد،
    //       قدیمی‌ترین پیام‌ها را حذف کن تا خطا نگیریم.
    //
    // بودجه = ContextWindow − MaxOutput − Margin
    //   - ContextWindow: از مشخصات مدل (مثلاً 8192 یا 131072)
    //   - MaxOutput: حداکثر توکن خروجی (از پریست یا مدل)
    //   - Margin: حاشیه امن برای system prompt، knowledge، و سرباره‌ها
    //
    // نکته: اصل _chatHistory دست‌نخورده می‌ماند؛ فقط یک کپی trim می‌شود.
    // ---------------------------------------------------------
    public static List<ChatMessage> TrimHistory(List<ChatMessage> history, LLM model,Preset preset)
    {
        if (history == null || history.Count == 0)
            return new List<ChatMessage>();

        // ۱) Context Window مدل (اگر صفر بود، fallback محافظه‌کارانه)
        int contextWindow = model.ContextWindowTokens > 0
            ? model.ContextWindowTokens
            : 4000;

        // ۲) سقف خروجی (پریست بر مدل اولویت دارد)
        int maxOutput = preset?.MaxTokens ?? model.MaxTokens;

        // ۳) حاشیه‌ی امن
        int margin = 500;

        // ۴) بودجه‌ی نهایی
        int budget = contextWindow - maxOutput - margin;
        if (budget < 1000)
            budget = 1000;  // حداقل تا یک پیام جا شود

        // ۵) کپی از تاریخچه
        List<ChatMessage> trimmed = new List<ChatMessage>(history);

        // ۶) محاسبه‌ی مجموع تخمینی با ضریب اطمینان
        //    دلیل: تخمین Length/4 برای فارسی کمی کم‌برآورد می‌کند
        //    (تجربه‌ی عملی: Prompt واقعی حدود ۲۵٪ بیشتر از تخمین بود)
        const double SafetyFactor = 1.3;

        int rawTotal = 0;
        foreach (ChatMessage m in trimmed)
            rawTotal += EstimateTokens(m.Content);

        int total = (int)(rawTotal * SafetyFactor);

        int removed = 0;

        // ۷) حذف از قدیمی‌ترین تا زیر بودجه
        //    شرط «Count > 1» تضمین می‌کند پیام آخر کاربر هرگز حذف نشود.
        while (total > budget && trimmed.Count > 1)
        {
            total -= (int)(EstimateTokens(trimmed[0].Content) * SafetyFactor);
            trimmed.RemoveAt(0);
            removed++;
        }

        // ۸) اگر اولین پیام باقی‌مانده assistant بود، حذفش کن
        //    چون تاریخچه باید با user شروع شود.
        while (trimmed.Count > 0 && trimmed[0].Role != "user")
        {
            total -= (int)(EstimateTokens(trimmed[0].Content) * SafetyFactor);
            trimmed.RemoveAt(0);
            removed++;
        }

        // ۹) اگر چیزی حذف شد، یک یادداشت system اضافه کن
        //    تا مدل بداند تاریخچه ناقص است.
        if (removed > 0)
        {
            trimmed.Insert(0, new ChatMessage
            {
                Role = "system",
                Content = "[Note: earlier messages were trimmed to fit context window]"
            });

           // LogDebug($"[trim] removed {removed} message(s). budget={budget}, estimated total (×1.3)={total}");
        }

        return trimmed;
    }

    // ---------------------------------------------------------
    // TryParseJsonAnswer
    // ---------------------------------------------------------
    /// <summary>
    /// پاسخ مدل را به‌عنوان JSON parse می‌کند.
    /// code fenceهای ```json ... ``` را حذف می‌کند.
    /// در صورت موفقیت، دیکشنری فیلدها؛ در غیر این صورت null.
    /// </summary>
    public static Dictionary<string, string> TryParseJsonAnswer(string raw, out string error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(raw))
        {
            error = "empty response";
            return null;
        }

        string text = raw.Trim();

        // حذف code fence ابتدایی
        if (text.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            text = text.Substring(7).TrimStart();
        else if (text.StartsWith("```"))
            text = text.Substring(3).TrimStart();

        // حذف code fence انتهایی
        if (text.EndsWith("```"))
            text = text.Substring(0, text.Length - 3).TrimEnd();

        try
        {
            using JsonDocument doc = JsonDocument.Parse(text);

            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                error = "root is not a JSON object";
                return null;
            }

            var result = new Dictionary<string, string>();

            foreach (JsonProperty prop in doc.RootElement.EnumerateObject())
            {
                string value;
                if (prop.Value.ValueKind == JsonValueKind.String)
                    value = prop.Value.GetString() ?? "";
                else if (prop.Value.ValueKind == JsonValueKind.Array)
                    value = string.Join(", ", prop.Value.EnumerateArray().Select(x => x.ToString()));
                else
                    value = prop.Value.ToString();

                result[prop.Name] = value;
            }

            return result;
        }
        catch (JsonException ex)
        {
            error = ex.Message;
            return null;
        }
    }


}