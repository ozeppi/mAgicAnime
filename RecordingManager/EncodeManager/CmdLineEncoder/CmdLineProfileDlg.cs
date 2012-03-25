//=========================================================================
///	<summary>
///		コマンドラインエンコーダプロファイルクラス
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
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace magicAnime.RecordingManager.EncodeManager.CmdLineEncoder
{
	public partial class CmdLineProfileDlg : Form
	{
		public CmdLineProfileDlg()
		{
			InitializeComponent();
		}

		private void ReferenceButton_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				ExecuteFilePathTextBox.Text = openFileDialog.FileName;
			}
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			if (ExecuteFilePathTextBox.Text.Equals("")	||
				ProfileNameTextBox.Text.Equals("")		||
				OutputTypeComboBox.SelectedIndex < 0	)
			{
				MessageBox.Show(
					"設定されてないパラメータがあります。", "エラー",
					MessageBoxButtons.OK, MessageBoxIcon.Warning  );
				return;
			}

			// XMLにプロファイルを保存
            string  dir;
            string  path;
            dir = CmdLineEncoder.CmdLineEncoderPlugin.ProfilePath;
            path = Path.Combine( dir, ProfileNameTextBox.Text );
            path = Path.ChangeExtension( path, @".XML");

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			XmlTextWriter	xw = new XmlTextWriter( path, Encoding.Unicode );

			xw.Formatting = System.Xml.Formatting.Indented;

			xw.WriteStartDocument();
			xw.WriteStartElement("CmdLineProfile");

			xw.WriteElementString("Execute", ExecuteFilePathTextBox.Text);
			xw.WriteElementString("Argument", ArgumentTextBox.Text);
			xw.WriteElementString("Extension", ExtensionTextBox.Text);
			xw.WriteElementString("Minimize", MinimizeCheckBox.Checked ? "1" : "0");
			xw.WriteElementString( "OutputType", OutputTypeComboBox.SelectedIndex.ToString() );

			xw.WriteEndElement();
			xw.WriteEndDocument();

			xw.Close();

			//

			Close();
		}

		//
		// ABSTRACT	: プロファイルを読み込む
		//
		public bool LoadProfile(string path)
		{
			if (!File.Exists(path))
				return false;

			XmlReader xr = new XmlTextReader(path);

			while (xr.Read())
			{
				if (xr.NodeType == XmlNodeType.Element)
				{
					if (xr.LocalName.Equals("Execute"))
					{
						ExecuteFilePathTextBox.Text = xr.ReadElementContentAsString();
					}
					else if (xr.LocalName.Equals("Argument"))
					{
						ArgumentTextBox.Text = xr.ReadElementContentAsString();
					}
					else if (xr.LocalName.Equals("Extension"))
					{
						ExtensionTextBox.Text = xr.ReadElementContentAsString();
					}
					else if (xr.LocalName.Equals("Minimize"))
					{
						MinimizeCheckBox.Checked = (xr.ReadElementContentAsInt()==0)?false:true;
					}
					else if ( xr.LocalName.Equals( "OutputType" ) )
					{
						OutputTypeComboBox.SelectedIndex = xr.ReadElementContentAsInt();
					}
				}
			
			}

			ProfileNameTextBox.Text = Path.GetFileNameWithoutExtension(path);

			xr.Close();

			return true;
		}

		private void ArgumentTextBox_TextChanged(object sender, EventArgs e)
		{
			string str;

			try
			{
				str = string.Format(
					ArgumentTextBox.Text								,
					@"C:\ムービー\録画.MPG"								,
					@"C:\エンコード\エンコ済." + ExtensionTextBox.Text	,
					"まじあにねっと"									,
					1													,
					"東奔西走アニメライフ"								,
					"帝都テレビ"										,
					DateTime.Now.ToShortDateString()					,
					DateTime.Now.ToShortTimeString()					,
					@"C:\ムービー\録画"									,
					@"C:\エンコード\エンコ済"							);

				SampleTextBox.Text = str;
			}catch(Exception ex)
			{
			
				SampleTextBox.Text = "引数の書式が正しくありません";
			}
		}

		private void insertTagButton_Click(object sender, EventArgs e)
		{
			if (0 <= tagComboBox.SelectedIndex)
			{
				ArgumentTextBox.Text += string.Format( "{{{0}}}", tagComboBox.SelectedIndex );
			
			}

		}

	}
}