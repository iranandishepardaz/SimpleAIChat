namespace SimpleAIChat
{
    partial class ChatPage01
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
            label1 = new Label();
            cmbModel = new ComboBox();
            chkStream = new CheckBox();
            btnSend = new Button();
            txtQuestion = new TextBox();
            pnlConversation = new Panel();
            lblStatus = new Label();
            btnNewChat = new Button();
            btnCancell = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 9);
            label1.Name = "label1";
            label1.Size = new Size(43, 19);
            label1.TabIndex = 13;
            label1.Text = "Model";
            // 
            // cmbModel
            // 
            cmbModel.FormattingEnabled = true;
            cmbModel.Location = new Point(56, 6);
            cmbModel.Margin = new Padding(3, 4, 3, 4);
            cmbModel.Name = "cmbModel";
            cmbModel.Size = new Size(121, 27);
            cmbModel.TabIndex = 3;
            // 
            // chkStream
            // 
            chkStream.AutoSize = true;
            chkStream.Checked = true;
            chkStream.CheckState = CheckState.Checked;
            chkStream.Location = new Point(192, 8);
            chkStream.Margin = new Padding(3, 4, 3, 4);
            chkStream.Name = "chkStream";
            chkStream.Size = new Size(109, 23);
            chkStream.TabIndex = 10;
            chkStream.Text = "Stream Answer";
            chkStream.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Location = new Point(613, 419);
            btnSend.Margin = new Padding(3, 4, 3, 4);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(60, 29);
            btnSend.TabIndex = 1;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtQuestion
            // 
            txtQuestion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtQuestion.Location = new Point(5, 419);
            txtQuestion.Margin = new Padding(3, 4, 3, 4);
            txtQuestion.Multiline = true;
            txtQuestion.Name = "txtQuestion";
            txtQuestion.RightToLeft = RightToLeft.Yes;
            txtQuestion.ScrollBars = ScrollBars.Vertical;
            txtQuestion.Size = new Size(602, 64);
            txtQuestion.TabIndex = 0;
            // 
            // pnlConversation
            // 
            pnlConversation.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlConversation.AutoScroll = true;
            pnlConversation.Location = new Point(7, 40);
            pnlConversation.Name = "pnlConversation";
            pnlConversation.Size = new Size(666, 372);
            pnlConversation.TabIndex = 14;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Vazir Medium", 9F, FontStyle.Italic);
            lblStatus.ForeColor = SystemColors.ControlDark;
            lblStatus.Location = new Point(320, 11);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(56, 19);
            lblStatus.TabIndex = 15;
            lblStatus.Text = "Status...";
            // 
            // btnNewChat
            // 
            btnNewChat.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNewChat.Location = new Point(613, 454);
            btnNewChat.Margin = new Padding(3, 4, 3, 4);
            btnNewChat.Name = "btnNewChat";
            btnNewChat.Size = new Size(60, 29);
            btnNewChat.TabIndex = 2;
            btnNewChat.Text = "New";
            btnNewChat.UseVisualStyleBackColor = true;
            btnNewChat.Click += btnNewChat_Click;
            // 
            // btnCancell
            // 
            btnCancell.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancell.Location = new Point(613, 422);
            btnCancell.Margin = new Padding(3, 4, 3, 4);
            btnCancell.Name = "btnCancell";
            btnCancell.Size = new Size(60, 29);
            btnCancell.TabIndex = 1;
            btnCancell.Text = "Cancell";
            btnCancell.UseVisualStyleBackColor = true;
            btnCancell.Visible = false;
            btnCancell.Click += btnCancell_Click;
            // 
            // ChatPage01
            // 
            AutoScaleDimensions = new SizeF(7F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(685, 496);
            Controls.Add(lblStatus);
            Controls.Add(pnlConversation);
            Controls.Add(label1);
            Controls.Add(cmbModel);
            Controls.Add(chkStream);
            Controls.Add(btnNewChat);
            Controls.Add(btnCancell);
            Controls.Add(btnSend);
            Controls.Add(txtQuestion);
            Font = new Font("Vazir Medium", 9F);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ChatPage01";
            Text = "ChatPage01";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cmbModel;
        private CheckBox chkStream;
        private Button btnSend;
        private TextBox txtQuestion;
        private Panel pnlConversation;
        private Label lblStatus;
        private Button btnNewChat;
        private Button btnCancell;
    }
}