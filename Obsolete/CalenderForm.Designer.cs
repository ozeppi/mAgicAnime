namespace magicAnime
{
	partial class CalenderForm
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
			this.schedulePicture = new System.Windows.Forms.PictureBox();
			this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
			((System.ComponentModel.ISupportInitialize)(this.schedulePicture)).BeginInit();
			this.SuspendLayout();
			// 
			// schedulePicture
			// 
			this.schedulePicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.schedulePicture.BackColor = System.Drawing.SystemColors.Window;
			this.schedulePicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.schedulePicture.Location = new System.Drawing.Point(1, 30);
			this.schedulePicture.Name = "schedulePicture";
			this.schedulePicture.Size = new System.Drawing.Size(305, 162);
			this.schedulePicture.TabIndex = 2;
			this.schedulePicture.TabStop = false;
			this.schedulePicture.Paint += new System.Windows.Forms.PaintEventHandler(this.schedulePicture_Paint);
			// 
			// dateTimePicker1
			// 
			this.dateTimePicker1.Checked = false;
			this.dateTimePicker1.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.dateTimePicker1.Location = new System.Drawing.Point(1, 4);
			this.dateTimePicker1.Name = "dateTimePicker1";
			this.dateTimePicker1.Size = new System.Drawing.Size(135, 20);
			this.dateTimePicker1.TabIndex = 3;
			// 
			// CalenderForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(308, 193);
			this.ControlBox = false;
			this.Controls.Add(this.dateTimePicker1);
			this.Controls.Add(this.schedulePicture);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "CalenderForm";
			this.Text = "週間放送予定";
			((System.ComponentModel.ISupportInitialize)(this.schedulePicture)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox schedulePicture;
		private System.Windows.Forms.DateTimePicker dateTimePicker1;

	}
}