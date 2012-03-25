namespace magicAnime.UserInterface
{
	partial class StationTableDialog
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
			this.registCheckBox = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.registerdStationComboBox = new System.Windows.Forms.ComboBox();
			this.referenceTvStationTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// registCheckBox
			// 
			this.registCheckBox.AutoSize = true;
			this.registCheckBox.Location = new System.Drawing.Point(14, 110);
			this.registCheckBox.Name = "registCheckBox";
			this.registCheckBox.Size = new System.Drawing.Size(215, 16);
			this.registCheckBox.TabIndex = 13;
			this.registCheckBox.Text = "録画ソフトには以下のテレビ局名を渡す。";
			this.registCheckBox.UseVisualStyleBackColor = true;
			this.registCheckBox.CheckedChanged += new System.EventHandler(this.registCheckBox_CheckedChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 79);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(57, 12);
			this.label5.TabIndex = 12;
			this.label5.Text = "テレビ局名:";
			// 
			// registerdStationComboBox
			// 
			this.registerdStationComboBox.Enabled = false;
			this.registerdStationComboBox.FormattingEnabled = true;
			this.registerdStationComboBox.Location = new System.Drawing.Point(85, 132);
			this.registerdStationComboBox.Name = "registerdStationComboBox";
			this.registerdStationComboBox.Size = new System.Drawing.Size(177, 20);
			this.registerdStationComboBox.TabIndex = 11;
			// 
			// referenceTvStationTextBox
			// 
			this.referenceTvStationTextBox.Location = new System.Drawing.Point(85, 76);
			this.referenceTvStationTextBox.Name = "referenceTvStationTextBox";
			this.referenceTvStationTextBox.ReadOnly = true;
			this.referenceTvStationTextBox.Size = new System.Drawing.Size(177, 19);
			this.referenceTvStationTextBox.TabIndex = 10;
			this.referenceTvStationTextBox.Text = "島神テレビ";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 19);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(321, 48);
			this.label6.TabIndex = 9;
			this.label6.Text = "番組表の局名と録画ソフトの局名が異なる場合は設定して下さい。\r\n例) ｔｖｋ → TVKテレビ\r\n一覧からは録画ソフトにプリセットされた局名が選べます。\r\nこの設" +
				"定は全ての番組に共通です。";
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(364, 9);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(93, 35);
			this.okButton.TabIndex = 14;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(364, 50);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(93, 32);
			this.cancelButton.TabIndex = 15;
			this.cancelButton.Text = "キャンセル";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// StationTableDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(469, 173);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.registCheckBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.registerdStationComboBox);
			this.Controls.Add(this.referenceTvStationTextBox);
			this.Controls.Add(this.label6);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StationTableDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "テレビ局名";
			this.Shown += new System.EventHandler(this.StationTableForm_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox registCheckBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox registerdStationComboBox;
		private System.Windows.Forms.TextBox referenceTvStationTextBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
	}
}