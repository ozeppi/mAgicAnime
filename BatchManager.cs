//=========================================================================
///	<summary>
///		�o�b�`�����}�l�[�W��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬	Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		�o�b�`�W���u�I�u�W�F�N�g
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public abstract class BatchJob
	{
		public abstract	void Do();				// �W���u�̏��������s[����]
		public virtual		void Cancel() {}		// �W���u�̏����ɃL�����Z����������[�񓯊�]
	}

	//=========================================================================
	///	<summary>
	///		�o�b�`�W���u�}�l�[�W��
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	/// <history>2008/11/16 �����o����S��static�������폜</history>
	//=========================================================================
// <MOD> 2008/11/16 ->
	public class BatchManager
//	public static class BatchManager
// <MOD> 2008/11/16 <-
	{
		//-------------------------
		// �����o
		//-------------------------
// <MOD> 2008/11/16 ->
		private BatchJob[]				currentJobs;		// ���s���̃W���u���X�g
		private List<Thread>			execThreadList;		// �W���u���s�p�X���b�h���X�g
		private List<AutoResetEvent>	stoppedFlags;		// �X���b�h�I���t���O���X�g
//		private BatchJob				currentJob;			// ���s���̃W���u
//		private static Thread			backGroundThread;	// �W���u���s�p�X���b�h
// <MOD> 2008/11/16 <-
		private bool					abortFlag;			// �A�{�[�g�t���O
		private Queue<BatchJob>		jobQueue;			// �W���u�L���[
		private readonly int			concurrentNums;		// ���s���s��
// <ADD> 2008/11/16 ->
		private Mutex					lockCallback;		// ��ʂւ̃R�[���o�b�N��r������(1�ɐ���)
// <ADD> 2008/11/16 <-

// <MOD> 2008/11/16 �Ō�̃W���u�����������t���O��ǉ� ->
		public delegate void OnProcessedEvent(BatchJob job,bool last);
		public event OnProcessedEvent ProcessedEvent;				// �W���u�������̒ʒm�C�x���g
// <MOD> 2008/11/16 <-
		public delegate void OnJobErrorEvent(string t);			// �W���u�G���[���̒ʒm�C�x���g
		public event OnJobErrorEvent JobErrorEvent;

		//-------------------------
		// ���\�b�h
		//-------------------------

// <MOD> 2008/11/16 ->
		internal BatchManager( int nums )
//		static BatchManager()
// <MOD> 2008/11/16 <-
		{
			jobQueue = new Queue<BatchJob>();
			abortFlag = false;

// <MOD> 2008/11/16 ->
			concurrentNums = nums;

			currentJobs		= new BatchJob[concurrentNums];

			execThreadList	= new List<Thread>();
			stoppedFlags	= new List<AutoResetEvent>();

			lockCallback	= new Mutex();

			//-----------------------------
			// n�{�̎��s�p�X���b�h������
			//-----------------------------
			for( int i = 0 ; i < concurrentNums ; ++i )
			{
				int							threadNumber		= i;
				ParameterizedThreadStart	paramedThread;
				Thread						newThread;

				paramedThread	= new ParameterizedThreadStart( JobProc );
				newThread		= new Thread( paramedThread );

				execThreadList.Add( newThread );
				stoppedFlags.Add( new AutoResetEvent( false ) );
			}

			for( int i = 0 ; i < concurrentNums ; ++i )
			{
				execThreadList[ i ].Start( i );
			}
//			backGroundThread = new Thread(new ThreadStart(BackGroundProc));
//			backGroundThread.Start();
// <MOD> 2008/11/16 <-
		}

		//=========================================================================
		///	<summary>
		///		�o�b�`�}�l�[�W�����I��
		/// </summary>
		/// <remarks>
		///		�S�W���u���L�����Z������B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void Finalize()
		{
#if DEBUG
			Logger.Output("Finalize");
#endif
			abortFlag = true;
// <MOD> 2008/11/16 ->
				CancelJobs();
//// <MOD> 2008/07/10 ->
//            CancelJobs();
//            CancelCurrentJob();
//            //lock (jobQueue)
//            //{
//            //    jobQueue.Clear();
//            //}
//// <MOD> 2008/07/10 <-
// <MOD> 2008/11/16 <-

			// �W���u���s�p�̃X���b�h��҂����킹
// <MOD> 2008/11/16 ->
			List<WaitHandle>	waits = new List<WaitHandle>();

			foreach( AutoResetEvent eve in stoppedFlags )
				waits.Add( eve );

// <PENDING> 2008/11/16 STA�X���b�h�ŕ����n���h����Wait���T�|�[�g����Ȃ����� ->
			foreach( WaitHandle handle in waits )
				handle.WaitOne();
//			WaitHandle.WaitAll( waits.ToArray() );
// <PENDING> 2008/11/16 <-

//			backGroundThread.Join();
// <MOD> 2008/11/16 <-
		}

		//=========================================================================
		///	<summary>
		///		�o�b�`�����X���b�h �v���V�[�W��
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/11/16 �����W���u�������s�Ή�</history>
		//=========================================================================
		private void JobProc( object param )
//		private void BackGroundProc()
		{
			BatchJob	job;
			int			threadNumber = (int)param;

#if DEBUG
			Logger.Output("JobProc �J�n");
			Logger.Output(string.Format("...threadNumber[{0:0}]", threadNumber));
#endif

			while( !abortFlag )
			{
				Thread.Sleep( 200 );

				job = PeekJob();

				while(job != null)
				{
					try
					{
						lock( currentJobs )
						{
							currentJobs[threadNumber] = job;
						}
//						currentJob = job;

						// �f�L���[�����W���u�����s
						job.Do();
					}
					catch (Exception e)
					{
						// �o�b�`�W���u�ł���ȏ�A��O�̓��b�Z�[�W��\���������O�ɏo�͂��ׂ�
						JobErrorEvent("(�o�b�`�W���u�G���[) " + e.Message);
					}
					finally
					{
						lock( currentJobs )
						{
							currentJobs[threadNumber] = null;
						}
					}

					if ( !abortFlag )						// �L�����Z������Procesed�C�x���g�����s���Ȃ�
					{
						//-------------------------
						// ��ʂփW���u������ʒm
						//-------------------------
// <MOD> 2008/11/16 ->
						if( lockCallback.WaitOne() )		// ��ʂւ̃R�[���o�b�N�͎�ނ��킸�����ɍs��Ȃ�
						{
							//------------------------------
							// �Ō�̃W���u�����𔻒�
							//------------------------------
							bool	last		=	false;
							bool	executing	=	false;

							// ���Ɏ��s���̃W���u�����邩����
							lock( currentJobs )
							{
								for( int i = 0 ; i < concurrentNums ; ++i )
									executing |= (currentJobs[i] != null);
							}

							// �u�ҋ@�W���u�Ȃ��v���u���s���W���u�Ȃ��v�Ȃ犮��
							last = (JobCount == 0) && !executing;

							ProcessedEvent( job, last );

							lockCallback.ReleaseMutex();
						}
//						ProcessedEvent( job );			// �W���u�������ʂ��R�[���o�b�N
// <MOD> 2008/11/16 <-

						job = PeekJob();
					}
				}
			}

#if DEBUG
			Logger.Output(string.Format("JobProc({0}) �I���t���O���o", threadNumber));
#endif

			// �X���b�h�I���t���O�n�m
			stoppedFlags[threadNumber].Set();
		}

		//=========================================================================
		///	<summary>
		///		�W���u�𓊓�
		/// </summary>
		/// <remarks>
		///		�����W���u�I�u�W�F�N�g�͖������邪�A�W���u�̓��e���������ǂ�����
		///		��ʑ��ŕۏႷ�邱�ƁB
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public void AddJob(BatchJob job)
		{
#if DEBUG
			Logger.Output("BatchManager::AddJob");
			Logger.Output(string.Format("...job:{0:0}", job.ToString()));
#endif
			lock (jobQueue)
			{
				// �d�����ē������Ȃ��悤�`�F�b�N
				// (�W���u���e�̏d���`�F�b�N��BatchManagaer.Contains�ōs���Ă���)
				if (!jobQueue.Contains(job))
					jobQueue.Enqueue(job);
// <ADD> 2008/08/09 ->
				else
					Logger.Output( "(�����G���[) �d�������W���u�I�u�W�F�N�g�̓���" );
// <ADD> 2008/08/09 <-
			}

			Logger.Output("�W���u���� - " + job.ToString());
		}

		//=========================================================================
		///	<summary>
		///		�S�W���u���L�����Z��[����]
		/// </summary>
		/// <remarks>
		///		�S�Ă̎��s���̃W���u�Ƒҋ@�W���u���L�����Z������B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/11/16 CancelCurrentJob�Ɠ��������B</history>
		//=========================================================================
		public void CancelJobs()
		{
#if DEBUG
			Logger.Output("BatchManager::CancelJob");
#endif
			//----------------------------
			// �ҋ@���̃W���u���N���A
			//----------------------------
			if (0 < jobQueue.Count)
				Logger.Output("�ҋ@���̑S�W���u���L�����Z������܂���");

			lock (jobQueue)
			{
				jobQueue.Clear();
			}

			//------------------------------
			// ���s���̃W���u�ɒ��~��������
			//------------------------------
			BatchJob[]	cancelJobs;
			bool		executing	= false;

			lock( currentJobs )
			{
				cancelJobs = currentJobs;
			}

			foreach( BatchJob job in currentJobs )
				executing |= (job != null);

			if( executing )
			{
				Logger.Output("���s���̃W���u���L�����Z�����Ă��܂�...");

				for( int i = 0 ; i < concurrentNums ; ++i )
				{
					BatchJob	job = cancelJobs[ i ];

					if( job != null )
					{
						Logger.Output( string.Format( "...�W���u:{0}", job.ToString() ) );
						job.Cancel();

						//----------------------------
						// �W���u����~����̂�ҋ@
						//----------------------------
						bool		stopped		= false;
						DateTime	baseTime	= DateTime.Now;
		
						Logger.Output( "......�W���u��~�҂�" );

						for(;;)
						{
							lock( currentJobs )
							{
								stopped =	(currentJobs[ i ] != job)
										||	(currentJobs[ i ] == null);
							}
							if( stopped )
							{
								Logger.Output( ".........OK" );
								break;
							}

							long passage = (long)DateTime.Now.Subtract( baseTime ).TotalMilliseconds;
							if( 5000 < passage )
							{
							    Logger.Output( ".........�^�C���A�E�g" );
							    break;
							}

							Thread.Sleep( 100 );
						}
					}
				}
			}

		}

// <DEL> 2008/11/16 ->
		////=========================================================================
		/////	<summary>
		/////		���s���̃W���u���L�����Z��
		///// </summary>
		///// <remarks>
		///// </remarks>
		///// <history>2006/XX/XX �V�K�쐬</history>
		////=========================================================================
		//public void CancelCurrentJob()
		//{
		//    if ( currentJob != null )
		//    {
		//        currentJob.Cancel();
		//    }
		//}
// <DEL> 2008/11/16 <-

		//=========================================================================
		///	<summary>
		///		�W���u�I�u�W�F�N�g���L���[������o��
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		private BatchJob PeekJob()
		{
			BatchJob job;

			lock (jobQueue)
			{
				if (jobQueue.Count > 0)
					job = jobQueue.Dequeue();
				else
					job = null;
			}

			//Console.WriteLine("�G���R�[�h�W���u���擾���܂���");

			return job;
		}

		//=========================================================================
		///	<summary>
		///		�L���[�C���O����Ă���W���u�̐���Ԃ�
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public int JobCount
		{
			get
			{
				lock (jobQueue)
				{
					return jobQueue.Count;
				}
			}
		}

		//=========================================================================
		///	<summary>
		///		�L���[�C���O����Ă���W���u�̃��X�g��Ԃ�
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public BatchJob[] GetQueueingJobs()
		{
			BatchJob[] jobs;
			lock (jobQueue)
			{
				jobs = new BatchJob[jobQueue.Count];
				jobs = jobQueue.ToArray();
			}
			return jobs;
		}

		//=========================================================================
		///	<summary>
		///		���s���̃W���u��Ԃ�
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		/// <history>2008/11/16 ���s���s���̃W���u����ĂɕԂ�</history>
		//=========================================================================
		public BatchJob[] GetCurrentJob()
		{
			BatchJob[] jobs;
			lock( currentJobs )
			{
				jobs = currentJobs;
			}
			return jobs;
		}

		//=========================================================================
		///	<summary>
		///		�W���u�I�u�W�F�N�g�����ɃL���[�C���O����Ă��邩�Ԃ�
		/// </summary>
		/// <remarks>
		///		�W���u�̓��e�������ł��邩�̔���͍s��Ȃ��B
		/// </remarks>
		/// <history>2006/XX/XX �V�K�쐬</history>
		//=========================================================================
		public bool Contains(BatchJob target)
		{
			lock (jobQueue)
			{
				foreach (BatchJob job in jobQueue)
				{
					if (job.Equals(target))
					{
						return true;
					}
				}
			}
			return false;
		}

	}
}
