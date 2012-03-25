//=========================================================================
///	<summary>
///		iEPG/tvpi予約 モジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		iEPG/tvpi予約クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	class iEPGScheduler : Scheduler
	{
		//-----------------------------
		// オーバーライドプロパティ
		//-----------------------------
		public override string Name { get { return "iEPG"; } }
		public override AbilityFlag Ability
		{
			get { return AbilityFlag.MakeReservation; }
		}
		public override Type ProfileType
		{
			get { return null; }
		}

		//=========================================================================
		///	<summary>
		///		iEPG対応録画ソフトに予約を登録
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public override void MakeReservation(
			string		title		,	// [i] 録画タイトル
			string		uniqueID	,	// [i] ユニークID
			string		tvStation	,	// [i] テレビ局名
			DateTime	dateTime	,	// [i] 録画開始時間
			int			minute		,	// [i] 録画時間(分)
			string		Descript	,	// [i] 番組の説明
			uint		groupCode	,	// [i] 録画グループ
			Profile		param		)	// [i] 追加パラメータ
		{
			DateTime		endDateTime;
			FileStream		file		= null;
			StreamWriter	sw;
			string			tempName;

			endDateTime = dateTime.AddMinutes(minute);

			//----------------------------
			// テンポラリtvpiファイル作成
			//----------------------------

			file = File.Open(Path.GetTempFileName(), FileMode.Open);

			file.Close();

			tempName = Path.GetDirectoryName(file.Name) + @"\" +
						Path.GetFileNameWithoutExtension(file.Name) + ".tvpi";		// \が抜けていた 07/01/14

			if (File.Exists(tempName))												// 07/01/14
			{
				throw new Exception("テンポラリファイルが既に存在しています(" + tempName + ")");
			}

			File.Move(file.Name, tempName);

			file = new FileStream(tempName, FileMode.Open);

			//----------------------------------
			// tvpiファイルに予約情報を出力
			//----------------------------------
			sw = new StreamWriter(file, Encoding.GetEncoding(932));

			// ヘッダ書き出し
			sw.WriteLine( "Content-type: application/x-tv-program-info; charset=shift_jis");
			sw.WriteLine( "version: 1");

			// コンテンツ部書き出し
			sw.WriteLine( "station: "		+ tvStation	);
			sw.WriteLine( "program-title: "	+ title		);
//			sw.WriteLine("program-title: " + title.Substring(0,title.IndexOf(",")));
			sw.WriteLine( "year: "	+ string.Format("{0:D4}",dateTime.Year)		);
			sw.WriteLine( "month: "	+ string.Format("{0:D2}",dateTime.Month)	);
			sw.WriteLine( "date: "	+ string.Format("{0:D2}",dateTime.Day)		);

			sw.WriteLine( "start: " + string.Format("{0:D2}:{1:D2}", dateTime.Hour		, dateTime.Minute	) );
			sw.WriteLine( "end: "	+ string.Format("{0:D2}:{1:D2}", endDateTime.Hour	, endDateTime.Minute) );

			sw.WriteLine( "" );
//				sw.WriteLine(title.Substring(title.IndexOf(",")+1));
			sw.WriteLine( uniqueID );

			sw.Close();

			//--------------------------------------------
			// tvpiファイルに関連付けられたアプリを起動
			//--------------------------------------------
			try
			{
				Process process;

				process = Process.Start( tempName );

				process.WaitForExit();
			}
			catch (System.ComponentModel.Win32Exception)
			{
				throw new Exception("iEPG/tvpiファイルが録画ソフトに関連付けられていません。");
			}
			finally
			{
				if (file != null)
				{
					file.Close();
					File.Delete(file.Name);
				}
			}

		}

		//=========================================================================
		///	<summary>
		///		予約をキャンセル
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public override void CancelReservation(
			string title,
			string uniqueID)
		{
			//
			// iEPGではキャンセル不能
			//
		}

		//=========================================================================
		///	<summary>
		///		予約を変更
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public override void ChangeReservation(
			string title				,
			string uniqeuID				,
			DateTime newDateTime		,
			uint groupCode				,
			Profile param					)
		{
		}

		//=========================================================================
		///	<summary>
		///		予約を確認
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public override bool ExistReservation(
			string title,		// 録画タイトル
			string uniqueID	)	// ユニークID
		{
			return true;
		}

		//=========================================================================
		///	<summary>
		///		予約をフラッシュ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public override void Flush()
		{
		}

	}

}
