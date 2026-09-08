namespace SimpleAIChat
{
    partial class ChatPage03
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
            lblStatus = new Label();
            pnlConversation = new Panel();
            label1 = new Label();
            cmbModel = new ComboBox();
            chkStream = new CheckBox();
            btnNewChat = new Button();
            btnCancell = new Button();
            btnSend = new Button();
            txtQuestion = new TextBox();
            lblLLMRemarks = new Label();
            SuspendLayout();
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Vazir Medium", 9F, FontStyle.Italic);
            lblStatus.ForeColor = SystemColors.ControlDark;
            lblStatus.Location = new Point(10, 412);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(56, 19);
            lblStatus.TabIndex = 33;
            lblStatus.Text = "Status...";
            // 
            // pnlConversation
            // 
            pnlConversation.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlConversation.AutoScroll = true;
            pnlConversation.Location = new Point(10, 41);
            pnlConversation.Name = "pnlConversation";
            pnlConversation.Size = new Size(665, 366);
            pnlConversation.TabIndex = 32;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 10);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 31;
            label1.Text = "Model";
            // 
            // cmbModel
            // 
            cmbModel.FormattingEnabled = true;
            cmbModel.Location = new Point(59, 7);
            cmbModel.Margin = new Padding(3, 4, 3, 4);
            cmbModel.Name = "cmbModel";
            cmbModel.Size = new Size(287, 23);
            cmbModel.TabIndex = 29;
            cmbModel.SelectedIndexChanged += cmbModel_SelectedIndexChanged;
            // 
            // chkStream
            // 
            chkStream.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            chkStream.AutoSize = true;
            chkStream.Checked = true;
            chkStream.CheckState = CheckState.Checked;
            chkStream.Location = new Point(567, 415);
            chkStream.Margin = new Padding(3, 4, 3, 4);
            chkStream.Name = "chkStream";
            chkStream.Size = new Size(105, 19);
            chkStream.TabIndex = 30;
            chkStream.Text = "Stream Answer";
            chkStream.UseVisualStyleBackColor = true;
            // 
            // btnNewChat
            // 
            btnNewChat.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNewChat.Location = new Point(615, 470);
            btnNewChat.Margin = new Padding(3, 4, 3, 4);
            btnNewChat.Name = "btnNewChat";
            btnNewChat.Size = new Size(60, 29);
            btnNewChat.TabIndex = 28;
            btnNewChat.Text = "New";
            btnNewChat.UseVisualStyleBackColor = true;
            btnNewChat.Click += btnNewChat_Click;
            // 
            // btnCancell
            // 
            btnCancell.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancell.Location = new Point(615, 436);
            btnCancell.Margin = new Padding(3, 4, 3, 4);
            btnCancell.Name = "btnCancell";
            btnCancell.Size = new Size(60, 29);
            btnCancell.TabIndex = 26;
            btnCancell.Text = "Cancell";
            btnCancell.UseVisualStyleBackColor = true;
            btnCancell.Visible = false;
            btnCancell.Click += btnCancell_Click;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Location = new Point(615, 435);
            btnSend.Margin = new Padding(3, 4, 3, 4);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(60, 29);
            btnSend.TabIndex = 27;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtQuestion
            // 
            txtQuestion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtQuestion.Location = new Point(8, 435);
            txtQuestion.Margin = new Padding(3, 4, 3, 4);
            txtQuestion.Multiline = true;
            txtQuestion.Name = "txtQuestion";
            txtQuestion.RightToLeft = RightToLeft.Yes;
            txtQuestion.ScrollBars = ScrollBars.Vertical;
            txtQuestion.Size = new Size(601, 64);
            txtQuestion.TabIndex = 25;
            // 
            // lblLLMRemarks
            // 
            lblLLMRemarks.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblLLMRemarks.AutoSize = true;
            lblLLMRemarks.Font = new Font("Vazir Medium", 9F, FontStyle.Italic);
            lblLLMRemarks.ForeColor = SystemColors.ControlDark;
            lblLLMRemarks.Location = new Point(352, 9);
            lblLLMRemarks.Name = "lblLLMRemarks";
            lblLLMRemarks.Size = new Size(56, 19);
            lblLLMRemarks.TabIndex = 33;
            lblLLMRemarks.Text = "Status...";
            // 
            // ChatPage03
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 511);
            Controls.Add(lblLLMRemarks);
            Controls.Add(lblStatus);
            Controls.Add(pnlConversation);
            Controls.Add(label1);
            Controls.Add(cmbModel);
            Controls.Add(chkStream);
            Controls.Add(btnNewChat);
            Controls.Add(btnCancell);
            Controls.Add(btnSend);
            Controls.Add(txtQuestion);
            Name = "ChatPage03";
            Text = "ChatPage03";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblStatus;
        private Panel pnlConversation;
        private Label label1;
        private ComboBox cmbModel;
        private CheckBox chkStream;
        private Button btnNewChat;
        private Button btnCancell;
        private Button btnSend;
        private TextBox txtQuestion;
        private Label lblLLMRemarks;
    }
}