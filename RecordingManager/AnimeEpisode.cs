//=========================================================================
///	<summary>
///		�G�s�\�[�h�f�[�^�N���X
///	</summary>
/// <remarks>
///		�A�j���e�b�̘^���Ԃ��Ǘ����A�\��E�G���R�[�h���̏������s��
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬 Dr.Kurusugawa</history>
/// <history>2007/08/16	�ԑg���Ƃ̃X�P�W���[���ݒ�ɑΉ�</history>
/// <history>2007/11/11	State�̕ύX��property set����SetState���\�b�h�ɕύX</history>
/// <history>2009/12/28	�Â����C�R�����g�폜</history>
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
using System.Runtime.InteropServices;
using KernelAPI;
using magicAnime.Properties;
using magicAnime.OnLine_DataBase;
using Helpers;
//using Microsoft.VisualBasic.FileIO;

namespace magicAnime
{
	public class EpisodeMethodException : Exception
	{
		public EpisodeMethodException(string mes)
			: base(mes)
		{
		}
	};

	//=========================================================================
	///	<summary>
	///		�A�j���e�b�̘^���Ԃ��Ǘ�����
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public class AnimeEpisode
	{
		//---------------------------
		// �񋓎q�̒�`
		//---------------------------

		//=========================================================================
		///	<summary>
		///		�G�s�\�[�h�̏��
		///	</summary>
		/// <remarks>
		///		Ver1.9.18�ȍ~�A���̏��͕\���Ɏg���݂̂�
		///		�����̉۔��f�Ɏg�p���Ă͂Ȃ�Ȃ��B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		public enum State
		{
			Notfound	,	// �^��t�@�C����������Ȃ�
			Planned		,	// �����v�����m��
			Recorded	,	// �^���
			Encoded		,	// �ăG���R�[�h��
			Stored		,	// �ۑ���
			Scheduling	,	// �^��\���
			Encoding	,	// �G���R�[�h��
			Undecided	,	// ����
			Changed		,	// �������ԕύX
			DontCare	,	// �������Ȃ�
			LostSchedule,	// �\�����
			Busy		,	// ����������
		};

		//---------------------------
		// �����o�ϐ�
		//---------------------------
		private	AnimeProgram	mParent;                            // �ԑg�I�u�W�F�N�g
		private bool			mHasPlan		= false;			// �����X�P�W���[���m��
		private	DateTime		mStartTime;                         // �����J�n����
		private int				mLength			= 0;				// ��������[min]
		private bool			mIsReserved		= false;			// �\��σt���O
		private DateTime		mReserveStartTime;					// �^��\�t�g�ɗ\�񂵂Ă���J�n����
		private bool			mHasFile		= false;			// ����t�@�C������
		private string			mFilePath;							// ����t�@�C���p�X
		private	bool			mThumbnailMaked	= false;            // �T���l�C���쐬�σt���O
		private bool			mIsUnread		= true;				// ���ǃt���O
		private bool			mIsReserveError	= false;			// �\��G���[�t���O
		private bool			mIsEncoded		= false;			// �ăG���R�[�h��
		private bool			mIsStored		= false;			// �ۑ���ɓ]����
		private bool			mIsBusy			= false;			// �������t���O
		public	string			mSubTitle		= "";				// �T�u�^�C�g��
		private bool			mIsDirty		= false;			// �f�[�^�ύX�t���O
		private int				mStoryNumber	= 0;				// �G�s�\�[�h�ԍ�
		private int				mRepeatNumber	= -1;				// n��ڂ̕���
		private bool			mPlanError		= false;			// �����v�����f�[�^�ُ�

		private	Encoder			encoding		= null;				// �G���R�[�h�������̃G���R�[�_�I�u�W�F�N�g


		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//========================================================================
		public AnimeEpisode(
			AnimeProgram	animeProgram	,
			State			initialState	,
			string			inFilePath		,
			DateTime		inDateTime		,
			int				inStoryNumber	,
			string			inSubTitle		)
		{


			this.mStoryNumber	= inStoryNumber;
			this.mSubTitle		= inSubTitle;
			this.mIsUnread		= true;
			
			this.mParent		= animeProgram;

			this.Dirty			= true;
		}

		//=========================================================================
		///	<summary>
		///		�R���X�g���N�^
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public AnimeEpisode( AnimeProgram animeProgram, int storyNumber )
		{
			this.mFilePath	= null;
			this.mSubTitle	= "";
			
			this.mParent	= animeProgram;
			
			this.mStoryNumber = storyNumber;

			this.Dirty = true;
		}


		//=========================================================================
		///	<summary>
		///		�e�I�u�W�F�N�g�ւ̎Q�Ƃ�Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public AnimeProgram Parent	{ get { return mParent; } }

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
			get { return mIsDirty; }
			set { mIsDirty = value; }
		}

		//=========================================================================
		///	<summary>
		///		���ǃt���O
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal bool Unread
		{
			get { return this.mIsUnread; }
			set
			{
				this.mIsUnread = value;
				this.mIsDirty = true;
			}
		}

		internal bool IsReserved
		{
			get{ return mIsReserved; }
			set{ mIsReserved = value; }
		}

		// �\�񂵂Ă��鎞���ƕ����������قȂ邩����
		internal bool JudgeTimeChanged
		{
			get
			{
				if( this.mHasPlan && this.mIsReserved )
					return (this.StartRecordDateTime != this.mReserveStartTime);
				return false;
			}
		}

