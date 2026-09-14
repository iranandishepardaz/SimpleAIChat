using System;
using System.Windows.Forms;
using ApcoAgentCore.Models;
using ApcoAgentCore.Services;

namespace SimpleAIChat
{
    //در این ویرایش با استفاده از هسته مدل یک پرسش و پاسخ بسیار خوب و پایدار بصورت کامل یا استریم داریم 
    //پرسشها از هم مستقل هستند  و پیوستگی ندارند
    public partial class ChatPage0 : Form
    {
        private readonly LLMService00 _llmService;
        private List<LLM> _models;
        private LLM _currentModel;

        public ChatPage0()
        {
            InitializeComponent();
            _llmService = new LLMService00();
            lblStatus.Text = "Initializing...";
            LoadModels();
            InitializeComboBox();
        }

        private void LoadModels()
        {
            try
            {
                _models = LLMService00.LoadModels(false);

                if (_models.Count == 0)
                {
                    MessageBox.Show("No active models found!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading models:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComboBox()
        {
            cmbModel.Items.Clear();

            foreach (var model in _models)
            {
                string displayName = $"{model.Provider} - {model.Name}";
                if (!string.IsNullOrEmpty(model.Remarks))
                {
                    displayName += $" ({model.Remarks})";
                }
                cmbModel.Items.Add(displayName);
            }

            if (cmbModel.Items.Count > 0)
                cmbModel.SelectedIndex = 0;

            lblStatus.Text = "Ready.";
        }

        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbModel.SelectedIndex >= 0 && cmbModel.SelectedIndex < _models.Count)
            {
                _currentModel = _models[cmbModel.SelectedIndex];

                string status = $"Model: {_currentModel.Provider} - {_currentModel.Name}";
                status += _currentModel.Type == "Local" ? " (Local)" : " (Cloud)";

                if (!string.IsNullOrEmpty(_currentModel.Remarks))
                    status += $" | {_currentModel.Remarks}";

                lblStatus.Text = status;
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                MessageBox.Show("Please enter a question.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_currentModel == null)
            {
                MessageBox.Show("Please select a model.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            txtAnswer.Text = "";
            lblStatus.Text = "Sending request...";

            try
            {
                bool useStream = chkStreamAnswer.Checked;

                string result = await _llmService.SendMessageAsync(
                    _currentModel,
                    txtQuestion.Text,
                    useStream,
                    onChunkReceived: (chunk) =>
                    {
                        // بروزرسانی UI در حین دریافت استریم
                        txtAnswer.Invoke(new Action(() =>
                        {
                            txtAnswer.Text = chunk.Replace("\n", "\r\n");
                            txtAnswer.SelectionStart = txtAnswer.Text.Length;
                            txtAnswer.ScrollToCaret();
                            lblStatus.Text = $"Receiving... ({chunk.Length} chars)";
                        }));
                    }
                );

                // اگر استریم نبود، نتیجه را یکجا نمایش بده
                if (!useStream)
                {
                    txtAnswer.Text = result.Replace("\n", "\r\n");
                    lblStatus.Text = "✅ Answer is ready.";
                }
                else
                {
                    lblStatus.Text = $"✅ Stream completed. ({result.Length} chars)";
                }
            }
            catch (Exception ex)
            {
                txtAnswer.Text = $"❌ Error:\r\n{ex.Message}";
                lblStatus.Text = $"❌ Error: {ex.Message}";
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (_currentModel != null)
            {
                string info = _llmService.GetModelInfo(_currentModel);
                MessageBox.Show(info, "Model Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void cmbHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbHistory.SelectedIndex)
            {
                case 0: new ChatPage1().ShowDialog(); break;
                case 1: new ChatPage00().ShowDialog(); break;
                case 2: new ChatPage01().ShowDialog(); break;
                case 3: new ChatPage02().ShowDialog(); break;
                case 4: new ChatPage03().ShowDialog(); break;
                case 5: new ChatPage04().ShowDialog(); break;
            }

        }
    }
}