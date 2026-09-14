using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ApcoAgentCore.Models;
using ApcoAgentCore.Services;

namespace SimpleAIChat
{
    public partial class SettingsForm : Form
    {
        private List<LLM> _models = new List<LLM>();
        private List<Preset> _presets = new List<Preset>();

        private bool _loadingModel;
        private bool _loadingPreset;

        public SettingsForm()
        {
            InitializeComponent();
            LoadAll();
        }

        private void LoadAll()
        {
            _models = LLMService.LoadModels(LoadAll: true);
            _presets = PresetService.LoadPresets();

            RefreshModelList();
            RefreshPresetList();
        }

        // =====================================================
        // MODELS
        // =====================================================

        private void RefreshModelList()
        {
            int selected = lstModels.SelectedIndex;

            lstModels.Items.Clear();
            foreach (LLM m in _models)
                lstModels.Items.Add($"{m.Provider} - {m.Name}");

            if (_models.Count == 0)
            {
                ClearModelEditor();
                return;
            }

            if (selected < 0 || selected >= _models.Count)
                selected = 0;

            lstModels.SelectedIndex = selected;
        }

        private void lstModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingModel) return;
            if (lstModels.SelectedIndex < 0 || lstModels.SelectedIndex >= _models.Count)
            {
                ClearModelEditor();
                return;
            }