		internal bool HasPlan
		{
			get { return mHasPlan; }
			set
			{
				mHasPlan	= value;
				Dirty		= true;
			}
		}

		internal bool HasFile
		{
			get { return mHasFile; }
			set
			{
				mHasFile	= value;
				Dirty		= true;
			}
		}

		internal bool IsEncoded
		{
			get{ return mIsEncoded; }
			set
			{
				mIsEncoded	= value;
				Dirty		= true;
			}
		}

		internal bool IsStored
		{
			get { return mIsStored; }
			set
			{
				mIsStored	= value;
				Dirty		= true;
			}
		}

		internal bool IsBusy
		{
			get{ return mIsBusy; }
		}

		internal DateTime ReserveDateTime
		{
			get{ return mReserveStartTime; }
			set
			{
				mReserveStartTime	= value;
				Dirty				= true;
			}
		}

		internal bool IsReserveError
		{
			get{ return mIsReserveError; }
			set
			{
				mIsReserveError	= value;
				Dirty			= true;
			}
		}

		// �������Ԃ��I����Ă��邩����
		internal bool JudgeTimeEnd(DateTime now)
		{
			if( mHasPlan )
				return EndDateTime < now;
			return false;
		}

		// �\��^���K�v�Ƃ��邩�Ԃ�(���ɘ^�悳��Ă����false)
		internal bool IsRecordRequired
		{
			get{ return !mHasFile; }
		}
		internal int Length
		{
			get{ return mLength; }
			set
			{
				mLength = value;
			}
		}
		internal int RepeatNumber
		{
			get{ return mRepeatNumber; }
		}
		// �\��^�撆�������͘^���ł��邩�Ԃ�
		internal bool IsRecorded
		{
			get
			{
				if( this.HasPlan && this.IsReserved )
					return (this.StartDateTime <= DateTime.Now);
				else if( this.HasFile )
					return true;
				return false;
			}
		}
		// �����J�n���Ԃ��߂������Ԃ�
		internal bool IsStartedOnair
		{
			get
			{
				if( this.HasPlan )
					return (this.StartDateTime <= DateTime.Now);
				return false;
			}
		}
		// �����v�����f�[�^�ُ�̗L��
		internal bool PlanError
		{
			get{ return mPlanError; }
			set
			{
				mPlanError		= value;
				Dirty			= true;
			}
		}
		
		//=========================================================================
		///	<summary>
		///		�����J�n������Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public DateTime StartDateTime
		{
			get
			{
				return mStartTime;
			}
			set
			{
				mStartTime	= value;
				Dirty		= true;
			}
		}

		//=========================================================================
		///	<summary>
		///		�����I��������Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public DateTime EndDateTime
		{
			get
			{
				return mStartTime.AddMinutes(mLength);
			}
		}

		//=========================================================================
		///	<summary>
		///		�^��J�n������Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public DateTime StartRecordDateTime
		{
			get
			{
				return mStartTime.AddMinutes( Settings.Default.reserveStart
											+ Parent.adjustStartTime );
			}
		}

		//=========================================================================
		///	<summary>
		///		�^��I��������Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public DateTime EndRecordDateTime
		{
			get
			{
				return EndDateTime.AddMinutes( Settings.Default.reserveEnd
											 + Parent.adjustEndTime );
			}
		}

