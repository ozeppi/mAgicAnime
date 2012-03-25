namespace magicAnime
{
	partial class AboutBox
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			this.logoPictureBox = new System.Windows.Forms.PictureBox();
			this.okButton = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelProductVersion = new System.Windows.Forms.Label();
			this.labelProductName = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.licenseListBox = new System.Windows.Forms.ListBox();
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// logoPictureBox
			// 
			this.logoPictureBox.BackColor = System.Drawing.SystemColors.Menu;
			this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
			this.logoPictureBox.Location = new System.Drawing.Point(9, 17);
			this.logoPictureBox.Margin = new System.Windows.Forms.Padding(0);
			this.logoPictureBox.Name = "logoPictureBox";
			this.logoPictureBox.Size = new System.Drawing.Size(64, 64);
			this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.logoPictureBox.TabIndex = 35;
			this.logoPictureBox.TabStop = false;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.Location = new System.Drawing.Point(389, 271);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(135, 25);
			this.okButton.TabIndex = 30;
			this.okButton.Text = "&OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// timer
			// 
			this.timer.Interval = 3000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelProductVersion);
			this.groupBox1.Controls.Add(this.labelProductName);
			this.groupBox1.Controls.Add(this.labelVersion);
			this.groupBox1.Location = new System.Drawing.Point(88, 11);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(436, 56);
			this.groupBox1.TabIndex = 32;
			this.groupBox1.TabStop = false;
			// 
			// labelProductVersion
			// 
			this.labelProductVersion.AutoSize = true;
			this.labelProductVersion.BackColor = System.Drawing.Color.Transparent;
			this.labelProductVersion.Location = new System.Drawing.Point(18, 31);
			this.labelProductVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelProductVersion.MaximumSize = new System.Drawing.Size(0, 16);
			this.labelProductVersion.Name = "labelProductVersion";
			this.labelProductVersion.Size = new System.Drawing.Size(37, 16);
			this.labelProductVersion.TabIndex = 34;
			this.labelProductVersion.Text = "HOGE";
			this.labelProductVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelProductVersion.UseCompatibleTextRendering = true;
			// 
			// labelProductName
			// 
			this.labelProductName.AutoSize = true;
			this.labelProductName.BackColor = System.Drawing.Color.Transparent;
			this.labelProductName.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelProductName.Location = new System.Drawing.Point(18, 15);
			this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelProductName.MaximumSize = new System.Drawing.Size(0, 16);
			this.labelProductName.Name = "labelProductName";
			this.labelProductName.Size = new System.Drawing.Size(47, 16);
			this.labelProductName.TabIndex = 33;
			this.labelProductName.Text = "HOGE";
			this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelProductName.UseCompatibleTextRendering = true;
			// 
			// labelVersion
			// 
			this.labelVersion.AutoSize = true;
			this.labelVersion.BackColor = System.Drawing.Color.Transparent;
			this.labelVersion.Location = new System.Drawing.Point(185, 31);
			this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelVersion.MaximumSize = new System.Drawing.Size(0, 16);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(37, 16);
			this.labelVersion.TabIndex = 32;
			this.labelVersion.Text = "HOGE";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelVersion.UseCompatibleTextRendering = true;
			// 
			// licenseListBox
			// 
			this.licenseListBox.FormattingEnabled = true;
			this.licenseListBox.ItemHeight = 12;
			this.licenseListBox.Location = new System.Drawing.Point(88, 73);
			this.licenseListBox.Name = "licenseListBox";
			this.licenseListBox.Size = new System.Drawing.Size(436, 184);
			this.licenseListBox.TabIndex = 35;
			// 
			// AboutBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(536, 307);
			this.Controls.Add(this.licenseListBox);
			this.Controls.Add(this.logoPictureBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "バージョン情報";
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labelProductVersion;
		private System.Windows.Forms.Label labelProductName;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.ListBox licenseListBox;
		private System.Windows.Forms.PictureBox logoPictureBox;

	}
}
