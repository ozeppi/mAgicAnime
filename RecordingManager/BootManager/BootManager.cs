//=========================================================================
///	<summary>
///		mAgicAnime電源管理モジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成</history>
/// <history>2010/05/01 Subversionで管理するため不要なコメント削除</history>
//=========================================================================
//#define	BOOTTIMER_RESTART_ALWAYS	// Schedule.iniにアクセスする時、常にBootTimerを再起動
										// (BootTimerが共有違反エラーになるのを防止するため)
#define		BOOTTIMER_SUSPEND			// 共有違反エラーを防止するためBootTimerをサスペンド

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using magicAnime.Properties;
using KernelAPI;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		PC起動管理クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	public class BootManager
	{
		// BootTimer(1.407以降)制御メッセージ
		const int	BT_MESSAGE_SCHEDULE_UPDATE	= 1125;		// スケジュールを再読み込み
		const int	BT_MESSAGE_CLOSE			= 1126;		// BootTimerを閉じる

		const int	BootTimerClosingTimeOut = 3000;	// BootTimer終了タイムアウト

		//=========================================================================
		///	<summary>
		///		時間帯クラス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public class TimeZone
		{
			public DateTime startDateTime;
			public DateTime endDateTime;
			public string	comment;

			// 時間帯のオーバーラップを検査する
			public bool IsOverlap(TimeZone target)
			{
				bool a, b;

				a = (this.startDateTime < target.startDateTime)
					&& (this.endDateTime < target.startDateTime);
				b = (target.startDateTime < this.startDateTime)
					&& (target.endDateTime < this.startDateTime);

				return !(a||b);
			}
		}

		//=========================================================================
		///	<summary>
		///		時間帯比較コンパレータ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public class ITimeZoneComparer : IComparer<TimeZone>
		{
			public int Compare(TimeZone a, TimeZone b)
			{
				if (a.startDateTime == b.startDateTime) return 0;
				return a.startDateTime > b.startDateTime ? 1 : -1;
			}

		}

		//-------------------
		// メンバ変数
		//-------------------
		const int		maxScheduleCount	= 255;
		List<TimeZone>	timeZoneList;

		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public BootManager()
		{
			timeZoneList = new List<TimeZone>();
		}

		public void Clear()
		{
			timeZoneList.Clear();
		}

		//=========================================================================
		///	<summary>
		///		PCを起動～シャットダウンする時間帯を追加
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Add(
			DateTime bootupDateTime,
			DateTime shutdownDateTime,
			string	comment)
		{
			TimeZone timeZone = new TimeZone();

			if (shutdownDateTime < bootupDateTime)
				throw new Exception("内部エラー: shutdownDateTime < bootupDateTime");

			timeZone.startDateTime	= bootupDateTime;
			timeZone.endDateTime	= shutdownDateTime;
			timeZone.comment		= comment;

			timeZoneList.Add( timeZone );
		}

		public void Sort(ITimeZoneComparer comparer)
		{
			timeZoneList.Sort( comparer );
		}

		//=========================================================================
		///	<summary>
		///		近接する起動時間帯をまとめる
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Unification()
		{

			if ( timeZoneList.Count < 2 ) return;
			//Properties.Settings.Default.bootupTimeUnification;

			for (int i = 0; i < timeZoneList.Count; ++i)
			{
				uint interval; // [ms]
				TimeZone zone		= timeZoneList[i];
				TimeZone nextZone;

				if (i < timeZoneList.Count - 1)
					nextZone = timeZoneList[i + 1];
				else
					nextZone = timeZoneList[0];

				if (zone.IsOverlap(nextZone))
				{
					//-----------------------------
					// 重複している時間帯を統合
					//-----------------------------

					timeZoneList.Remove(zone);
					timeZoneList.Remove(nextZone);

					// 終了時間はnextZoneの方が早い場合もあるため
					DateTime endTime	= zone.endDateTime < nextZone.endDateTime
										? nextZone.endDateTime
										: zone.endDateTime;

	 				Add(zone.startDateTime,
						endTime,
						zone.comment + " " + nextZone.comment);

					timeZoneList.Sort(new ITimeZoneComparer());

					Unification();

					break;
				}
				else
				{
					//----------------------------------------
					// 次の起動時間まで一定時間以下なら統合
					//----------------------------------------

					interval = (uint)((nextZone.startDateTime.Ticks
										- zone.endDateTime.Ticks) / 10000000);

					if (interval < Properties.Settings.Default.bootupTimeUnification * 60)
					{
						timeZoneList.Remove(zone);
						timeZoneList.Remove(nextZone);

						Add(zone.startDateTime,
							nextZone.endDateTime,
							zone.comment + " " + nextZone.comment);

						timeZoneList.Sort(new ITimeZoneComparer());

						Unification();

						break;
					}
				}

			}

		}

		//=========================================================================
		///	<summary>
		///		BootTimerに起動スケジュールを登録する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void ApplyBootTimer(
			DateTime now )	// [i] 現在時刻(既に過ぎたスケジュールは無視する)
		{
			List<string> list = new List<string>();
			string	bootTimer, iniFile;
			bool	isModified = false;

			IntPtr	hWindow;

			if( !CheckBootTimer( out hWindow, out bootTimer ) )
			{
				throw new UpdatingException("BootTimerが常駐していません");
			}


			// マルチブート対応
			string	multiBootIniPath;
			multiBootIniPath = Path.Combine( Path.GetDirectoryName(bootTimer), @"MultiBoot.ini" );

			if( File.Exists( multiBootIniPath ) )
			{
				PrivateProfile prof = new PrivateProfile();
				prof.Open( multiBootIniPath );
				iniFile = prof.GetKeyString( "MultiBoot", "Path", "" );
				if( !string.IsNullOrEmpty( iniFile ) )
					iniFile = Path.Combine( iniFile, @"Schedule.ini" );
			}
			else
			{
				iniFile = Path.Combine( Path.GetDirectoryName(bootTimer), @"Schedule.ini" );
			}

#if _DEBUG
			Logger.Output("(DEBUG)INIファイル:" + iniFile);
#endif
			if( string.IsNullOrEmpty( iniFile ) ||
				!File.Exists( iniFile ) )
			{
				throw new UpdatingException("Schedule.iniが開けません。");
			}

#if BOOTTIMER_RESTART_ALWAYS
			if( !StopBootTimer( hWindow, BootTimerClosingTimeOut ) )
				throw new UpdatingException("BootTimerの停止に失敗");
#else
#if BOOTTIMER_SUSPEND
			//--------------------------
			// BootTimerをサスペンド
			//--------------------------
			int			btmrPid = 0;
			Process		btmrProc;

			KernelAPI.Window.GetWindowThreadProcessId(hWindow, out btmrPid);
			btmrProc = Process.GetProcessById(btmrPid);

			if( btmrProc == null || string.IsNullOrEmpty(btmrProc.ProcessName) )
				throw new UpdatingException("BootTimerが常駐していません。");

			KernelAPI.WinThread.SuspendResumeAllThread(btmrProc, true);
#endif
#endif
			try
			{
				PrivateProfile profile = new PrivateProfile();
				profile.Open( iniFile );

				//-----------------------------------------
				// mAgicAnimeで管理されないエントリを退避
				//-----------------------------------------
				var	registedEntries = new List<string>(); // mAgicAnimeで登録したエントリ

				for (uint i = 0; i < maxScheduleCount; ++i)
				{
					string temp;
					string text;
					
					text = string.Format("Schedule{0:0}", i);	// キー名
					temp = profile.GetKeyString("Schedule", text, "");
					
					if (temp.Equals(""))
						break;
					else
					{
						if( temp.IndexOf("<mAgicAnime>") == -1 )
							list.Add( temp );
						else
							registedEntries.Add( temp );
					}
				}

				//----------------------------
				// スケジュールエントリ追加
				//----------------------------
				var	newEntries = new List<string>();	// 登録するエントリ

				foreach (TimeZone timeZone in timeZoneList)
				{
					string text;

					//-------------------
					// 起動エントリ
					//-------------------
					if( Settings.Default.autoBootup )
					{
						DateTime dateTime = timeZone.startDateTime;
						string   comment  = timeZone.comment;
						if (now < dateTime)
						{
							// 起動時間を進める
							dateTime = dateTime.AddMinutes( -(double)Settings.Default.bootupHasten );

							// 起動エントリ文字列(BootTimer 1.407対応)
							//< 動作日時>,<終了日時>,<繰返情報>,<(未使用)>,<メイン・コマンド>,<サブ・コマンド>,
　							// <グループ>,<(未使用)>,<(未使用)>,<実行プログラム>,<引数>,<備考>
							text = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString() + ":00,";
							text += dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString() + ":00,";
							text += "00000,,0,0,100,,,,,<mAgicAnime>" + comment;
							
							newEntries.Add( text );
						}
					}

					//------------------------
					// シャットダウンエントリ
					//------------------------
					if( Settings.Default.autoShutdown )
					{
						DateTime dateTime = timeZone.endDateTime;
						string   comment  = timeZone.comment;
						if (now < dateTime)
						{
							// 終了時間を遅らせる
							dateTime = dateTime.AddMinutes( (double)Settings.Default.shutdownPutoff );

							// シャットダウンエントリ文字列(時刻+終了方法)
							string dateFormat = "yyyy/MM/dd";
							string timeFormat = "HH:mm";
							text =  dateTime.ToString(dateFormat) + " " + dateTime.ToString(timeFormat) + ":00,";
							text += dateTime.ToString(dateFormat) + " " + dateTime.ToString(timeFormat) + ":00,";
							text += string.Format(
								"00000,,1,{0:0},100,,,,,<mAgicAnime>" + comment ,
								Properties.Settings.Default.shutdownMethod	);
							
							newEntries.Add( text );
						}
					}
				}

				// 変更があるか判定
				if( newEntries.Count == registedEntries.Count )
				{
					try
					{
						for( int i =0 ; i < newEntries.Count ; ++i )
						{
							string newEntry = newEntries[i];
							string regEntry = registedEntries[i];

							DateTime ns = DateTime.Parse( newEntry.Split(',')[0] );
							DateTime ne = DateTime.Parse( newEntry.Split(',')[1] );
							DateTime rs = DateTime.Parse( regEntry.Split(',')[0] );
							DateTime re = DateTime.Parse( regEntry.Split(',')[1] );

							isModified	|= (ns != rs)
										|| (ne != re);
						}
					}
					catch(Exception ex)
					{
						isModified = true;
					}
				}
				else
				{
					isModified = true;
				}

				newEntries.ForEach( entry => list.Add( entry ) );

				//----------------
				// INIに書き戻す
				//----------------
				if( isModified )
				{
#if !BOOTTIMER_RESTART_ALWAYS && !BOOTTIMER_SUSPEND
					// BootTimerを再起動
					if( !StopBootTimer( hWindow, BootTimerClosingTimeOut ) )
						throw new UpdatingException("BootTimerの停止に失敗");
#endif
					for (int i = 0; i < maxScheduleCount; ++i)
					{
						string keyName;

						keyName = string.Format("Schedule{0:0}", i);	// キー名

						profile.WriteKeyString(
							"Schedule"							,
							keyName								,
							(i < list.Count) ? list[i] : null	);
					}

#if !BOOTTIMER_RESTART_ALWAYS && !BOOTTIMER_SUSPEND
					RestartBootTimer( bootTimer );
#endif
					
//#if _DEBUG
					Logger.Output("(BootTimer)起動スケジュールを反映");
//#endif
				}
				else
				{
//#if _DEBUG
					Logger.Output("(BootTimer)起動スケジュール変更なし");
//#endif
				}

				profile.Close();
			}
			catch(Exception ex)
			{
				throw;
			}
			finally
			{
#if BOOTTIMER_RESTART_ALWAYS
			RestartBootTimer( bootTimer );
#endif
#if BOOTTIMER_SUSPEND
			// BootTimerをリジューム
			KernelAPI.WinThread.SuspendResumeAllThread(btmrProc, false);
#endif
			}

#if BOOTTIMER_SUSPEND
			// スケジュールを再読み込みさせる
			if( isModified  )
				Window.SendMessage(
					hWindow	,
					BT_MESSAGE_SCHEDULE_UPDATE ,
					IntPtr.Zero	,
					IntPtr.Zero	);
#endif

		}

		//=========================================================================
		///	<summary>
		///		BootTimerの常駐を確認する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2010/01/30 StopBootTimerと分離</history>
		//=========================================================================
		private bool CheckBootTimer(
			out IntPtr	hChild		,
			out string	filePath	)
		{
			IntPtr	hWindow;
			Process	bootTimerProcess;
			int		processId;

			hChild		= IntPtr.Zero;
			filePath	= null;

			try
			{
				//---------------------------
				// BootTimerのプロセスを探す
				//---------------------------

				// BootTimerMainFormを探す
				hChild = (IntPtr)Window.FindWindowEx(
					IntPtr.Zero			,
					IntPtr.Zero			,
					null				,
					"BootTimerMainForm"	);

				if( hChild == IntPtr.Zero )
					return false;

				//=======================================
				// ウィンドウから実行ファイルパスを得る
				//=======================================
				Window.GetWindowThreadProcessId(hChild, out processId);

				bootTimerProcess	= Process.GetProcessById(processId);
				filePath			= bootTimerProcess.MainModule.FileName;

				return true;
			}
			catch(Exception ex)
			{
			}
			return false;
		}

		//=========================================================================
		///	<summary>
		///		BootTimerを一旦終了させる
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/30 StopBootTimerと分離</history>
		//=========================================================================
		private bool StopBootTimer(
			IntPtr	hWindow	,
			int		timeOut	)	// <ADD> 2010/04/22
		{
			//=======================================
			// BootTimerを閉じる
			//=======================================
            Window.SendMessage(
                hWindow				,
                BT_MESSAGE_CLOSE	,
                IntPtr.Zero			,
                IntPtr.Zero			);

#if BOOTTIMER_CLOSE_WAIT
// <ADD> 2010/04/22 実際に終了するのを待機 ->
			int			procID;
			DateTime	startTime = DateTime.Now;

			for(;;)
			{
				Window.GetWindowThreadProcessId(hWindow, out procID);

				if( procID == 0 )
					break;

				if( timeOut <= DateTime.Now.Subtract(startTime).TotalMilliseconds )
					return false;

				Thread.Sleep(50);
			}
// <ADD> 2010/04/22 <-
#endif

			return true;
		}


		//=========================================================================
		///	<summary>
		///		BootTimerを起動し直す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static void RestartBootTimer(string filePath)
		{
			if( string.IsNullOrEmpty( filePath ) )
				return;

			try
			{
				Process proc = Process.Start(filePath);
				// 安定するまで待たないと連続更新した時にBootTimerが落ちる
				proc.WaitForInputIdle();
			}
			catch (Exception)
			{
			}
		}

		
	}

}
