//=========================================================================
///	<summary>
///		mAgicAnime�ԑg�f�[�^ ���W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬 Dr.Kurusugawa</history>
/// <history>2010/05/01 Subversion�ŊǗ����邽�ߕs�v�ȃR�����g�폜</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using KernelAPI;
using MakeThumbnail;
using magicAnime.Properties;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		mAgicAnime�ԑg�f�[�^ �N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	/// <history>2009/04/30 �^��e���r�ǂƂ���ڂ���e���r�ǂ�ʂɂ���</history>
	//=========================================================================
	public class AnimeProgram
	{
		//---------------------
		// �f�[�^�����o
		//---------------------
		public class EpisodeList : List<AnimeEpisode> { };

		public		string		title;
		public		 bool		linkOnlineDatabase;	// �I�����C���f�[�^��x�[�X(����ڂ���)�ƘA��
		public		int			syoboiTid;			// ����ڂ���TID
		public		string		tvStation;			// �^��e���r�ǖ�(�^��\�t�g�֓n���ǖ�)
		public		string		syoboiTvStation;	// ����ڂ���e���r�ǖ�(�f�[�^���������߂̋ǖ�)
		public		bool		WithoutPower;		// �d���Ǘ����珜�O
		public		long		adjustStartTime		= 0;		// �^��J�n����+n[��]
		public		long		adjustEndTime		= 0;		// �^��I������+n[��]

		// ����ڂ���f�[�^�I���|���V
		public enum SyobocalPolicy
		{
			SpecifyLatest,			// �ł���ɕ�������f�[�^��I��
			SpecifyNumber,			// n��ڂ̕����f�[�^��I��
			SpecifyEarly			// ���݂���ł�������������f�[�^��I��
		};

		public		SyobocalPolicy			syobocalPolicy			= SyobocalPolicy.SpecifyEarly;
		// �����f�[�^n���
		public		int						syobocalSpecifyNumber	= 1;
		public int							priority				= 30;	// �D��x(�Œ�10�`�ō�50)
		public bool							enableFilterKeyword		= false;	// �^��t�@�C�����w�蕶����Ńt�B���^
		public string						filterKeyword			= "";		// �t�B���^������

		private	int							mStoryCount;
		private	uint						mHashCode			= 0;
		private	AnimeServer					mParent;
		private bool						mIsDirty			= false;	// �ύX�t���O

		private List<Scheduler.Profile>		mSchedulerProfiles;
		private Type						mEncoderType		= null;		// �G���R�[�_�N���X��Type
		private EncodeProfile				mEncoderProfile		= null;		// �G���R�[�_�ݒ�
		private EpisodeList					mEpisodes;						// ���̔ԑg�̊e�b(Episode)�̏W��
		private	Mutex						mEpisodeLock		= new Mutex();
		private DateTime					mLastUpdate;					// �f�[�^��x�[�X����̍ŏI�X�V����
		private Image						mThambnailImage		= null;
		private Mutex						mThambnailLock		= new Mutex();


		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public AnimeProgram( AnimeServer parent )
		{
			this.mParent			= parent;
			this.title				= "";
			this.mStoryCount		= 0;
			this.linkOnlineDatabase	= true;
			this.syoboiTid			= 0;
			this.mLastUpdate		= new DateTime(2000, 1, 1);
			this.mSchedulerProfiles = new List<Scheduler.Profile>();
			this.WithoutPower		= false;

			this.mIsDirty			= true;
		}


		//=========================================================================
		///	<summary>
		///		�e�I�u�W�F�N�g��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public		AnimeServer		Parent
		{
			get	{	return mParent; }
		}

		//=========================================================================
		///	<summary>
		///		�ύX�t���O
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal bool Dirty
		{
			get
			{
				bool childDirty = false;

				try
				{
					mEpisodeLock.WaitOne();
					foreach (AnimeEpisode episode in mEpisodes)
						childDirty |= episode.Dirty;
				}
				finally
				{
					mEpisodeLock.ReleaseMutex();
				}

				return childDirty | mIsDirty;
			}
			set
			{
				if (!value)
				{
					try
					{
						mEpisodeLock.WaitOne();
						foreach (AnimeEpisode episode in mEpisodes)
							episode.Dirty = false;
					}
					finally
					{
						mEpisodeLock.ReleaseMutex();
					}
				}
				mIsDirty = value;
			}
		}

		//=========================================================================
		///	<summary>
		///		�b���̎擾�ƕύX
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public int		StoryCount
		{
			get { return mStoryCount; }
			set
			{
				int i;

				try
				{
					mEpisodeLock.WaitOne();

					this.mIsDirty = true;

					if( mEpisodes == null )
					{
						mEpisodes = new EpisodeList();
					}

					//-----------------------------------
					// Episode���X�g��b���ɂ��킹�Ċg��
					//-----------------------------------
					if( mStoryCount < value )
					{
						for( i = mStoryCount ; i < value ; ++i )
						{
							mEpisodes.Add(new AnimeEpisode(this, i + 1));
						}
					}
					else
					{
						for( i = mStoryCount ; value < i ; --i )
						{
							mEpisodes.RemoveAt( i - 1 );
						}
					}

					mStoryCount = value;
				}
				finally
				{
					mEpisodeLock.ReleaseMutex();
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�ԑg���Ƃ̘^��ݒ�
		///	</summary>
		/// <remarks>
		///		(�X�P�W���[�����Ƃɐݒ��ێ�)
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public Scheduler.Profile SchedulerProfile( Type profType )
		{
			if (profType == null)
				return null;

			if (!profType.IsSubclassOf(typeof(Scheduler.Profile)))
				throw new Exception();

			foreach ( Scheduler.Profile prof in mSchedulerProfiles )
			{
				if ( prof.GetType() == profType )
					return prof;
			}

			// �Ȃ���Βǉ�����
			Scheduler.Profile newProf = Activator.CreateInstance( profType ) as Scheduler.Profile;
			if ( newProf != null )
			{
				mSchedulerProfiles.Add( newProf );
				return newProf;
			}
			return null;
		}

		//=========================================================================
		///	<summary>
		///		�G���R�[�_��ʂ�Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public Type EncoderType
		{
			get { return mEncoderType; }
			set
			{
				if (value != null)
				{
					Encoder encoder;

					if (value != mEncoderType)
					{
						encoder = (Encoder)Activator.CreateInstance(value);

						// �G���R�[�_���ύX���ꂽ��v���t�@�C���I�u�W�F�N�g����蒼��
						mEncoderProfile = (EncodeProfile)Activator.CreateInstance(encoder.ProfileType);
						mEncoderType = value;
					}
				}
				else {
					mEncoderProfile = null;
					mEncoderType = null;
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�G���R�[�_�v���t�@�C����Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public EncodeProfile EncoderProfile
		{
			get{ return mEncoderProfile; }
			set
			{
				if (value != null)
				{
					Encoder encoder;

					encoder = (Encoder)Activator.CreateInstance(EncoderType);

					if (value.GetType() != encoder.ProfileType)
						throw new Exception("�����G���[: EncoderProfile���قȂ�܂��B");
				}
				mEncoderProfile = value;
			}
		}

		//=========================================================================
		///	<summary>
		///		�S�G�s�\�[�h�̃��X�g��Ԃ�
		///	</summary>
		/// <remarks>
		///		���X�g�I�u�W�F�N�g�̃R�s�[��Ԃ��B
		///		��ʂ����X�g��ύX���Ă�������̃f�[�^�ɉe�����Ȃ��B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public EpisodeList Episodes
		{
			get
			{
				try
				{
					mEpisodeLock.WaitOne();

					EpisodeList coppied = new EpisodeList();
					
					foreach( AnimeEpisode ep in mEpisodes )
						coppied.Add( ep );

					return coppied;
				}
				finally
				{
					mEpisodeLock.ReleaseMutex();
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�ԑg�ŗLID��Ԃ�
		///	</summary>
		/// <remarks>
		///		����ڂ���TID�Ƃ͈قȂ�
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public uint UniqueID
		{
			get
			{
				if (mHashCode == 0)
				{
					Guid newGuid = System.Guid.NewGuid();
					mHashCode = (uint)newGuid.GetHashCode();
				}

				return mHashCode;
			}
		}

		//=========================================================================
		///	<summary>
		///		�ŏI�X�V������Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public DateTime LastUpdate
		{
			get { return mLastUpdate; }
		}

		//=========================================================================
		///	<summary>
		///		�T���l�C���p�p�X��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		// 
		static internal string ThambnailPath
		{
			get
			{
                return Path.Combine( Program.AppDataPath, "Thumbnails" );
			}
		}

		//=========================================================================
		///	<summary>
		///		�T���l�C���t�@�C������Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal string ThambnailFile
		{
			get
			{
				return ThambnailPath + @"\" + this.UniqueID.ToString() + ".png";
			}
		}

		//=========================================================================
		///	<summary>
		///		�T���l�C���C���[�W�T�C�Y��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal Size ThambnailSize
		{
			get
			{
				return new Size(120,90);
			}
		}

		//=========================================================================
		///	<summary>
		///		�T���l�C���C���[�W��Ԃ�
		///	</summary>
		/// <remarks>
		///		�T���l�C���͔ԑg���ƂɒP��̃t�@�C���ŕێ�����
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2010/01/02 �C���[�W�쐬���̏ꍇ��null��Ԃ�</history>
		//=========================================================================
		public Image ThambnailImage
		{
			get
			{
				if( mThambnailLock.WaitOne( 0 ) )
				{
					try
					{
						if (mThambnailImage!=null)
							return mThambnailImage;

						//-----------------------------
						// �T���l�C���i�[�t�H���_����
						//-----------------------------
						if (!Directory.Exists( ThambnailPath ))
							Directory.CreateDirectory( ThambnailPath );

						//-------------------------
						// �T���l�C���t�@�C������
						//-------------------------
						if( File.Exists(ThambnailFile))
						{
							//------------------------
							// �K��̃t�@�C�����J��
							//------------------------
							mThambnailImage = new Bitmap( ThambnailFile );
						}
						else
						{
							//---------------
							// �V�K�쐬
							//---------------
							mThambnailImage = new Bitmap(1, 1);
							mThambnailImage.Save(
								ThambnailFile							,
								System.Drawing.Imaging.ImageFormat.Png	);	// png�ŕۑ�

							foreach( AnimeEpisode episode in Episodes )
							{
								episode.ThumbnailMaked = false;				// �T���l�C���쐬�σt���O���N���A
							}
						}

						return mThambnailImage;
					}
					finally
					{
						mThambnailLock.ReleaseMutex();
					}
				}
				else
				{
					return null;
				}
			}
		}

		
		//=========================================================================
		///	<summary>
		///		��n�b�̃G�s�\�[�h���擾����(1�`)
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public AnimeEpisode this[int storyNumber]
		{
			get
			{
				if (storyNumber < 1 || mStoryCount < storyNumber)
					throw new Exception("�����G���[: �b�����͈͊O�ł�");
				return (AnimeEpisode)mEpisodes[storyNumber - 1];
			}
		}

		public delegate void EnumRecordCallBack(AnimeEpisode record, object param);

		//=========================================================================
		///	<summary>
		///		�SEpisode��񋓂���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public int EnumEpisodes(EnumRecordCallBack callBack, object param)
		{
			foreach (AnimeEpisode episode in this.Episodes)
			{
				callBack(episode, param);
			}

			return mEpisodes.Count;
		}


		//=========================================================================
		///	<summary>
		///		����������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <returns>
		///			0	����������擾
		///			-1	����������s��
		///			-2	�����Ȃ�(�����I��)
		///	</returns>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public enum NextEpisode
		{
			NextDecided	,
			NextUnknown	,
			EndProgram	,
		};


		//=========================================================================
		///	<summary>
		///		����������t���擾
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal NextEpisode GetNextEpisode(
			DateTime			dateTime	,
			out AnimeEpisode	earlyOnAir	)
		{
			earlyOnAir = null;

			//----------------------------------
			// �ŏI�b�ȍ~�̓��t�Ȃ�����I��
			//----------------------------------
			if( Episodes.Count == 0 )
				return NextEpisode.NextUnknown;

			AnimeEpisode lastEpisode = Episodes[Episodes.Count-1];

			if ( lastEpisode.HasPlan &&
				 lastEpisode.StartDateTime < dateTime)
			{
				earlyOnAir	= null;
				return NextEpisode.EndProgram;
			}
			
			//
			// dateTime�ȍ~�A���X�g�̒��ōł�������������Episode��������
			//
			foreach (AnimeEpisode episode in Episodes)
			{
				if( episode.HasPlan &&
					dateTime <= episode.StartDateTime)
				{
					if (earlyOnAir != null)
					{
						if( episode.StartDateTime < earlyOnAir.StartDateTime )
						{
							earlyOnAir = episode;
						}
					}
					else
					{
						earlyOnAir = episode;
					}
				}
				
			}

			if (earlyOnAir==null)
				return NextEpisode.NextUnknown;

			return NextEpisode.NextDecided;
		}

		//=========================================================================
		///	<summary>
		///		�^��Episode�̏�Ԃ��X�V
		///	</summary>
		/// <remarks>
		///		�I�t���C���ł̏�ԕω����X�V
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal void UpdateState( DateTime now , string []files )
		{
			foreach (AnimeEpisode animeRecord in Episodes)
			{
				animeRecord.UpdateState(now,files);
			}
		}

		//=========================================================================
		///	<summary>
		///		�����v�������X�V
		///	</summary>
		/// <remarks>
		///		�������Ԃ̎擾�����łȂ��A�b�����������ꍇ�̒ǉ����s���B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal uint UpdatePlan(DateTime lastUpdate)	// [i] �X�V����
		{
			SyoboiCalender syoboiCalender = mParent.mSyoboiCalender;
			List<SyoboiCalender.SyoboiRecord> syoboiList = null;
			uint updateCount = 0;

			if ( syoboiTid				== 0		) return 0;
			if ( linkOnlineDatabase		== false	) return 0;

			try{
				string			title;
				List<String>	sourcePage;

				syoboiList = syoboiCalender.DownloadOnAirList( syoboiTid, out title, out sourcePage );

#if DEBUG		// ���������f�[�^����̏ꍇ�̃e�X�g
				if( Program.DebugOption.mForceEmpty )
					syoboiList.Clear();
#endif

				//--------------------------------
				// �b�����������ꍇ�̓��X�g�g�[
				//--------------------------------
				int maxNumber = 0;

				foreach (SyoboiCalender.SyoboiRecord record in syoboiList)
					maxNumber = System.Math.Max(maxNumber, record.number);

				if (StoryCount < maxNumber)
					StoryCount = maxNumber;	// �b�����������ꍇ�������₷

				//-------------------------------
				// �G�s�\�[�h���ƕ����f�[�^�X�V
				//-------------------------------
				bool isPlanAbnormal = false;

				foreach (AnimeEpisode episode in Episodes)
				{
					bool abnormal;
					episode.UpdatePlan( syoboiList, out abnormal );
					isPlanAbnormal |= abnormal;
				}

				// �ُ픭�����A����ڂ���TID�y�[�W���o�b�N�A�b�v
				if( isPlanAbnormal )
				{
					string	strDateTime;
					string	fileName;
					string	filePath;
					
					strDateTime = string.Format(	"{0:D4}{1:D2}{2:D2}",
													DateTime.Now.Year	,
													DateTime.Now.Month	,
													DateTime.Now.Day	);

					fileName = String.Format("AbnormalData_tid{0}_{1}.html", syoboiTid, strDateTime);
					filePath = Path.Combine(Program.AppDataPath, fileName);

					var writer = new StreamWriter(filePath, true, Encoding.Unicode);
					sourcePage.ForEach( line => writer.WriteLine(line) );
					writer.Close();

					Logger.Output("�ُ�f�[�^���܂�TID�y�[�W��ۑ����܂����B(" + filePath + ")");
				}

				this.mLastUpdate = lastUpdate;
			}
			catch (Exception e)
			{
				throw new Exception(
					"�I�����C���f�[�^�|�x�[�X�̃_�E�����[�h�Ɏ��s���܂���\n"
					+ e.Message + "\n" + title);
			}

			return updateCount;
		}

		//=========================================================================
		///	<summary>
		///		�T���l�C�����X�V
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2010/01/02 MakeThumbnail�𕪗�</history>
		//=========================================================================
		internal void UpdateThumbnail()
		{
			List<AnimeEpisode>	targets = new List<AnimeEpisode>();

			foreach (AnimeEpisode episode in Episodes)
				if (!episode.ThumbnailMaked)
					targets.Add( episode );


			foreach(AnimeEpisode episode in targets)
			{
				try
				{
					MakeThumbnail( episode );
				}
				catch(UpdatingException ex)
				{
					Logger.Output( "(�T���l�C��) " + ex.Message + "(" + episode.ToString() + ")" );
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�w�肵���G�s�\�[�h�̃T���l�C�����쐬
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2010/01/02 MakeThumbnail�𕪗�</history>
		//=========================================================================
		private bool MakeThumbnail(AnimeEpisode episode)
		{
			bool	bMake = false;

			// �u�ăG���R�[�h��ɃT���l�C���쐬�v�I�v�V����
			if( Properties.Settings.Default.makeThumbnailAfterEncode )
			{
				bMake = episode.HasFile && episode.IsEncoded;
			}
			else
			{
				bMake = episode.HasFile;
			}

			if( bMake )
			{
				Process		prc;
				string		args;

				if (File.Exists(episode.FilePath))
				{
					bool fileUsed = false;

					//-----------------------------
					// �^�撆�ł��邱�Ƃ𔻒�
					//-----------------------------
					try
					{
						FileStream openTest = new FileStream(
							episode.FilePath	,
							FileMode.Open		, 
							FileAccess.Read		,
							FileShare.None		);

						openTest.Close();
					}
					catch(IOException e)
					{
						fileUsed = true;	// �t�@�C���g�p���ŊJ���Ȃ�
					}

					if(!fileUsed)
					{
						MemoryStream bmpImage;

						try
						{
							using( VideoImage videoImg = new VideoImage( episode.FilePath ) )
							{
								bmpImage = videoImg.GetFrameImage( Settings.Default.thumbnailSecond );
							}
						}
						catch(VideoImage.CannotOpenException ex)
						{
							throw new UpdatingException("�T���l�C���쐬�G���[(�t�@�C�����J���Ȃ�)");
						}
						catch(VideoImage.RenderFailedException ex)
						{
							throw new UpdatingException("�T���l�C���쐬�G���[(�C���[�W��؂�o���Ȃ�)");
						}
						catch(Exception ex)
						{
							throw new UpdatingException("�T���l�C���쐬�G���[");
						}

						//----------------------------------------------
						// �ԑg�T���l�C���t�@�C���փC���[�W��A��
						//----------------------------------------------
						try
						{
							mThambnailLock.WaitOne();

							if ( ThambnailImage == null )
								throw new UpdatingException( "�����G���[(UpdateThumbnail)" );

							int oldw = mThambnailImage.Width;								// ���T���l�C���̕�
							int imgw = Math.Max(
											ThambnailSize.Width * episode.StoryNumber,
											oldw									);		// �V�T���l�C���̕�
							int imgx = ThambnailSize.Width * ( episode.StoryNumber - 1 );	// �T���l�C����ǉ�����X���W

							// ��������������ƈ����Ȃ��̂Ń��~�b�g��������
							if (32000 < imgx)
							{
								Logger.Output("�T���l�C���t�@�C�����傫�����邽�ߒǉ��ł��܂���B");
								return false;
							}
							Image newimg = new Bitmap(imgw, ThambnailSize.Height);		// �V�T���l�C��
							Image tmpimg = new Bitmap(bmpImage);

							Graphics g = Graphics.FromImage( newimg );

							g.DrawImage( mThambnailImage,
								0									,
								0									,
								new Rectangle(
									0							,
									0							,
									oldw						,
									mThambnailImage.Size.Height ),
								GraphicsUnit.Pixel					);						// ���T���l�C������R�s�[

							g.DrawImage(
								tmpimg						,
								new Rectangle(
									imgx				,
									0					,
									ThambnailSize.Width	,
									ThambnailSize.Height)	,
								new Rectangle(
									0					,
									0					,
									tmpimg.Size.Width	,
									tmpimg.Size.Height	)	,	// �V�T���l�C���ɒǉ�
								GraphicsUnit.Pixel			);

							mThambnailImage.Dispose();										// �X�V�O�̃C���[�W��j��

							newimg.Save(
								ThambnailFile	,
								ImageFormat.Png	);

							tmpimg.Dispose();

							mThambnailImage = newimg;										// �V�C���[�W�ɒu������

							episode.ThumbnailMaked = true;

							this.mIsDirty = true;
						}
						finally
						{
							mThambnailLock.ReleaseMutex();
						}
					}
				}
			
			}
				
			return true;
		}

		//=========================================================================
		///	<summary>
		///		�ԑg��V�K�\��E�\��ύX�E�\��m�F����
		///	</summary>
		/// <remarks>
		///		�����̕����\���S�ė\��ςɂ���B(�V�K�\��܂��͗\��ύX)
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal void CheckReserve(ReserveManager manager)
		{
			foreach (AnimeEpisode episode in Episodes)
			{
				if( episode.IsReserved )
				{
					episode.CheckReserve(manager);
				}
			}
		}


		//=========================================================================
		///	<summary>
		///		�ԑg�f�[�^��XML�ɏ����o��
		///	</summary>
		/// <remarks>
		///		���(AnimeServer)��Save����Ă΂��
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Write(System.Xml.XmlWriter xw)
		{
			xw.WriteStartElement("ProgramInformation");

			// �ԑg���̏����o��

			xw.WriteElementString("Hash"				, Convert.ToString(mHashCode));

			xw.WriteElementString("Title"				, title);
			xw.WriteElementString("StoryCount"			, Convert.ToString(mStoryCount));
			xw.WriteElementString("LinkOnlineDatabase"	, linkOnlineDatabase ? "1" : "0");
			xw.WriteElementString("SyoboiTID"			, Convert.ToString(syoboiTid));
			
			xw.WriteElementString("TvStation"			, tvStation);
			xw.WriteElementString("SyoboiTvStation"		, syoboiTvStation);

			xw.WriteElementString("LastUpdate"			, Convert.ToString(mLastUpdate.Ticks));
			xw.WriteElementString("WithoutPower"		, WithoutPower? "1" : "0");
			xw.WriteElementString("AdjustStartTime"		, adjustStartTime.ToString()	);
			xw.WriteElementString("AdjustEndTime"		, adjustEndTime.ToString()		);
			xw.WriteElementString("SyobocalPolicy"		, Enum.GetName( syobocalPolicy.GetType(), syobocalPolicy)	);
			xw.WriteElementString("SyobocalSpecifyNumber", syobocalSpecifyNumber.ToString()							);
			xw.WriteElementString("EnableFilterKeyword"	, Convert.ToString( enableFilterKeyword )	);
			xw.WriteElementString("FilterKeyword"		, filterKeyword								);
			xw.WriteElementString("Priority"			, Convert.ToString(priority)				);

			// �ԑg�e�b�f�[�^�̏����o��

			foreach (AnimeEpisode episode in Episodes)
				episode.Write( xw );

			// �G���R�[�h���̏����o��

			if (mEncoderType != null)
			{
				xw.WriteElementString("EncodeClass", mEncoderType.FullName);

				xw.WriteStartElement(mEncoderProfile.TagName);

				mEncoderProfile.Write(xw);
				
				xw.WriteEndElement();
			}
			else
				xw.WriteElementString("EncodeClass", null);

			// �X�P�W���[�����̏����o��
			foreach ( Scheduler.Profile prof in mSchedulerProfiles )
			{
				xw.WriteStartElement( "SchedulerProfile" );
				xw.WriteAttributeString( "Type", prof.GetType().FullName );
				xw.WriteAttributeString( "Assembly", prof.GetType().Assembly.FullName );

				prof.Export( xw );
				
				xw.WriteEndElement();
			}

			xw.WriteEndElement();
		}

		//=========================================================================
		///	<summary>
		///		XML����ǂݍ���
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Read(System.Xml.XmlReader xr)
		{
			mSchedulerProfiles.Clear();

			mEpisodes = new EpisodeList();
			mEncoderProfile	= null;
			int	length = 0;

			while (xr.Read())
			{

				if (xr.NodeType == System.Xml.XmlNodeType.Element)
				{

					if (xr.LocalName.Equals("Title"))
						title = xr.ReadString();
					else if (xr.LocalName.Equals("StoryCount"))
						mStoryCount = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("Length"))
						length = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("LinkOnlineDatabase"))
						linkOnlineDatabase = xr.ReadElementContentAsInt() == 1 ? true : false;
					else if (xr.LocalName.Equals("SyoboiTID"))
						syoboiTid = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("TvStation"))
						tvStation = xr.ReadElementContentAsString();
					else if (xr.LocalName.Equals("SyoboiTvStation"))
						syoboiTvStation = xr.ReadElementContentAsString();
					else if (xr.LocalName.Equals("Hash"))
						mHashCode = (uint)xr.ReadElementContentAsDecimal();
					else if (xr.LocalName.Equals("LastUpdate"))
					{
						string strDateTime = xr.ReadElementContentAsString();
						mLastUpdate = new DateTime(Convert.ToInt64(strDateTime));
					}
					else if (xr.LocalName.Equals("WithoutPower"))
						WithoutPower = xr.ReadElementContentAsInt() == 1 ? true : false;
					else if (xr.LocalName.Equals("AdjustStartTime"))
						adjustStartTime = xr.ReadElementContentAsLong();
					else if (xr.LocalName.Equals("AdjustEndTime"))
						adjustEndTime = xr.ReadElementContentAsLong();
					else if (xr.LocalName.Equals("SyobocalPolicy"))
						syobocalPolicy = (SyobocalPolicy)Enum.Parse(syobocalPolicy.GetType(), xr.ReadElementContentAsString());
					else if (xr.LocalName.Equals("SyobocalSpecifyNumber"))
						syobocalSpecifyNumber = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("EnableFilterKeyword"))
						enableFilterKeyword = bool.Parse(xr.ReadElementContentAsString());
					else if (xr.LocalName.Equals("FilterKeyword"))
						filterKeyword = xr.ReadElementContentAsString();
					else if(xr.LocalName.Equals("Priority"))
						priority = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("Records"))
					{
						AnimeEpisode episode;

						// StoryCount++;
						episode = new AnimeEpisode(this, 0);

						episode.Read(xr);

						mEpisodes.Add(episode);
					}
					else if (xr.LocalName.Equals("EncodeClass"))
					{
						Encoder encoder;
						string typeName;

						typeName = xr.ReadElementString();

						mEncoderType = EncodeManager.FindEncoder(typeName);

						if (mEncoderType != null)
						{
							encoder = (Encoder)Activator.CreateInstance(mEncoderType);
							mEncoderProfile = Activator.CreateInstance(encoder.ProfileType) as EncodeProfile;
						}
					}
					else if (mEncoderProfile != null
						   && xr.LocalName.Equals(mEncoderProfile.TagName))
					{
						mEncoderProfile.Read(xr);
					}
					// �X�P�W���[���ݒ��ǂݍ���
					else if (xr.LocalName.Equals("SchedulerProfile"))
					{
						string profType = xr.GetAttribute("Type");
						string assemName = xr.GetAttribute("Assembly");

						if (profType != null && assemName != null)
						{
							Scheduler.Profile prof;
							System.Runtime.Remoting.ObjectHandle obj;

							try
							{
								if ((obj = Activator.CreateInstance(assemName, profType)) != null)
								{
									if ((prof = obj.Unwrap() as Scheduler.Profile) != null)
									{
										prof.Import(xr);
										mSchedulerProfiles.Add(prof);
									}
								}
							}
							catch (TypeLoadException ex)
							{
							}
						}
					}
				}
				else if (xr.NodeType == System.Xml.XmlNodeType.EndElement)
					if (xr.LocalName.Equals("ProgramInformation"))
						break;
			}

			// ver1.9.02�ȑO����̃f�[�^�ڍs�p
			// �u����ڂ���e���r�ǖ��v��������΁u�^��e���r�ǖ��v����
			if (string.IsNullOrEmpty(syoboiTvStation))
			{
				syoboiTvStation = tvStation;
			}

			if( 0 < length )
			{
				foreach( AnimeEpisode ep in this.Episodes )
					ep.Length = length;
			}

		}

	}


}