            LoadModelToEditor(_models[lstModels.SelectedIndex]);
        }

        private void LoadModelToEditor(LLM model)
        {
            _loadingModel = true;
            try
            {
                txtModelId.Text = model.Id;
                txtProvider.Text = model.Provider;
                txtModelName.Text = model.Name;
                txtModelType.Text = model.Type;
                txtEndpoint.Text = model.Endpoint;
                txtApiKeyVariable.Text = model.ApiKeyEnvironmentVariable;

                numTimeout.Value = Clamp(model.TimeoutSeconds, numTimeout);
                numTemperature.Value = Clamp((decimal)model.Temperature, numTemperature);
                numMaxTokens.Value = Clamp(model.MaxTokens, numMaxTokens);
                numContextWindow.Value = Clamp(model.ContextWindowTokens, numContextWindow);

                chkSuppotTools.Checked = model.SupportsTools;
                chkModelEnabled.Checked = model.Enabled;
                chkUseProxy.Checked = model.UseProxy;

                txtProxyHost.Text = model.ProxyHost;
                numProxyPort.Value = Clamp(model.ProxyPort, numProxyPort);

                txtRemarks.Text = model.Remarks;

                UpdateProxyControls();
            }
            finally
            {
                _loadingModel = false;
            }
        }

        private void SaveCurrentModel()
        {
            if (_loadingModel) return;
            if (lstModels.SelectedIndex < 0 || lstModels.SelectedIndex >= _models.Count)
                return;

            LLM model = _models[lstModels.SelectedIndex];
            model.Id=txtModelId.Text;   
            model.Provider = txtProvider.Text.Trim();
            model.Name = txtModelName.Text.Trim();
            model.Type = txtModelType.Text.Trim();
            model.Endpoint = txtEndpoint.Text.Trim();
            model.ApiKeyEnvironmentVariable = txtApiKeyVariable.Text.Trim();

            model.TimeoutSeconds = (int)numTimeout.Value;
            model.Temperature = (double)numTemperature.Value;
            model.MaxTokens = (int)numMaxTokens.Value;
            model.ContextWindowTokens = (int)numContextWindow.Value;
            model.SupportsTools =chkSuppotTools.Checked;

            model.Enabled = chkModelEnabled.Checked;
            model.UseProxy = chkUseProxy.Checked;
            model.ProxyHost = txtProxyHost.Text.Trim();
            model.ProxyPort = (int)numProxyPort.Value;
            model.Remarks = txtRemarks.Text.Trim();

            lstModels.Items[lstModels.SelectedIndex] = $"{model.Provider} - {model.Name}";
        }

        private void btnAddModel_Click(object sender, EventArgs e)
        {
            SaveCurrentModel();

            LLM model = new LLM
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "New Model",
                Provider = "Provider",
                Type = "Cloud",
                Endpoint = "",
                ApiKeyEnvironmentVariable = "",
                TimeoutSeconds = 120,
                Temperature = 0,
                MaxTokens = 500,
                ContextWindowTokens = 8192,
                Enabled = true,
                UseProxy = false,
                ProxyHost = "127.0.0.1",
                ProxyPort = 1088,
                Remarks = ""
            };

            _models.Add(model);
            RefreshModelList();
            lstModels.SelectedIndex = _models.Count - 1;

            txtModelName.Focus();
            txtModelName.SelectAll();
        }

        private void btnDeleteModel_Click(object sender, EventArgs e)
        {
            if (lstModels.SelectedIndex < 0) return;

            DialogResult result = MessageBox.Show(
                this,
                "Delete the selected model?",
                "Delete Model",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            _models.RemoveAt(lstModels.SelectedIndex);
            RefreshModelList();
        }

        private void btnModelUp_Click(object sender, EventArgs e)
        {
            int i = lstModels.SelectedIndex;
            if (i <= 0) return;

            SaveCurrentModel();

            // جابجایی در لیست داده
            LLM tmp = _models[i];
            _models[i] = _models[i - 1];
            _models[i - 1] = tmp;

            // بازسازی لیست نمایش
            _loadingModel = true;
            try
            {
                RefreshModelList();
                lstModels.SelectedIndex = i - 1;
            }
            finally
            {
                _loadingModel = false;
            }
        }

        private void btnModelDown_Click(object sender, EventArgs e)
        {
            int i = lstModels.SelectedIndex;
            if (i < 0 || i >= _models.Count - 1) return;

            SaveCurrentModel();

            LLM tmp = _models[i];
            _models[i] = _models[i + 1];
            _models[i + 1] = tmp;

            _loadingModel = true;
            try
            {
                RefreshModelList();
                lstModels.SelectedIndex = i + 1;
            }
            finally
            {
                _loadingModel = false;
            }
        }

        private void ClearModelEditor()
        {
            _loadingModel = true;
            try
            {
                txtModelId.Clear();
                txtProvider.Clear();
                txtModelName.Clear();
                txtModelType.Clear();
                txtEndpoint.Clear();
                txtApiKeyVariable.Clear();

                numTimeout.Value = Clamp(120, numTimeout);
                numTemperature.Value = Clamp(0m, numTemperature);
                numMaxTokens.Value = Clamp(500, numMaxTokens);
                numContextWindow.Value = Clamp(8192, numContextWindow);

                chkSuppotTools.Checked = false;
                chkModelEnabled.Checked = true;
                chkUseProxy.Checked = false;

                txtProxyHost.Text = "127.0.0.1";
                numProxyPort.Value = Clamp(1088, numProxyPort);

                txtRemarks.Clear();

                UpdateProxyControls();
            }
            finally
            {
                _loadingModel = false;
            }
        }

        private void chkUseProxy_CheckedChanged(object sender, EventArgs e)
        {
            UpdateProxyControls();
        }

        private void UpdateProxyControls()
        {
            bool enabled = chkUseProxy.Checked;
            txtProxyHost.Enabled = enabled;
            numProxyPort.Enabled = enabled;
            lblProxyHost.Enabled = enabled;
            lblProxyPort.Enabled = enabled;
        }

        // =====================================================
        // PRESETS
        // =====================================================

        private void RefreshPresetList()
        {
            int selected = lstPresets.SelectedIndex;

            lstPresets.Items.Clear();
            foreach (Preset p in _presets)
                lstPresets.Items.Add(p.Name);

            if (_presets.Count == 0)
            {
                ClearPresetEditor();
                return;
            }

            if (selected < 0 || selected >= _presets.Count)
                selected = 0;

            lstPresets.SelectedIndex = selected;
        }

        private void lstPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingPreset) return;
            if (lstPresets.SelectedIndex < 0 || lstPresets.SelectedIndex >= _presets.Count)
            {
                ClearPresetEditor();
                return;
            }

            LoadPresetToEditor(_presets[lstPresets.SelectedIndex]);
        }

        private void LoadPresetToEditor(Preset preset)
        {
            _loadingPreset = true;
            try
            {
                txtPresetId.Text = preset.Id;
                txtPresetName.Text = preset.Name;
                txtPresetSystemPrompt.Text = preset.SystemPrompt;
                txtPresetKnowledgeFile.Text = preset.KnowledgeFile;
                string fmt = string.IsNullOrWhiteSpace(preset.OutputFormat) ? "text" : preset.OutputFormat;
                int fmtIndex = cmbPresetOutputFormat.Items.IndexOf(fmt);
                cmbPresetOutputFormat.SelectedIndex = fmtIndex >= 0 ? fmtIndex : 0;

                SetNullableDouble(numPresetTemperature, chkPresetTemperature, preset.Temperature);
                SetNullableInt(numPresetMaxTokens, chkPresetMaxTokens, preset.MaxTokens);
                SetNullableDouble(numPresetTopP, chkPresetTopP, preset.TopP);
            }
            finally
            {
                _loadingPreset = false;
            }
        }

        private void SaveCurrentPreset()
        {
            if (_loadingPreset) return;
            if (lstPresets.SelectedIndex < 0 || lstPresets.SelectedIndex >= _presets.Count)
                return;

            Preset preset = _presets[lstPresets.SelectedIndex];

            preset.Id = txtPresetId.Text.Trim();
            preset.Name = txtPresetName.Text.Trim();
            preset.SystemPrompt = txtPresetSystemPrompt.Text;
            preset.KnowledgeFile = txtPresetKnowledgeFile.Text.Trim();

            preset.Temperature = GetNullableDouble(numPresetTemperature, chkPresetTemperature);
            preset.MaxTokens = GetNullableInt(numPresetMaxTokens, chkPresetMaxTokens);
            preset.TopP = GetNullableDouble(numPresetTopP, chkPresetTopP);
            preset.OutputFormat = cmbPresetOutputFormat.SelectedItem as string ?? "text";

            lstPresets.Items[lstPresets.SelectedIndex] = preset.Name;
        }

        private void btnAddPreset_Click(object sender, EventArgs e)
        {
            SaveCurrentPreset();

            Preset preset = new Preset
            {
                Id = "preset-" + Guid.NewGuid().ToString("N").Substring(0, 6),
                Name = "New Preset",
                SystemPrompt = "",
                KnowledgeFile = "",
                Temperature = null,
                MaxTokens = null,
                TopP = null
            };

            _presets.Add(preset);
            RefreshPresetList();
            lstPresets.SelectedIndex = _presets.Count - 1;

            txtPresetName.Focus();
            txtPresetName.SelectAll();
        }

        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            if (lstPresets.SelectedIndex < 0) return;

            DialogResult result = MessageBox.Show(
                this,
                "Delete the selected preset?",
                "Delete Preset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            _presets.RemoveAt(lstPresets.SelectedIndex);
            RefreshPresetList();
        }

        private void btnPresetUp_Click(object sender, EventArgs e)
        {
            int i = lstPresets.SelectedIndex;
            if (i <= 0) return;

            SaveCurrentPreset();

            Preset tmp = _presets[i];
            _presets[i] = _presets[i - 1];
            _presets[i - 1] = tmp;

            _loadingPreset = true;
            try
            {
                RefreshPresetList();
                lstPresets.SelectedIndex = i - 1;
            }
            finally
            {
                _loadingPreset = false;
            }
        }

        private void btnPresetDown_Click(object sender, EventArgs e)
        {
            int i = lstPresets.SelectedIndex;
            if (i < 0 || i >= _presets.Count - 1) return;

            SaveCurrentPreset();

            Preset tmp = _presets[i];
            _presets[i] = _presets[i + 1];
            _presets[i + 1] = tmp;

            _loadingPreset = true;
            try
            {
                RefreshPresetList();
                lstPresets.SelectedIndex = i + 1;
            }
            finally
            {
                _loadingPreset = false;
            }
        }

        private void ClearPresetEditor()
        {
            _loadingPreset = true;
            try
            {
                txtPresetId.Clear();
                txtPresetName.Clear();
                txtPresetSystemPrompt.Clear();
                txtPresetKnowledgeFile.Clear();

                chkPresetTemperature.Checked = false;
                numPresetTemperature.Value = Clamp(0.7m, numPresetTemperature);

                chkPresetMaxTokens.Checked = false;
                numPresetMaxTokens.Value = Clamp(500, numPresetMaxTokens);

                chkPresetTopP.Checked = false;
                numPresetTopP.Value = Clamp(1m, numPresetTopP);
            }
            finally
            {
                _loadingPreset = false;
            }
        }

        private void btnBrowseKnowledgeFile_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Knowledge files|*.md;*.txt;*.json|All files|*.*",
                Title = "Select knowledge file"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
                txtPresetKnowledgeFile.Text = dialog.FileName;
        }

        // =====================================================
        // SAVE / CANCEL
        // =====================================================

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveCurrentModel();
                SaveCurrentPreset();

                ValidateAll();

                LLMService.SaveModels(_models);
                PresetService.SavePresets(_presets);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Save Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ValidateAll()
        {
            foreach (LLM model in _models)
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    throw new InvalidOperationException("Every model must have an Id.");

                if (string.IsNullOrWhiteSpace(model.Name))
                    throw new InvalidOperationException("Every model must have a name.");

                if (string.IsNullOrWhiteSpace(model.Provider))
                    throw new InvalidOperationException($"Model '{model.Name}' must have a provider.");

                if (string.IsNullOrWhiteSpace(model.Endpoint))
                    throw new InvalidOperationException($"Model '{model.Name}' must have an endpoint.");
            }

            foreach (Preset preset in _presets)
            {
                if (string.IsNullOrWhiteSpace(preset.Id))
                    throw new InvalidOperationException("Every preset must have an Id.");

                if (string.IsNullOrWhiteSpace(preset.Name))
                    throw new InvalidOperationException("Every preset must have a name.");
            }

            // یکتایی Id
            var duplicateModelIds = _models
                .GroupBy(m => m.Id, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateModelIds.Count > 0)
                throw new InvalidOperationException(
                    "Duplicate model Id(s): " + string.Join(", ", duplicateModelIds));

            var duplicatePresetIds = _presets
                .GroupBy(p => p.Id, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicatePresetIds.Count > 0)
                throw new InvalidOperationException(
                    "Duplicate preset Id(s): " + string.Join(", ", duplicatePresetIds));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // =====================================================
        // HELPERS
        // =====================================================

        private static decimal Clamp(int value, NumericUpDown num)
        {
            return Clamp((decimal)value, num);
        }

        private static decimal Clamp(decimal value, NumericUpDown num)
        {
            if (value < num.Minimum) return num.Minimum;
            if (value > num.Maximum) return num.Maximum;
            return value;
        }

        private static void SetNullableDouble(NumericUpDown num, CheckBox chk, double? value)
        {
            if (value.HasValue)
            {
                chk.Checked = true;
                num.Value = Clamp((decimal)value.Value, num);
            }
            else
            {
                chk.Checked = false;
                num.Value = Clamp(num.Minimum, num);
            }
        }

        private static void SetNullableInt(NumericUpDown num, CheckBox chk, int? value)
        {
            if (value.HasValue)
            {
                chk.Checked = true;
                num.Value = Clamp(value.Value, num);
            }
            else
            {
                chk.Checked = false;
                num.Value = Clamp((int)num.Minimum, num);
            }
        }

        private static double? GetNullableDouble(NumericUpDown num, CheckBox chk)
        {
            return chk.Checked ? (double)num.Value : (double?)null;
        }

        private static int? GetNullableInt(NumericUpDown num, CheckBox chk)
        {
            return chk.Checked ? (int)num.Value : (int?)null;
        }
    }
}