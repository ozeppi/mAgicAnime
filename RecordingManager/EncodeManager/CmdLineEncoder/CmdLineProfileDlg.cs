//=========================================================================
///	<summary>
///		�R�}���h���C���G���R�[�_�v���t�@�C���N���X
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬	Dr.Kurusugawa</history>
/// <history>2010/05/01 Subversion�ŊǗ����邽�ߕs�v�ȃR�����g�폜</history>
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
					"�ݒ肳��ĂȂ��p�����[�^������܂��B", "�G���[",
					MessageBoxButtons.OK, MessageBoxIcon.Warning  );
				return;
			}

			// XML�Ƀv���t�@�C����ۑ�
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
		// ABSTRACT	: �v���t�@�C����ǂݍ���
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
					@"C:\���[�r�[\�^��.MPG"								,
					@"C:\�G���R�[�h\�G���R��." + ExtensionTextBox.Text	,
					"�܂����ɂ˂���"									,
					1													,
					"���z�����A�j�����C�t"								,
					"��s�e���r"										,
					DateTime.Now.ToShortDateString()					,
					DateTime.Now.ToShortTimeString()					,
					@"C:\���[�r�[\�^��"									,
					@"C:\�G���R�[�h\�G���R��"							);

				SampleTextBox.Text = str;
			}catch(Exception ex)
			{
			
				SampleTextBox.Text = "�����̏���������������܂���";
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