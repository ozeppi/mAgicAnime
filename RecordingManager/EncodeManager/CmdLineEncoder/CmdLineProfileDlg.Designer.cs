namespace magicAnime.RecordingManager.EncodeManager.CmdLineEncoder
{
	partial class CmdLineProfileDlg
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
			this.label1 = new System.Windows.Forms.Label();
			this.ArgumentTextBox = new System.Windows.Forms.TextBox();
			this.OkButton = new System.Windows.Forms.Button();
			this.MinimizeCheckBox = new System.Windows.Forms.CheckBox();
			this.ExecuteFilePathTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.ReferenceButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.label4 = new System.Windows.Forms.Label();
			this.ProfileNameTextBox = new System.Windows.Forms.TextBox();
			this.SampleTextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.ExtensionTextBox = new System.Windows.Forms.TextBox();
			this.tagComboBox = new System.Windows.Forms.ComboBox();
			this.insertTagButton = new System.Windows.Forms.Button();
			this.OutputTypeComboBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 8, 156 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 108, 12 );
			this.label1.TabIndex = 4;
			this.label1.Text = "コマンドライン引数(&C):";
			// 
			// ArgumentTextBox
			// 
			this.ArgumentTextBox.Location = new System.Drawing.Point( 11, 172 );
			this.ArgumentTextBox.Multiline = true;
			this.ArgumentTextBox.Name = "ArgumentTextBox";
			this.ArgumentTextBox.Size = new System.Drawing.Size( 309, 36 );
			this.ArgumentTextBox.TabIndex = 3;
			this.ArgumentTextBox.TextChanged += new System.EventHandler( this.ArgumentTextBox_TextChanged );
			// 
			// OkButton
			// 
			this.OkButton.Location = new System.Drawing.Point( 353, 12 );
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size( 108, 30 );
			this.OkButton.TabIndex = 6;
			this.OkButton.Text = "OK";
			this.OkButton.UseVisualStyleBackColor = true;
			this.OkButton.Click += new System.EventHandler( this.OkButton_Click );
			// 
			// MinimizeCheckBox
			// 
			this.MinimizeCheckBox.AutoSize = true;
			this.MinimizeCheckBox.Location = new System.Drawing.Point( 12, 94 );
			this.MinimizeCheckBox.Name = "MinimizeCheckBox";
			this.MinimizeCheckBox.Size = new System.Drawing.Size( 203, 16 );
			this.MinimizeCheckBox.TabIndex = 7;
			this.MinimizeCheckBox.Text = "エンコーダのウィンドウを最小化する(&M)";
			this.MinimizeCheckBox.UseVisualStyleBackColor = true;
			// 
			// ExecuteFilePathTextBox
			// 
			this.ExecuteFilePathTextBox.Font = new System.Drawing.Font( "MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)) );
			this.ExecuteFilePathTextBox.Location = new System.Drawing.Point( 12, 69 );
			this.ExecuteFilePathTextBox.Name = "ExecuteFilePathTextBox";
			this.ExecuteFilePathTextBox.ReadOnly = true;
			this.ExecuteFilePathTextBox.Size = new System.Drawing.Size( 239, 20 );
			this.ExecuteFilePathTextBox.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point( 10, 54 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 144, 12 );
			this.label3.TabIndex = 9;
			this.label3.Text = "エンコーダ実行ファイルパス(&X):";
			// 
			// ReferenceButton
			// 
			this.ReferenceButton.Location = new System.Drawing.Point( 257, 58 );
			this.ReferenceButton.Name = "ReferenceButton";
			this.ReferenceButton.Size = new System.Drawing.Size( 63, 35 );
			this.ReferenceButton.TabIndex = 10;
			this.ReferenceButton.Text = "参照(&R)";
			this.ReferenceButton.UseVisualStyleBackColor = true;
			this.ReferenceButton.Click += new System.EventHandler( this.ReferenceButton_Click );
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "EXE";
			this.openFileDialog.FileName = "openFileDialog";
			this.openFileDialog.Filter = "実行ファイル(*.EXE)|*.EXE|バッチファイル(*.BAT)|*.BAT";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point( 10, 7 );
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size( 87, 12 );
			this.label4.TabIndex = 12;
			this.label4.Text = "プロファイル名(&N):";
			// 
			// ProfileNameTextBox
			// 
			this.ProfileNameTextBox.Font = new System.Drawing.Font( "MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)) );
			this.ProfileNameTextBox.Location = new System.Drawing.Point( 12, 22 );
			this.ProfileNameTextBox.MaxLength = 100;
			this.ProfileNameTextBox.Name = "ProfileNameTextBox";
			this.ProfileNameTextBox.Size = new System.Drawing.Size( 308, 20 );
			this.ProfileNameTextBox.TabIndex = 11;
			// 
			// SampleTextBox
			// 
			this.SampleTextBox.Location = new System.Drawing.Point( 12, 293 );
			this.SampleTextBox.Multiline = true;
			this.SampleTextBox.Name = "SampleTextBox";
			this.SampleTextBox.ReadOnly = true;
			this.SampleTextBox.Size = new System.Drawing.Size( 309, 36 );
			this.SampleTextBox.TabIndex = 13;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point( 9, 278 );
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size( 45, 12 );
			this.label5.TabIndex = 14;
			this.label5.Text = "サンプル:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point( 10, 127 );
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size( 125, 12 );
			this.label6.TabIndex = 15;
			this.label6.Text = "エンコード後の拡張子(&E):";
			// 
			// ExtensionTextBox
			// 
			this.ExtensionTextBox.Font = new System.Drawing.Font( "MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)) );
			this.ExtensionTextBox.Location = new System.Drawing.Point( 141, 124 );
			this.ExtensionTextBox.MaxLength = 3;
			this.ExtensionTextBox.Name = "ExtensionTextBox";
			this.ExtensionTextBox.Size = new System.Drawing.Size( 37, 22 );
			this.ExtensionTextBox.TabIndex = 16;
			this.ExtensionTextBox.Text = "AVI";
			// 
			// tagComboBox
			// 
			this.tagComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.tagComboBox.FormattingEnabled = true;
			this.tagComboBox.Items.AddRange( new object[] {
            "{0} 入力ファイル名",
            "{1} 出力ファイル名",
            "{2} 番組タイトル",
            "{3} 番組話数",
            "{4} 番組サブタイトル",
            "{5} 放送局",
            "{6} 放送日(24時間)",
            "{7} 放送時刻(24時間)",
            "{8} 入力ファイル名(拡張子なし)",
            "{9} 出力ファイル名(拡張子なし)"} );
			this.tagComboBox.Location = new System.Drawing.Point( 12, 216 );
			this.tagComboBox.Name = "tagComboBox";
			this.tagComboBox.Size = new System.Drawing.Size( 210, 20 );
			this.tagComboBox.TabIndex = 17;
			// 
			// insertTagButton
			// 
			this.insertTagButton.Location = new System.Drawing.Point( 227, 212 );
			this.insertTagButton.Name = "insertTagButton";
			this.insertTagButton.Size = new System.Drawing.Size( 93, 26 );
			this.insertTagButton.TabIndex = 18;
			this.insertTagButton.Text = "タグ挿入";
			this.insertTagButton.UseVisualStyleBackColor = true;
			this.insertTagButton.Click += new System.EventHandler( this.insertTagButton_Click );
			// 
			// OutputTypeComboBox
			// 
			this.OutputTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.OutputTypeComboBox.FormattingEnabled = true;
			this.OutputTypeComboBox.Items.AddRange( new object[] {
            "出力ファイル名に一時ファイル名(DOS形式)を渡す",
            "出力ファイル名に保存ファイル名(DOS形式)を渡す",
            "出力ファイル名に一時ファイル名(正式名)を渡す",
            "出力ファイル名に保存ファイル名(正式名)を渡す"} );
			this.OutputTypeComboBox.Location = new System.Drawing.Point( 10, 244 );
			this.OutputTypeComboBox.Name = "OutputTypeComboBox";
			this.OutputTypeComboBox.Size = new System.Drawing.Size( 310, 20 );
			this.OutputTypeComboBox.TabIndex = 19;
			// 
			// CmdLineProfileDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 475, 337 );
			this.Controls.Add( this.OutputTypeComboBox );
			this.Controls.Add( this.insertTagButton );
			this.Controls.Add( this.tagComboBox );
			this.Controls.Add( this.ExtensionTextBox );
			this.Controls.Add( this.label6 );
			this.Controls.Add( this.label5 );
			this.Controls.Add( this.SampleTextBox );
			this.Controls.Add( this.label4 );
			this.Controls.Add( this.ProfileNameTextBox );
			this.Controls.Add( this.ReferenceButton );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.ExecuteFilePathTextBox );
			this.Controls.Add( this.MinimizeCheckBox );
			this.Controls.Add( this.OkButton );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.ArgumentTextBox );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CmdLineProfileDlg";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "コマンドライン プロファイル";
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.CheckBox MinimizeCheckBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button ReferenceButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		public System.Windows.Forms.TextBox ArgumentTextBox;
		public System.Windows.Forms.TextBox ExecuteFilePathTextBox;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.TextBox ProfileNameTextBox;
		public System.Windows.Forms.TextBox SampleTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		public System.Windows.Forms.TextBox ExtensionTextBox;
		private System.Windows.Forms.ComboBox tagComboBox;
		private System.Windows.Forms.Button insertTagButton;
		public System.Windows.Forms.ComboBox OutputTypeComboBox;
	}
}