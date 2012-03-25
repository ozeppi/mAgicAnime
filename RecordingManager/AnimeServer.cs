//=========================================================================
///	<summary>
///		mAgicAnime�ԑg�Ǘ��T�[�r�X ���W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬 Dr.Kurusugawa</history>
/// <history>2010/02/20 �Â��R�����g���폜</history>
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
using System.Collections;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;
using System.Xml;
using System.Net;
using Microsoft.Win32;
using KernelAPI;
using magicAnime.Properties;


namespace magicAnime
{
	[FlagsAttribute]
	public enum updateOption
	{
		Force			= 128,	// �����X�V
	};

	// �����\�񃂁[�h
	public enum ReserveControl
	{
		nDaysFromNow,		// �f�[�^�X�V����n���Ԃ̗\��
		ImmediatlyBefore,	// �������O�ɗ\��
		noAutoReserve		// �����ŗ\�񂵂Ȃ�
	};

	public enum IdentifyFileMethod
	{
		ByCreateTimeStamp	= 0,
		ByUpdateTimeStamp	= 2,
		ByFileNameWithID	= 1,
	};

	public class UpdatingException : Exception
	{
		public UpdatingException(string mes)
			: base(mes)
		{
		}
	};
	
	//=========================================================================
	///	<summary>
	///		mAgicAnime�ԑg�Ǘ��T�[�r�X�N���X
	///	</summary>
	/// <remarks>
	///		�����f�[�^�̊Ǘ����玩���ăG���R�[�h�܂ōs���B
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public class AnimeServer : IDisposable
	{
		//-------------------------
		// Singleton�C���X�^���X
		//-------------------------
		private static AnimeServer theInstance;
		public static AnimeServer GetInstance()
		{
			if ( theInstance == null )
				new AnimeServer();

			return theInstance;
		}

		public const int RetryInterval	= 250;	// �l�b�g���[�N�ڑ��̃��g���C�Ԋu

		//---------------------
		// �^��`
		//---------------------
		public class AnimeList : List<AnimeProgram> {};

		// �X�e�[�^�X�N���X
		internal struct MyStatus
		{
			internal bool	updateSequenceBusy;	// �f�[�^�X�V�V�[�P���X���쒆
			internal string	updateDetail;		// �f�[�^�X�V�ڍ�
			internal string resultLastUpdate;	// �Ō�̃f�[�^�X�V�Ɍ���
			internal bool	completeUpdate;		// �f�[�^�X�V�����t���O
		}

		public delegate void ProgressUpdateDelegate(int Phase,int MaxPhase, string d);
		public delegate void UpdateProgress(string Phase, int Perc, string desc);

		// ���O�\���f���Q�[�g
		public delegate void LogoutDelegate(string message);
		// �f�[�^�X�V�V�[�P���X���s�f���Q�[�g
		private delegate void UpdateProcDelegate(
			updateOption		option		,	// [i] �f�[�^�X�V�I�v�V����
			List<AnimeProgram>	prog		,	// [i] �X�V�Ώۂ̔ԑg
			UpdateProgress		onProgress	,	// [i] �v���O���X�\���R�[���o�b�N
			LogoutDelegate		logout		);	// [i] ���O�\���R�[���o�b�N
		// �u�܂��Ȃ��������ԁv�o���[���̃|�b�v�A�b�v�����f���Q�[�g
		public delegate void PopupSoonBalloonDelegate( AnimeEpisode e );

		//---------------------
		// �����o
		//---------------------
		internal SyoboiCalender		mSyoboiCalender = new SyoboiCalender();	// ����ڂ���A�N�Z�X�I�u�W�F�N�g
		// ������ăG���R�[�h���s�����G�s�\�[�h�̃��X�g
		internal List<WeakReference> mAutoEncodedEpisodes = new List<WeakReference>();

		private AnimeList			mAnimeList		= new AnimeList();
		private Thread				mBackGroundSequence;				// �o�b�N�O���E���h�V�[�P���X�p�X���b�h
		private ManualResetEvent	mPulseSequenceTerminate;			// �V�[�P���X�I���v��
		private DateTime?			mPrevSequenceTime = null;			// �o�b�N�O���E���h�V�[�P���X�̑O����s����
		private bool				mAutoShutdown	= false;			// �G���R�[�h�������Ɏ����V���b�g�_�E��
		private bool				mNowEncoding;
		private	DateTime			mUpdateTime		= DateTime.Now;
		private	DateTime?			mBeforeAutoUpdateTime;				// �O��̎����X�V����
		private FileSystemWatcher	mFileWatcher;						// �^��f�B���N�g���Ď��I�u�W�F�N�g
		public PopupSoonBalloonDelegate		mPopupSoonBallon;
		private AnimeProgram.EpisodeList	mPopuppedSoonEpisodes;
		private bool				mDoingUpdateSequence = false;		// �f�[�^�X�V�V�[�P���X���s���t���O
		private BatchManager		mEncodeJobManager;					// �G���R�[�h�W���u�}�l�[�W��
		private BootManager			mBootManager	= new BootManager();// �u�[�g�}�l�[�W��
		// �������Ԃ��d�����Ă���Episode�̃��X�g
		private	List<AnimeEpisode>	mDoubleBookingEpisodes = null;
		private Mutex				mLockStatus = new Mutex();
		private MyStatus			mMyStatus;
		// �����O�ɋ����f�[�^�X�V�����G�s�\�[�h�̃��X�g
		private class UpdatedSoon
		{
			internal int			mDonePoint;		// �X�V�|�C���g(����n���O)
			internal WeakReference	mEpisode;		// �ΏۃG�s�\�[�h
		};
		private List<UpdatedSoon>	mUpdatedSoonList = new List<UpdatedSoon>();

		//---------------------
		// �v���p�e�B
		//---------------------
		public	List<AnimeEpisode> DoubleBookingEpisodes
		{
			get{ return mDoubleBookingEpisodes; }
		}
		public bool AutoShutdown
		{
			get	{ return mAutoShutdown; }
			set { mAutoShutdown = value; }
		}
		public	DateTime	UpdateDateTime		{ get{ return mUpdateTime; } }

		//=========================================================================
		///	<summary>
		///		�S�ԑg�̃��X�g��Ԃ�
		///	</summary>
		/// <remarks>
		///		���X�g�I�u�W�F�N�g�̃R�s�[��Ԃ��B
		///		��ʂ����X�g��ύX���Ă�������̃f�[�^�ɉe�����Ȃ��B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public	AnimeList	Animes
		{
			get
			{
				lock(mAnimeList)
				{
					AnimeList coppied = new AnimeList();

					foreach( AnimeProgram prog in mAnimeList )
						coppied.Add( prog );

					return coppied;
				}
			}
		}


		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public AnimeServer()
		{
			if ( theInstance != null )
				throw new Exception("����AnimeServer�C���X�^���X�����݂��܂��B");

			theInstance = this;

			mPopuppedSoonEpisodes = new AnimeProgram.EpisodeList();
		
			//------------------------------
			// �ԑg�f�[�^�t�@�C���̃��[�h
			//------------------------------
			lock(this)
			{

				if (File.Exists(AnimeProgramsXmlPath))
				{
					if( !Load( AnimeProgramsXmlPath ) )
					{
						//------------------------------------------
						// animePrograms.YYYYMMHH.xml�Ƀo�b�N�A�b�v
						//------------------------------------------
						try
						{
							string	strBackup;
							string strDateTime;
							
							strDateTime = string.Format(	"{0:D4}{1:D2}{2:D2}",
															DateTime.Now.Year	,
															DateTime.Now.Month	,
															DateTime.Now.Day	);

							strBackup = Path.ChangeExtension( AnimeProgramsXmlPath, strDateTime + ".xml" );

							File.Copy(	AnimeProgramsXmlPath	,
										strBackup				);
							Logger.Output( "�ǂݍ��ݎ��s�����t�@�C�����o�b�N�A�b�v���܂���: " + strBackup );
						}
						catch(Exception ex)
						{
						}

						//------------------------------------
						// ��O�̃f�[�^�t�@�C����ǂݍ���
						//------------------------------------
						if( File.Exists( AnimeProgramsPreviousXmlPath ) )
						{
							Logger.Output( "��O�̃f�[�^�t�@�C����ǂݍ��݂܂� - " + AnimeProgramsPreviousXmlPath );
							if( !Load( AnimeProgramsPreviousXmlPath ) )
							{
								Logger.Output( "������ǂݍ��݂Ɏ��s���܂���" );
							}
						}
					}
				}
			}

			//---------------------------------
			// �o�b�N�O���E���h�V�[�P���X�J�n
			//---------------------------------
			
			mPulseSequenceTerminate = new ManualResetEvent( false );

			mBackGroundSequence = new Thread( new ThreadStart( this.BackGroundMain ) );
			mBackGroundSequence.Start();

			//------------------------------------
			// �G���R�[�h�W���u�}�l�[�W��������
			//------------------------------------
			mEncodeJobManager = new BatchManager( Settings.Default.concurrentNums );

			mEncodeJobManager.ProcessedEvent	+= OnEncodeJobProcessed;
			mEncodeJobManager.JobErrorEvent	+= Logger.Output;

			//-----------------------------------------
			// �W���u���s���̃A�v���I�������f�m�F
			//-----------------------------------------

			// �G���R�[�h���ɃA�v�������ꍇ�̏�����o�^
			Program.AppClosing += ClosingProc;

			try
			{
				if ( Settings.Default.autoUpdate )
					StartWatchingCaptureFolder( Settings.Default.captureFolder );

			}catch(Exception e)
			{
				Logger.Output(e.Message);
			}

		}

		//=========================================================================
		///	<summary>
		///		Dispose
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Dispose()
		{
			if( mBackGroundSequence != null )
			{
				Logger.Output( "�o�b�N�O���E���h�V�[�P���X��~" );
				mPulseSequenceTerminate.Set();

				try
				{
					while( !mBackGroundSequence.Join( 100 ) )
					{
						Logger.Output( "...�V�[�P���X��~�҂�" );
					}

					Logger.Output( "...�V�[�P���X����~���܂���" );
				}
				catch( ThreadStateException )
				{
				}
			}

			Program.AppClosing -= ClosingProc;

			mEncodeJobManager.Finalize();

			EndWatchingCaptureFolder();
		}

		//=========================================================================
		///	<summary>
		///		�W���u���s���̃A�v���I���m�F�n���h��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
	    private void ClosingProc(
	        object							sender	,
	        Program.AppClosingEventArags	e		)
		{
		    DialogResult	dr;
			BatchJob[]		jobs;
			bool			executing = false;

			jobs = mEncodeJobManager.GetCurrentJob();

			foreach( BatchJob job in jobs )
				executing |= (job != null);

			if( !executing )
			{
				e.Cancel = false;
			}
			else
			{
				//--------------------------------
				// (�W���u���s��)�m�F��ʂ�\��
				//--------------------------------

				dr = MessageBox.Show(
					"�G���R�[�h�W���u�����s���ł��B\n"	+
					"�����𒆒f���Ă�낵���ł����H"	,
					"�m�F"								,
					MessageBoxButtons.OKCancel			,
					MessageBoxIcon.Exclamation			,
					MessageBoxDefaultButton.Button2		);

				if (dr != DialogResult.OK)
				{
					e.Cancel = true;
					return;
				}

				//----------------------
				// ���s���̃W���u�𒆒f
				//----------------------
				mEncodeJobManager.CancelJobs();
			}
		}

		//=========================================================================
		///	<summary>
		///		�ԑg�f�[�^�t�@�C�����i�[����t�@�C���̃p�X��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		static public string AnimeProgramsXmlPath
		{
			get
			{
				string filePath;
                filePath = Path.Combine( Program.AppDataPath, "animePrograms.xml" );
				return filePath;
			}
		}

		//=========================================================================
		///	<summary>
		///		��O�̃f�[�^�t�@�C���̃p�X��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/06 �V�K�쐬</history>
		//=========================================================================
		static public string AnimeProgramsPreviousXmlPath
		{
			get
			{
				return Path.ChangeExtension( AnimeProgramsXmlPath, "previous.xml" );
			}
		}
		
		//=========================================================================
		///	<summary>
		///		�ԑg�f�[�^�t�@�C����f���o��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Save()
		{
			string			fileName	= AnimeProgramsXmlPath;
			XmlTextWriter	xmlWriter;

			//--------------------------
			// �ۑ��O�̃t�@�C����ޔ�
			//--------------------------
			try
			{
				string		prevName	= AnimeProgramsPreviousXmlPath;

				if( File.Exists( prevName ) )
					File.Delete( prevName );

				File.Move( fileName, prevName );
			}
			catch(Exception ex)
			{
			}
			
			lock(this)
			{
				xmlWriter = new XmlTextWriter( fileName, System.Text.Encoding.UTF8 );

				xmlWriter.Formatting = Formatting.Indented;

				xmlWriter.WriteStartDocument();
				
				xmlWriter.WriteStartElement("�ԑg");

				foreach(AnimeProgram p in Animes)
					p.Write(xmlWriter);

				xmlWriter.WriteEndElement();
			
				xmlWriter.WriteEndDocument();

				xmlWriter.Flush();
				xmlWriter.Close();
			}
		
		}
		
		//=========================================================================
		///	<summary>
		///		�ԑg�f�[�^�t�@�C����ǂݍ���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/05/06 �����Ƀt�@�C������n���悤�ɂ���</history>
		//=========================================================================
		public bool Load( string fileName )
		{
			XmlTextReader x = null;
			AnimeProgram a = null;

			try
			{
				lock(mAnimeList)
				{
					x = new XmlTextReader(fileName);
					mAnimeList.Clear();

					while (x.Read())
					{
						if (x.NodeType == XmlNodeType.Element)
						{
							if (x.LocalName.Equals("ProgramInformation"))
							{
								a = new AnimeProgram( this );
								a.Read(x);
								mAnimeList.Add(a);
							}

						}//else if (x.NodeType == System.Xml.XmlNodeType.EndElement)

					}
					x.Close();
				}
			}
			catch (Exception ex)
			{
				if ( x != null )
				{
					x.Close();
				}

				Logger.Output( "�f�[�^�t�@�C���̓ǂݍ��݂Ɏ��s���܂��� - " + fileName );
				Logger.Output( "�G���[�ڍ�: " + ex.Message );

				return false;
			}

			return true;
		}

		//=========================================================================
		///	<summary>
		///		�V�K�A�j����ǉ�����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void AddAnime(AnimeProgram prog)
		{
			AddAnime(prog, -1);
		}
		public void AddAnime(AnimeProgram prog, int index)
		{
			lock( mAnimeList )
			{
				if( 0 <= index )
					mAnimeList.Insert( index, prog );
				else
					mAnimeList.Add( prog );
			}
		}

		//=========================================================================
		///	<summary>
		///		�A�j���^��Ǘ����폜����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void DeleteAnime(AnimeProgram prog)
		{
		
			lock(this)
			{
				lock( mAnimeList )
					mAnimeList.Remove( prog );

				//---------------------------------
				// �^��\�t�g���̗\����L�����Z��
				//---------------------------------
				foreach(AnimeEpisode episode in prog.Episodes)
				{
					if( episode.IsReserved )
					{
						ReserveManager rm = new ReserveManager();
						string descript;

						descript = string.Format(
							"{0:0} {1:0}�b"			,
							prog.title				,
							episode.StoryNumber		);	// �b��

						rm.CancelReservation( descript,episode.GetUniqueString() );
					}

				}

			}

		}

		//=========================================================================
		///	<summary>
		///		�A�j���ꗗ���\�[�g
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/06 �V�K�쐬</history>
		//=========================================================================
		internal void SortAnime(AnimeSort sort)
		{
			lock( mAnimeList )
				mAnimeList.Sort( sort );
			
		}

		//=========================================================================
		///	<summary>
		///		�^��f�[�^���I�����C���ōX�V����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void UpdateOnline(
			List<AnimeProgram>	animes	,		// [i] �X�V�Ώۂ̔ԑg
			bool				force	,		// [i] �����X�V
			ProgressUpdateDelegate callBack )	// [i] �v���O���X�ʒm
		{
			int i = 0, count;
			List<SyoboiCalender.SyoboiUpdate> updateList = null;

			SyoboiCalender syoboCal = this.mSyoboiCalender;

			count = animes.Count;

			callBack(0, count, "�X�V�����_�E�����[�h���Ă��܂�");
			for( int j = 0 ; ; ++j )
			{
				try
				{
					updateList = syoboCal.DownloadUpdateList();
					if( updateList != null )
						break;
				}
				catch(Exception ex)
				{
					if( ex.GetType().IsSubclassOf(typeof(WebException)) )
						throw;

					if( j < Settings.Default.retryDownload )
					{
						Logger.Output(
							string.Format(
								"�ڑ��G���[�̂��߃��g���C({0}/{1})",
								j + 1, Settings.Default.retryDownload));

						Thread.Sleep( RetryInterval );
					}
					else
						throw new UpdatingException("����ڂ���ւ̃l�b�g���[�N�ڑ��G���[");
				}
			}

			foreach (AnimeProgram prog in animes)
			{
				DateTime updateDate = new DateTime(2000,1,1);

				if (prog.linkOnlineDatabase)
				{
					bool update = false;

					// �u�O��X�V�����v���ُ�(���ݎ������i��ł���)�ȏꍇ�̑΍�
					if (DateTime.Now < prog.LastUpdate)
					{
						Logger.Output("(����ڂ���) mAgicAnime�̑O��X�V�������s���̂��߁A�擾�������܂�(" + prog.title + ")");
						update = true;
					}
					else
					{
						//-------------------------------
						// 1�T�ԍX�V���Ȃ���΋����X�V
						//-------------------------------
						if (7 <= (DateTime.Now - prog.LastUpdate).Days)
						{
							update = true;
							updateDate = DateTime.Now;
							Logger.Output("(����ڂ���) ��T�ԍX�V��񂪂Ȃ������̂ŋ����f�[�^�X�V���܂�(" + prog.title + ")");
						}
						else
						{
							//-------------------------------
							// ���̔ԑg�̐V���f�[�^��T��
							//-------------------------------
							foreach (SyoboiCalender.SyoboiUpdate u in updateList)
							{
								if (prog.syoboiTid == u.tid)
								{
									// �O��X�V��������ɂ���ڂ��邪�X�V����Ă�����X�V����
									if (prog.LastUpdate < u.updateDate)
									{
										update = true;
										updateDate = (updateDate < u.updateDate) ? u.updateDate : updateDate;
										Logger.Output("(����ڂ���) �ԑg��񂪍X�V����Ă��܂�(" + u.updateDate + " - " + prog.title + ")");
									}
								}
							}

						}

					}

					if (force && !update)
					{
						updateDate	= DateTime.Now;
						update		= true;

						if( force )
							Logger.Output("(����ڂ���)�����f�[�^�X�V���܂�(" + prog.title + ")");
					}

					//-------------------------------
					// �ԑg�f�[�^���X�V
					//-------------------------------
					if(update)
						prog.UpdatePlan(updateDate);

					if (callBack != null)
						callBack(i++, count, prog.title);

				}
			}

		}

