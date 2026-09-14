namespace SimpleAIChat
{
    partial class ChatPage0
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtQuestion = new TextBox();
            btnSend = new Button();
            txtAnswer = new TextBox();
            cmbModel = new ComboBox();
            label1 = new Label();
            btnInfo = new Button();
            chkStreamAnswer = new CheckBox();
            lblStatus = new Label();
            cmbHistory = new ComboBox();
            SuspendLayout();
            // 
            // txtQuestion
            // 
            txtQuestion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtQuestion.Location = new Point(16, 37);
            txtQuestion.Multiline = true;
            txtQuestion.Name = "txtQuestion";
            txtQuestion.RightToLeft = RightToLeft.Yes;
            txtQuestion.ScrollBars = ScrollBars.Vertical;
            txtQuestion.Size = new Size(909, 83);
            txtQuestion.TabIndex = 0;
            txtQuestion.Text = "سلام لطفا خودت را معرفی کن";
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSend.Location = new Point(847, 121);
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
            txtAnswer.Location = new Point(16, 148);
            txtAnswer.Multiline = true;
            txtAnswer.Name = "txtAnswer";
            txtAnswer.RightToLeft = RightToLeft.Yes;
            txtAnswer.Size = new Size(909, 468);
            txtAnswer.TabIndex = 0;
            txtAnswer.TextAlign = HorizontalAlignment.Right;
            // 
            // cmbModel
            // 
            cmbModel.FormattingEnabled = true;
            cmbModel.Items.AddRange(new object[] { "gemma2:2b", "gemma3:1b", "qwen2.5-coder:3b" });
            cmbModel.Location = new Point(63, 8);
            cmbModel.Name = "cmbModel";
            cmbModel.Size = new Size(414, 23);
            cmbModel.TabIndex = 2;
            cmbModel.Text = "gemma2:2b";
            cmbModel.SelectedIndexChanged += cmbModel_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 12);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 3;
            label1.Text = "Model";
            // 
            // btnInfo
            // 
            btnInfo.Font = new Font("Segoe UI", 11F);
            btnInfo.Location = new Point(489, 5);
            btnInfo.Margin = new Padding(0);
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new Size(22, 28);
            btnInfo.TabIndex = 2;
            btnInfo.TabStop = false;
            btnInfo.Text = "؟";
            btnInfo.TextAlign = ContentAlignment.TopCenter;
            btnInfo.UseVisualStyleBackColor = true;
            btnInfo.Click += btnInfo_Click;
            // 
            // chkStreamAnswer
            // 
            chkStreamAnswer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkStreamAnswer.AutoSize = true;
            chkStreamAnswer.Checked = true;
            chkStreamAnswer.CheckState = CheckState.Checked;
            chkStreamAnswer.Location = new Point(769, 123);
            chkStreamAnswer.Name = "chkStreamAnswer";
            chkStreamAnswer.Size = new Size(63, 19);
            chkStreamAnswer.TabIndex = 4;
            chkStreamAnswer.Text = "Stream";
            chkStreamAnswer.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblStatus.ForeColor = SystemColors.ControlDarkDark;
            lblStatus.Location = new Point(18, 619);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(40, 15);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Status";
            // 
            // cmbHistory
            // 
            cmbHistory.FormattingEnabled = true;
            cmbHistory.Items.AddRange(new object[] { "History 1", "History 2", "History 3", "History 4", "History 5", "History 6", "History 7", "" });
            cmbHistory.Location = new Point(769, 5);
            cmbHistory.Margin = new Padding(3, 4, 3, 4);
            cmbHistory.Name = "cmbHistory";
            cmbHistory.Size = new Size(153, 23);
            cmbHistory.TabIndex = 39;
            cmbHistory.SelectedIndexChanged += cmbHistory_SelectedIndexChanged;
            // 
            // ChatPage0
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(934, 641);
            Controls.Add(cmbHistory);
            Controls.Add(lblStatus);
            Controls.Add(chkStreamAnswer);
            Controls.Add(label1);
            Controls.Add(cmbModel);
            Controls.Add(btnInfo);
            Controls.Add(btnSend);
            Controls.Add(txtAnswer);
            Controls.Add(txtQuestion);
            Name = "ChatPage0";
            Text = "ChatPage0";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtQuestion;
        private Button btnSend;
        private TextBox txtAnswer;
        private ComboBox cmbModel;
        private Label label1;
        private Button btnInfo;
        private CheckBox chkStreamAnswer;
        private Label lblStatus;
        private ComboBox cmbHistory;
    }
}
