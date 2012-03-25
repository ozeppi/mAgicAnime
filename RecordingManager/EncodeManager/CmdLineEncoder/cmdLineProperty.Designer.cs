namespace magicAnime.RecordingManager.EncodeManager.CmdLineEncoder
{
	partial class cmdLineProperty
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
			this.ProfileComboBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.NewProfileButton = new System.Windows.Forms.Button();
			this.ModifyButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ProfileComboBox
			// 
			this.ProfileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ProfileComboBox.FormattingEnabled = true;
			this.ProfileComboBox.Location = new System.Drawing.Point(12, 37);
			this.ProfileComboBox.Name = "ProfileComboBox";
			this.ProfileComboBox.Size = new System.Drawing.Size(220, 23);
			this.ProfileComboBox.TabIndex = 4;
			this.ProfileComboBox.SelectedIndexChanged += new System.EventHandler(this.ProfileComboBox_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(11, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(93, 15);
			this.label3.TabIndex = 5;
			this.label3.Text = "プロファイル(&P):";
			// 
			// NewProfileButton
			// 
			this.NewProfileButton.Location = new System.Drawing.Point(12, 66);
			this.NewProfileButton.Name = "NewProfileButton";
			this.NewProfileButton.Size = new System.Drawing.Size(99, 30);
			this.NewProfileButton.TabIndex = 6;
			this.NewProfileButton.Text = "新規作成(&N)";
			this.NewProfileButton.UseVisualStyleBackColor = true;
			this.NewProfileButton.Click += new System.EventHandler(this.NewProfileButton_Click);
			// 
			// ModifyButton
			// 
			this.ModifyButton.Location = new System.Drawing.Point(117, 66);
			this.ModifyButton.Name = "ModifyButton";
			this.ModifyButton.Size = new System.Drawing.Size(114, 30);
			this.ModifyButton.TabIndex = 7;
			this.ModifyButton.Text = "変更(&M)";
			this.ModifyButton.UseVisualStyleBackColor = true;
			this.ModifyButton.Click += new System.EventHandler(this.ModifyButton_Click);
			// 
			// cmdLineProperty
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(338, 125);
			this.ControlBox = false;
			this.Controls.Add(this.ModifyButton);
			this.Controls.Add(this.NewProfileButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.ProfileComboBox);
			this.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "cmdLineProperty";
			this.Text = "cmdLineProperty";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox ProfileComboBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button NewProfileButton;
		private System.Windows.Forms.Button ModifyButton;
	}
}