namespace SimpleAIChat
{
    partial class ChatPage1
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
            btnClearDebug = new Button();
            txtDebug = new RichTextBox();
            lblStatus = new Label();
            chkDebug = new CheckBox();
            chkStreamAnswer = new CheckBox();
            label1 = new Label();
            cmbModel = new ComboBox();
            btnInfo = new Button();
            btnSend = new Button();
            txtAnswer = new TextBox();
            txtQuestion = new TextBox();
            btnCancel = new Button();
            cmbHistory = new ComboBox();
            btnCopyLog = new Button();
            btnSaveLog = new Button();
            SuspendLayout();
            // 
            // btnClearDebug
            // 
            btnClearDebug.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClearDebug.Font = new Font("Segoe UI", 12F);
            btnClearDebug.Location = new Point(836, 450);
            btnClearDebug.Name = "btnClearDebug";
            btnClearDebug.Size = new Size(33, 31);
            btnClearDebug.TabIndex = 9;
            btnClearDebug.TabStop = false;
            btnClearDebug.Text = "🗑";
            btnClearDebug.UseVisualStyleBackColor = true;
            btnClearDebug.Click += btnClearDebug_Click;
            // 
            // txtDebug
            // 
            txtDebug.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDebug.Location = new Point(13, 410);
            txtDebug.Name = "txtDebug";
            txtDebug.Size = new Size(817, 177);
            txtDebug.TabIndex = 17;
            txtDebug.TabStop = false;
            txtDebug.Text = "";
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblStatus.ForeColor = SystemColors.ControlDarkDark;
            lblStatus.Location = new Point(15, 590);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(40, 15);
            lblStatus.TabIndex = 16;
            lblStatus.Text = "Status";
            // 
            // chkDebug
            // 
            chkDebug.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkDebug.AutoSize = true;
            chkDebug.Location = new Point(552, 11);
            chkDebug.Name = "chkDebug";
            chkDebug.Size = new Size(61, 19);
            chkDebug.TabIndex = 14;
            chkDebug.TabStop = false;
            chkDebug.Text = "Debug";
            chkDebug.UseVisualStyleBackColor = true;
            chkDebug.CheckedChanged += chkDebug_CheckedChanged;
            // 
            // chkStreamAnswer
            // 
            chkStreamAnswer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkStreamAnswer.AutoSize = true;
            chkStreamAnswer.Checked = true;
            chkStreamAnswer.CheckState = CheckState.Checked;
            chkStreamAnswer.Location = new Point(646, 11);
            chkStreamAnswer.Name = "chkStreamAnswer";
            chkStreamAnswer.Size = new Size(63, 19);
            chkStreamAnswer.TabIndex = 15;
            chkStreamAnswer.TabStop = false;
            chkStreamAnswer.Text = "Stream";
            chkStreamAnswer.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 13);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 13;
            label1.Text = "Model";
            // 
            // cmbModel
            // 
            cmbModel.FormattingEnabled = true;
            cmbModel.Items.AddRange(new object[] { "gemma2:2b", "gemma3:1b", "qwen2.5-coder:3b" });
            cmbModel.Location = new Point(60, 9);
            cmbModel.Name = "cmbModel";
            cmbModel.Size = new Size(414, 23);
            cmbModel.TabIndex = 11;
            cmbModel.TabStop = false;
            cmbModel.Text = "gemma2:2b";
            cmbModel.SelectedIndexChanged += cmbModel_SelectedIndexChanged;
            // 
            // btnInfo
            // 
            btnInfo.Font = new Font("Segoe UI", 11F);
            btnInfo.Location = new Point(486, 6);
            btnInfo.Margin = new Padding(0);
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new Size(22, 28);
            btnInfo.TabIndex = 12;
            btnInfo.TabStop = false;
            btnInfo.Text = "؟";
            btnInfo.TextAlign = ContentAlignment.TopCenter;
            btnInfo.UseVisualStyleBackColor = true;
            btnInfo.Click += btnInfo_Click;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSend.Location = new Point(794, 122);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 1;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtAnswer
            // 
            txtAnswer.AcceptsReturn = true;
            txtAnswer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtAnswer.Font = new Font("Vazir Medium", 10F);
            txtAnswer.Location = new Point(13, 149);
            txtAnswer.Multiline = true;
            txtAnswer.Name = "txtAnswer";
            txtAnswer.RightToLeft = RightToLeft.Yes;
            txtAnswer.Size = new Size(859, 255);
            txtAnswer.TabIndex = 7;
            txtAnswer.TabStop = false;
            txtAnswer.TextAlign = HorizontalAlignment.Right;
            // 
            // txtQuestion
            // 
            txtQuestion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtQuestion.Location = new Point(13, 38);
            txtQuestion.Multiline = true;
            txtQuestion.Name = "txtQuestion";
            txtQuestion.RightToLeft = RightToLeft.Yes;
            txtQuestion.ScrollBars = ScrollBars.Vertical;
            txtQuestion.Size = new Size(859, 83);
            txtQuestion.TabIndex = 0;
            txtQuestion.Text = "سلام لطفا خودت را معرفی کن";
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.Location = new Point(713, 122);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Visible = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // cmbHistory
            // 
            cmbHistory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbHistory.FormattingEnabled = true;
            cmbHistory.Items.AddRange(new object[] { "History 1", "History 2", "History 3", "History 4", "History 5", "History 6", "History 7", "" });
            cmbHistory.Location = new Point(719, 9);
            cmbHistory.Margin = new Padding(3, 4, 3, 4);
            cmbHistory.Name = "cmbHistory";
            cmbHistory.Size = new Size(153, 23);
            cmbHistory.TabIndex = 40;
            cmbHistory.SelectedIndexChanged += cmbHistory_SelectedIndexChanged;
            // 
            // btnCopyLog
            // 
            btnCopyLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCopyLog.Font = new Font("Segoe UI", 12F);
            btnCopyLog.Location = new Point(836, 413);
            btnCopyLog.Name = "btnCopyLog";
            btnCopyLog.Size = new Size(33, 31);
            btnCopyLog.TabIndex = 9;
            btnCopyLog.TabStop = false;
            btnCopyLog.Text = "⧉";
            btnCopyLog.UseVisualStyleBackColor = true;
            btnCopyLog.Click += btnCopyLog_Click;
            // 
            // btnSaveLog
            // 
            btnSaveLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSaveLog.Font = new Font("Segoe UI", 12F);
            btnSaveLog.Location = new Point(836, 487);
            btnSaveLog.Name = "btnSaveLog";
            btnSaveLog.Size = new Size(33, 31);
            btnSaveLog.TabIndex = 9;
            btnSaveLog.TabStop = false;
            btnSaveLog.Text = "💾";
            btnSaveLog.UseVisualStyleBackColor = true;
            btnSaveLog.Click += btnSaveLog_Click;
            // 
            // ChatPage1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 611);
            Controls.Add(cmbHistory);
            Controls.Add(btnSaveLog);
            Controls.Add(btnCopyLog);
            Controls.Add(btnClearDebug);
            Controls.Add(txtDebug);
            Controls.Add(lblStatus);
            Controls.Add(chkDebug);
            Controls.Add(chkStreamAnswer);
            Controls.Add(label1);
            Controls.Add(cmbModel);
            Controls.Add(btnInfo);
            Controls.Add(btnCancel);
            Controls.Add(btnSend);
            Controls.Add(txtAnswer);
            Controls.Add(txtQuestion);
            Name = "ChatPage1";
            Text = "ChatPage1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnClearDebug;
        private RichTextBox txtDebug;
        private Label lblStatus;
        private CheckBox chkDebug;
        private CheckBox chkStreamAnswer;
        private Label label1;
        private ComboBox cmbModel;
        private Button btnInfo;
        private Button btnSend;
        private TextBox txtAnswer;
        private TextBox txtQuestion;
        private Button btnCancel;
        private ComboBox cmbHistory;
        private Button btnCopyLog;
        private Button btnSaveLog;
    }
}