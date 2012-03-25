// <DEL> 2009/11/23 ->
//namespace magicAnime
//{
//    partial class UpdateDialog
//    {
//        /// <summary>
//        /// 必要なデザイナ変数です。
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// 使用中のリソースをすべてクリーンアップします。
//        /// </summary>
//        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows フォーム デザイナで生成されたコード

//        /// <summary>
//        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
//        /// コード エディタで変更しないでください。
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.components = new System.ComponentModel.Container();
//            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDialog));
//            this.closedButton = new System.Windows.Forms.Button();
//            this.pictureBox1 = new System.Windows.Forms.PictureBox();
//            this.ProgressLabel = new System.Windows.Forms.Label();
//            this.detailsButton = new System.Windows.Forms.Button();
//            this.Timer = new System.Windows.Forms.Timer(this.components);
//            this.ErrorTextBox = new System.Windows.Forms.TextBox();
//            this.closeTimer = new System.Windows.Forms.Timer(this.components);
//            this.autoCloseCheckBox = new System.Windows.Forms.CheckBox();
//            this.progressBar = new System.Windows.Forms.ProgressBar();
//            this.descriptionLabel = new System.Windows.Forms.Label();
//            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
//            this.SuspendLayout();
//            // 
//            // closedButton
//            // 
//            this.closedButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
//            this.closedButton.Location = new System.Drawing.Point(329, 12);
//            this.closedButton.Name = "closedButton";
//            this.closedButton.Size = new System.Drawing.Size(124, 32);
//            this.closedButton.TabIndex = 0;
//            this.closedButton.Text = "閉じる(&C)";
//            this.closedButton.UseVisualStyleBackColor = true;
//            this.closedButton.Click += new System.EventHandler(this.closedButton_Click);
//            // 
//            // pictureBox1
//            // 
//            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
//            this.pictureBox1.Location = new System.Drawing.Point(12, 21);
//            this.pictureBox1.Name = "pictureBox1";
//            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
//            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
//            this.pictureBox1.TabIndex = 1;
//            this.pictureBox1.TabStop = false;
//            // 
//            // ProgressLabel
//            // 
//            this.ProgressLabel.AutoSize = true;
//            this.ProgressLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
//            this.ProgressLabel.Location = new System.Drawing.Point(64, 21);
//            this.ProgressLabel.Name = "ProgressLabel";
//            this.ProgressLabel.Size = new System.Drawing.Size(62, 15);
//            this.ProgressLabel.TabIndex = 2;
//            this.ProgressLabel.Text = "Ready...";
//            // 
//            // detailsButton
//            // 
//            this.detailsButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
//            this.detailsButton.Location = new System.Drawing.Point(329, 48);
//            this.detailsButton.Name = "detailsButton";
//            this.detailsButton.Size = new System.Drawing.Size(124, 32);
//            this.detailsButton.TabIndex = 6;
//            this.detailsButton.Text = "詳細(&D) >>";
//            this.detailsButton.UseVisualStyleBackColor = true;
//            this.detailsButton.Click += new System.EventHandler(this.detailsButton_Click);
//            // 
//            // Timer
//            // 
//            this.Timer.Interval = 250;
//            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
//            // 
//            // ErrorTextBox
//            // 
//            this.ErrorTextBox.BackColor = System.Drawing.SystemColors.Info;
//            this.ErrorTextBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
//            this.ErrorTextBox.Location = new System.Drawing.Point(2, 111);
//            this.ErrorTextBox.Multiline = true;
//            this.ErrorTextBox.Name = "ErrorTextBox";
//            this.ErrorTextBox.ReadOnly = true;
//            this.ErrorTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
//            this.ErrorTextBox.ShortcutsEnabled = false;
//            this.ErrorTextBox.Size = new System.Drawing.Size(463, 98);
//            this.ErrorTextBox.TabIndex = 7;
//            // 
//            // closeTimer
//            // 
//            this.closeTimer.Tick += new System.EventHandler(this.closeTimer_Tick);
//            // 
//            // autoCloseCheckBox
//            // 
//            this.autoCloseCheckBox.AutoSize = true;
//            this.autoCloseCheckBox.Checked = true;
//            this.autoCloseCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
//            this.autoCloseCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
//            this.autoCloseCheckBox.Location = new System.Drawing.Point(286, 86);
//            this.autoCloseCheckBox.Name = "autoCloseCheckBox";
//            this.autoCloseCheckBox.Size = new System.Drawing.Size(174, 19);
//            this.autoCloseCheckBox.TabIndex = 8;
//            this.autoCloseCheckBox.Text = "完了後、自動で閉じる(&I)";
//            this.autoCloseCheckBox.UseVisualStyleBackColor = true;
//            // 
//            // progressBar
//            // 
//            this.progressBar.Location = new System.Drawing.Point(67, 59);
//            this.progressBar.Name = "progressBar";
//            this.progressBar.Size = new System.Drawing.Size(220, 12);
//            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
//            this.progressBar.TabIndex = 9;
//            this.progressBar.Value = 50;
//            // 
//            // descriptionLabel
//            // 
//            this.descriptionLabel.AutoSize = true;
//            this.descriptionLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
//            this.descriptionLabel.Location = new System.Drawing.Point(64, 41);
//            this.descriptionLabel.Name = "descriptionLabel";
//            this.descriptionLabel.Size = new System.Drawing.Size(16, 15);
//            this.descriptionLabel.TabIndex = 10;
//            this.descriptionLabel.Text = "...";
//            // 
//            // UpdateDialog
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(465, 111);
//            this.ControlBox = false;
//            this.Controls.Add(this.descriptionLabel);
//            this.Controls.Add(this.progressBar);
//            this.Controls.Add(this.autoCloseCheckBox);
//            this.Controls.Add(this.ErrorTextBox);
//            this.Controls.Add(this.detailsButton);
//            this.Controls.Add(this.ProgressLabel);
//            this.Controls.Add(this.pictureBox1);
//            this.Controls.Add(this.closedButton);
//            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
//            this.MaximizeBox = false;
//            this.MinimizeBox = false;
//            this.Name = "UpdateDialog";
//            this.ShowIcon = false;
//            this.ShowInTaskbar = false;
//            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
//            this.Text = "mAgicAnime - データ更新";
//            this.Shown += new System.EventHandler(this.UpdateDialog_Shown);
//            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateDialog_FormClosing);
//            this.Load += new System.EventHandler(this.UpdateDialog_Load);
//            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
//            this.ResumeLayout(false);
//            this.PerformLayout();

//        }

//        #endregion

//        private System.Windows.Forms.Button closedButton;
//        private System.Windows.Forms.PictureBox pictureBox1;
//        private System.Windows.Forms.Label ProgressLabel;
//        private System.Windows.Forms.Button detailsButton;
//        private System.Windows.Forms.Timer Timer;
//        private System.Windows.Forms.TextBox ErrorTextBox;
//        private System.Windows.Forms.Timer closeTimer;
//        private System.Windows.Forms.CheckBox autoCloseCheckBox;
//        private System.Windows.Forms.ProgressBar progressBar;
//        private System.Windows.Forms.Label descriptionLabel;
//    }
//}
// <DEL> 2009/11/23 <-
