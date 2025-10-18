namespace Ntk.Chrome.Forms
{
    partial class TestForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.requestPanel = new System.Windows.Forms.Panel();
            this.lblRequestInfo = new System.Windows.Forms.Label();
            this.lblUrl = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.lblParams = new System.Windows.Forms.Label();
            this.txtParams = new System.Windows.Forms.TextBox();
            this.lblCustomUrl = new System.Windows.Forms.Label();
            this.txtCustomUrl = new System.Windows.Forms.TextBox();
            this.resultPanel = new System.Windows.Forms.Panel();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnManualTest = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.requestPanel.SuspendLayout();
            this.resultPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(150, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "تست ورود به سایت";
            // 
            // requestPanel
            // 
            this.requestPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.requestPanel.Controls.Add(this.lblRequestInfo);
            this.requestPanel.Controls.Add(this.lblUrl);
            this.requestPanel.Controls.Add(this.txtUrl);
            this.requestPanel.Controls.Add(this.lblParams);
            this.requestPanel.Controls.Add(this.txtParams);
            this.requestPanel.Controls.Add(this.lblCustomUrl);
            this.requestPanel.Controls.Add(this.txtCustomUrl);
            this.requestPanel.Location = new System.Drawing.Point(20, 60);
            this.requestPanel.Name = "requestPanel";
            this.requestPanel.Size = new System.Drawing.Size(760, 260);
            this.requestPanel.TabIndex = 1;
            // 
            // lblRequestInfo
            // 
            this.lblRequestInfo.AutoSize = true;
            this.lblRequestInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblRequestInfo.Location = new System.Drawing.Point(10, 10);
            this.lblRequestInfo.Name = "lblRequestInfo";
            this.lblRequestInfo.Size = new System.Drawing.Size(120, 17);
            this.lblRequestInfo.TabIndex = 0;
            this.lblRequestInfo.Text = "اطلاعات درخواست";
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(10, 40);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(85, 15);
            this.lblUrl.TabIndex = 1;
            this.lblUrl.Text = "آدرس وب سایت:";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(10, 60);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.ReadOnly = true;
            this.txtUrl.Size = new System.Drawing.Size(730, 23);
            this.txtUrl.TabIndex = 2;
            // 
            // lblParams
            // 
            this.lblParams.AutoSize = true;
            this.lblParams.Location = new System.Drawing.Point(10, 100);
            this.lblParams.Name = "lblParams";
            this.lblParams.Size = new System.Drawing.Size(85, 15);
            this.lblParams.TabIndex = 3;
            this.lblParams.Text = "پارامترهای ورودی:";
            // 
            // txtParams
            // 
            this.txtParams.Location = new System.Drawing.Point(10, 120);
            this.txtParams.Multiline = true;
            this.txtParams.Name = "txtParams";
            this.txtParams.ReadOnly = true;
            this.txtParams.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtParams.Size = new System.Drawing.Size(730, 65);
            this.txtParams.TabIndex = 4;
            // 
            // lblCustomUrl
            // 
            this.lblCustomUrl.AutoSize = true;
            this.lblCustomUrl.Location = new System.Drawing.Point(10, 200);
            this.lblCustomUrl.Name = "lblCustomUrl";
            this.lblCustomUrl.Size = new System.Drawing.Size(125, 15);
            this.lblCustomUrl.TabIndex = 5;
            this.lblCustomUrl.Text = "آدرس سفارشی برای تست:";
            // 
            // txtCustomUrl
            // 
            this.txtCustomUrl.Location = new System.Drawing.Point(10, 220);
            this.txtCustomUrl.Name = "txtCustomUrl";
            this.txtCustomUrl.Size = new System.Drawing.Size(730, 23);
            this.txtCustomUrl.TabIndex = 6;
            // 
            // resultPanel
            // 
            this.resultPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultPanel.Controls.Add(this.lblResult);
            this.resultPanel.Controls.Add(this.txtResult);
            this.resultPanel.Location = new System.Drawing.Point(20, 340);
            this.resultPanel.Name = "resultPanel";
            this.resultPanel.Size = new System.Drawing.Size(760, 300);
            this.resultPanel.TabIndex = 2;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblResult.Location = new System.Drawing.Point(10, 10);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(75, 17);
            this.lblResult.TabIndex = 0;
            this.lblResult.Text = "نتیجه تست:";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(10, 35);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(730, 250);
            this.txtResult.TabIndex = 1;
            // 
            // btnTest
            // 
            this.btnTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnTest.ForeColor = System.Drawing.Color.White;
            this.btnTest.Location = new System.Drawing.Point(20, 660);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(150, 40);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "شروع تست";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnManualTest
            // 
            this.btnManualTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnManualTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManualTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnManualTest.ForeColor = System.Drawing.Color.White;
            this.btnManualTest.Location = new System.Drawing.Point(190, 660);
            this.btnManualTest.Name = "btnManualTest";
            this.btnManualTest.Size = new System.Drawing.Size(150, 40);
            this.btnManualTest.TabIndex = 4;
            this.btnManualTest.Text = "تست دستی";
            this.btnManualTest.UseVisualStyleBackColor = false;
            this.btnManualTest.Click += new System.EventHandler(this.btnManualTest_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(630, 660);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(150, 40);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "بستن";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 720);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnManualTest);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.resultPanel);
            this.Controls.Add(this.requestPanel);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new System.Drawing.Size(800, 720);
            this.Name = "TestForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "فرم تست ورود";
            this.requestPanel.ResumeLayout(false);
            this.requestPanel.PerformLayout();
            this.resultPanel.ResumeLayout(false);
            this.resultPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Panel requestPanel;
        private Label lblRequestInfo;
        private Label lblUrl;
        private TextBox txtUrl;
        private Label lblParams;
        private TextBox txtParams;
        private Label lblCustomUrl;
        private TextBox txtCustomUrl;
        private Panel resultPanel;
        private Label lblResult;
        private TextBox txtResult;
        private Button btnTest;
        private Button btnManualTest;
        private Button btnClose;
    }
}
