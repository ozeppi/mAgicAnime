//=========================================================================
///	<summary>
///		�R�}���h���C���G���R�[�_�N���X
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬	Dr.Kurusugawa</history>
/// <history>2010/05/12 Subversion�ŊǗ����邽�ߕs�v�ȃR�����g�폜</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using KernelAPI;
using magicAnime.Properties;

namespace magicAnime.RecordingManager.EncodeManager.CmdLineEncoder
{
	//=========================================================================
	///	<summary>
	///		�R�}���h���C���G���R�[�_ �v���t�@�C��
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public class CmdLineEncoderProfile : magicAnime.EncodeProfile
	{
		public string profileName;

		public override string TagName { get { return "cmdLineProfile"; } }
		public override string ToString()
		{
			return profileName;
		}

		public CmdLineEncoderProfile()
		{
			profileName = "";
		}

		public override void Write(XmlWriter xmlWriter)
		{
			xmlWriter.WriteElementString("ProfileXmlName", profileName);
		}

		public override void Read(XmlReader xmlReader)
		{

			while (xmlReader.Read())
			{

				if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (xmlReader.LocalName.Equals("ProfileXmlName"))
						profileName = xmlReader.ReadElementContentAsString();
				}
				else if (xmlReader.NodeType == System.Xml.XmlNodeType.EndElement)
					if (xmlReader.LocalName.Equals( TagName ))
						return;

			}

		}

	}

	//=========================================================================
	///	<summary>
	///		�R�}���h���C���G���R�[�_ �v���O�C���N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public class CmdLineEncoderPlugin : magicAnime.Encoder
	{

		public override string Name		{	get { return "�R�}���h���C��"; }	}
		public override string Extension	{	get { return ".AVI"; } }
		public override Type ProfileType	{	get { return typeof(CmdLineEncoderProfile); } }

		private bool abortEncode;
		private System.Diagnostics.Process process;

		//
		//
		//
		public class CmdLineInternalProfile
		{
			public string ProfileName;
			public string Argument;
			public string ExecutePath;
			public string Extension;
			public bool Minimize;
			public int OutputType;			// 0:�ꎞ�t�@�C����(DOS�`��) 1:�ۑ��t�@�C����(DOS�`��)
											// 2:�ꎞ�t�@�C����(LFN�`��) 3:�ۑ��t�@�C����(LFN�`��)
		}
        public static string ProfilePath
        {
            get
            {
                return Path.Combine( Program.AppDataPath, "CmdLineProfiles" );
            }
        }

		public static void CreateProfileDirectory()
		{
			//
			// �v���t�@�C���ۑ��f�B���N�g�����쐬���Ă���
			//
            string dir = ProfilePath;

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}

		//=========================================================================
		///	<summary>
		///		�G���R�[�h���s
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public override void DoEncode(
			string			sourceFile	,	// [i] �\�[�X�t�@�C��
			ref string		outputFile	,	// [io] �o�̓t�@�C��
			EncodeProfile	_profie		,	// [i] �G���R�[�h�v���t�@�C��
			object			_ai			)	// [i] �ǉ��p�����[�^
		{
			string strDate;
			string sourceTempName = null;
			string outputTempName = null;
			CmdLineEncoderProfile			profile	= (CmdLineEncoderProfile)_profie;
			Encoder.TvProgramAdditionalInfo	ai		= null;

			if ( _ai.GetType() == typeof(magicAnime.Encoder.TvProgramAdditionalInfo) )
			{
				ai = (Encoder.TvProgramAdditionalInfo)_ai;
			}

			try
			{
				//--------------------------
				// �v���t�@�C����Ǎ�
				//--------------------------

				CmdLineInternalProfile prof = LoadProfile(profile.profileName);

				if (prof == null)
					throw new Exception("�v���t�@�C����������܂���");

				bool	isShortName	=	(prof.OutputType == 0)
									||	(prof.OutputType == 1);
				bool	isTempName	=	(prof.OutputType == 0)
									||	(prof.OutputType == 2);

				//
				// ���̓t�@�C���p�X
				//
				if( isShortName	)
				{
					sourceTempName = FileSystem.ConvertToShortPathName( sourceFile );
				}
				else
				{
					sourceTempName = sourceFile;
				}

				if (!File.Exists(sourceTempName))
					throw new Exception("�G���R�[�h���̃t�@�C����������܂���");

				//
				// �o�̓t�@�C���p�X
				//
				string ext;

				ext = prof.Extension.Trim();
				if ( !ext.Equals( "" ) )
					ext = "." + ext;

				if( isTempName )
				{
					//------------------------------
					// �ꎞ�t�@�C������n��
					//------------------------------
					string dir;

					strDate = DateTime.Now.ToString();
					strDate = strDate.Replace( " ", "_" ).Replace( " ", "_" ).Replace( ":", "-" ).Replace( "/", "-" );

					if (prof.OutputType == 0)
					{
						dir = FileSystem.ConvertToShortPathName(Path.GetDirectoryName(outputFile));
						if (dir == "")
							throw new Exception("DOS�`���̃f�B���N�g�������擾�ł��܂���(" + outputFile.ToString() + ")");
					}
					else
						dir = Path.GetDirectoryName(outputFile);

					outputTempName = dir + "\\ENCODED" + strDate + ext;

				}
				else if( !isTempName )
				{
					//------------------------------
					// �ۑ��t�@�C������n��
					//------------------------------
					string dir;

					if (prof.OutputType == 1)
					{
						dir = FileSystem.ConvertToShortPathName(Path.GetDirectoryName(outputFile));
						if (dir == "")
							throw new Exception("DOS�`���̃f�B���N�g�������擾�ł��܂���(" + outputFile.ToString() + ")");
					}
					else
						dir = Path.GetDirectoryName(outputFile);

					outputTempName = dir + "\\" + Path.GetFileNameWithoutExtension( outputFile ) + ext;
				}
				else
				{
					throw new Exception("�ݒ肪����������܂���B");
				}

				ExecEncoder(sourceTempName, outputTempName, prof, ai );

				if (!File.Exists(outputTempName))
					throw new Exception("�G���R�[�h�����o�̓t�@�C�����s���s��");

				// �����t�@�C����outputFile�̊g���q��ύX����
				outputFile = outputFile.Substring(0, outputFile.Length - Path.GetExtension(outputFile).Length) + ext;

				// �ꎞ�t�@�C�����𐳎����ɕύX����
				if( isTempName )
				{
					File.Move( outputTempName, outputFile );
				}

			}
			catch (AbortException e)
			{
				if (outputTempName!=null && File.Exists(outputTempName))
					File.Delete(outputTempName);
				throw e;
			}


		}


		//
		// ABSTRACT	: �G���R�[�_EXE�����s
		//
		private void ExecEncoder(
			string							sourcePath,
			string							outputPath,
			CmdLineInternalProfile			profile,
			Encoder.TvProgramAdditionalInfo	ai)
		{
			string args;

			if (!File.Exists(profile.ExecutePath))
				throw new Exception("�G���R�[�_�̎��s�t�@�C����������܂���");

			// �G���R�[�_�ɓn���p�����[�^�̃Z�b�g
			if ( ai != null )												// �ǉ���񂠂�
			{
				string		sourcePathWithoutEXT;
				string		outputPathWithoutEXT;

				// �g���q�Ȃ��̃p�X���擾
				sourcePathWithoutEXT	=	Path.Combine( Path.GetDirectoryName( sourcePath )	,
											Path.GetFileNameWithoutExtension( sourcePath )		);
				outputPathWithoutEXT	=	Path.Combine( Path.GetDirectoryName( outputPath )	,
											Path.GetFileNameWithoutExtension( outputPath )		);

				args = string.Format(
								profile.Argument	,
								sourcePath			,
								outputPath			,
								ai.Title			,
								ai.StoryNumber		,
								ai.Subtitle			,
								ai.TvStation		,
								ai.StartDate		,
								ai.StartTime		,
								sourcePathWithoutEXT,
								outputPathWithoutEXT);
			}
			else															// �ǉ����Ȃ�
			{
				args = string.Format(
								profile.Argument	,
								sourcePath			,
								outputPath			);
			}

			// ���s�t�@�C�����Ăяo���ăG���R�[�h����
			ProcessStartInfo psi = new ProcessStartInfo( profile.ExecutePath );

			abortEncode = false;

			psi.Arguments	= args;											// ����
			psi.WindowStyle = ( profile.Minimize ) ?	ProcessWindowStyle.Minimized :
														ProcessWindowStyle.Normal;
			psi.WorkingDirectory = Path.GetDirectoryName( profile.ExecutePath );
//			psi.UseShellExecute = false;

			process = Process.Start( psi );

			Thread.Sleep(1000);

			if (profile.Minimize)												// �E�B���h�E�ŏ���
			{
				if( process.MainWindowHandle != null )
				{
					KernelAPI.Window.SendMessage(
						process.MainWindowHandle		,
						34								,
						(IntPtr)0						,
						(IntPtr)0						);						// WM_ICONIFY���M
				}
			}

			process.WaitForExit();

			if (abortEncode)
				throw new AbortException();

		}

		//
		// ABSTRACT	: �v���t�@�C��XML��ǂݍ���
		//
		static public CmdLineInternalProfile LoadProfile(string name)
		{
			CmdLineInternalProfile prof = new CmdLineInternalProfile();

			try
			{
				// XML�Ƀv���t�@�C����ۑ�
				string  path;
                path = Path.Combine( ProfilePath, name.Trim() + @".XML" );

				if (!File.Exists(path))
					return null;

				XmlReader xr = new XmlTextReader(path);

				while (xr.Read())
				{
					if (xr.NodeType == XmlNodeType.Element)
					{
						if (xr.LocalName.Equals("Execute"))
						{
							prof.ExecutePath = xr.ReadElementContentAsString();
						}
						else if (xr.LocalName.Equals("Argument"))
						{
							prof.Argument = xr.ReadElementContentAsString();
						}
						else if (xr.LocalName.Equals("Extension"))
						{
							prof.Extension = xr.ReadElementContentAsString();
						}
						else if (xr.LocalName.Equals("Minimize"))
						{
							prof.Minimize = (xr.ReadElementContentAsInt() == 0) ? false : true;
						}
						else if ( xr.LocalName.Equals( "OutputType" ) )
						{
							prof.OutputType = xr.ReadElementContentAsInt();
						}
					}

				}

				prof.ProfileName = Path.GetFileNameWithoutExtension(path);

				xr.Close();

				return prof;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		/////////////////////////////////////////////////////////////////////////////
		// FUNCTION	:	AbortEncodeProcess
		// DESCRIPT	:	�������̃G���R�[�h�𒆒f����
		/////////////////////////////////////////////////////////////////////////////
		public override void AbortEncodeProcess()
		{
			abortEncode = true;

			process.Kill();

		}

		/////////////////////////////////////////////////////////////////////////////
		// FUNCTION	:	CreatePropertyPage
		// DESCRIPT	:	�v���t�@�C����ҏW����v���p�e�B�y�[�W���쐬����
		/////////////////////////////////////////////////////////////////////////////
		public override Form CreatePropertyPage(
			Control parent,
			EncodeProfile profile)
		{
			cmdLineProperty page = new cmdLineProperty();

			page.TopLevel	= false;
			page.Parent		= parent;
			page.ShowPage(profile);

			return page;
		}

		public override void ShowOptionDialog()
		{
//			Form dlg = new aviutlOptionDlg();
//			dlg.ShowDialog();

		}

	}

}
