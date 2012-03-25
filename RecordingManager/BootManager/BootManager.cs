//=========================================================================
///	<summary>
///		mAgicAnime�d���Ǘ����W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬</history>
/// <history>2010/05/01 Subversion�ŊǗ����邽�ߕs�v�ȃR�����g�폜</history>
//=========================================================================
//#define	BOOTTIMER_RESTART_ALWAYS	// Schedule.ini�ɃA�N�Z�X���鎞�A���BootTimer���ċN��
										// (BootTimer�����L�ᔽ�G���[�ɂȂ�̂�h�~���邽��)
#define		BOOTTIMER_SUSPEND			// ���L�ᔽ�G���[��h�~���邽��BootTimer���T�X�y���h

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
	///		PC�N���Ǘ��N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public class BootManager
	{
		// BootTimer(1.407�ȍ~)���䃁�b�Z�[�W
		const int	BT_MESSAGE_SCHEDULE_UPDATE	= 1125;		// �X�P�W���[�����ēǂݍ���
		const int	BT_MESSAGE_CLOSE			= 1126;		// BootTimer�����

		const int	BootTimerClosingTimeOut = 3000;	// BootTimer�I���^�C���A�E�g

		//=========================================================================
		///	<summary>
		///		���ԑуN���X
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public class TimeZone
		{
			public DateTime startDateTime;
			public DateTime endDateTime;
			public string	comment;

			// ���ԑт̃I�[�o�[���b�v����������
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
		///		���ԑє�r�R���p���[�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
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
		// �����o�ϐ�
		//-------------------
		const int		maxScheduleCount	= 255;
		List<TimeZone>	timeZoneList;

		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
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
		///		PC���N���`�V���b�g�_�E�����鎞�ԑт�ǉ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Add(
			DateTime bootupDateTime,
			DateTime shutdownDateTime,
			string	comment)
		{
			TimeZone timeZone = new TimeZone();

			if (shutdownDateTime < bootupDateTime)
				throw new Exception("�����G���[: shutdownDateTime < bootupDateTime");

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
		///		�ߐڂ���N�����ԑт��܂Ƃ߂�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
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
					// �d�����Ă��鎞�ԑт𓝍�
					//-----------------------------

					timeZoneList.Remove(zone);
					timeZoneList.Remove(nextZone);

					// �I�����Ԃ�nextZone�̕��������ꍇ�����邽��
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
					// ���̋N�����Ԃ܂ň�莞�Ԉȉ��Ȃ瓝��
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
		///		BootTimer�ɋN���X�P�W���[����o�^����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void ApplyBootTimer(
			DateTime now )	// [i] ���ݎ���(���ɉ߂����X�P�W���[���͖�������)
		{
			List<string> list = new List<string>();
			string	bootTimer, iniFile;
			bool	isModified = false;

			IntPtr	hWindow;

			if( !CheckBootTimer( out hWindow, out bootTimer ) )
			{
				throw new UpdatingException("BootTimer���풓���Ă��܂���");
			}


			// �}���`�u�[�g�Ή�
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
			Logger.Output("(DEBUG)INI�t�@�C��:" + iniFile);
#endif
			if( string.IsNullOrEmpty( iniFile ) ||
				!File.Exists( iniFile ) )
			{
				throw new UpdatingException("Schedule.ini���J���܂���B");
			}

#if BOOTTIMER_RESTART_ALWAYS
			if( !StopBootTimer( hWindow, BootTimerClosingTimeOut ) )
				throw new UpdatingException("BootTimer�̒�~�Ɏ��s");
#else
#if BOOTTIMER_SUSPEND
			//--------------------------
			// BootTimer���T�X�y���h
			//--------------------------
			int			btmrPid = 0;
			Process		btmrProc;

			KernelAPI.Window.GetWindowThreadProcessId(hWindow, out btmrPid);
			btmrProc = Process.GetProcessById(btmrPid);

			if( btmrProc == null || string.IsNullOrEmpty(btmrProc.ProcessName) )
				throw new UpdatingException("BootTimer���풓���Ă��܂���B");

			KernelAPI.WinThread.SuspendResumeAllThread(btmrProc, true);
#endif
#endif
			try
			{
				PrivateProfile profile = new PrivateProfile();
				profile.Open( iniFile );

				//-----------------------------------------
				// mAgicAnime�ŊǗ�����Ȃ��G���g����ޔ�
				//-----------------------------------------
				var	registedEntries = new List<string>(); // mAgicAnime�œo�^�����G���g��

				for (uint i = 0; i < maxScheduleCount; ++i)
				{
					string temp;
					string text;
					
					text = string.Format("Schedule{0:0}", i);	// �L�[��
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
				// �X�P�W���[���G���g���ǉ�
				//----------------------------
				var	newEntries = new List<string>();	// �o�^����G���g��

				foreach (TimeZone timeZone in timeZoneList)
				{
					string text;

					//-------------------
					// �N���G���g��
					//-------------------
					if( Settings.Default.autoBootup )
					{
						DateTime dateTime = timeZone.startDateTime;
						string   comment  = timeZone.comment;
						if (now < dateTime)
						{
							// �N�����Ԃ�i�߂�
							dateTime = dateTime.AddMinutes( -(double)Settings.Default.bootupHasten );

							// �N���G���g��������(BootTimer 1.407�Ή�)
							//< �������>,<�I������>,<�J�ԏ��>,<(���g�p)>,<���C���E�R�}���h>,<�T�u�E�R�}���h>,
�@							// <�O���[�v>,<(���g�p)>,<(���g�p)>,<���s�v���O����>,<����>,<���l>
							text = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString() + ":00,";
							text += dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString() + ":00,";
							text += "00000,,0,0,100,,,,,<mAgicAnime>" + comment;
							
							newEntries.Add( text );
						}
					}

					//------------------------
					// �V���b�g�_�E���G���g��
					//------------------------
					if( Settings.Default.autoShutdown )
					{
						DateTime dateTime = timeZone.endDateTime;
						string   comment  = timeZone.comment;
						if (now < dateTime)
						{
							// �I�����Ԃ�x�点��
							dateTime = dateTime.AddMinutes( (double)Settings.Default.shutdownPutoff );

							// �V���b�g�_�E���G���g��������(����+�I�����@)
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

				// �ύX�����邩����
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
				// INI�ɏ����߂�
				//----------------
				if( isModified )
				{
#if !BOOTTIMER_RESTART_ALWAYS && !BOOTTIMER_SUSPEND
					// BootTimer���ċN��
					if( !StopBootTimer( hWindow, BootTimerClosingTimeOut ) )
						throw new UpdatingException("BootTimer�̒�~�Ɏ��s");
#endif
					for (int i = 0; i < maxScheduleCount; ++i)
					{
						string keyName;

						keyName = string.Format("Schedule{0:0}", i);	// �L�[��

						profile.WriteKeyString(
							"Schedule"							,
							keyName								,
							(i < list.Count) ? list[i] : null	);
					}

#if !BOOTTIMER_RESTART_ALWAYS && !BOOTTIMER_SUSPEND
					RestartBootTimer( bootTimer );
#endif
					
//#if _DEBUG
					Logger.Output("(BootTimer)�N���X�P�W���[���𔽉f");
//#endif
				}
				else
				{
//#if _DEBUG
					Logger.Output("(BootTimer)�N���X�P�W���[���ύX�Ȃ�");
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
			// BootTimer�����W���[��
			KernelAPI.WinThread.SuspendResumeAllThread(btmrProc, false);
#endif
			}

#if BOOTTIMER_SUSPEND
			// �X�P�W���[�����ēǂݍ��݂�����
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
		///		BootTimer�̏풓���m�F����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2010/01/30 StopBootTimer�ƕ���</history>
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
				// BootTimer�̃v���Z�X��T��
				//---------------------------

				// BootTimerMainForm��T��
				hChild = (IntPtr)Window.FindWindowEx(
					IntPtr.Zero			,
					IntPtr.Zero			,
					null				,
					"BootTimerMainForm"	);

				if( hChild == IntPtr.Zero )
					return false;

				//=======================================
				// �E�B���h�E������s�t�@�C���p�X�𓾂�
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
		///		BootTimer����U�I��������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/30 StopBootTimer�ƕ���</history>
		//=========================================================================
		private bool StopBootTimer(
			IntPtr	hWindow	,
			int		timeOut	)	// <ADD> 2010/04/22
		{
			//=======================================
			// BootTimer�����
			//=======================================
            Window.SendMessage(
                hWindow				,
                BT_MESSAGE_CLOSE	,
                IntPtr.Zero			,
                IntPtr.Zero			);

#if BOOTTIMER_CLOSE_WAIT
// <ADD> 2010/04/22 ���ۂɏI������̂�ҋ@ ->
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
		///		BootTimer���N��������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		static void RestartBootTimer(string filePath)
		{
			if( string.IsNullOrEmpty( filePath ) )
				return;

			try
			{
				Process proc = Process.Start(filePath);
				// ���肷��܂ő҂��Ȃ��ƘA���X�V��������BootTimer��������
				proc.WaitForInputIdle();
			}
			catch (Exception)
			{
			}
		}

		
	}

}
