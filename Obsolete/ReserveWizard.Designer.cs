namespace magicAnime
{
	partial class ReserveWizard
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
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
			this.panel = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.nextButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.registTitleLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.tvStationLabel = new System.Windows.Forms.Label();
			this.panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel.BackColor = System.Drawing.SystemColors.Window;
			this.panel.Controls.Add(this.label4);
			this.panel.Controls.Add(this.pictureBox1);
			this.panel.Controls.Add(this.label3);
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(616, 59);
			this.panel.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label4.Location = new System.Drawing.Point(48, 29);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(324, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "mAgicTV5を利用した自動録画予約の初回設定を行います。";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::magicAnime.Properties.Resources.netfol;
			this.pictureBox1.Location = new System.Drawing.Point(10, 10);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label3.Location = new System.Drawing.Point(48, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(148, 15);
			this.label3.TabIndex = 0;
			this.label3.Text = "録画予約の初回設定";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label1.Location = new System.Drawing.Point(12, 73);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(442, 75);
			this.label1.TabIndex = 6;
			this.label1.Text = "初回のみiEPGを利用して録画登録します。\r\n今回登録した予約データをもとに、次回以降は自動的に録画予約します。\r\n\r\n右のボタンを押すと、mAgicTVの録画予" +
				"約ウィンドウが開きます。\r\n次の項目を確認して下さい。";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label2.Location = new System.Drawing.Point(12, 163);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(249, 45);
			this.label2.TabIndex = 7;
			this.label2.Text = "・テレビ局、画質を選択\r\n・継続予約の種類は\"単発\"にしておく\r\n・\"時刻\"や\"番組名\"は変更しない";
			// 
			// nextButton
			// 
			this.nextButton.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.nextButton.Location = new System.Drawing.Point(478, 73);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(126, 28);
			this.nextButton.TabIndex = 8;
			this.nextButton.Text = "登録(&R)";
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tvStationLabel);
			this.groupBox1.Controls.Add(this.registTitleLabel);
			this.groupBox1.Location = new System.Drawing.Point(15, 224);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(373, 62);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			// 
			// registTitleLabel
			// 
			this.registTitleLabel.AutoSize = true;
			this.registTitleLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.registTitleLabel.Location = new System.Drawing.Point(10, 15);
			this.registTitleLabel.Name = "registTitleLabel";
			this.registTitleLabel.Size = new System.Drawing.Size(96, 15);
			this.registTitleLabel.TabIndex = 0;
			this.registTitleLabel.Text = "録画タイトル名";
			// 
			// cancelButton
			// 
			this.cancelButton.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.cancelButton.Location = new System.Drawing.Point(478, 107);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(126, 28);
			this.cancelButton.TabIndex = 10;
			this.cancelButton.Text = "キャンセル(&C)";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// tvStationLabel
			// 
			this.tvStationLabel.AutoSize = true;
			this.tvStationLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.tvStationLabel.Location = new System.Drawing.Point(10, 35);
			this.tvStationLabel.Name = "tvStationLabel";
			this.tvStationLabel.Size = new System.Drawing.Size(55, 15);
			this.tvStationLabel.TabIndex = 1;
			this.tvStationLabel.Text = "テレビ局";
			// 
			// ReserveWizard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(616, 306);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nextButton);
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReserveWizard";
			this.Text = "初回録画予約";
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label registTitleLabel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label tvStationLabel;
	}
}