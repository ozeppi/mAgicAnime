//=========================================================================
///	<summary>
///		録画ソフト連動プラグイン 基底クラス モジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		録画ソフト連動プラグイン基底クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	public abstract class Scheduler
	{
		//--------------------
		// 型の宣言
		//--------------------

// <ADD> 2008/10/21 スケジューラ例外の属性クラス ->
		[AttributeUsage(AttributeTargets.Class)]
		public class SchedulerExceptionAtribute : System.Attribute
		{
			private string m_Description;

			public SchedulerExceptionAtribute(string Description) { this.Description = Description; }

			public string Description
			{
				get { return m_Description; }
				set { m_Description = value; }
			}
		}
// <ADD> 2008/10/21 <-

// <ADD> 2008/10/21 スケジューラ例外の基底クラス ->
		[Scheduler.SchedulerExceptionAtribute("予約のエラーです")]
		public class SchedulerBaseExecption : Exception { }

		// 無効な予約時間を指定された時の例外
		[Scheduler.SchedulerExceptionAtribute("無効な予約時間です")]
		public class InvalidScheduleTimeException : SchedulerBaseExecption {}
		// 録画時間が0の例外
		[Scheduler.SchedulerExceptionAtribute("録画時間が0分以下になります")]
		public class ZeroLengthScheduleTimeException : SchedulerBaseExecption {}
// <ADD> 2008/10/21 <-
// <PENDING> 2008/10/21 この例外をSchedulerBaseExceptionから派生するのは保留(互換性のため) ->
		// 予約重複時の例外
		public class DoubleBookingException : Exception { }
		// (時間変更前の)予約消失を伴う予約重複時の例外
		public class DoubleBookingWithLostException : Exception { }
// <PENDING> 2008/10/21 <-

		[Flags]
		public enum AbilityFlag
		{
			MakeReservation		= 0x0001,	// 新規予約可能
			ChangeReservation	= 0x0002,	// 予約変更可能
			CancelReservation	= 0x0010,	// 予約キャンセル可能
			ExistReservation	= 0x0020,	// 予約チェック可能
		}

		//--------------------
		// 基底プロパティ
		//--------------------
		public abstract string Name { get; }
		public virtual string Folder { get{ return null; } }
		public virtual string Extension { get { return null; } }
		public virtual bool SubDirectory { get { return false; } }
		public abstract AbilityFlag Ability { get; }
		public virtual List<string> GetStations() { return new List<string>(); }	// テレビ局一覧(ver1.7.1)


// ▼ 2007/12/09 継承クラスでコンストラクタを使うため追加
		//=========================================================================
		///	<summary>
		///		コンストラクタ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public Scheduler() {}
// ▼ 2007/12/09

// ▼ 2008/01/03 常駐を解除する前のクリーンアップに追加
		//=========================================================================
		///	<summary>
		///		終了前のクリーンアップ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public virtual void Close() {}
// ▲ 2008/01/03


		//=========================================================================
		///	<summary>
		///		番組ごとに適用する録画プロファイルクラス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/XX/XX 新規作成 ver 1.8.01</history>
		//=========================================================================
		public class Profile
		{
			public virtual void Import(XmlReader xr) {}
			public virtual void Export(XmlWriter xw) {}
		}
		public abstract Type ProfileType { get; }

// ▽ 2007/08/16 追加
		//=========================================================================
		///	<summary>
		///		録画プロファイルを編集するプロパティページ
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public class ProfilePage : Form
		{
//			Control Parent;		// 親フォーム等
			public ProfilePage()
			{
			}
			public void Create(Control Parent)
			{
				this.TopLevel = false;
				this.Parent = Parent;
				this.Show();
			}
			public virtual void Load( Profile profile ) { }
			public virtual void Save( Profile profile ) { }
		}

		//=========================================================================
		///	<summary>
		///		プロファイルの型情報を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public virtual Type ProfilePageType { get { return null; } }
//		public virtual ProfilePage OpenProfilePage( Control parent, Profile profile ) { }
// △ 2007/08/16 追加


		//=========================================================================
		///	<summary>
		///		時間を指定して録画予約を新規登録する
		///	</summary>
		/// <remarks>
		///		同時録画できる環境では、時間重複しても失敗するとは限らない。
		///		ただし、同じ録画タイトルの場合は失敗する。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public abstract void MakeReservation(
			string				title		,
			string				uniqueID	,
			string				tvStation	,
			DateTime			dateTime	,
			int					minute		,
			string				Descript	,
			uint				groupCode	,
			Scheduler.Profile	param		);

		//=========================================================================
		///	<summary>
		///		指定された予約をキャンセルする
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public abstract void CancelReservation(string title,string uniqueID);

		//=========================================================================
		///	<summary>
		///		指定された予約を変更する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public abstract void ChangeReservation(
			string title			,
			string uniqueID			,
			DateTime newDateTime	,
			uint groupCode			,
			Scheduler.Profile param	);

		//=========================================================================
		///	<summary>
		///		指定された予約が在るか確認する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public abstract bool ExistReservation(string title, string uniqueID);

		//=========================================================================
		///	<summary>
		///		予約データのフラッシュを行う
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public abstract void Flush();
	}

}