		//=========================================================================
		///	<summary>
		///		�ԑg�̃T���l�C�����X�V����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal void UpdateThumbnail()
		{
			if (Settings.Default.forbiddenThumbnail)	// �T���l�C���쐬���֎~
				return;

			foreach (AnimeProgram p in Animes)
				p.UpdateThumbnail();
		
		}

		//=========================================================================
		///	<summary>
		///		�S�Ă̔ԑg�ɑ΂��A�������Ԃ̏d�����`�F�b�N����
		///	</summary>
		/// <remarks>
		///		���Ԃ��d������S�ẴG�s�\�[�h������o��(�D��x������)
		///		�`���[�i�̐������肸�^��ł��Ȃ��Ȃ�G�s�\�[�h������o��(�D��x�L����)
		/// </remarks>
		/// <history>2007/11/11 �V�K�쐬</history>
		/// <history>2009/11/21 n�ȏ�̏d���`�F�b�N�ɑΉ�</history>
		/// <history>2009/11/23 �`���[�i�̐�������镪�͏d���Ƃ��Ȃ��悤�ύX</history>
		//=========================================================================
		private delegate void SweepDoubleBookingDelegate(
			SweepDoubleBookingDelegate d,
			int m,
			int i,
			DateTime startTime,
			DateTime endTime,
			List<int> path);

