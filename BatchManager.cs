//=========================================================================
///	<summary>
///		バッチ処理マネージャ
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成	Dr.Kurusugawa</history>
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
	///		バッチジョブオブジェクト
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	public abstract class BatchJob
	{
		public abstract	void Do();				// ジョブの処理を実行[同期]
		public virtual		void Cancel() {}		// ジョブの処理にキャンセルをかける[非同期]
	}

	//=========================================================================
	///	<summary>
	///		バッチジョブマネージャ
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	/// <history>2008/11/16 メンバから全てstatic属性を削除</history>
	//=========================================================================
// <MOD> 2008/11/16 ->
	public class BatchManager
//	public static class BatchManager
// <MOD> 2008/11/16 <-
	{
		//-------------------------
		// メンバ
		//-------------------------
// <MOD> 2008/11/16 ->
		private BatchJob[]				currentJobs;		// 実行中のジョブリスト
		private List<Thread>			execThreadList;		// ジョブ実行用スレッドリスト
		private List<AutoResetEvent>	stoppedFlags;		// スレッド終了フラグリスト
//		private BatchJob				currentJob;			// 実行中のジョブ
//		private static Thread			backGroundThread;	// ジョブ実行用スレッド
// <MOD> 2008/11/16 <-
		private bool					abortFlag;			// アボートフラグ
		private Queue<BatchJob>		jobQueue;			// ジョブキュー
		private readonly int			concurrentNums;		// 平行実行数
// <ADD> 2008/11/16 ->
		private Mutex					lockCallback;		// 上位へのコールバックを排他制御(1つに制限)
// <ADD> 2008/11/16 <-

// <MOD> 2008/11/16 最後のジョブが完了したフラグを追加 ->
		public delegate void OnProcessedEvent(BatchJob job,bool last);
		public event OnProcessedEvent ProcessedEvent;				// ジョブ処理時の通知イベント
// <MOD> 2008/11/16 <-
		public delegate void OnJobErrorEvent(string t);			// ジョブエラー時の通知イベント
		public event OnJobErrorEvent JobErrorEvent;

		//-------------------------
		// メソッド
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
			// n本の実行用スレッドを準備
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
		///		バッチマネージャを終了
		/// </summary>
		/// <remarks>
		///		全ジョブをキャンセルする。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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

			// ジョブ実行用のスレッドを待ち合わせ
// <MOD> 2008/11/16 ->
			List<WaitHandle>	waits = new List<WaitHandle>();

			foreach( AutoResetEvent eve in stoppedFlags )
				waits.Add( eve );

// <PENDING> 2008/11/16 STAスレッドで複数ハンドルのWaitがサポートされないため ->
			foreach( WaitHandle handle in waits )
				handle.WaitOne();
//			WaitHandle.WaitAll( waits.ToArray() );
// <PENDING> 2008/11/16 <-

//			backGroundThread.Join();
// <MOD> 2008/11/16 <-
		}

		//=========================================================================
		///	<summary>
		///		バッチ処理スレッド プロシージャ
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/11/16 複数ジョブ同時実行対応</history>
		//=========================================================================
		private void JobProc( object param )
//		private void BackGroundProc()
		{
			BatchJob	job;
			int			threadNumber = (int)param;

#if DEBUG
			Logger.Output("JobProc 開始");
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

						// デキューしたジョブを実行
						job.Do();
					}
					catch (Exception e)
					{
						// バッチジョブである以上、例外はメッセージを表示せずログに出力すべき
						JobErrorEvent("(バッチジョブエラー) " + e.Message);
					}
					finally
					{
						lock( currentJobs )
						{
							currentJobs[threadNumber] = null;
						}
					}

					if ( !abortFlag )						// キャンセル時はProcesedイベントを実行しない
					{
						//-------------------------
						// 上位へジョブ完了を通知
						//-------------------------
// <MOD> 2008/11/16 ->
						if( lockCallback.WaitOne() )		// 上位へのコールバックは種類を問わず同時に行わない
						{
							//------------------------------
							// 最後のジョブ完了を判定
							//------------------------------
							bool	last		=	false;
							bool	executing	=	false;

							// 他に実行中のジョブがあるか判定
							lock( currentJobs )
							{
								for( int i = 0 ; i < concurrentNums ; ++i )
									executing |= (currentJobs[i] != null);
							}

							// 「待機ジョブなし」かつ「実行中ジョブなし」なら完了
							last = (JobCount == 0) && !executing;

							ProcessedEvent( job, last );

							lockCallback.ReleaseMutex();
						}
//						ProcessedEvent( job );			// ジョブ処理結果をコールバック
// <MOD> 2008/11/16 <-

						job = PeekJob();
					}
				}
			}