		//=========================================================================
		///	<summary>
		///		���݂̃G�s�\�[�h��Ԃ�Ԃ�
		///	</summary>
		/// <remarks>
		///		Ver1.9.18�ȍ~�A��Ԃ̒l�͕ێ����Ȃ��B
		///		�e�t���O�����Ԃ����߂Ă����Ԃ��̂݁B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public State CurrentState
		{
			get
			{
				if( IsBusy )
					return State.Busy;

				if( HasFile )
				{
					if( IsStored )
						return State.Stored;
					if( IsEncoded && !IsStored )
						return State.Encoded;
					return State.Recorded;
				}

				if( !HasPlan )
					return State.Undecided;
				else
				{
					if( JudgeTimeChanged )
						return State.Changed;
					else
					{
						if( IsReserveError )
							return State.LostSchedule;
						if( IsReserved )
							if( JudgeTimeEnd( DateTime.Now ) )
								return State.Notfound;
							else
								return State.Scheduling;
						else
							if( JudgeTimeEnd( DateTime.Now ) )
								return State.Notfound;
							else
								return State.Planned;
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�Ή����铮��t�@�C���p�X��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public string FilePath	
		{
			get
			{
				return mHasFile ? mFilePath : "";
			}
			set
			{
				mFilePath	= value;
				this.Dirty	= true;
			}
		}

		public int StoryNumber		{ get { return mStoryNumber;  } }

		//=========================================================================
		///	<summary>
		///		�Đ��\������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public bool IsPlayable
		{
			get
			{
				return mHasFile && !mIsBusy;
			}
		}

		//=========================================================================
		///	<summary>
		///		�ۑ���ɓ]���\������
		///	</summary>
		/// <remarks>
		///		�G���R�[�h�ρA�������͘^��ςōăG���R�[�h�Ȃ��Ȃ�TRUE
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal bool IsStorable
		{
			get
			{
				if( mHasFile )
				{
					if( mIsEncoded )
						return true;
					if( !mIsEncoded	&&
						Parent.EncoderProfile == null	)
						return true;
				}
				return false;
			}
		}

		//=========================================================================
		///	<summary>
		///		�T���l�C���쐬�ς݂��Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal bool ThumbnailMaked
		{
			get { return mThumbnailMaked;  }
			set { mThumbnailMaked = value;  }
		}

		//=========================================================================
		///	<summary>
		///		���������ɏd�������邩�Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal bool IsDoubleBooking()
		{
			List<AnimeEpisode>	conflicts = Parent.Parent.DoubleBookingEpisodes;
			if( conflicts != null )
			{
				return conflicts.Contains( this );
			}
			return false;
		}


		//=========================================================================
		///	<summary>
		///		�����v�������X�V
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2007/05/06 �X�V�̏�����ύX</history>
		/// <history>2009/01/04 ���\�b�h���ύX</history>
		/// <history>2010/04/17 �Â��R�����g�폜</history>
		//=========================================================================
		internal void UpdatePlan(
			List<SyoboiCalender.SyoboiRecord>	syoboiList	,	// [i] ����ڂ���f�[�^
			out bool							abnormal	)	// [o] true:�����f�[�^�Ɉُ�����o
		{
			DateTime					now			 = DateTime.Now;
			SyoboiCalender.SyoboiRecord	syoboiRecord = null;

			abnormal = false;

			//-----------------------------
			// ���������f�[�^���X�V
			//-----------------------------
			// ������ύX�ł���̂͗\��^�悪�J�n����O�܂�
			bool isRecording	= this.IsReserved && this.IsStartedOnair;
			bool isRecorded		= this.HasFile;

			bool dontCare		= this.mIsBusy || (isRecording || isRecorded);

			//------------------------------------
			// �Y������G�s�\�[�h�̃f�[�^������
			//------------------------------------

			var records = SyoboiRecordFilter.EpisodeAndStationFilter(
				syoboiList,
				mStoryNumber,
				mParent.syoboiTvStation);

			// �{�����ƍĕ����̃f�[�^�����

			if(Parent.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyLatest)
			{
				// �ł���̕�����I��
				syoboiRecord = SyoboiRecordFilter.LatestPlanFilter( records, out mRepeatNumber );
			}
			else if (Parent.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyNumber)
			{
				// n��ڂ̕�����I��
				syoboiRecord = SyoboiRecordFilter.NumberPlanFilter(
					records,
					Parent.syobocalSpecifyNumber,
					out mRepeatNumber);
			}
			else if (Parent.syobocalPolicy == AnimeProgram.SyobocalPolicy.SpecifyEarly)
			{
				// �ł�����������I��
				bool	keep = false;

				// ���ݎ����ɉ����đI����������v�������ς�邪
				// �\��^��J�n��ɂ͕ς��Ȃ��B
				if( this.HasPlan && this.IsReserved )
					keep = (this.StartDateTime <= now);

				if( keep )
				{
					// �^��J�n��̓v������ύX���Ȃ�
					if( 0 <= mRepeatNumber )
						syoboiRecord = SyoboiRecordFilter.NumberPlanFilter(
							records,
							mRepeatNumber,
							out mRepeatNumber );
					else
						records = null;
				}
				else
				{
					syoboiRecord = SyoboiRecordFilter.EarlytPlanFilter(
						records,
						now,
						out mRepeatNumber );

					if( syoboiRecord == null  )
					{
						// ����̕������Ȃ��ꍇ�͍Ō�̕�����I�����Ă���
						syoboiRecord = SyoboiRecordFilter.LatestPlanFilter(
							records,
							out mRepeatNumber );
					}
				}
			}

			if (syoboiRecord != null)
			{
				//-------------------------------------------------------------
				// �I�����C��DB�ɍX�V�����������ꍇ�A���[�J���f�[�^���X�V
				//-------------------------------------------------------------
				if( mPlanError )
				{
					Logger.Output("�T�[�o�[�̕����f�[�^���������Ă��܂� - " + this.ToString());
					mPlanError	= false;
				}

				// �T�u�^�C�g�����Z�b�g
				if( !string.IsNullOrEmpty(syoboiRecord.subtitle) &&
					(mSubTitle != syoboiRecord.subtitle)		 )
				{
					mSubTitle	= syoboiRecord.subtitle;
					Dirty		= true;
				}

				if( !dontCare )
				{
					// �擾���������v�����ŏ㏑��
					if( !mHasPlan
					||	(mStartTime	!= syoboiRecord.onAirDateTime)
					||	(mLength	!= syoboiRecord.length)			)
					{
						mHasPlan	= true;
						mStartTime	= syoboiRecord.onAirDateTime;
						mLength		= syoboiRecord.length;
						Dirty		= true;
					}

				}
			}
			else
			{
				//--------------------------------------
				// �f�[�^�[�x�[�X�ɏ�񂪂Ȃ������ꍇ
				//--------------------------------------
				if( !dontCare )
				{
					if( mHasPlan && Settings.Default.keepPlan )
					{
						// ���炩�̌����ł���ڂ��邩��f�[�^�������Ă��Ă������͎c���Ă���
						if( !mPlanError )
						{
							mPlanError	= true;
							Dirty		= true;
							Logger.Output("�T�[�o�[�̕����f�[�^�������Ă��܂�������̃f�[�^���ێ����܂� - " + this.ToString());
							abnormal	= true;
						}
					}
					else
					{
						mHasPlan	= false;
					}
					Dirty		= true;

					if( IsReserved )
					{
						Logger.Output("�\��σG���g���̕����f�[�^�������Ȃ��Ă��܂� - " + this.ToString());
						abnormal	= true;
					}
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�^���Ԃ̍X�V��^��t�@�C����Ή��t����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal void UpdateState(
			DateTime	now			,	// �X�V�����
			string[]	movieFiles	)	// �^��t�@�C�����X�g
		{
			if( mHasPlan && !mHasFile )
			{
				{
					//----------------------------------------
					// �������Ԃɍ��v����^��t�@�C��������
					//----------------------------------------
					string	filterKeyword;
					filterKeyword	= Parent.enableFilterKeyword ?
										Parent.filterKeyword : null;

					string filename = AnimeServer.FindCapturedFile(
								GetUniqueString()	,
								StartDateTime		,
								EndDateTime			,
								movieFiles			,
								filterKeyword		);

					if ( filename != null ) // �t�@�C����������Θ^��ςɂ���
					{
						this.mHasFile	= true;
						this.mFilePath	= filename;
						this.mIsUnread	= true;
						this.mIsEncoded	= false;	// �������t�@�C���́u�ăG���R�ρv��u�]���ρv�ł͂Ȃ�
						this.mIsStored	= false;
						this.Dirty		= true;
					}
				}
			}
		
			if( !mHasFile )
			{
				//-----------------------------
				// �ۑ��σt�@�C��������
				//-----------------------------
				try
				{
					bool		isExist		= false;
					string		targetDir	= Settings.Default.saveFolder;
					
					if( !string.IsNullOrEmpty(targetDir)	&&
						Directory.Exists( targetDir )		)
					{
						if( Settings.Default.classificatePrograms )
							targetDir = Path.Combine( targetDir, PathHelper.ToFileTitle( mParent.title ) );

						isExist = Directory.Exists( targetDir );

						if( isExist )
						{
							string[]	files;
							string		pattern = GetFormattedFileName() + ".*";

							files = Directory.GetFiles( targetDir, pattern );

							if( files.Length == 1 )
							{
								this.mHasFile	= true;
								this.mFilePath	= files[0];
								this.mIsEncoded	= false;
								this.mIsStored	= true;
								this.Dirty		= true;
							}
						}
					}
				}
				catch(Exception ex)
				{
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		���̃G�s�\�[�h�̘^��\��ɗ��p�����ӂȕ������Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public string GetUniqueString()
		{
			string unique;

			unique = mParent.UniqueID.ToString() + "_";			// ��������
			unique += mStoryNumber.ToString();

			return unique;
		}

		//=========================================================================
		///	<summary>
		///		���̃G�s�\�[�h�̗\���ۗ����ׂ������f
		///	</summary>
		/// <remarks>
		///		�D��x�������͏��false��Ԃ��B
		///		��ʑ��ł͂��̌��ʂ�����Reserve���Ăяo�������f���ׂ��B
		///		(�蓮�ŋ����\�񂵂����ꍇ�͂��̌���ł͂Ȃ�)
		/// </remarks>
		/// <history>2009/11/23 �V�K�쐬</history>
		//=========================================================================
		internal bool IsReservePending()
		{
			if( Settings.Default.enablePriority )
			{
				AnimeServer server = Parent.Parent;
					
				server.CheckDoubleBooking();

				return server.DoubleBookingEpisodes.Contains(this);
			}
			else
				return false;
		}

		//=========================================================================
		///	<summary>
		///		���̃G�s�\�[�h��^��\�t�g�ɗ\��
		///	</summary>
		/// <remarks>
		///		���ɗ\�񂳂�Ă���ꍇ�͉������Ȃ��B
		///		�������Ԃ̕ύX��\��ɔ��f������B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2009/06/28 �^��\�t�g"�Ȃ�"�͗\�񎸔s����</history>
		//=========================================================================
		internal bool Reserve(
			ReserveManager		manager			,	// [i] �^��\��}�l�[�W��
			out string			errorMessage	)	// [o] �\�񎸔s�ڍ�
		{
			string descript;

			descript = string.Format("{0:0} {1:0}�b", Parent.title, StoryNumber);

			if( !IsReserved || JudgeTimeChanged )
			{
				DateTime	start	= DateTime.Now;
				int			length	= 0;

				//-----------------------------------
				// �^��J�n�����Ƙ^��I���������v�Z
				//-----------------------------------
				try
				{
					start	= this.StartRecordDateTime;
					length	= (int)(this.EndRecordDateTime - start).TotalMinutes;

					if( length <= 0 )
						throw new Scheduler.ZeroLengthScheduleTimeException();

					//-------------------------------
					// �ԑg�̗\��v���t�@�C�����擾
					//-------------------------------
					Scheduler.Profile prof = null;

					if (ReserveManager.DefaultScheduler				!= null		&&
						ReserveManager.DefaultScheduler.ProfileType != null		)
					{
						prof = Parent.SchedulerProfile( ReserveManager.DefaultScheduler.ProfileType );
					}

					if( !IsReserved )
					{
						//--------------------------------------
						// ����̕����E�\�񂪏��� �� �V�K�\��
						//--------------------------------------
						if (manager.MakeReservation(
								descript,
								GetUniqueString(),
								Parent.tvStation,
								start,
								(uint)length,
								Parent.UniqueID,
								prof))
						{
							this.mIsReserved		= true;
							this.mIsReserveError	= false;
							this.mReserveStartTime	= start;
							this.Dirty				= true;
						}
						else
						{
							this.mIsReserved		= false;
							this.mIsReserveError	= true;
							this.Dirty				= true;
							errorMessage = "�^��\�t�g���w�肳��Ă��܂���B(�I�v�V������ʂőI�����ĉ�����)";
							return false;
						}
					}
					else if( JudgeTimeChanged )
					{
						ReserveManager.ChangeResult res;
						
						//--------------------------------------
						// �������ԕύX �� �������ԕύX���s��
						//--------------------------------------
						res = manager.ChangeReservation(
								descript				,
								GetUniqueString()		,
								Parent.tvStation		,
								start					,
								(uint)length			,
								Parent.UniqueID			,
								prof					);
						
						if ( res == ReserveManager.ChangeResult.OK )				// �ύX�ɐ����H
						{
							this.mIsReserved		= true;
							this.mIsReserveError	= false;
							this.mReserveStartTime	= start;
							this.Dirty				= true;
						}
						else if ( res == ReserveManager.ChangeResult.Lost )			// �\�񂪎���ꂽ�H
						{
							this.mIsReserved		= false;
							this.mIsReserveError	= true;
							this.Dirty				= true;
						}
					}

				}
				catch (Scheduler.DoubleBookingException e)										// �\�񂪏d���H
				{
					Logger.Output( "(�\��Ǘ�)�\�񂪏d�����邽�߁A�\���o�^�ł��܂���B " + start.ToString() + " - " + descript );
					this.mIsReserved		= false;
					this.mIsReserveError	= true;
					this.Dirty				= true;
					errorMessage = "�\�񂪏d�����邽�߁A�\���o�^�ł��܂���B " + start.ToString() + " - " + descript;
					return false;
				}
				catch(Scheduler.SchedulerBaseExecption e)
				{
					object		[]objAttributes;
					string		errorDetail			= "�\�񎞂ɃG���[���������܂����B";

					objAttributes = e.GetType().GetCustomAttributes(
						typeof(Scheduler.SchedulerExceptionAtribute), true );

					if( objAttributes != null )
					{
						Scheduler.SchedulerExceptionAtribute	exceptionAtrribute;
						exceptionAtrribute = objAttributes[0] as Scheduler.SchedulerExceptionAtribute;

						if( exceptionAtrribute != null )
							errorDetail = exceptionAtrribute.Description;
					}

					Logger.Output( "(�\��Ǘ�)"
						+ errorDetail
						+ " "
						+ start.ToString()
						+ "�`"
						+ EndDateTime.ToString()
						+ " - "
						+ descript				);

					this.mIsReserved		= false;
					this.mIsReserveError	= true;
					this.Dirty				= true;

					errorMessage = errorDetail + " " + descript;
					Logger.Output( "(�\��Ǘ�)" + errorMessage );
					return false;
				}
				catch(System.Exception ex)
				{
					this.mIsReserved		= false;
					this.mIsReserveError	= true;
					this.Dirty				= true;

					errorMessage = ex.Message + " " + descript;
					Logger.Output( "(�\��Ǘ�)" + errorMessage );
					return false;
				}

			}

			errorMessage = "";
			return true;
		}

		//=========================================================================
		///	<summary>
		///		�^��\�t�g�ɗ\�񂪓����Ă��邩�m�F
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/10/21 �\��m�F�ɕs�\�ȃv���O�C���Ȃ疳��</history>
		//=========================================================================
		internal void CheckReserve(ReserveManager manager)
		{
			string descript;

			descript = string.Format("{0:0} {1:0}�b", Parent.title, StoryNumber);

			bool	evaluated;
			bool	exist;

			if( IsReserved )
			{
				//------------------------------
				// �\�񂪏����Ă��Ȃ����`�F�b�N
				//------------------------------
				evaluated = manager.ExistReservation( descript, GetUniqueString(), out exist);

				if( evaluated && !exist )
				{
					this.mIsReserved		= false;
					this.mIsReserveError	= true;
					this.Dirty				= true;
				}
			}
			else
			{
				//------------------------------------
				// �������\�񂪕������Ă��邩�`�F�b�N
				//------------------------------------
                evaluated = manager.ExistReservation(descript, GetUniqueString(), out exist);

                if ( evaluated && exist )
                {
					this.mIsReserved		= true;
					this.mIsReserveError	= false;
					this.mReserveStartTime	= this.mStartTime;	// <PENDING> 2009/12/28
					this.Dirty				= true;
                }
			}
		}

		//=========================================================================
		///	<summary>
		///		�^��\�t�g�̗\����L�����Z������
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2008/10/22 �V�K�쐬</history>
		//=========================================================================
		internal bool CancelReserve(ReserveManager manager)
		{
			string		descript;

			descript = string.Format("{0:0} {1:0}�b", Parent.title, StoryNumber);

			if( !IsReserved )
				return false;

			try
			{
				if( !manager.CancelReservation( descript, GetUniqueString() ) )
					return false;
			}
			catch(Exception ex)
			{
				return false;
			}

			this.mIsReserved	= false;
			this.Dirty			= true;

			return true;
		}


		//=========================================================================
		///	<summary>
		///		���̃G�s�\�[�h�̘^��t�@�C�����ăG���R�[�h����
		///	</summary>
		/// <remarks>
		///		�G���R�[�h�I���܂Ő���͖߂�Ȃ��B
		///		EncodeJob�N���X�𗘗p���Ĕ񓯊��Ŏ��s����B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		internal void Encode()
		{
			Encoder	encoder		= null;

			if( !mHasFile )
				throw new EpisodeMethodException("�t�@�C�����w�肳��Ă��Ȃ�");
			if( mIsEncoded )
				throw new EpisodeMethodException("���ɃG���R�[�h����Ă���");
			if( mIsBusy )
				throw new EpisodeMethodException("�������̂��߃G���R�[�h�J�n�ł��Ȃ�");

			if (Properties.Settings.Default.encodedFolder.Equals(""))
				throw new EpisodeMethodException("�G���R�[�h�o�͐�t�H���_���ݒ肳��Ă��܂���");

			if (Parent.EncoderType == null)
				throw new EpisodeMethodException("�G���R�[�h�v���t�@�C�����w�肳��Ă��܂���");

			if (!File.Exists(mFilePath))
				throw new EpisodeMethodException("�G���R�[�h���̃t�@�C��������܂���" + mFilePath);

			try
			{
				string encodedFile;

				encodedFile = Path.Combine(
								Settings.Default.encodedFolder	,
								GetFormattedFileName()			);

				//----------------------
				// encoding��ԂɕύX
				//----------------------
				this.mIsBusy	= true;
				this.mIsDirty	= true;

				//----------------------
				// �\��ǉ����̏���
				//----------------------
				Encoder.TvProgramAdditionalInfo ai = new Encoder.TvProgramAdditionalInfo();

				ai.Title		= PathHelper.ToFileTitle(Parent.title);
				ai.StoryNumber	= this.StoryNumber.ToString();
				ai.Subtitle		= PathHelper.ToFileTitle(this.mSubTitle);
				ai.TvStation	= Parent.tvStation;
				ai.StartDate	= this.StartDateTime.ToShortDateString();
				ai.StartTime	= this.StartDateTime.ToShortTimeString();

				//----------------------
				// �G���R�[�_�̏���
				//----------------------
				encoder = (Encoder)Activator.CreateInstance(Parent.EncoderType);

				encodedFile += encoder.Extension;

				encoding = encoder;

				//--------------------
				// �G���R�[�_���s
				//--------------------

				encoder.DoEncode(
					FilePath				,
					ref encodedFile			,
					Parent.EncoderProfile	,
					ai						);							// �G���R�[�_�v���O�C���Ăяo��

				//----------------------------------------
				// �G���R�[�h��A���̘^��t�@�C���폜
				//----------------------------------------
				if ( Settings.Default.removeSourceWhenEncoded )
				{
					try
					{
						File.Delete( mFilePath + "." );

						//
						// �ꏊ���K��̘^��t�H���_�Ȃ�T�u�f�B���N�g�����ƍ폜
						//
						if (Settings.Default.captureSubDir)
						{
							string subFolder, parentFolder;

							subFolder		= Path.GetDirectoryName( mFilePath + "." );
							parentFolder	= Directory.GetParent( subFolder ).FullName;

							if (Settings.Default.captureFolder.Equals(parentFolder))
							{
								string[] files;

								// �܂��f�B���N�g�����̃t�@�C����S�č폜���Ă���
								files = Directory.GetFiles(subFolder);
								foreach (string f in files)
									File.Delete(f);

								Directory.Delete(subFolder, true);
							}
						}

					}
					catch (Exception e)
					{
						Logger.Output(e.Message);
					}

				}

				//-----------------------
				// �G���R�[�h�Ϗ�Ԃ�
				//-----------------------
				this.FilePath	= encodedFile;
				this.mIsEncoded	= true;
			}
			catch (AbortException x)
			{
				this.mIsBusy	= false;
				throw x;
			}
			catch (Exception x)
			{
				this.mIsBusy	= false;
				throw x;
			}
			finally
			{
				this.encoding	= null;
				this.mIsBusy	= false;
				this.mIsDirty	= true;
			}
		}

		//=========================================================================
		///	<summary>
		///		�G���R�[�h�����𒆒f
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void CancelEncode()
		{
			if (encoding!=null)
			{
				encoding.AbortEncodeProcess();
			}
		}


		//=========================================================================
		///	<summary>
		///		�t�H�[�}�b�g���ꂽ�t�@�C�������擾����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public string GetFormattedFileName()
		{
			string formattedName, titleName, subTitleName;

			titleName		= PathHelper.ToFileTitle(mParent.title);
			subTitleName	= PathHelper.ToFileTitle(mSubTitle);

			try
			{
				string	dateStr = "";
				string	timeStr = "";
				string	station	= "";

				if(this.HasPlan)
				{
					var helper = new DateTimeHelper(
						this.StartDateTime,
						Settings.Default.hoursPerDay - 24);
					
					dateStr	= helper.ToShortDateString().Replace("/", "");
					timeStr	= helper.ToShortTimeString().Replace(":", "");
				}

				station = Parent.tvStation;

				formattedName = string.Format(
					Settings.Default.saveNameFormat,
					titleName,
					mStoryNumber,
					subTitleName,
					dateStr,
					timeStr,
					station);
			}
			catch(Exception ex)
			{
				throw new UpdatingException("�ۑ��t�@�C��������������������܂���B");
			}

			return formattedName;
		}

		//=========================================================================
		///	<summary>
		///		�t�H�[�}�b�g���ꂽ�ԑg�G�s�\�[�h�������Ԃ�
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public override string ToString()
		{
			string myName;

			try
			{
				string	dateStr = "";
				string	timeStr = "";
				string	station	= "";

				if(this.HasPlan)
				{
					var helper = new DateTimeHelper(
						this.StartDateTime,
						Settings.Default.hoursPerDay - 24);
					
					dateStr	= helper.ToShortDateString();
					timeStr	= helper.ToShortTimeString();
				}

				station = Parent.tvStation;

				myName = string.Format(
					Settings.Default.saveNameFormat,
					mParent.title,
					mStoryNumber,
					mSubTitle,
					dateStr,
					timeStr,
					station);
			}
			catch(Exception ex)
			{
				throw new UpdatingException("�ۑ��t�@�C��������������������܂���B");
			}

			return myName;
		}

		//=========================================================================
		///	<summary>
		///		���̃G�s�\�[�h�̘^��t�@�C����ۑ���ֈړ�����
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Store()
		{
			string		storeName, storeDir;

			if( !mHasFile )
				throw new EpisodeMethodException("�t�@�C�����w�肳��Ă��Ȃ�");
			if( mIsStored )
				throw new EpisodeMethodException("���ɓ]����");
			if( mIsBusy )
				throw new EpisodeMethodException("�������̂��ߓ]���ł��Ȃ�");

			try
			{
				//----------------------
				// ��������ԂɑJ��
				//----------------------
				this.mIsBusy	= true;
				this.mIsDirty	= true;

				if (Settings.Default.saveFolder.Trim().Equals(""))
					throw new EpisodeMethodException("�I�v�V�����ōŏI�ۑ��悪�ݒ肳��Ă��܂���B");

				if ( !File.Exists( mFilePath ) )
					throw new EpisodeMethodException("�t�@�C�������݂��Ȃ����߁A�]���ł��܂���B" + mFilePath);

				storeDir = Settings.Default.saveFolder;	// �ۑ���t�H���_

				//----------------------------------
				// �^�C�g�����Ƃ̃T�u�t�H���_�쐬
				//----------------------------------
				if (Settings.Default.classificatePrograms)
				{
					string subDir;

					subDir		= PathHelper.ToFileTitle(mParent.title);

					storeDir	= Path.Combine( storeDir, subDir );

					if (!Directory.Exists(storeDir))
						Directory.CreateDirectory(storeDir);
				}

				storeName = Path.Combine( storeDir, GetFormattedFileName() + Path.GetExtension(mFilePath) );

				File.Move( mFilePath + ".", storeName );

				//----------------------------------
				// �t������t�@�C�����ꏏ�Ɉړ�����
				//----------------------------------
				if ( Settings.Default.copyWithOthers )
				{
					string []files;

					files = Directory.GetFiles(
								Path.GetDirectoryName( mFilePath ) + ".",
								Path.GetFileNameWithoutExtension( mFilePath ) + ".*" );
				
					foreach( string f in files )
					{
						// �t�@�C����"A.B.C"�̂������d�g���q".B.C"��؂�o��
						string	fname;
						string	multiExt;
						for(fname = f;;)
						{
							if( string.IsNullOrEmpty( Path.GetExtension( fname ) ) )
								break;
							fname = Path.GetFileNameWithoutExtension(fname);
						}
						multiExt = Path.GetFileName(f).Substring( fname.Length );

						File.Move( f, Path.Combine( storeDir, GetFormattedFileName() + multiExt ) );
					}
				
				}

				//----------------------------------
				// ���̃T�u�f�B���N�g�����폜����
				//----------------------------------
				if ( Settings.Default.captureSubDir &&
					 Settings.Default.removeSubdir )
				{
					// �ꏊ���K��̘^��t�H���_���`�F�b�N
					string subFolder, parentFolder;

					subFolder		= Path.GetDirectoryName( mFilePath + "." ) + ".";
					parentFolder	= Directory.GetParent( subFolder ).FullName;

					if ( Settings.Default.captureFolder.Equals( parentFolder ) )
					{
						string[] files;

						// �܂��f�B���N�g�����̃t�@�C����S�č폜���Ă���
						files = Directory.GetFiles( subFolder );
						foreach ( string f in files )
							File.Delete( f + "." );

						Directory.Delete( subFolder, true );
					}
				}

				this.mIsStored	= true;
				this.FilePath	= storeName;
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				this.mIsBusy	= false;
				this.Dirty		= true;
			}
		}

		//=========================================================================
		///	<summary>
		///		����t�@�C�����K��̃t�@�C�����Ƀ��l�[������
		///	</summary>
		/// <remarks>
		///		�ۑ���ւ̈ړ��͍s��Ȃ��B
		/// </remarks>
		/// <history>2008/10/22 �V�K�쐬</history>
		//=========================================================================
		public bool RenameFile( out string newName )
		{
			string		storeName;

			newName = null;

			if( !mHasFile )
				return false;

			if( !File.Exists( mFilePath ) )
				return false;

			try
			{
				string subDir = Path.GetDirectoryName(mFilePath);

				newName = Path.Combine(subDir, GetFormattedFileName() + Path.GetExtension(mFilePath));

				File.Move(mFilePath + ".", newName);

				mFilePath = newName;

			}
			catch (Exception)
			{
				newName = null;
				return false;
			}
			finally
			{
				this.Dirty = true;
			}

			return true;
		}

		//=========================================================================
		///	<summary>
		///		XML�֏����o��
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Write(System.Xml.XmlWriter xw)
		{
			xw.WriteStartElement("Records");

			xw.WriteElementString("StoryNumber"		, Convert.ToString(mStoryNumber));
			xw.WriteElementString("FilePath"		, mFilePath);
			xw.WriteElementString("DateTime"		, Convert.ToString(mStartTime.Ticks));
			xw.WriteElementString("SubTitle"		, mSubTitle);
			xw.WriteElementString("Unread"			, mIsUnread ? "1":"0" );

			xw.WriteElementString("ThumbnailMaked"	, mThumbnailMaked ?"1":"0");

			xw.WriteElementString("HasPlan"			, mHasPlan.ToString()		);
			xw.WriteElementString("IsReserved"		, mIsReserved.ToString()	);
			xw.WriteElementString("ReserveStartTime", Convert.ToString(mReserveStartTime.Ticks));
			xw.WriteElementString("HasFile"			, mHasFile.ToString()		);
			xw.WriteElementString("IsReserveError"	, mIsReserveError.ToString());
			xw.WriteElementString("IsEncoded"		, mIsEncoded.ToString()		);
			xw.WriteElementString("IsStored"		, mIsStored.ToString()		);
			xw.WriteElementString("Length"			, mLength.ToString()		);
			xw.WriteElementString("RepeatNumber"	, mRepeatNumber.ToString()	);
			xw.WriteElementString("PlanError"		, mPlanError.ToString()		);

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
			while (xr.Read())
			{
				if (xr.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (xr.LocalName.Equals("StoryNumber"))
						mStoryNumber = xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("FilePath"))
						mFilePath = xr.ReadElementContentAsString();
					// Ver1.9.18�ȑO����̈ȍ~
					else if (xr.LocalName.Equals("State"))
						MigrationState( (State)xr.ReadElementContentAsInt() );
					else if (xr.LocalName.Equals("SubTitle"))
						mSubTitle = xr.ReadElementContentAsString();
					else if (xr.LocalName.Equals("Unread"))
						mIsUnread = xr.ReadElementContentAsInt() == 1 ? true : false;
					else if (xr.LocalName.Equals("ThumbnailMaked"))
						mThumbnailMaked = xr.ReadElementContentAsInt() == 1 ? true : false;
					else if (xr.LocalName.Equals("DateTime"))
					{
						string strDateTime = xr.ReadElementContentAsString();
						mStartTime = new DateTime(Convert.ToInt64(strDateTime));
					}
					else if (xr.LocalName.Equals("HasPlan"))
						mHasPlan			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("IsReserved"))
						mIsReserved			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("ReserveStartTime"))
						mReserveStartTime	= new DateTime( Convert.ToInt64( xr.ReadElementContentAsString() ) );
					else if (xr.LocalName.Equals("HasFile"))
						mHasFile			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("IsReserveError"))
						mIsReserveError		= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("IsEncoded"))
						mIsEncoded			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("IsStored"))
						mIsStored			= bool.Parse( xr.ReadElementContentAsString() );
					else if (xr.LocalName.Equals("Length"))
						mLength				= xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("RepeatNumber"))
						mRepeatNumber		= xr.ReadElementContentAsInt();
					else if (xr.LocalName.Equals("PlanError"))
						PlanError			= bool.Parse( xr.ReadElementContentAsString() );
				}
				else if (xr.NodeType == System.Xml.XmlNodeType.EndElement)
					if (xr.LocalName.Equals("Records"))
						break;
			}

		}

		// Ver1.9.18�ȑO����̃f�[�^�ڍs�p
		private void MigrationState( State state )
		{
			this.mHasPlan		= false;
			this.mHasFile		= false;
			this.mIsReserved	= false;
			this.mIsReserveError= false;
			this.mIsEncoded		= false;
			this.mIsStored		= false;

			switch( state )
			{
			case State.Notfound:
				this.mHasPlan		= true;
				break;
			case State.Planned:
				this.mHasPlan		= true;
				break;
			case State.Recorded:
				this.mHasPlan		= true;
				this.mHasFile		= true;
				break;
			case State.Encoded:
				this.mHasPlan		= true;
				this.mHasFile		= true;
				this.mIsEncoded		= true;
				break;
			case State.Stored:
				this.mHasPlan		= true;
				this.mHasFile		= true;
				this.mIsStored		= true;
				break;
			case State.Scheduling:
				this.mHasPlan		= true;
				this.mHasPlan		= true;
				this.mIsReserved	= true;
				break;
			case State.Changed:
				this.mHasPlan		= true;
				this.mIsReserved	= true;
				this.mReserveStartTime = DateTime.MinValue;	// <PENDING> 2009/12/28
				break;
			case State.LostSchedule:
				this.mHasPlan			= true;
				this.mIsReserveError	= true;
				break;
			}

		}
	}


}