		internal void CheckDoubleBooking()
		{
			try
			{
				List<AnimeEpisode>	episodes		= QueryEpisode( OnAirCondition );
				List<AnimeEpisode>	conflicts		= new List<AnimeEpisode>();
				int					threshould		= Settings.Default.overlapThreshould;
				int					margin			= Settings.Default.overlapMargin;
				bool				enablePriority	= Settings.Default.enablePriority;

				// �������ԏ��Ƀ\�[�g
				episodes.Sort( new SortAnimeListOrderUpperDateTime() );

				// n�d�����`�F�b�N���邽�߁Ak^n�̑S�g�ݍ��킹���X�B�[�v����B
				SweepDoubleBookingDelegate sweep = delegate(
					SweepDoubleBookingDelegate _sweep, // [i]
					int			m			,	// [i] 
					int			index		,	// [i] 
					DateTime	startTime	,	// [i] 
					DateTime	endTime		,	// [i] 
					List<int>	_path		)	// [i] �H�����m�[�h�����O���邽�߂̌o�H
				{
					bool pathTerminate = true;	// �g�ݍ��킹�c���[�̖��[

					for (int i = index; i < episodes.Count - 1; ++i)
					{
						AnimeEpisode episode		= episodes[ i ];

						// ���炩�ɏd�Ȃ�Ȃ����ԑт𖳎����ĒT���͈͂����߂�
						if (endTime < episode.StartDateTime)
							break;

						if (!_path.Contains(i))
						{
							if ( episode.StartDateTime < endTime &&
								 startTime < episode.EndDateTime  )
							{
								DateTime overlapStartTime	= (startTime < episode.StartDateTime)
															? episode.StartDateTime : startTime;
								DateTime overlapEndTime		= (endTime < episode.EndDateTime)
															? endTime : episode.EndDateTime;

	//							if (m <= threshould )
								{
									if (margin < (overlapEndTime - overlapStartTime).TotalMinutes)
									{
										_path.Add(i);

										// ���炩�ɏd�Ȃ�Ȃ����ԑт𖳎����ĒT���͈͂����߂�
										// �d���͈� T1&T2&���=[startTime,endTime] �����߂�ߒ���
										// �ł��������ԑ�Tn�̃C���f�b�N�Xn�𓾂�B
										int minIndex = -1;
#if _TEST
//									_path.ForEach(delegate(int a)
//									{
//										minIndex = (minIndex < 0) ? a : Math.Min(minIndex, a);
//									});
#else
										minIndex = 0;
#endif

										_sweep(_sweep, m + 1, minIndex, overlapStartTime, overlapEndTime, _path);

										_path.RemoveAt(_path.Count - 1);

										pathTerminate = false;
									}
								}

							}
						}

					}

					if( pathTerminate )
					{
						//----------------------------------------
						// �G�s�\�[�h�̑g�ݍ��킹���d���Ƃ��ċL�^
						//----------------------------------------
						List<AnimeEpisode> episodesByPriority = new List<AnimeEpisode>();

						foreach (int j in _path)
							episodesByPriority.Add(episodes[j]);

						// ���ԑт��d������G�s�\�[�h��ԑg�D��x���Ƀ\�[�g
						episodesByPriority.Sort(delegate(AnimeEpisode epA, AnimeEpisode epB)
						{
							if( epA == null && epB != null )	return -1;
							if (epA != null && epB == null)		return +1;

							if (epA.Parent.priority == epB.Parent.priority)
	// <PENDING> 2009/11/23 �D��x�������Ȃ�K���Ɍ��߂� ->
							{
								if (epA.Parent.UniqueID == epB.Parent.UniqueID)
									return 0;
								return ( epA.Parent.UniqueID < epB.Parent.UniqueID ) ? +1 : -1;
								//return 0;
							}
	// <PENDING> 2009/11/23 <-

							return (epA.Parent.priority < epB.Parent.priority) ? +1 : -1;
						});


						foreach (AnimeEpisode epi in episodesByPriority)
						{
							// �`���[�i�����������̂͏d���ɂ��Ȃ�(�D��x�L����)
							if(  enablePriority && (threshould <= episodesByPriority.IndexOf(epi)) ||
								!enablePriority && (2 <= episodesByPriority.Count)					)
							{
								if (!conflicts.Contains(epi))
									conflicts.Add(epi);
							}
						}
					}
				};

				List<int> path = new List<int>();
				sweep(sweep, 1, 0, DateTime.MinValue, DateTime.MaxValue, path);

				mDoubleBookingEpisodes = conflicts;
			}
			catch (Exception ex)
			{
			}
		}


		//=========================================================================
		///	<summary>
		///		�ԑg�G�s�\�[�h����������̍~���Ƀ\�[�g����R���y�A�N���X
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		class SortAnimeListOrderLowerDateTime : IComparer<AnimeEpisode>
		{
			public int Compare( AnimeEpisode x, AnimeEpisode y )
			{
				if ( x.StartDateTime > y.StartDateTime )
					return -1;
				else if ( y.StartDateTime > x.StartDateTime )
					return +1;
				else
					return 0;
			}
		}


		//=========================================================================
		///	<summary>
		///		�ԑg�G�s�\�[�h����������̏����Ƀ\�[�g����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		class SortAnimeListOrderUpperDateTime : IComparer<AnimeEpisode>
		{
			public int Compare( AnimeEpisode x, AnimeEpisode y )
			{
				if ( x.StartDateTime > y.StartDateTime )
					return +1;
				else if ( y.StartDateTime > x.StartDateTime )
					return -1;
				else
					return 0;
			}
		}

