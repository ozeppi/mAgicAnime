//=========================================================================
///	<summary>
///		mAgicAnimeアプリケーションモジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成	Dr.Kurusugawa</history>
/// <history>2010/02/20 古いコメントを削除</history>
/// <history>2010/05/01 Subversionで管理するため不要なコメント削除</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System.Management;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using magicAnime.Properties;
using Microsoft.Win32;
using Helpers;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		mAgicAnimeアプリケーションクラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	/// <history>2008/05/02 メンバ変数を先頭に移動</history>
	/// <history>2008/05/02 オプション変更イベントを追加</history>
	//=========================================================================
	static class Program
	{
		//---------------------
		// 初期化終了管理用
		//---------------------
		internal static bool			mCleanupped	= false;
		private static bool				mShutDown	= false;	// メッセージループを出た後にOSシャットダウン

		//---------------------
		// ログ
		//---------------------
		private static TextLogger		mTextLogger;
		private static MemoryLogger		mMemoryLogger;

		//---------------------
		// GUI
		//---------------------
		internal static ApplicationContext		mAppContext;
		internal static MainForm				mMainForm;
		internal static UserInterface.TrayIcon	mTrayIcon;

		//--------------------------
		// アプリ終了確認イベント
		//--------------------------
		public class AppClosingEventArags : EventArgs
		{
			public bool Cancel;
		}
		public delegate void AppClosingEventHandler(object sender, AppClosingEventArags e);
		public static event AppClosingEventHandler AppClosing;

		//-------------------------
		// オプション変更イベント
		//-------------------------
		internal delegate void OptionChangedEvent(object e,EventArgs a);
		internal static OptionChangedEvent	OptionChanged;

		//-------------------
		// その他
		//-------------------
		// エラーメッセージボックスをメインスレッドで表示するためのデリゲート
		private delegate void ShowExceptionDelegate(Exception e, MessageBoxIcon icon);

// <ADD> 2010/04/17 デバッグオプション ->
#if DEBUG
		internal class _DebugOption
		{
			internal bool	mForceEmpty;

			internal _DebugOption()
			{
				mForceEmpty = false;
			}
		};

		private static _DebugOption	mDebugOption = new _DebugOption();
#endif
// <ADD> 2010/04/17 <-

		//=========================================================================
		///	<summary>
		///		Mainプロシージャ
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
//		[MTAThread]
		[STAThread]
		static void Main()
		{
			//------------------------
			// 多重起動を防止する
			//------------------------
			// 同じ場所から複数インスタンスが起動された場合は弾くよう修正
			// 異なる場所から起動された場合は多重起動を認める。
			try
			{
				Process		myProcess;

				Process	[]procs = Process.GetProcesses();
				myProcess = Process.GetCurrentProcess();

				foreach( Process proc in procs  )
				{
					try
					{
						if( (proc.Id != myProcess.Id)									&&
							(proc.MainModule.FileName == myProcess.MainModule.FileName)	)
						{
							MessageBox.Show(
								"mAgicAnimeは既に起動しています。\n" +
								"(別々のパスから起動すれば複数起動できます)",
								null										,
								MessageBoxButtons.OK						,
								MessageBoxIcon.Warning						);
							return;
						}
					}
					catch( Exception ex )
					{
						// プロセスによっては弾かれるため
					}
				}
			}
			catch( Exception ex )
			{
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			try
			{
                //-----------------------------
                // アプリデータフォルダ作成
                //-----------------------------
                try
                {
                    if( !Directory.Exists(AppDataPath) )
                    {
                        Directory.CreateDirectory(AppDataPath);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("データフォルダの準備中にエラーが発生しました。",
                                    null,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                    return;
                }
				//--------------------------
                // バージョンアップ時処理
				//--------------------------
                List<string>    upgradedLog = new List<string>();
                CheckUpgraded( ref upgradedLog );

				//------------------------------------
                // 初回起動時のコンフィギュレーション
				//------------------------------------
				AppConfiguration( ref upgradedLog );

				//--------------------------
				// ロガーの初期化
				//--------------------------
				mTextLogger		= new TextLogger(LogPath);
				mMemoryLogger	= new MemoryLogger();
				var teeLogger	= new TeeLogger( new List<Logger>{ mTextLogger, mMemoryLogger } );
				teeLogger.SetDefault();

				Logger.Output("------------------------------------------------");
				Logger.Output("起動(Ver " + Application.ProductVersion + ")");

                for( int i = 0 ; i < upgradedLog.Count ; ++i )
                    Logger.Output( upgradedLog[i] );


				//--------------------------
				// プラグインの初期化
				//--------------------------
				ReserveManager.InitPlugins();
				EncodeManager.InitPlugins();


				//--------------------------
				// メインフォームの初期化
				//--------------------------

				mMainForm = new MainForm();

				SystemEvents.SessionEnding += new SessionEndingEventHandler( OnSessionClosing );

				//------------------------
				// トレイアイコン準備
				//------------------------
				mTrayIcon				= new UserInterface.TrayIcon();
				mTrayIcon.Initialize();

				//------------------------
				// その他の初期化
				//------------------------
				OptionChanged += ApplyIconOption;
				// オプション変更時にデータ更新
				OptionChanged += delegate (object sender, EventArgs args)
				{
					AnimeServer.GetInstance().BeginUpdate( 0 );

					PathHelper.SetFileNameRule( Settings.Default.fileTitleMode );
				};

				PathHelper.SetFileNameRule( Settings.Default.fileTitleMode );

				//------------------------------------
				// 起動時に情報更新(オプション指定時)
				//------------------------------------
				if( Settings.Default.bootRefresh )
				{
					AnimeServer server = AnimeServer.GetInstance();
					server.BeginUpdate( 0 );
				}

				if( !Settings.Default.inTaskTray )
					mMainForm.Show();

				// 未読機能をOFFの場合は表示しない
				if(	!Settings.Default.disableUnread
				&&	Settings.Default.notifyUnreadBalloon )
				{
					mTrayIcon.PopupUnreadBalloon(); // 未読を表示(オプション設定時)
				}

				mAppContext = new ApplicationContext();
				mAppContext.MainForm = new UserInterface.DummyForm();

				Application.Run( mAppContext );

				mTrayIcon.mNotifyIcon.Visible = false; // 残骸が残るため
				mTrayIcon.Cleanup();

//				Application.Run( mainForm );
			}
			catch (Exception ex)
			{
				ShowException(ex, MessageBoxIcon.Exclamation);
			}
			finally
			{
				Cleanup();
			}

			// メッセージループの外側でシャットダウンをかける
            if( mShutDown )
            {
                ComputerPowerDown();
            }
			
		}

		//=========================================================================
		///	<summary>
		///		アプリケーションのクリーンアップ
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/03/23 新規作成</history>
		//=========================================================================
		internal static void Cleanup()
		{
			if( mCleanupped )
				return;
			else
				mCleanupped = true;

			// プラグインをアンロード
			ReserveManager.CleanupPlugins();

			AnimeServer.GetInstance().Save();
			AnimeServer.GetInstance().Dispose();

			Logger.Output("終了");
			Logger.GetInstance().Dispose();
		}

		//=========================================================================
		///	<summary>
		///		OSのセッション終了時のハンドラ
		/// </summary>
		/// <remarks>
		///		放送時間直前や放送中ならシャットダウンを拒否する。
		/// </remarks>
		/// <history>2008/03/26 新規作成</history>
		//=========================================================================
		private static void OnSessionClosing( object sender, SessionEndingEventArgs args )
		{
			Logger.Output( "Windowsセッション終了" );

//// <TEST> 2009/03/25 ->
//            if (true)
//            {
//                MessageBox.Show("シャットダウンをキャンセルしました。");
//                Logger.Output("...放送中または直前のため終了をキャンセルします");
//                args.Cancel = true;
//                return;
//            }
//// <TEST> 2009/03/25 <-

			Cleanup();	// メッセージループを出てからでは間に合わない場合がある

			Application.Exit();
		}

		//=========================================================================
		///	<summary>
		///		ログのパス名を返す
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public static string LogPath
		{
			get
			{
				string filePath;
                filePath = Path.Combine( AppDataPath, "mAgicAnime.log" );

				return filePath;
			}
		}

		//=========================================================================
		///	<summary>
		///		アプリケーションを閉じる前の問い合わせ
		/// </summary>
		/// <remarks>
		///		falseを返した場合、呼び出し側は原則としてアプリケーションを
		///		終了させてはならない。
		/// </remarks>
		/// <history>2007/12/23 新規作成</history>
		//=========================================================================
		internal static bool QueryClose()
		{
			AppClosingEventArags	e		= new AppClosingEventArags();
			bool					cancel	= false;

			//--------------------------------------
			// 終了確認イベントを実行して成否を取得
			//--------------------------------------
			if (AppClosing != null)
				AppClosing(null, e);

			cancel = e.Cancel;

			return !cancel;
		}

		//=========================================================================
		///	<summary>
		///		メインウィンドウを表示
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/12/22 新規作成</history>
		//=========================================================================
		internal static void ShowMainForm()
		{
			if( mMainForm != null )
			{
				mMainForm.Show();
//				Application.Run( appContext );
			}
		}

		//=========================================================================
		///	<summary>
		///		トレイアイコンを表示
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/12/22 新規作成</history>
		//=========================================================================
		internal static void ShowTrayIcon()
		{
			if( mTrayIcon != null )
				mTrayIcon.mNotifyIcon.Visible = true;
		}

		//=========================================================================
		///	<summary>
		///		アイコン関連のオプションを適用
		/// </summary>
		/// <remarks>
		///		オプション変更イベントをトリガとして呼び出される.
		/// </remarks>
		/// <history>2007/12/22 新規作成</history>
		/// <history>2008/05/02 メソッド名変更</history>
		//=========================================================================
		internal static void ApplyIconOption(object e,EventArgs a)
		{
			if( Settings.Default.inTaskTray )
			{
				mTrayIcon.mNotifyIcon.Visible = true;
//				appContext.MainForm = mainForm;
			}
			else
			{
				mTrayIcon.mNotifyIcon.Visible = false;
//				appContext.MainForm	= null;
			}
		}
		
		//=========================================================================
		///	<summary>
		///		オプションに従ったシャットダウンを試みる
		/// </summary>
		/// <remarks>
		///		trueを返した場合はシャットダウンに入るため、呼び出し側は
		///		メインスレッドをExitすること。サスペンドの場合はfalseが返る。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/03/24 メソッド名変更(QueryShutdown->TryShutdown)</history>
		//=========================================================================
		public static bool TryShutdown()
		{
			Logger.Output("mAgicAnimeによるシャットダウン");

			if (Settings.Default.autoShutdownType == 0)
			{
				//--------------------
				// 電源を切断
				//--------------------

				// メッセージループの外側でシャットダウンをかける
				mShutDown = true;
				Application.Exit();

				return true;
			}
			else if (Settings.Default.autoShutdownType == 1)
			{
				//-----------------
				// サスペンド実行
				//-----------------
				Application.SetSuspendState( PowerState.Suspend, false, false );
			}
			else if (Settings.Default.autoShutdownType == 2)
			{
				//-----------------
				// 休止実行
				//-----------------
				Application.SetSuspendState( PowerState.Hibernate, false, false );
			}
			return false;
		}

		//=========================================================================
		///	<summary>
		///		シャットダウンするためのセキュリティ特権を有効にする
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
        private static void AdjustToken()
        {
            const uint		TOKEN_ADJUST_PRIVILEGES		= 0x20;
            const uint		TOKEN_QUERY					= 0x8;
            const int		SE_PRIVILEGE_ENABLED		= 0x2;
            const string	SE_SHUTDOWN_NAME			= "SeShutdownPrivilege";

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            IntPtr procHandle = KernelAPI.Platform.GetCurrentProcess();

			//----------------------
            // プロセストークン取得
			//----------------------
            IntPtr tokenHandle;

            KernelAPI.Platform.OpenProcessToken(
				procHandle								,
                TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY	,
				out tokenHandle							);

			//----------------------
            // LUIDを取得する
			//----------------------
            KernelAPI.Platform.TOKEN_PRIVILEGES tp = new KernelAPI.Platform.TOKEN_PRIVILEGES();
            tp.Attributes		= SE_PRIVILEGE_ENABLED;
            tp.PrivilegeCount	= 1;

            KernelAPI.Platform.LookupPrivilegeValue( null, SE_SHUTDOWN_NAME, out tp.Luid );
            
			//----------------------
			// 特権を有効にする
			//----------------------
            KernelAPI.Platform.AdjustTokenPrivileges(
                tokenHandle	,
				false		,
				ref tp		,
				0			,
				IntPtr.Zero	,
				IntPtr.Zero	);
        }

		//=========================================================================
		///	<summary>
		///		コンピュータの電源を落とす
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static public void ComputerPowerDown()
		{
            AdjustToken();
            KernelAPI.Platform.ExitWindowsEx(KernelAPI.Platform.EWX_POWEROFF, 0);
        }
		
		//=========================================================================
		///	<summary>
		///		例外メッセージをメッセージボックスに表示
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2009/03/08 必ずメインスレッドで表示するようにした</history>
		//=========================================================================
		static public void ShowException(
			Exception		e		,
			MessageBoxIcon	icon	)
		{
			// 全てメインスレッド以外で処理
			if (mMainForm != null &&
				mMainForm.InvokeRequired)
			{
				mMainForm.Invoke( (ShowExceptionDelegate)ShowException, new object[]{ e, icon } );
				return;
			}

			MessageBox.Show(
				e.Message			,
				"エラー"			,
				MessageBoxButtons.OK,
				icon				);
		}
		
		//=========================================================================
		///	<summary>
		///		アプリケーションのコンフィグ保存パスを返す
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/12/27 新規作成</history>
		//=========================================================================
        static public string AppConfigPath
        {
            get
            {
                string  configPath;
			    string	configFolder;

			    configPath = ConfigurationManager.OpenExeConfiguration(
				    ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
			    configFolder = Path.GetDirectoryName( configPath );

			    if( !Directory.Exists( configFolder ) )
                    throw new Exception("AppConfigPath内部エラー");

                return configFolder;
            }
        }

		//=========================================================================
		///	<summary>
		///		アプリケーションのデータ保存パスを返す
		/// </summary>
		/// <remarks>
        ///     mAgicAnimeは別の場所から複数のインスタンスを起動できるため、
        ///     このパスはexeの配置場所によって異なる。
		/// </remarks>
		/// <history>2009/12/27 新規作成</history>
		//=========================================================================
        static public string AppDataPath
        {
            get
            {
			    string	appDataFolder;
                string  exePath;
                byte[]  origBin;
                byte[]  hashBin;
                string  hashedStr    = "";

                appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

			    if( !Directory.Exists( appDataFolder ) )
                    throw new Exception("AppConfigPath内部エラー");

                // exeの配置場所に対するハッシュを求める
                exePath = Application.ExecutablePath.ToUpper();

                origBin = ASCIIEncoding.ASCII.GetBytes(exePath);
                hashBin = new MD5CryptoServiceProvider().ComputeHash(origBin);

                for( int i = 0 ; i < hashBin.Length ; ++i )
                    hashedStr += hashBin[i].ToString("x2");

                appDataFolder = Path.Combine( appDataFolder, "mAgicAnime" );
                appDataFolder = Path.Combine( appDataFolder, "mAgicAnime_data_" + hashedStr );

                return appDataFolder;
            }
        }

		//=========================================================================
		///	<summary>
		///		バージョンがUPされた時の処理
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/12/27 新規作成</history>
		//=========================================================================
        static private void CheckUpgraded( ref List<string> log )
        {
            // 前回起動時のバージョン
            string  prevVer     = Settings.Default.productVersion;
            int     majorVer    = 0;
            int     minorVer    = 0;
            int     revision    = 0;

			VersionHelper.ParseVersion( prevVer, out majorVer, out minorVer, out revision );

			if( Settings.Default.UpgradeRequire )
			{
				Settings.Default.Upgrade();
				Settings.Default.UpgradeRequire = false;
			}

            //----------------------------------------
            // データ保存フォルダ変更に伴う移行
            //----------------------------------------
            if( VersionHelper.isVersionOlder(
					1, 9, 17,
                    majorVer, minorVer, revision) )
            {
                log.Add("バージョンアップグレード(データ保存フォルダ移動)");
                log.Add("...新データフォルダ: " + Program.AppDataPath);

                string[] targets = new string[]
                {
                    "animePrograms.xml",
                    "animePrograms.previous.xml",
					Path.GetFileName(ReserveManager.TvStationsXml),
//                    "tvStationsII.xml",
                    "mAgicAnime.log",
                    "CmdLineProfiles",
                    "Thumbnails"
                };

                string  exeDir = Path.GetDirectoryName( Application.ExecutablePath );

                for( int i = 0 ; i < targets.Length ; ++i )
                {
                    string  src = Path.Combine( exeDir, targets[i] );
                    string  dest = Path.Combine( Program.AppDataPath, targets[i] );

                    try
                    {
                        if( (File.GetAttributes( src ) & FileAttributes.Directory) != 0 )
                        {
                            if( Directory.Exists( src ) )
                            {
                                Directory.CreateDirectory( dest );
                                string[] srcfiles = Directory.GetFiles( src );
                                foreach( string file in srcfiles )
                                    File.Copy( file, Path.Combine( dest, Path.GetFileName( file )) );

                                Directory.Move( src, Path.ChangeExtension(src, ".obsolete") );
                            }
                        }
                        else
                        {
                            if( File.Exists( src ) )
                                File.Move( src, dest );
                        }

                        log.Add("..." + targets[i] + "を移動しました。");
                    }
                    catch(Exception ex)
                    {
                    }
                }

                log.Add("...移動完了しました。");
            }

            // チェック完了の証拠としてバージョン記録
            Settings.Default.productVersion = Application.ProductVersion;
            Settings.Default.Save();
        }

		//=========================================================================
		///	<summary>
		///		初回起動時のコンフィギュレーション
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/12/27 新規作成</history>
		//=========================================================================
		static private void AppConfiguration( ref List<string> log )
		{
			// 初期の設定ファイルをユーザーフォルダへコピー
			string  exeDir	= Path.GetDirectoryName( Application.ExecutablePath );
			string	target	= ReserveManager.TvStationsXml;

			if( !File.Exists( target ) )
			{
				log.Add("初期の設定ファイルをユーザーフォルダへコピー");
				try
				{
					string	srcfile	= Path.GetFileNameWithoutExtension( target )
									+ ".original"
									+ Path.GetExtension( target );
					string	src		= Path.Combine( exeDir, srcfile );

					log.Add("...(転送元" + src + ")");
					log.Add("...(転送先" + target + ")");
					File.Copy( src, target );
					log.Add("...OK");
				}
				catch(Exception ex)
				{
					log.Add("...NG");
				}
			}

			// データディレクトリにヒント情報を書き込む
			try
			{
				string			txtPath	= Path.Combine( AppDataPath, "refer.txt" );
				StreamWriter	wr		= new StreamWriter( txtPath, false, Encoding.Unicode );
				wr.WriteLine(";The following exe file refers to this directory. ");
				wr.WriteLine(Application.ExecutablePath);
				wr.Dispose();
			}
			catch(Exception ex)
			{
			}
		}

		public static TextLogger 	FileLogger
		{
			get{ return mTextLogger; }
		}
		public static MemoryLogger 	MemoryLogger
		{
			get{ return mMemoryLogger; }
		}

// <ADD> 2010/04/17 デバッグオプション ->
#if DEBUG
		public static _DebugOption	DebugOption
		{
			get { return mDebugOption; }
		}
#endif
// <ADD> 2010/04/17 <-
	}

}

namespace magicAnime.Properties
{
    partial class Settings
    {
        //=========================================================================
        ///	<summary>
        ///		SettingのUpgradeが必要かどうか返す
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>2006/XX/XX 新規作成</history>
        //=========================================================================
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.NoSettingsVersionUpgrade()]					// 前バージョンから値を引き継がない
        [global::System.Configuration.DefaultSettingValueAttribute( "True" )]		// 新バージョンの初期値はTrue
        public bool UpgradeRequire
        {
            get
            {
                return ( (bool)( this[ "UpgradeRequire" ] ) );
            }
            set
            {
                this[ "UpgradeRequire" ] = value;
            }
        }
    }
}

namespace magicAnime.Properties
{
    partial class Settings
    {
        //=========================================================================
        ///	<summary>
        ///		Settingを保存したmAgicAnimeのバージョン
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>2006/XX/XX 新規作成</history>
        //=========================================================================
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute( "" )]
        public string productVersion
        {
            get
            {
                return ( (string)( this[ "productVersion" ] ) );
            }
            set
            {
                this[ "productVersion" ] = value;
            }
        }

    }

}


