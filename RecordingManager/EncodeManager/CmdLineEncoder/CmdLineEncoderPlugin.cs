//=========================================================================
///	<summary>
///		コマンドラインエンコーダクラス
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成	Dr.Kurusugawa</history>
/// <history>2010/05/12 Subversionで管理するため不要なコメント削除</history>
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
	///		コマンドラインエンコーダ プロファイル
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
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
	///		コマンドラインエンコーダ プラグインクラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	public class CmdLineEncoderPlugin : magicAnime.Encoder
	{

		public override string Name		{	get { return "コマンドライン"; }	}
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
			public int OutputType;			// 0:一時ファイル名(DOS形式) 1:保存ファイル名(DOS形式)
											// 2:一時ファイル名(LFN形式) 3:保存ファイル名(LFN形式)
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
			// プロファイル保存ディレクトリを作成しておく
			//
            string dir = ProfilePath;

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}

		//=========================================================================
		///	<summary>
		///		エンコード実行
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public override void DoEncode(
			string			sourceFile	,	// [i] ソースファイル
			ref string		outputFile	,	// [io] 出力ファイル
			EncodeProfile	_profie		,	// [i] エンコードプロファイル
			object			_ai			)	// [i] 追加パラメータ
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
				// プロファイルを読込
				//--------------------------

				CmdLineInternalProfile prof = LoadProfile(profile.profileName);

				if (prof == null)
					throw new Exception("プロファイルが見つかりません");

				bool	isShortName	=	(prof.OutputType == 0)
									||	(prof.OutputType == 1);
				bool	isTempName	=	(prof.OutputType == 0)
									||	(prof.OutputType == 2);

				//
				// 入力ファイルパス
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
					throw new Exception("エンコード元のファイルが見つかりません");

				//
				// 出力ファイルパス
				//
				string ext;

				ext = prof.Extension.Trim();
				if ( !ext.Equals( "" ) )
					ext = "." + ext;

				if( isTempName )
				{
					//------------------------------
					// 一時ファイル名を渡す
					//------------------------------
					string dir;

					strDate = DateTime.Now.ToString();
					strDate = strDate.Replace( " ", "_" ).Replace( " ", "_" ).Replace( ":", "-" ).Replace( "/", "-" );

					if (prof.OutputType == 0)
					{
						dir = FileSystem.ConvertToShortPathName(Path.GetDirectoryName(outputFile));
						if (dir == "")
							throw new Exception("DOS形式のディレクトリ名を取得できません(" + outputFile.ToString() + ")");
					}
					else
						dir = Path.GetDirectoryName(outputFile);

					outputTempName = dir + "\\ENCODED" + strDate + ext;

				}
				else if( !isTempName )
				{
					//------------------------------
					// 保存ファイル名を渡す
					//------------------------------
					string dir;

					if (prof.OutputType == 1)
					{
						dir = FileSystem.ConvertToShortPathName(Path.GetDirectoryName(outputFile));
						if (dir == "")
							throw new Exception("DOS形式のディレクトリ名を取得できません(" + outputFile.ToString() + ")");
					}
					else
						dir = Path.GetDirectoryName(outputFile);

					outputTempName = dir + "\\" + Path.GetFileNameWithoutExtension( outputFile ) + ext;
				}
				else
				{
					throw new Exception("設定が正しくありません。");
				}

				ExecEncoder(sourceTempName, outputTempName, prof, ai );

				if (!File.Exists(outputTempName))
					throw new Exception("エンコードした出力ファイルが行方不明");

				// 正式ファイル名outputFileの拡張子を変更する
				outputFile = outputFile.Substring(0, outputFile.Length - Path.GetExtension(outputFile).Length) + ext;

				// 一時ファイル名を正式名に変更する
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
		// ABSTRACT	: エンコーダEXEを実行
		//
		private void ExecEncoder(
			string							sourcePath,
			string							outputPath,
			CmdLineInternalProfile			profile,
			Encoder.TvProgramAdditionalInfo	ai)
		{
			string args;

			if (!File.Exists(profile.ExecutePath))
				throw new Exception("エンコーダの実行ファイルが見つかりません");

			// エンコーダに渡すパラメータのセット
			if ( ai != null )												// 追加情報あり
			{
				string		sourcePathWithoutEXT;
				string		outputPathWithoutEXT;

				// 拡張子なしのパスを取得
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
			else															// 追加情報なし
			{
				args = string.Format(
								profile.Argument	,
								sourcePath			,
								outputPath			);
			}

			// 実行ファイルを呼び出してエンコードする
			ProcessStartInfo psi = new ProcessStartInfo( profile.ExecutePath );

			abortEncode = false;

			psi.Arguments	= args;											// 引数
			psi.WindowStyle = ( profile.Minimize ) ?	ProcessWindowStyle.Minimized :
														ProcessWindowStyle.Normal;
			psi.WorkingDirectory = Path.GetDirectoryName( profile.ExecutePath );
//			psi.UseShellExecute = false;

			process = Process.Start( psi );

			Thread.Sleep(1000);

			if (profile.Minimize)												// ウィンドウ最小化
			{
				if( process.MainWindowHandle != null )
				{
					KernelAPI.Window.SendMessage(
						process.MainWindowHandle		,
						34								,
						(IntPtr)0						,
						(IntPtr)0						);						// WM_ICONIFY送信
				}
			}

			process.WaitForExit();

			if (abortEncode)
				throw new AbortException();

		}

		//
		// ABSTRACT	: プロファイルXMLを読み込む
		//
		static public CmdLineInternalProfile LoadProfile(string name)
		{
			CmdLineInternalProfile prof = new CmdLineInternalProfile();

			try
			{
				// XMLにプロファイルを保存
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
		// DESCRIPT	:	処理中のエンコードを中断する
		/////////////////////////////////////////////////////////////////////////////
		public override void AbortEncodeProcess()
		{
			abortEncode = true;

			process.Kill();

		}

		/////////////////////////////////////////////////////////////////////////////
		// FUNCTION	:	CreatePropertyPage
		// DESCRIPT	:	プロファイルを編集するプロパティページを作成する
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