		//=========================================================================
		///	<summary>
		///		�K��̗\�񏈗�(����n���Ԃ̕�������\��)���s��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal void ReserveProc(
			List<AnimeProgram>	animes		,	// [i] �Ώۂ̔ԑg
			ReserveManager		manager		,	// [i] �^��\��}�l�[�W��
			LogoutDelegate		logout		,	// [i] ���O�o�̓R�[���o�b�N
			bool				changeOnly	)	// [i] �\�񎞊Ԃ̕ύX�̂�
		{
			//lock(this)
			//{
			List<AnimeEpisode>	list = new List<AnimeEpisode>();
			DateTime			now = DateTime.Now;

			CheckDoubleBooking();

			//----------------------------------------
			// �\�񂷂�G�s�\�[�h��������ԏ��ɗ�
			//----------------------------------------
			foreach ( AnimeProgram prog in animes )
			{
				foreach( AnimeEpisode episode in prog.Episodes )
				{
					// ������n���Ԃ̕�����\�񂷂�
					DateTime dateLimit = now.AddDays( Settings.Default.reserveDays );

					bool	isEnd	= episode.JudgeTimeEnd( now );
					bool	make	=	episode.HasPlan
									&&	!episode.IsReserved
									&&	episode.IsRecordRequired
									&&	(episode.StartDateTime < dateLimit)
									&&	!isEnd;
					bool	change	= episode.JudgeTimeChanged;
					bool	ignore	= episode.IsReserveError;		// �\��G���[���o�Ă���Ƃ������\�񂵂Ȃ�

					if( ((make && !changeOnly) || change) && !ignore )
					{
						list.Add( episode );
					}
				}
			}

			list.Sort( new SortAnimeListOrderLowerDateTime() );

			//--------------------
			// �\�񏈗����s��
			//--------------------

			for ( int i = 0; i < list.Count;++i )
			{
				string errorMessage;

				if( list[i].IsReservePending() )
				{
					Logger.Output( "...�`���[�i��������Ȃ����߁A�\��͕ۗ����܂�(" + list[i].ToString() + ")" );
				}
				else
				{
					if( !list[i].Reserve( manager, out errorMessage ) )
					{
						// �\��G���[���������b�Z�[�W�\��
						if( logout != null )
							logout( errorMessage );
					}
				}
			}

			//

			manager.Flush();

			//----------------------------------------
			// ����ɗ\�񂪓o�^����Ă��邩�`�F�b�N
			//----------------------------------------
			foreach (AnimeProgram p in Animes)
			{
				p.CheckReserve( manager );
			}
			
		}

		//=========================================================================
		///	<summary>
		///		�^��Episode�̏�Ԃ��X�V����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void UpdateState(
			List<AnimeProgram>	animes	,	// [i] �X�V�Ώۂ̔ԑg
			DateTime			now		,	// [i] ���ݎ���
			string				[]files	)	// [i] �팟���Ώۂ̃t�@�C���ꗗ
		{
			if (files == null)
				files = ListupMovies();

			foreach (AnimeProgram p in animes)
			{
				p.UpdateState( now,files );
			}
		}

		
		//=========================================================================
		///	<summary>
		///		�ݒ肳�ꂽ�^��t�H���_����t�@�C����񋓂���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		static public string[] ListupMovies()
		{
			string[] files;

			if (!Directory.Exists(Settings.Default.captureFolder))
			{
				Logger.Output( "�^��t�H���_��������܂���(" + Settings.Default.captureFolder.ToString() + ")" );
				files = new string[0];
				return files;
			}

			if (!Settings.Default.captureSubDir)										// �T�u�f�B���N�g���̒��ɘ^��t�@�C���������H
			{

				files = Directory.GetFiles(
					Settings.Default.captureFolder		,
					"*" + Settings.Default.strExtension	);								// �^��t�@�C����񋓂���
			} else
			{

				files = Directory.GetDirectories( Settings.Default.captureFolder );		// �^��T�u�f�B���N�g����񋓂���
			}

			return files;
		}

	
		//=========================================================================
		///	<summary>
		///		�S�ԑg�̑SEpisode��񋓂���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public int EnumAllEpisodes(
			AnimeProgram.EnumRecordCallBack	callBack	,
			object							param		)
		{
			int count = 0;

			foreach (AnimeProgram p in this.Animes)
			{
				count += p.EnumEpisodes( callBack ,param );
			}

			return count;
		}

		public delegate bool QueryEpisodeCondition( AnimeEpisode r );		// QueryEpisode�̏���

		//=========================================================================
		///	<summary>
		///		�����ɍ��v����Episode�̃��X�g�𓾂�
		///	</summary>
		/// <remarks>
		///		condition��null�̏ꍇ�͑S�Ă�Episode��񋓂���B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public AnimeProgram.EpisodeList QueryEpisode( QueryEpisodeCondition condition )
		{
			AnimeProgram.EpisodeList newList = new AnimeProgram.EpisodeList();

			foreach( AnimeProgram prog in Animes )
			{
				foreach( AnimeEpisode episode in prog.Episodes )
				{
					if( condition == null							||
						(condition != null && condition( episode )) )
					{
						newList.Add( episode );
					}
				}
			}

			return newList;
		}

		//=========================================================================
		///	<summary>
		///		Episode���G���R�[�h�҂��ȏ���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public static bool WaitingEncodeCondition(AnimeEpisode r)
		{
			return ( r.Parent.EncoderType != null &&
					 r.HasFile && !r.IsEncoded && !r.IsStored );
		}

		//=========================================================================
		///	<summary>
		///		Episode�������҂��ȏ���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public static bool OnAirCondition(AnimeEpisode r)
		{
			return r.HasPlan && !r.JudgeTimeEnd( DateTime.Now );
		}

		//=========================================================================
		///	<summary>
		///		�ł�������������G�s�\�[�h���擾
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/06/25 �V�K�쐬</history>
		//=========================================================================
		public AnimeEpisode QueryEarliestEpisode()
		{
			List<AnimeEpisode> epis;

			epis = QueryEpisode(AnimeServer.OnAirCondition);

			Comparison<AnimeEpisode> dateTimeComparison
				= delegate(AnimeEpisode a, AnimeEpisode b)
			{
				return a.StartDateTime.CompareTo(b.StartDateTime);
			};

			// �������Ƀ\�[�g
			epis.Sort(dateTimeComparison);

			return (0 < epis.Count) ? epis[0] : null;
		}

		//=========================================================================
		///	<summary>
		///		�w�肳�ꂽ��������^��t�@�C������������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2007/04/18 �V���[�g�t�@�C�����ɕϊ�����K�v���Ȃ��Ȃ���</history>
		/// <history>2009/04/30 �t�@�C�������w�蕶����Ńt�B���^����I�v�V�����ǉ�</history>
		//=========================================================================
		static public string FindCapturedFile(
			string		UniqueID	,	// [i] �G�s�\�[�h�̎���ID
			DateTime	startTime	,	// [i] ��������
			DateTime	endTime		,	// [i] �����I������
			string[]	movieFiles	,	// [i] ����t�@�C�����X�g
			string		filterName	)	// [i] �t�@�C�����̃t�B���^(�I�v�V����)
		{
			int				minMergin = Settings.Default.captureMergin;
			List<string>	matchList = new List<string>();
		
			foreach(string fn in movieFiles)
			{
				DateTime	timeStamp;
				DateTime	targetTime;
				string		sfn;				// �t�@�C����
				bool		matched	= false;	// ���v�t���O
				bool		isDir	= false;

				switch (Settings.Default.specifiedFile)
				{
				case IdentifyFileMethod.ByCreateTimeStamp:
				case IdentifyFileMethod.ByUpdateTimeStamp:
					//-----------------------
					// �쐬(�X�V)�����œ���
					//-----------------------
					sfn = fn + ".";

					isDir	=	Directory.Exists(sfn)
							&&	(0 < (File.GetAttributes(sfn) & FileAttributes.Directory));

					if( Settings.Default.specifiedFile == IdentifyFileMethod.ByCreateTimeStamp )
					{
						timeStamp	= isDir	?	Directory.GetCreationTime(sfn)	:
												File.GetCreationTime(sfn)		;
						targetTime	= startTime;
					}
					else
					{
						timeStamp	= isDir	?	Directory.GetLastWriteTime(sfn)	:
												File.GetLastWriteTime(sfn)		;
						targetTime	= endTime;
					}

					matched	= (Math.Abs( (targetTime - timeStamp).TotalMinutes ) < minMergin);
					
					break;
				case IdentifyFileMethod.ByFileNameWithID:
					//-----------------------
					// �t�@�C������ID�œ���
					//-----------------------
					if (0 <= fn.IndexOf(UniqueID+"_"))							// �t�@�C������ID�����邩�H
					{
						matched = true;
					}

					break;
				}

				if( matched )													// ���v�H
				{
					if (Settings.Default.captureSubDir)							// �T�u�f�B���N�g�����Ƀt�@�C��������邩�H
					{
						string[]	files;
						string		dir;

						dir = fn + ".";

						if ((File.GetAttributes( dir ) & FileAttributes.Directory) > 0)		// �f�B���N�g�����H
						{
							string ext = Settings.Default.strExtension;			// �^��t�@�C���̋K��̊g���q
							files = Directory.GetFiles( dir, "*" +  ext );		// �T�u�f�B���N�g�����̃t�@�C�����

							if (files.Length > 0)
								matchList.Add( files[0] );		// �T�u�f�B���N�g�����̈�Ԗڂ̘^��t�@�C�����^�[�Q�b�g�Ƃ���
						}

					}else{
						matchList.Add( fn );
					}

				}
				
			}

			//------------------------------------------
			// ���X�g�̃t�@�C�������w�蕶����Ńt�B���^
			//------------------------------------------
			if( !string.IsNullOrEmpty( filterName ) )
			{
				List<string>	remList = new List<string>();
				foreach (string fn in matchList)
				{
					if ( fn.ToUpper().IndexOf(filterName.ToUpper()) < 0 )
					{
						remList.Add( fn );
					}
				}

				foreach( string fn in remList )
					matchList.Remove( fn );
			}

			if( 1 <= matchList.Count )
				return matchList[ 0 ];
		
			return null;
		}

		//=========================================================================
		///	<summary>
		///		�蓮�ŃG���R�[�h�W���u�𓊓�����(�񓯊�)
		///	</summary>
		/// <remarks>
		///		�����W���u���d�����ē�������Ă�����
		///		�蓮�G���R�[�h���Ɏ����G���R�[�h�̎��Ԃ����Ă��W���u��ǉ����Ȃ�
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void AddEncodeJob(AnimeEpisode record)
		{
			EncodeJob job = new EncodeJob(record);

			if( mEncodeJobManager.Contains(job) )
				return;

			if( !mNowEncoding )
			{
				mAutoShutdown = false;
			}

			mNowEncoding = true;

			mEncodeJobManager.AddJob(job);
		}

		//=========================================================================
		///	<summary>
		///		�o�b�`�o�^����Ă���G���R�[�h�W���u���X�g��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public List<EncodeJob> GetQueueingEncodeJobs()
		{
			List<EncodeJob> encodeJobs = new List<EncodeJob>();
			BatchJob[] jobs;

			jobs = mEncodeJobManager.GetQueueingJobs();

			// EncodeJob�̂ݐ􂢏o��
			foreach (BatchJob job in jobs)
			{
				if (job.GetType() == typeof(EncodeJob))
				{
					encodeJobs.Add((EncodeJob)job);
				}
			}

			return encodeJobs;
		}

		//=========================================================================
		///	<summary>
		///		�G���R�[�h���̃W���u��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public EncodeJob[] GetCurrentJobs()
		{

			BatchJob[]			jobs;
			List<EncodeJob>		encodeJobs = new List<EncodeJob>();
			
			jobs = mEncodeJobManager.GetCurrentJob();

			foreach( BatchJob job in jobs )
			{
				if (job != null &&
					job.GetType() == typeof(EncodeJob))
				{
					encodeJobs.Add( (EncodeJob)job );
				}
			}

			if( 0 < encodeJobs.Count )
				return encodeJobs.ToArray();

			return new EncodeJob[0];
		}

		//=========================================================================
		///	<summary>
		///		�G���R�[�h�W���u��S�ăL�����Z��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/11/16 �V�K�쐬</history>
		//=========================================================================
		internal void CancelJobs()
		{
			mEncodeJobManager.CancelJobs();
		}

		//=========================================================================
		///	<summary>
		///		�G���R�[�h�W���u��1�I���������̏���
		///	</summary>
		/// <remarks>
		///		�����V���b�g�_�E���v���p�e�B��True�̂Ƃ��A�V���b�g�_�E����������B
		///		�蓮�Œǉ������Ƃ��̓V���b�g�_�E�����Ȃ��B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void OnEncodeJobProcessed(
			BatchJob		job		,	// [i] ���������W���u
			bool			last	)	// [i] TRUE:�Ō�̃W���u FALSE:���ɃW���u����
		{
			//if (job.GetType() == typeof(EncodeJob))
			{
				// �c��W���u��0�Ȃ�V���b�g�_�E��...
				if ( last )
				{
					mNowEncoding = false;

					if( AutoShutdown )
					{
						Logger.Output( "�G���R�[�h�W���u�S�����ɂ��V���b�g�_�E��" );
						Program.TryShutdown();
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		���G���R�[�h�̑S�Ă�Episode���G���R�[�h�o�b�`�W���u��������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public int BatchEncodeAll()
		{
			//---------------------------------------------
			// �G���R�[�h�\�ȏ�Ԃ�Episode�����X�g�A�b�v
			//---------------------------------------------
			AnimeProgram.EpisodeList list = QueryEpisode( WaitingEncodeCondition );

			mAutoShutdown = Settings.Default.autoShutdownAtEncoded;
			mNowEncoding = true;

			//-------------------------------------
			// �G���R�[�h�o�b�`�W���u���G���L���[
			//-------------------------------------
			foreach( AnimeEpisode episode in list )
			{
				mEncodeJobManager.AddJob( new EncodeJob( episode ) );
			}
			
			// �ǉ������W���u����Ԃ�
			int		totalCount = mEncodeJobManager.JobCount;

			if( totalCount == 0 )
				mNowEncoding = false;

			string		log;
			log = string.Format(	"�S�G���R�[�h�W���u�𓊓� - �ǉ��W���u��:{0:0} �ҋ@�W���u��:{1:0}",
									list.Count		,
									totalCount		);
			Logger.Output( log );

			return list.Count;
		}

		//=========================================================================
		///	<summary>
		///		�N������̎������������s
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/04 OnInterval���番��</history>
		//=========================================================================
		private void FirstProc()
		{
			if( Settings.Default.autoTransferAtBoot )
			{
				//-------------------------
				// �N�����ɕۑ���֓]��
				//-------------------------
				try
				{
					int count = 0;

					Logger.Output( "[�V�[�P���X] �N�����ɕۑ���֓]��..." );

					AnimeProgram.EpisodeList list;

					list = QueryEpisode( delegate (AnimeEpisode ep)
					{
						bool bExclude	=	Settings.Default.autoMoveWithoutFirstEpisode
										&&	(ep.StoryNumber == 1);

						return		ep.IsStorable
								&&	!ep.IsStored
								&&	!bExclude;
					});

					foreach( AnimeEpisode ep in list )
					{
						try
						{
							ep.Store();
							++count;
						}
						catch( EpisodeMethodException ex )
						{
							Logger.Output( "(�����]��)" + ex.Message );
						}
					}

					if ( 0 < count )
					{
						Logger.Output( "(�N���������]��)�ۑ���(" + Settings.Default.saveFolder + ")��" + count.ToString() + "�{���ړ����܂����B" );
					}
				}
				catch ( Exception ex )
				{
					Logger.Output( "(�����]��)" + ex.Message + ex.StackTrace );
				}
			}
		}


		//=========================================================================
		///	<summary>
		///		�o�b�N�O���E���h�V�[�P���X
		///	</summary>
		/// <remarks>
		///		�o�b�N�O���E���h�Ŏ��s���A�w��̎����ɃG���R�[�h�W���u�̓����Ȃ�
		///		���s���B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/07/20 �X���b�h�ŏ�������</history>
		/// <history>2008/10/01 �����O�Ƀo���[���\������@�\�ǉ�</history>
		//=========================================================================
		void BackGroundMain()
		{
			Logger.Output( "[�V�[�P���X] �o�b�N�O���E���h�V�[�P���X���J�n���܂���" );

			//----------------------
			// �N������̃E�F�C�g
			//----------------------
			long	wait = Settings.Default.startupWait;

			Logger.Output("[�V�[�P���X] �N������̃E�F�C�g(" + wait.ToString() + "sec)");

			DateTime	endTime = DateTime.Now.AddSeconds( wait );

			for (; ; )
			{
				if (mPulseSequenceTerminate.WaitOne(0, false))
				{
					Logger.Output("...���f");
					break;
				}

				if( 0 <= DateTime.Now.CompareTo( endTime ) )
				{
					Logger.Output("...OK");
					break;
				}
			}

			//----------------------
			// �N������̎�������
			//----------------------

			Logger.Output( "[�V�[�P���X] �N����̏�������" );

			FirstProc();

			for(;;)
			{
				//----------------------------------
				// �I���v�����m�F�A�y�уX���[�v
				//----------------------------------
				if( mPulseSequenceTerminate.WaitOne( 5000, false ) )
				{
					Logger.Output( "[�V�[�P���X] �o�b�N�O���E���h�V�[�P���X���I�����܂�" );
					break;
				}

				// �O��Ɠ��ꎞ��������
				DateTime	nowDateTime	= DateTime.Now;
				bool		haveJust	= false;

				if( mPrevSequenceTime.HasValue )
				{
					haveJust	= ( (mPrevSequenceTime.Value.Hour	== nowDateTime.Hour)	&&
									(mPrevSequenceTime.Value.Minute	== nowDateTime.Minute)	);
				}

				//----------------------------------------
				// �����I����A��莞�Ԍ�ɕۑ���֓]��
				//----------------------------------------
				if( Settings.Default.autoTransferAtAfterRecord )
				{
					AutoTrasnferProc();
				}

				//----------------------------------------
				// �����I����A��莞�Ԍ�ɍăG���R�[�h
				//----------------------------------------
				if( Settings.Default.autoEncodeInAfterRecord )
				{
					AutoEncodeProc();
				}

				//-------------------------------------------
				// �w�莞���Ɏ����I�ɃG���R�[�h�W���u�𓊓�
				//-------------------------------------------
				if(	Settings.Default.scheduleEncodeEveryday	&&
					mNowEncoding == false					)
				{
					int			encodeTime	= Settings.Default.scheduleEncodeTime;
					bool		arise		= false;

					arise = ( nowDateTime.Hour	== encodeTime / 60	&&
							 nowDateTime.Minute	== encodeTime % 60	);

					if ( arise && !haveJust )
					{
						int count;

						//-------------------------
						// �o�b�`�G���R�[�h����
						//-------------------------
						Logger.Output( "[�V�[�P���X] �o�b�`�G���R�[�h�J�n" );

						count = BatchEncodeAll();

						// �G���R�[�h����t�@�C�����Ȃ���΃V���b�g�_�E��
						if(count == 0)
						{
							Logger.Output( "[�V�[�P���X] ...�K�v�Ȃ�" );
							if( mAutoShutdown )
							{
								Logger.Output( "[�V�[�P���X] ...�����ɃV���b�g�_�E��" );
								Program.TryShutdown();
							}
						}
					}
				}

				//----------------------------
				// �������O�ɋ����f�[�^�X�V
				//----------------------------
				if( Settings.Default.updateOnAirSoon )
					UpdateOnAirSoon();

				//----------------------------
				// ��莞�Ԃ��ƂɃf�[�^�X�V
				//----------------------------
				if( Settings.Default.autoRefresh )
				{
					// ����͍X�V�����A������X�V����(�N�����X�V�Əd�����邽��)
					if( !mBeforeAutoUpdateTime.HasValue )
					{
						mBeforeAutoUpdateTime = nowDateTime;
					}

					if ( ( DateTime.Now - mBeforeAutoUpdateTime.Value ).TotalMinutes >= Settings.Default.updateInterval )
					{
						Logger.Output( "[�V�[�P���X] �����X�V" );
						BeginUpdate( 0 );

						mBeforeAutoUpdateTime = DateTime.Now;
					}

				}

				//-------------------------------
				// �ԑg�������O�Ƀo���[���\��
				//-------------------------------
				if( Settings.Default.showSoonBalloon )
				{
					DateTime	now				= DateTime.Now;
					int			popupMergin		= Settings.Default.timeSoonBalloon;

				   // ����������莞�ԑO�`�����������𔻒�
				   QueryEpisodeCondition isBroadcastingSoon = delegate (AnimeEpisode r)
				   {
						if( !r.HasPlan )
							return false;

						return (r.StartDateTime.AddMinutes( -popupMergin ) <= now)	&&
								(now < r.StartDateTime)										;
				   };

				   AnimeProgram.EpisodeList	soonEpisodes;
				   soonEpisodes = QueryEpisode( isBroadcastingSoon );

					// ���Ƀ|�b�v�A�b�v�\�������G�s�\�[�h�����O
					Predicate<AnimeEpisode> isAlreadyPopupped = delegate (AnimeEpisode r)
					{
						return mPopuppedSoonEpisodes.Contains( r );
					};
					soonEpisodes.RemoveAll( isAlreadyPopupped );
					
					// �|�b�v�A�b�v�σ��X�g����������Ԃ��߂����G�s�\�[�h���폜
					Predicate<AnimeEpisode> isAlreadyBroadcasted = delegate (AnimeEpisode r)
					{
						if( !r.HasPlan )
							return true;

						return r.EndDateTime < now;
					};
					mPopuppedSoonEpisodes.RemoveAll( isAlreadyBroadcasted );


				   foreach( AnimeEpisode episode in soonEpisodes )
				   {
						if( mPopupSoonBallon != null )
							mPopupSoonBallon( episode );

						// ���Ƀ|�b�v�A�b�v�\�������G�s�\�[�h���L�����Ă���
						mPopuppedSoonEpisodes.Add( episode );
					}
				}

				mPrevSequenceTime = nowDateTime;

				// �������O�ɗ\������郂�[�h
				if (Settings.Default.reserveControl == ReserveControl.ImmediatlyBefore)
				{
					ReserveImmediatlyBefore( Settings.Default.reserveImmediatlyBeforeMinutes );
				}
			}

		}

		//=========================================================================
		///	<summary>
		///		�����I����̎����]������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/04 OnInterval���番��</history>
		//=========================================================================
		private void AutoTrasnferProc()
		{
			try
			{
//				AnimeEpisode stored = null;
				DateTime now = DateTime.Now;

				List<AnimeEpisode> episodes;

				episodes = QueryEpisode( delegate (AnimeEpisode ep)
				{
					if( ep.HasFile && !ep.IsStored && ep.IsStorable )
					{
						DateTime endTime = ep.EndDateTime.AddMinutes( Settings.Default.autoTransferTime );

						// �^���A������H
						if ( ( endTime <= now ) &&
							(( now - endTime ).Ticks / TimeSpan.TicksPerSecond) < 60 )
						{
							bool	exclude = false;
							// ��1�b�͏��O����I�v�V����
							exclude =	Settings.Default.autoMoveWithoutFirstEpisode
									&&	(1 == ep.StoryNumber);

							return !exclude;
						}
					}
					return false;
				} );

				foreach( AnimeEpisode ep in episodes )
				{
					ep.Store();
					Logger.Output( "(�����㎩���]��)�ۑ���(" + Settings.Default.saveFolder + ")��" + ep.ToString() + "���ړ����܂����B" );
				}

			}
			catch ( Exception ex )
			{
				Logger.Output( "(�����]��)" + ex.Message + ex.StackTrace );
			}
		}

		//=========================================================================
		///	<summary>
		///		�����I����̍ăG���R�[�h�J�n����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/05/04 �V�K�쐬</history>
		//=========================================================================
		private void AutoEncodeProc()
		{
			try
			{
				AnimeEpisode				target		= null;
				DateTime					now			= DateTime.Now;
				AnimeProgram.EpisodeList	targetEpisodes;

				// �u�^��ρv���u�����I����n���v�̃G�s�\�[�h������
				targetEpisodes = QueryEpisode(delegate(AnimeEpisode ep)
				{
					if( ep.HasFile && !ep.IsEncoded && !ep.IsStored )
					{
						DateTime triggerTime = ep.EndDateTime.AddMinutes(
												Settings.Default.autoEncodeMergin );

						// �����I����n���H
						return	(triggerTime <= now)
							&&	(now < triggerTime.AddMinutes(1));
					}
					return false;
				} );

				foreach (AnimeEpisode episode in targetEpisodes)
				{
					// ���ɏ��������G�s�\�[�h�͖���
					bool alreadyProcessed = false;
					alreadyProcessed = mAutoEncodedEpisodes.Exists( delegate (WeakReference epRef)
					{
						AnimeEpisode ep = epRef.Target as AnimeEpisode;
						return (ep != null) && (ep == episode);
					} );

					if (!alreadyProcessed)
					{
						mAutoEncodedEpisodes.Add(new WeakReference(episode));

						// ���̕������Ԃ܂Ŏ��Ԃ��Ȃ���΃G���R�[�h�J�n���Ȃ�
						if (Settings.Default.dontBeginEncodeLessThanMinutes)
						{
							AnimeEpisode earliestEpis;
							earliestEpis = QueryEarliestEpisode();

							TimeSpan span = (earliestEpis.StartDateTime - DateTime.Now);

							if (span.TotalMinutes < Settings.Default.maxDelayTimeDontBeginEncode)
							{
								Logger.Output("(�����㎩���ăG���R�[�h)���̕����܂Ŏ��Ԃ��Ȃ����߃G���R�[�h�J�n���܂��� - " + episode.ToString());
								break;
							}
						}

						//-------------------------
						// �G���R�[�h�W���u����
						//-------------------------
						AddEncodeJob(episode);

						if (target != null)
						{
							Logger.Output("(�����㎩���ăG���R�[�h)�o�b�`�W���u�𓊓����܂��� - " + target.ToString());
						}
					}
				}
			}
			catch ( Exception ex )
			{
				Logger.Output( "(�����㎩���ăG���R�[�h)" + ex.Message + ex.StackTrace );
			}
		}

		//=========================================================================
		///	<summary>
		///		�������On���O�ɗ\�������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/06/26 �V�K�쐬</history>
		//=========================================================================
		private void ReserveImmediatlyBefore(
			int		beforeMin	)	// [i] �����J�nn���O
		{
			try
			{
				DateTime			now			= DateTime.Now;
				List<AnimeEpisode>	immedEpisodes;

				CheckDoubleBooking();

				immedEpisodes = QueryEpisode(delegate(AnimeEpisode ep)
				{
					if( ep.HasPlan && ep.IsRecordRequired && !ep.IsReserved )
					{
						return	(	(ep.StartDateTime.AddMinutes(-beforeMin) <= now)
								&&	(now < ep.StartDateTime) );
					}
					return false;
				} );

				foreach( AnimeEpisode ep in immedEpisodes )
				{
					string	err;

					if( ep.IsReservePending() )
					{
						Logger.Output( "(�������O�\��) �`���[�i��������Ȃ����߁A�\��͕ۗ����܂�(" + ep.ToString() + ")" );
					}
					else
					{
						if( ep.Reserve(new ReserveManager(), out err) )
							Logger.Output( "(�������O�\��) �\�񊮗� - " + ep.ToString() );
						else
							Logger.Output("(�������O�\��) �\�񎸔s(" + err + ") - " + ep.ToString());
					}
				}

				foreach (AnimeEpisode ep in immedEpisodes)
					ep.Dirty = true;
			}
			catch( Exception ex )
			{
				Logger.Output( "(�������O�\��) �G���[(" + ex.Message + ")" );
			}
		}

		//=========================================================================
		///	<summary>
		///		�f�[�^�X�V�������J�n����(�񓯊�)
		///	</summary>
		/// <remarks>
		///		�J�n�ɐ���������true��Ԃ��B
		/// </remarks>
		/// <history>2006/11/26 �V�K�쐬</history>
		/// <history>2008/10/21 �񓯊��ɉ��C�B�G���[���O�\���p�R�[���o�b�N�ǉ��B</history>
		//=========================================================================
		public bool BeginUpdate(
			updateOption		option	)	// [i] �f�[�^�X�V�I�v�V����
		{
			return BeginUpdate( option, null );
		}
		public bool BeginUpdate(
			updateOption		option	,	// [i] �f�[�^�X�V�I�v�V����
			List<AnimeProgram>	animes	)	// [i] �X�V����ԑg���X�g
		{
			UpdateProcDelegate	delUpdate	= UpdateSequence;
			ManualResetEvent	endFlag		= new ManualResetEvent( false );
			// �o�ߕ\��
			UpdateProgress		progDelgate	= delegate (string Phase, int perc, string text )
			{
				if( mLockStatus.WaitOne() )
				{
					mMyStatus.updateDetail = Phase + " " + text;
					mLockStatus.ReleaseMutex();
				}
			};
			//---------------------
			// ���O�o��
			//---------------------
			LogoutDelegate		logoutDelgate	= delegate (string text)
			{
				if (mLockStatus.WaitOne())
				{
					if ( !string.IsNullOrEmpty( mMyStatus.resultLastUpdate ) )
						mMyStatus.resultLastUpdate += System.Environment.NewLine;

					mMyStatus.resultLastUpdate += text;

					Logger.Output( text );

					mLockStatus.ReleaseMutex();
				}
			};

			//---------------------
			// ����������
			//---------------------
			AsyncCallback completeCallback = delegate(IAsyncResult result)
			{
				if (mLockStatus.WaitOne())
				{
					mMyStatus.completeUpdate = true;
					mLockStatus.ReleaseMutex();
				}

				mDoingUpdateSequence = false;
				endFlag.Set();
			};

// <PENDIG> 2009/11/23 �����I�Ɍ��ʂ��N���A����H ->
//			if (mLockStatus.WaitOne())
//			{
//				mThisStatus.resultLastUpdate = "";
//				mLockStatus.ReleaseMutex();
//			}
// <PENDIG> 2009/11/23 <-

			if( animes == null )
				animes = this.Animes;

			try
			{
				if( mDoingUpdateSequence )
				{
					// ���Ɏ��s��...
					return false;
				}

				endFlag.Reset();

				// �f�[�^�X�V�V�[�P���X���J�n����
				delUpdate.BeginInvoke(
					option,
					animes,
					progDelgate,
					logoutDelgate,
					completeCallback,
					null);

				mDoingUpdateSequence = true;
			}
			catch(Exception)
			{
				return false;
			}

			return true;
		}


		//=========================================================================
		///	<summary>
		///		�f�[�^�X�V�V�[�P���X
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/21 �V�K�쐬</history>
		//=========================================================================
		private void UpdateSequence(
			updateOption		option		,	// [i] �f�[�^�X�V�I�v�V����
			List<AnimeProgram>	animes		,	// [i] �X�V�Ώۂ̔ԑg
			UpdateProgress		onProgress	,	// [i] �v���O���X�\���R�[���o�b�N
			LogoutDelegate		logout		)	// [i] ���O�\���R�[���o�b�N
		{

			try
			{
				//-------------------------------------
				// �I�����C���f�[�^�[�x�[�X����X�V
				//-------------------------------------

				if( onProgress != null )
				{
					onProgress( "�I�����C���f�[�^�x�[�X�ɃA�N�Z�X", 0, null );
				}

				AnimeServer.ProgressUpdateDelegate onOnlineProgress = delegate(
					int			perc	,
					int			max		,
					string		descipt	)
				{
					//---------------------------------------
					// �X�V���̌o�߂���ʂɃR�[���o�b�N
					//---------------------------------------
					if (max == 0) max = 1;
					if( onProgress != null )
					{
						onProgress(	"�I�����C���f�[�^�x�[�X�ɃA�N�Z�X",
									100 * perc / max				,
									descipt							);
					}
				};

				bool force = ((option & updateOption.Force) != 0);	// �����X�V�t���O

				UpdateOnline(animes, force, onOnlineProgress);

				//-------------------------------------
				// Episode��Ԃ̍X�V
				//-------------------------------------

				if( onProgress != null )
				{
					onProgress( "�^��t�@�C���̒T���Ə�Ԃ̍X�V", 0, null );
				}

				UpdateState(animes, DateTime.Now, null);

				//-------------------------------------
				// �^��\��
				//-------------------------------------

				{
					if (onProgress != null)
					{
						onProgress("�^���\��", 0, null);
					}

					bool	changeOnly = false;
					changeOnly = (Settings.Default.reserveControl != ReserveControl.nDaysFromNow);

					ReserveProc(animes, new ReserveManager(), logout, changeOnly);
				}

				//-------------------------------------
				// �d���ݒ�
				//-------------------------------------

				if (Settings.Default.autoPowerManagement)
				{
					if( onProgress != null )
					{
						onProgress( "PC�����N����ݒ�", 0, null );
					}

					ApplyBootSchedule();
				}

				//-------------------------------------
				// �T���l�C���X�V
				//-------------------------------------

				if( onProgress != null )
				{
					onProgress( "�T���l�C���X�V", 0, null );
				}

				UpdateThumbnail();

				// �������ԏd���̃`�F�b�N
				if( onProgress != null )
				{
					onProgress( "�������Ԃ̏d���`�F�b�N", 0, null );
				}

				CheckDoubleBooking();

				//-------------------------------------
				// �X�V����
				//-------------------------------------

				if( onProgress != null )
				{
					onProgress( "�X�V����", 0, null );
				}

				Save();
			}
			catch(UpdatingException e)
			{
				if( logout != null )
					logout("�f�[�^�X�V�G���[(" + e.Message +")");
			}
			catch (Exception e)
			{
				{
					Logger.Output("�����G���[�ڍ�: " + e.Message + "(" + e.StackTrace + ")");

					if( logout != null )
						logout("�\�����Ȃ������G���[���������܂����B(�ڍׂ̓��O���Q��)");
				}
			}

			GC.Collect();
		}

		//=========================================================================
		///	<summary>
		///		�A�v���P�[�V�����ݒ肪�ύX���ꂽ���A���I�ɐݒ�𔽉f
		///	</summary>
		/// <remarks>
		///		�I�v�V������ʂ�����ꂽ�^�C�~���O�ȂǂŌĂԂ��ƁB
		/// </remarks>
		/// <history>2009/02/14 �V�K�쐬</history>
		//=========================================================================
		internal void ApplyOption()
		{
			Logger.Output( "�A�v���P�[�V�����ݒ肪�ύX����܂����B" );

			try
			{
				//---------------------
				// �^��t�H���_�̊Ď�
				//---------------------
				EndWatchingCaptureFolder();

				if( Settings.Default.autoUpdate )
					StartWatchingCaptureFolder( Settings.Default.captureFolder );

			}
			catch (Exception e)
			{
				Logger.Output(e.Message);
			}
		}

		//=========================================================================
		///	<summary>
		///		�^��p�t�H���_�̊Ď����J�n����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private bool StartWatchingCaptureFolder( string watchPath )
		{
			Logger.Output("...�^��t�H���_���Ď�(" + watchPath + (")"));

			if (mFileWatcher != null)
			{
				Logger.Output("......�G���[(���ɃA�N�e�B�u)");
				return false;
			}

			try
			{

				if (!Directory.Exists(watchPath))
				{
					Logger.Output( "......�G���[(�p�X���ݒ肳��Ă��܂���)" );
					return false;
				}

				mFileWatcher = new FileSystemWatcher();

				mFileWatcher.Path			= watchPath;
				mFileWatcher.Filter			= "";
				mFileWatcher.NotifyFilter	= NotifyFilters.FileName | NotifyFilters.DirectoryName;
				mFileWatcher.Created			+= new FileSystemEventHandler( OnCreatedCaptureFile );

				mFileWatcher.EnableRaisingEvents = true;

				Logger.Output("......OK");
			}
			catch ( Exception )
			{
				mFileWatcher = null;
				Logger.Output("......�G���[");
				return false;
			}

			return true;
		}

		//=========================================================================
		///	<summary>
		///		�^��t�H���_�Ď��𒆎~����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private void EndWatchingCaptureFolder()
		{
			if ( mFileWatcher != null )
			{
				mFileWatcher.EnableRaisingEvents = false;
				mFileWatcher.Dispose();

				mFileWatcher = null;
			}
		}


		//=========================================================================
		///	<summary>
		///		�^��t�H���_�Ƀt�@�C�����쐬���ꂽ�ꍇ�̏���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/05/03 ���\�b�h���ύX(onFileChanged->OnCreatedCaptureFile)</history>
		//=========================================================================
		private void OnCreatedCaptureFile(
			System.Object source,
			System.IO.FileSystemEventArgs e )
		{
			try
			{
				if ( Settings.Default.autoUpdate )	// �����A�b�v�f�[�g�I�v�V����
				{

					if ( e.ChangeType == System.IO.WatcherChangeTypes.Created )
					{
						DateTime nowDateTime = DateTime.Now;

						// �T�u�f�B���N�g��������Ă��璆�Ƀt�@�C���������܂�
						// �^�C�����O������̂ŏ���sleep���Ă���
						if ( Settings.Default.captureSubDir )
						{
							Thread.Sleep( 3000 );

						} else
						{
							// ���֌W�Ȋg���q�̃t�@�C���Ȃ珈�����Ȃ�
							if ( !Path.GetExtension( e.FullPath ).Equals( Settings.Default.strExtension ) )
								return;
						}

						string[] temp = { e.FullPath };

						UpdateState( Animes, DateTime.Now, temp );
					}

				}

			}
			catch ( Exception ex )
			{
				Logger.Output( ex.Message );
			}

		}

		//=========================================================================
		///	<summary>
		///		BootTimer��PC�N���X�P�W���[���𔽉f
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2009/06/29 BootManager����ړ�</history>
		//=========================================================================
		public void ApplyBootSchedule()
		{
			if (!Settings.Default.autoPowerManagement)
				return;

			mBootManager.Clear();

			//--------------------------------------------------
			// �\��ς̃G�s�\�[�h���N���X�P�W���[�����X�g�ɓo�^
			//--------------------------------------------------
			AnimeProgram.EpisodeList	episodeList;
			DateTime					now			= DateTime.Now;

			// �����N���������������
			if( (Settings.Default.reserveControl == ReserveControl.ImmediatlyBefore)
			||  (Settings.Default.reserveControl == ReserveControl.noAutoReserve) )
			{
				//----------------------------------------
				// (�������O�ɗ\��)����n���Ԃ̕���������
				//----------------------------------------
				episodeList = QueryEpisode(delegate(AnimeEpisode ep)
				{
					if (ep.Parent.WithoutPower)
						return false;

					// �u����v�ȊO�ł���Ε������ԃf�[�^����
					if( ep.HasPlan )
					{
						// ����n���ȓ�������
						{
							int nDays = Settings.Default.autoBootNDays;

							var endTime = ep.EndDateTime.AddMinutes(Settings.Default.shutdownPutoff);

							if ((now <= endTime) &&
								(ep.EndDateTime < now.AddDays(nDays)))
							{
								return true;
							}
						}
					}
					return false;
				});
			}
			else
			{
				//----------------------
				// �\��ς̕���������
				//----------------------
				episodeList = QueryEpisode(delegate(AnimeEpisode episode)
				{
					if (episode.Parent.WithoutPower)
						return false;
					return	episode.HasPlan && episode.IsReserved;
				});
			}

			// �N���X�P�W���[���ɒǉ�
			foreach (AnimeEpisode episode in episodeList)
			{
				DateTime startTime, endTime;

				startTime	= episode.StartDateTime;
				endTime		= episode.EndDateTime;

				// ���l�̔ԑg�^�C�g��������
				mBootManager.Add(startTime, endTime, episode.Parent.title);
			}

			// �߂����ԑѓ��m����������
			mBootManager.Sort(new BootManager.ITimeZoneComparer());
			mBootManager.Unification();

			mBootManager.ApplyBootTimer( DateTime.Now );
		}

		//=========================================================================
		///	<summary>
		///		�ύX�t���O
		///	</summary>
		/// <remarks>
		///		�f�[�^���ύX����Ă����true��Ԃ��B
		///		��ʂ���false���Z�b�g���ĕ\���X�V���K�v�ȃ^�C�~���O��҂B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal bool Dirty
		{
			get
			{
				bool childDirty = false;
				lock (mAnimeList)
				{
					foreach (AnimeProgram prog in mAnimeList)
						childDirty |= prog.Dirty;
				}

				return childDirty;
			}
			set
			{
				if (!value)
				{
					lock (mAnimeList)
					{
						foreach (AnimeProgram prog in mAnimeList)
							prog.Dirty = false;
					}
				}
//				isDirty = value;
			}
		}

		//=========================================================================
		///	<summary>
		///		�I�u�W�F�N�g�̃X�e�[�^�X��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/23 �V�K�쐬</history>
		//=========================================================================
		internal MyStatus GetStatus()
		{
			MyStatus copied;

			if (!mLockStatus.WaitOne())
				throw new Exception("�����G���[(GetStatus)");

			mMyStatus.updateSequenceBusy = mDoingUpdateSequence;

			copied = mMyStatus;

			mLockStatus.ReleaseMutex();

			return copied;
		}

		//=========================================================================
		///	<summary>
		///		�f�[�^�X�V���ʂ��N���A
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/23 �V�K�쐬</history>
		//=========================================================================
		internal void ClearResultUpdate()
		{
			if (!mLockStatus.WaitOne())
				throw new Exception("�����G���[(ClearResultUpdate)");

			mMyStatus.resultLastUpdate = "";

			mLockStatus.ReleaseMutex();
		}

		//=========================================================================
		///	<summary>
		///		�f�[�^�X�V�����t���O���N���A
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/11/23 �V�K�쐬</history>
		//=========================================================================
		internal void ResetCompleteUpdate()
		{
			if (!mLockStatus.WaitOne())
				throw new Exception("�����G���[(ResetCompleteUpdate)");

			mMyStatus.completeUpdate = false;

			mLockStatus.ReleaseMutex();
		}

		//=========================================================================
		///	<summary>
		///		�������O�ɋ����f�[�^�X�V
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/27 �V�K�쐬</history>
		//=========================================================================
		private void UpdateOnAirSoon()
		{
			var			episodes	= QueryEpisode( ep => true );
			DateTime	now			= DateTime.Now;
			List<int>	untilOnAir	= Settings.Default.untilOnAirMinutes;
			var			target		= new List<AnimeProgram>();

			foreach( AnimeEpisode ep in episodes )
			{
				if( ep.HasPlan )
				{
					// �����܂ł̎���[min]
					long	remain		= (long)(ep.StartDateTime - now).TotalMinutes;
					var		passedPoint	= new List<int>();

					// �o�߂����`�F�b�N�|�C���g(n1,n2...���O)���擾
					foreach( int point in untilOnAir )
						if( remain < point )
							passedPoint.Add( point );

					passedPoint.Sort();

					if( 0 < remain )
					{
						if( 0 < passedPoint.Count )
						{
							// �ʉ߂������߂̃`�F�b�N�|�C���g
							int			minPoint	= passedPoint[0];
							UpdatedSoon entry;
							bool		exec		= false;

							// �Ō�Ɏ��s�����`�F�b�N�|�C���g���i�񂾂�����
							entry = mUpdatedSoonList.Find( e => (e.mEpisode.Target == ep) );

							if( entry != null )
								exec = (minPoint < entry.mDonePoint);
							else
								exec = true;

							if( exec )
							{
								Logger.Output( "[�V�[�P���X] ����" + minPoint.ToString() + "���O �����f�[�^�X�V"
											 + "(" + ep.Parent.title + ")" );

								if( !target.Contains( ep.Parent ) )
									target.Add( ep.Parent );

								// �X�V�|�C���g���L�^
								if( entry != null )
									entry.mDonePoint = minPoint;
								else
								{
									entry = new UpdatedSoon();
									entry.mDonePoint		= minPoint;
									entry.mEpisode			= new WeakReference(ep);
									mUpdatedSoonList.Add( entry );
								}
							}
						}
					}
				}
			}

			if( 0 < target.Count )
				BeginUpdate( updateOption.Force, target );

			// ���s�σ��X�g���N���[���A�b�v
			var deadList = mUpdatedSoonList.FindAll(
				delegate (UpdatedSoon entry)
			{
				AnimeEpisode ep = entry.mEpisode.Target as AnimeEpisode;
				if( ep != null )
					return (ep.HasPlan && ep.StartDateTime < now);
				return false;
			} );

			mUpdatedSoonList.RemoveAll( entry => deadList.Contains(entry) );
		}

	}


}
