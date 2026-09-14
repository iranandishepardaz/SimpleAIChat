namespace SimpleAIChat
{
    partial class AgentPage01
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblLLMRemarks = new Label();
            lblStatus = new Label();
            pnlConversation = new Panel();
            label1 = new Label();
            cmbModel = new ComboBox();
            chkStream = new CheckBox();
            btnNewChat = new Button();
            btnCancell = new Button();
            btnSend = new Button();
            txtQuestion = new TextBox();
            cmbHistory = new ComboBox();
            btnSaveLog = new Button();
            btnCopyLog = new Button();
            btnClearDebug = new Button();
            txtDebug = new RichTextBox();
            chkDebug = new CheckBox();
            pnlDebug = new Panel();
            btnSettings = new Button();
            lblPreset = new Label();
            cmbPreset = new ComboBox();
            btnInfo = new Button();
            chkVerbose = new CheckBox();
            pnlDebug.SuspendLayout();
            SuspendLayout();
            // 
            // lblLLMRemarks
            // 
            lblLLMRemarks.AutoSize = true;
            lblLLMRemarks.Font = new Font("Vazir Medium", 9F, FontStyle.Italic);
            lblLLMRemarks.ForeColor = SystemColors.ControlDark;
            lblLLMRemarks.Location = new Point(331, 34);
            lblLLMRemarks.Name = "lblLLMRemarks";
            lblLLMRemarks.Size = new Size(56, 19);
            lblLLMRemarks.TabIndex = 42;
            lblLLMRemarks.Text = "Status...";
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Vazir Medium", 9F, FontStyle.Italic);
            lblStatus.ForeColor = SystemColors.ControlDark;
            lblStatus.Location = new Point(13, 535);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(56, 19);
            lblStatus.TabIndex = 43;
            lblStatus.Text = "Status...";
            lblStatus.DoubleClick += lblStatus_DoubleClick;
            // 
            // pnlConversation
            // 
            pnlConversation.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlConversation.AutoScroll = true;
            pnlConversation.BackColor = SystemColors.ActiveCaption;
            pnlConversation.Location = new Point(11, 62);
            pnlConversation.Name = "pnlConversation";
            pnlConversation.Size = new Size(783, 463);
            pnlConversation.TabIndex = 41;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 36);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 40;
            label1.Text = "Model";
            // 
            // cmbModel
            // 
            cmbModel.FormattingEnabled = true;
            cmbModel.Location = new Point(61, 32);
            cmbModel.Margin = new Padding(3, 4, 3, 4);
            cmbModel.Name = "cmbModel";
            cmbModel.Size = new Size(264, 23);
            cmbModel.TabIndex = 38;
            cmbModel.SelectedIndexChanged += cmbModel_SelectedIndexChanged;
            // 
            // chkStream
            // 
            chkStream.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            chkStream.AutoSize = true;
            chkStream.Checked = true;
            chkStream.CheckState = CheckState.Checked;
            chkStream.Location = new Point(688, 535);
            chkStream.Margin = new Padding(3, 4, 3, 4);
            chkStream.Name = "chkStream";
            chkStream.Size = new Size(105, 19);
            chkStream.TabIndex = 39;
            chkStream.Text = "Stream Answer";
            chkStream.UseVisualStyleBackColor = true;
            // 
            // btnNewChat
            // 
            btnNewChat.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNewChat.Location = new Point(736, 596);
            btnNewChat.Margin = new Padding(3, 4, 3, 4);
            btnNewChat.Name = "btnNewChat";
            btnNewChat.Size = new Size(60, 29);
            btnNewChat.TabIndex = 37;
            btnNewChat.Text = "New";
            btnNewChat.UseVisualStyleBackColor = true;
            btnNewChat.Click += btnNewChat_Click;
            // 
            // btnCancell
            // 
            btnCancell.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancell.Location = new Point(736, 562);
            btnCancell.Margin = new Padding(3, 4, 3, 4);
            btnCancell.Name = "btnCancell";
            btnCancell.Size = new Size(60, 29);
            btnCancell.TabIndex = 35;
            btnCancell.Text = "Cancell";
            btnCancell.UseVisualStyleBackColor = true;
            btnCancell.Visible = false;
            btnCancell.Click += btnCancell_Click;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Location = new Point(736, 561);
            btnSend.Margin = new Padding(3, 4, 3, 4);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(60, 29);
            btnSend.TabIndex = 36;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtQuestion
            // 
            txtQuestion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtQuestion.Location = new Point(11, 561);
            txtQuestion.Margin = new Padding(3, 4, 3, 4);
            txtQuestion.Multiline = true;
            txtQuestion.Name = "txtQuestion";
            txtQuestion.RightToLeft = RightToLeft.Yes;
            txtQuestion.ScrollBars = ScrollBars.Vertical;
            txtQuestion.Size = new Size(719, 64);
            txtQuestion.TabIndex = 34;
            // 
            // cmbHistory
            // 
            cmbHistory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbHistory.FormattingEnabled = true;
            cmbHistory.Items.AddRange(new object[] { "ChatPage1", "ChatPage00", "ChatPage01", "ChatPage02", "ChatPage03", "ChatPage04" });
            cmbHistory.Location = new Point(649, 8);
            cmbHistory.Margin = new Padding(3, 4, 3, 4);
            cmbHistory.Name = "cmbHistory";
            cmbHistory.Size = new Size(145, 23);
            cmbHistory.TabIndex = 38;
            cmbHistory.SelectedIndexChanged += cmbHistory_SelectedIndexChanged;
            // 
            // btnSaveLog
            // 
            btnSaveLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSaveLog.Font = new Font("Segoe UI", 12F);
            btnSaveLog.Location = new Point(747, 82);
            btnSaveLog.Name = "btnSaveLog";
            btnSaveLog.Size = new Size(33, 31);
            btnSaveLog.TabIndex = 44;
            btnSaveLog.TabStop = false;
            btnSaveLog.Text = "💾";
            btnSaveLog.UseVisualStyleBackColor = true;
            btnSaveLog.Click += btnSaveLog_Click;
            // 
            // btnCopyLog
            // 
            btnCopyLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCopyLog.Font = new Font("Segoe UI", 12F);
            btnCopyLog.Location = new Point(747, 8);
            btnCopyLog.Name = "btnCopyLog";
            btnCopyLog.Size = new Size(33, 31);
            btnCopyLog.TabIndex = 45;
            btnCopyLog.TabStop = false;
            btnCopyLog.Text = "⧉";
            btnCopyLog.UseVisualStyleBackColor = true;
            btnCopyLog.Click += btnCopyLog_Click;
            // 
            // btnClearDebug
            // 
            btnClearDebug.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClearDebug.Font = new Font("Segoe UI", 12F);
            btnClearDebug.Location = new Point(747, 45);
            btnClearDebug.Name = "btnClearDebug";
            btnClearDebug.Size = new Size(33, 31);
            btnClearDebug.TabIndex = 46;
            btnClearDebug.TabStop = false;
            btnClearDebug.Text = "🗑";
            btnClearDebug.UseVisualStyleBackColor = true;
            btnClearDebug.Click += btnClearDebug_Click;
            // 
            // txtDebug
            // 
            txtDebug.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDebug.Location = new Point(5, 1);
            txtDebug.Name = "txtDebug";
            txtDebug.Size = new Size(738, 156);
            txtDebug.TabIndex = 47;
            txtDebug.TabStop = false;
            txtDebug.Text = "";
            // 
            // chkDebug
            // 
            chkDebug.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            chkDebug.AutoSize = true;
            chkDebug.Location = new Point(593, 535);
            chkDebug.Name = "chkDebug";
            chkDebug.Size = new Size(61, 19);
            chkDebug.TabIndex = 48;
            chkDebug.TabStop = false;
            chkDebug.Text = "Debug";
            chkDebug.UseVisualStyleBackColor = true;
            chkDebug.CheckedChanged += chkDebug_CheckedChanged;
            // 
            // pnlDebug
            // 
            pnlDebug.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlDebug.Controls.Add(btnSaveLog);
            pnlDebug.Controls.Add(btnCopyLog);
            pnlDebug.Controls.Add(btnClearDebug);
            pnlDebug.Controls.Add(txtDebug);
            pnlDebug.Location = new Point(8, 361);
            pnlDebug.Name = "pnlDebug";
            pnlDebug.Size = new Size(788, 167);
            pnlDebug.TabIndex = 49;
            pnlDebug.Visible = false;
            // 
            // btnSettings
            // 
            btnSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSettings.Font = new Font("Segoe UI", 12F);
            btnSettings.Location = new Point(610, 2);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(33, 31);
            btnSettings.TabIndex = 50;
            btnSettings.TabStop = false;
            btnSettings.Text = "💢";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // lblPreset
            // 
            lblPreset.AutoSize = true;
            lblPreset.Location = new Point(13, 9);
            lblPreset.Name = "lblPreset";
            lblPreset.Size = new Size(39, 15);
            lblPreset.TabIndex = 40;
            lblPreset.Text = "Preset";
            // 
            // cmbPreset
            // 
            cmbPreset.FormattingEnabled = true;
            cmbPreset.Location = new Point(61, 5);
            cmbPreset.Margin = new Padding(3, 4, 3, 4);
            cmbPreset.Name = "cmbPreset";
            cmbPreset.Size = new Size(228, 23);
            cmbPreset.TabIndex = 38;
            cmbPreset.SelectedIndexChanged += cmbPreset_SelectedIndexChanged;
            // 
            // btnInfo
            // 
            btnInfo.Font = new Font("Segoe UI", 11F);
            btnInfo.Location = new Point(303, 1);
            btnInfo.Margin = new Padding(0);
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new Size(22, 28);
            btnInfo.TabIndex = 51;
            btnInfo.TabStop = false;
            btnInfo.Text = "?";
            btnInfo.TextAlign = ContentAlignment.TopCenter;
            btnInfo.UseVisualStyleBackColor = true;
            btnInfo.Click += btnInfo_Click;
            // 
            // chkVerbose
            // 
            chkVerbose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            chkVerbose.AutoSize = true;
            chkVerbose.Location = new Point(481, 535);
            chkVerbose.Name = "chkVerbose";
            chkVerbose.Size = new Size(67, 19);
            chkVerbose.TabIndex = 48;
            chkVerbose.TabStop = false;
            chkVerbose.Text = "Verbose";
            chkVerbose.UseVisualStyleBackColor = true;
            // 
            // AgentPage01
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(802, 638);
            Controls.Add(btnInfo);
            Controls.Add(btnSettings);
            Controls.Add(pnlDebug);
            Controls.Add(chkVerbose);
            Controls.Add(chkDebug);
            Controls.Add(lblLLMRemarks);
            Controls.Add(lblStatus);
            Controls.Add(pnlConversation);
            Controls.Add(lblPreset);
            Controls.Add(label1);
            Controls.Add(cmbHistory);
            Controls.Add(cmbPreset);
            Controls.Add(cmbModel);
            Controls.Add(chkStream);
            Controls.Add(btnNewChat);
            Controls.Add(btnCancell);
            Controls.Add(btnSend);
            Controls.Add(txtQuestion);
            Name = "AgentPage01";
            Text = "Agent Page00";
            pnlDebug.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLLMRemarks;
        private Label lblStatus;
        private Panel pnlConversation;
        private Label label1;
        private ComboBox cmbModel;
        private CheckBox chkStream;
        private Button btnNewChat;
        private Button btnCancell;
        private Button btnSend;
        private TextBox txtQuestion;
        private ComboBox cmbHistory;
        private Button btnSaveLog;
        private Button btnCopyLog;
        private Button btnClearDebug;
        private RichTextBox txtDebug;
        private CheckBox chkDebug;
        private Panel pnlDebug;
        private Button btnSettings;
        private Label lblPreset;
        private ComboBox cmbPreset;
        private Button btnInfo;
        private CheckBox chkVerbose;
    }
}