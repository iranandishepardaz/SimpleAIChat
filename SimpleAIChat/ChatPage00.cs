using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleAIChat
{
    public partial class ChatPage00 : Form
    {
        // ---------------------------------------------------------
        // این HttpClient برای ارسال درخواست اینترنتی استفاده می‌شود.
        //
        // HttpClient یک کلاس آماده در .NET است که به ما اجازه می‌دهد
        // با سرورها از طریق HTTP ارتباط برقرار کنیم.
        //
        // فعلاً فقط یک نمونه از آن در کل فرم داریم.
        // ---------------------------------------------------------
        private HttpClient _httpClient = new HttpClient();


        // ---------------------------------------------------------
        // سازنده فرم
        //
        // این متد زمانی اجرا می‌شود که فرم ساخته می‌شود.
        // ---------------------------------------------------------
        public ChatPage00()
        {
            InitializeComponent();
            InitAll();
        }

        private void InitAll()
        {
            cmbModel.Items.Clear();
            cmbModel.Items.Add("gemma3-4b:latest");
            cmbModel.Items.Add("gemma2:2b");
            cmbModel.Items.Add("gemma3:1b");
            cmbModel.Items.Add("qwen2.5-coder:3b");
            cmbModel.SelectedIndex = 0;
        }

        // ---------------------------------------------------------
        // این متد زمانی اجرا می‌شود که کاربر روی دکمه ارسال کلیک کند.
        // ---------------------------------------------------------
        private async void btnSend_Click(object sender, EventArgs e)
        {
            // سؤال را از TextBox می‌خوانیم.
            string question = txtQuestion.Text;


            // اگر کاربر چیزی وارد نکرده باشد، ادامه نمی‌دهیم.
            if (string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("لطفاً یک سؤال وارد کنید.");
                return;
            }


            // برای اینکه کاربر بداند برنامه در حال کار است،
            // فعلاً یک متن ساده در قسمت پاسخ نمایش می‌دهیم.
            txtAnswer.Text = "ارسال شد \r\n در حال دریافت پاسخ...";


            try
            {
                // سؤال را برای مدل ارسال می‌کنیم
                // و پاسخ را دریافت می‌کنیم.
                string answer = "";
                if (chkStream.Checked)
                {
                    answer = await AskOllamaStream(question);
                }
                else
                {
                    answer = await AskOllamaFull(question);
                }

                // پاسخ مدل را در TextBox نمایش می‌دهیم.
                txtAnswer.Text = answer;
            }
            catch (Exception ex)
            {
                // اگر در هر قسمت از ارتباط با مدل خطایی اتفاق بیفتد،
                // متن خطا را نمایش می‌دهیم.
                txtAnswer.Text = "خطا:\r\n" + ex.Message;
            }
        }


        // ---------------------------------------------------------
        // این متد سؤال را به Ollama ارسال می‌کند.
        //
        // فعلاً تمام کار ارتباط با مدل را همین یک متد انجام می‌دهد.
        //
        // بعداً وقتی کاملاً متوجه شدیم چه اتفاقی می‌افتد،
        // می‌توانیم آن را به یک کلاس جدا منتقل کنیم.
        // ---------------------------------------------------------
        // ---------------------------------------------------------
        // این متد سؤال را به Ollama ارسال می‌کند.
        //
        // تفاوت این نسخه با نسخه قبلی:
        //
        // در نسخه قبلی:
        // stream = false
        //
        // یعنی Ollama کل پاسخ را آماده می‌کرد و بعد یکجا
        // برای ما می‌فرستاد.
        //
        // در این نسخه:
        // stream = true
        //
        // یعنی Ollama پاسخ را به چند قطعه کوچک تقسیم می‌کند
        // و هر قطعه را به محض آماده شدن برای ما می‌فرستد.
        // ---------------------------------------------------------
        private async Task<string> AskOllamaStream(string question)
        {
            // آدرس API مربوط به Ollama
            string url = "http://localhost:11434/api/generate";


            // -----------------------------------------------------
            // اطلاعات درخواست
            //
            // این بار stream را true قرار داده‌ایم.
            // -----------------------------------------------------
            var requestData = new { model = cmbModel.SelectedItem, prompt = question, stream = true };


            // تبدیل اطلاعات درخواست به JSON
            string json = JsonSerializer.Serialize(requestData);


            // آماده کردن JSON برای ارسال HTTP
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");


            // -----------------------------------------------------
            // درخواست HTTP را می‌سازیم.
            //
            // از PostAsync استفاده نمی‌کنیم، چون می‌خواهیم پاسخ
            // را به صورت تدریجی دریافت کنیم.
            // -----------------------------------------------------
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Content = content;


            // -----------------------------------------------------
            // ResponseHeadersRead بسیار مهم است.
            //
            // با این حالت، به محض اینکه Headerهای پاسخ دریافت شوند
            // کنترل را پس می‌گیریم و منتظر دریافت کل پاسخ نمی‌مانیم.
            // -----------------------------------------------------
            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);


            // اگر سرور خطا داده باشد، Exception ایجاد می‌شود.
            response.EnsureSuccessStatusCode();


            // -----------------------------------------------------
            // جریان داده را از پاسخ HTTP می‌گیریم.
            //
            // Stream یعنی می‌توانیم داده را کم‌کم بخوانیم.
            // -----------------------------------------------------
            Stream stream = await response.Content.ReadAsStreamAsync();


            // -----------------------------------------------------
            // StreamReader برای خواندن متن از Stream استفاده می‌شود.
            // -----------------------------------------------------
            StreamReader reader = new StreamReader(stream);


            // -----------------------------------------------------
            // پاسخ کامل را هم در این StringBuilder نگه می‌داریم.
            //
            // دلیلش این است که پاسخ به صورت تکه‌تکه دریافت می‌شود،
            // اما در نهایت ممکن است بخواهیم کل پاسخ را داشته باشیم.
            // -----------------------------------------------------
            StringBuilder fullAnswer = new StringBuilder();


            // -----------------------------------------------------
            // پاسخ قبلی را پاک می‌کنیم.
            //
            // چون قرار است پاسخ جدید به صورت زنده داخل TextBox
            // نمایش داده شود.
            // -----------------------------------------------------
            txtAnswer.Clear();


            // -----------------------------------------------------
            // Ollama در حالت Streaming هر قطعه را در یک خط JSON
            // ارسال می‌کند.
            //
            // بنابراین هر بار یک خط را می‌خوانیم.
            // -----------------------------------------------------
            while (true)
            {
                string? line = await reader.ReadLineAsync();


                // اگر خطی باقی نمانده باشد، کار تمام شده است.
                if (line == null)
                {
                    break;
                }


                // -----------------------------------------------------
                // خط دریافت شده یک JSON است.
                //
                // مثلاً:
                //
                // {"model":"gemma2:2b","response":"سلام","done":false}
                //
                // یا:
                //
                // {"model":"gemma2:2b","response":"!","done":false}
                // -----------------------------------------------------
                using JsonDocument document = JsonDocument.Parse(line);


                // -----------------------------------------------------
                // متن تولیدشده در این قطعه را از قسمت response
                // استخراج می‌کنیم.
                // -----------------------------------------------------
                string chunk = document.RootElement.GetProperty("response").GetString() ?? "";
                chunk = chunk.Replace("\n", "\r\n");


                // قطعه دریافت‌شده را به پاسخ کامل اضافه می‌کنیم.
                fullAnswer.Append(chunk);


                // -----------------------------------------------------
                // قطعه جدید را بلافاصله در TextBox نمایش می‌دهیم.
                //
                // بنابراین کاربر لازم نیست منتظر پایان پاسخ بماند.
                // -----------------------------------------------------
                txtAnswer.AppendText(chunk);


                // -----------------------------------------------------
                // مقدار done مشخص می‌کند که Ollama به پایان پاسخ
                // رسیده است یا هنوز ادامه دارد.
                // -----------------------------------------------------
                bool done = document.RootElement.GetProperty("done").GetBoolean();


                if (done)
                {
                    break;
                }
            }


            // کل پاسخ را برمی‌گردانیم.
            return fullAnswer.ToString();
        }


        private async Task<string> AskOllamaFull(string question)
        {
            // آدرس API مربوط به Ollama
            //
            // Ollama به صورت پیش‌فرض روی پورت 11434 اجرا می‌شود.
            string url = "http://localhost:11434/api/generate";


            // -----------------------------------------------------
            // اطلاعاتی که باید برای Ollama ارسال کنیم.
            //
            // model:
            // نام مدلی که می‌خواهیم از آن استفاده کنیم.
            //
            // prompt:
            // سؤال کاربر.
            //
            // stream:
            // فعلاً false قرار می‌دهیم تا کل پاسخ یکجا برگردد.
            // بعداً Streaming را جداگانه یاد می‌گیریم.
            // -----------------------------------------------------
            var requestData = new { model = cmbModel.SelectedItem, prompt = question, stream = false };


            // -----------------------------------------------------
            // شیء بالا را به JSON تبدیل می‌کنیم.
            //
            // مثلاً:
            //
            // {
            //     "model": "gemma2:2b",
            //     "prompt": "سلام",
            //     "stream": false
            // }
            // -----------------------------------------------------
            string json = JsonSerializer.Serialize(requestData);


            // -----------------------------------------------------
            // JSON را برای ارسال HTTP آماده می‌کنیم.
            //
            // UTF8 یعنی متن را با کدگذاری UTF-8 ارسال می‌کنیم.
            // این موضوع برای زبان فارسی اهمیت دارد.
            // -----------------------------------------------------
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");


            // -----------------------------------------------------
            // درخواست POST را به Ollama ارسال می‌کنیم.
            //
            // await یعنی منتظر می‌مانیم تا پاسخ برسد،
            // بدون اینکه رابط کاربری برنامه قفل شود.
            // -----------------------------------------------------
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);


            // -----------------------------------------------------
            // اگر سرور پاسخ خطا داده باشد،
            // این دستور یک Exception ایجاد می‌کند.
            //
            // مثلاً اگر Ollama اجرا نباشد،
            // یا مدل وجود نداشته باشد، این قسمت می‌تواند خطا بدهد.
            // -----------------------------------------------------
            response.EnsureSuccessStatusCode();


            // -----------------------------------------------------
            // متن پاسخ سرور را می‌خوانیم.
            //
            // هنوز JSON را از آن استخراج نکرده‌ایم.
            // فعلاً فقط متن خام پاسخ را داریم.
            // -----------------------------------------------------
            string responseJson = await response.Content.ReadAsStringAsync();


            // ---------------------------------------------------------
            // JSON را Parse می‌کنیم.
            //
            // Parse یعنی متن JSON را به ساختاری تبدیل می‌کنیم
            // که بتوانیم اطلاعات داخل آن را جداگانه بخوانیم.
            // ---------------------------------------------------------
            using JsonDocument document = JsonDocument.Parse(responseJson);


            // ---------------------------------------------------------
            // از داخل JSON فقط مقدار "response" را می‌خوانیم.
            //
            // این همان متنی است که مدل تولید کرده است.
            // ---------------------------------------------------------
            string answer = document.RootElement.GetProperty("response").GetString().Replace("\n", "\r\n");


            // پاسخ مدل را برمی‌گردانیم.
            return answer;
            /*   // پاسخ JSON را در خروجی برنامه نمایش می‌دهیم تا ببینیم
               // سرور دقیقاً چه اطلاعاتی برای ما فرستاده است.
               return responseJson;*/
        }
    }
}