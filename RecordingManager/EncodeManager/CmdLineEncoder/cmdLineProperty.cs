//=========================================================================
///	<summary>
///		コマンドラインエンコーダプロパティクラス
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成	Dr.Kurusugawa</history>
/// <history>2010/05/01 Subversionで管理するため不要なコメント削除</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace magicAnime.RecordingManager.EncodeManager.CmdLineEncoder
{
	public partial class cmdLineProperty : Form
	{
		private CmdLineEncoderProfile profile;
		public cmdLineProperty()
		{
			InitializeComponent();
		}

		public void ShowPage(EncodeProfile profile)
		{
			this.profile = (CmdLineEncoderProfile)profile;

			UpdateProfileList();

			Show();

		}

		//
		// ABSTRACT	 : プロファイルの一覧を更新する
		//
		private void UpdateProfileList()
		{
			ProfileComboBox.Items.Clear();

			CmdLineEncoderPlugin.CreateProfileDirectory();
            string      dir = CmdLineEncoder.CmdLineEncoderPlugin.ProfilePath;

			string[] files	= Directory.GetFiles( dir, "*.XML" );

			foreach (string file in files)
			{
				ProfileComboBox.Items.Add(Path.GetFileNameWithoutExtension(file));
			}

			ProfileComboBox.Text = this.profile.profileName;
		}

		//
		// ABSTRACT	 : 新規プロファイルボタン
		//
		private void NewProfileButton_Click(object sender, EventArgs e)
		{
			CmdLineProfileDlg dlg = new CmdLineProfileDlg();

			dlg.ExecuteFilePathTextBox.Text		= "";
			dlg.ArgumentTextBox.Text			= "\"{0}\" \"{1}\"";
			dlg.ProfileNameTextBox.Text			= "新規プロファイル";
			dlg.ExtensionTextBox.Text			= "AVI";
			dlg.OutputTypeComboBox.SelectedIndex	= 3;

			dlg.ShowDialog();

			UpdateProfileList();
		}

		//
		// ABSTRACT	 : プロファイル変更ボタン
		//
		private void ModifyButton_Click(object sender, EventArgs e)
		{
			if (ProfileComboBox.Text.Equals(""))
				return;

			CmdLineProfileDlg dlg = new CmdLineProfileDlg();
            string dir = CmdLineEncoder.CmdLineEncoderPlugin.ProfilePath;

			dlg.LoadProfile( Path.ChangeExtension( Path.Combine( dir, ProfileComboBox.Text ), ".XML" ) );
			dlg.ShowDialog();

			UpdateProfileList();
		}

		private void ProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			profile.profileName = ProfileComboBox.Text;
		}


	}

}