#if DEBUG
			Logger.Output(string.Format("JobProc({0}) 終了フラグ検出", threadNumber));
#endif

			// スレッド終了フラグＯＮ
			stoppedFlags[threadNumber].Set();
		}

		//=========================================================================
		///	<summary>
		///		ジョブを投入
		/// </summary>
		/// <remarks>
		///		同じジョブオブジェクトは無視するが、ジョブの内容が同じかどうかは
		///		上位側で保障すること。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void AddJob(BatchJob job)
		{
#if DEBUG
			Logger.Output("BatchManager::AddJob");
			Logger.Output(string.Format("...job:{0:0}", job.ToString()));
#endif
			lock (jobQueue)
			{
				// 重複して投入しないようチェック
				// (ジョブ内容の重複チェックはBatchManagaer.Containsで行っておく)
				if (!jobQueue.Contains(job))
					jobQueue.Enqueue(job);
// <ADD> 2008/08/09 ->
				else
					Logger.Output( "(内部エラー) 重複したジョブオブジェクトの投入" );
// <ADD> 2008/08/09 <-
			}

			Logger.Output("ジョブ投入 - " + job.ToString());
		}

		//=========================================================================
		///	<summary>
		///		全ジョブをキャンセル[同期]
		/// </summary>
		/// <remarks>
		///		全ての実行中のジョブと待機ジョブをキャンセルする。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/11/16 CancelCurrentJobと統合した。</history>
		//=========================================================================
		public void CancelJobs()
		{
#if DEBUG
			Logger.Output("BatchManager::CancelJob");
#endif
			//----------------------------
			// 待機中のジョブをクリア
			//----------------------------
			if (0 < jobQueue.Count)
				Logger.Output("待機中の全ジョブがキャンセルされました");

			lock (jobQueue)
			{
				jobQueue.Clear();
			}

			//------------------------------
			// 実行中のジョブに中止をかける
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
				Logger.Output("実行中のジョブをキャンセルしています...");

				for( int i = 0 ; i < concurrentNums ; ++i )
				{
					BatchJob	job = cancelJobs[ i ];

					if( job != null )
					{
						Logger.Output( string.Format( "...ジョブ:{0}", job.ToString() ) );
						job.Cancel();

						//----------------------------
						// ジョブが停止するのを待機
						//----------------------------
						bool		stopped		= false;
						DateTime	baseTime	= DateTime.Now;
		
						Logger.Output( "......ジョブ停止待ち" );

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
							    Logger.Output( ".........タイムアウト" );
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
		/////		実行中のジョブをキャンセル
		///// </summary>
		///// <remarks>
		///// </remarks>
		///// <history>2006/XX/XX 新規作成</history>
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
		///		ジョブオブジェクトをキューから取り出す
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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

			//Console.WriteLine("エンコードジョブを取得しました");

			return job;
		}

		//=========================================================================
		///	<summary>
		///		キューイングされているジョブの数を返す
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		キューイングされているジョブのリストを返す
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
		///		実行中のジョブを返す
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2008/11/16 平行実行中のジョブを一斉に返す</history>
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
		///		ジョブオブジェクトが既にキューイングされているか返す
		/// </summary>
		/// <remarks>
		///		ジョブの内容が同じであるかの判定は行わない。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
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
