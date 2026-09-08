namespace SimpleAIChat
{
    partial class ChatPage00
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
            btnSend = new Button();
            txtAnswer = new TextBox();
            txtQuestion = new TextBox();
            chkStream = new CheckBox();
            cmbModel = new ComboBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSend.Location = new Point(526, 110);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(116, 23);
            btnSend.TabIndex = 1;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtAnswer
            // 
            txtAnswer.AcceptsReturn = true;
            txtAnswer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtAnswer.Location = new Point(12, 138);
            txtAnswer.Multiline = true;
            txtAnswer.Name = "txtAnswer";
            txtAnswer.RightToLeft = RightToLeft.Yes;
            txtAnswer.Size = new Size(632, 293);
            txtAnswer.TabIndex = 4;
            txtAnswer.TabStop = false;
            // 
            // txtQuestion
            // 
            txtQuestion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtQuestion.Location = new Point(12, 20);
            txtQuestion.Multiline = true;
            txtQuestion.Name = "txtQuestion";
            txtQuestion.RightToLeft = RightToLeft.Yes;
            txtQuestion.ScrollBars = ScrollBars.Vertical;
            txtQuestion.Size = new Size(632, 83);
            txtQuestion.TabIndex = 0;
            // 
            // chkStream
            // 
            chkStream.AutoSize = true;
            chkStream.Checked = true;
            chkStream.CheckState = CheckState.Checked;
            chkStream.Location = new Point(355, 112);
            chkStream.Name = "chkStream";
            chkStream.Size = new Size(105, 19);
            chkStream.TabIndex = 2;
            chkStream.Text = "Stream Answer";
            chkStream.UseVisualStyleBackColor = true;
            // 
            // cmbModel
            // 
            cmbModel.FormattingEnabled = true;
            cmbModel.Location = new Point(60, 110);
            cmbModel.Name = "cmbModel";
            cmbModel.Size = new Size(121, 23);
            cmbModel.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 114);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 7;
            label1.Text = "Model";
            // 
            // ChatPage00
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(656, 450);
            Controls.Add(label1);
            Controls.Add(cmbModel);
            Controls.Add(chkStream);
            Controls.Add(btnSend);
            Controls.Add(txtAnswer);
            Controls.Add(txtQuestion);
            Name = "ChatPage00";
            Text = "ChatPage00";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSend;
        private TextBox txtAnswer;
        private TextBox txtQuestion;
        private CheckBox chkStream;
        private ComboBox cmbModel;
        private Label label1;
    }
}