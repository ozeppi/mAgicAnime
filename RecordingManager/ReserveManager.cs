//=========================================================================
///	<summary>
///		録画ソフト制御モジュール
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成 Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using magicAnime.Properties;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		録画ソフト制御ラッパークラス
	///	</summary>
	/// <remarks>
	///		録画ソフト連動プラグインを経由して予約の登録や削除を行う。
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	/// <history>2007/08/15	起動時に全プラグインのSchedulerインスタンスを作っておく</history>
	//=========================================================================
	public class ReserveManager
	{
		//----------------
		// 型の定義
		//----------------
		internal enum ChangeResult
		{
			Dontcare,	// 触らない
			OK		,	// 成功
			Lost	,	// 予約を失った
			Denied	,	// 予約を動かせない
		};
		
		//-------------------------------
		// メンバ変数
		//-------------------------------
		private static List<Scheduler> schedulerList;

		//=========================================================================
		///	<summary>
		///		プラグインインスタンスリストを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public static List<Scheduler> SchedulerList
		{
			get { return schedulerList; }
		}

		//=========================================================================
		///	<summary>
		///		全プラグインをロードする
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal static void InitPlugins()
		{
			Assembly plugin;

			schedulerList = new List<Scheduler>();
			
			//-------------------------
			// 内蔵iEPGプラグイン登録
			//-------------------------

			schedulerList.Add(new iEPGScheduler());

			//-----------------------------
			// プラグインDLLを探して認識
			//-----------------------------
			Process		my;
			string		d;
			string		[]dlls;

			my = Process.GetCurrentProcess();

			d = Path.GetDirectoryName( my.MainModule.FileName );

			dlls = Directory.GetFiles(d, "*.dll");

// <MOD> 2010/04/11 ->
			var assemblies	= new List<Assembly>();
			var modules		= new List<Module>();
			var types		= new List<Type>();
			var schedTypes	= new List<Type>();

			Array.ForEach<string>(dlls, delegate (string dll)
				{
					try
					{
						assemblies.Add( Assembly.LoadFrom(dll) );
					}
					catch(Exception ex)
					{
						// 無関係なアンマネージドDLLなどを無視
					}
				});

			assemblies.ForEach( assey => modules.AddRange( assey.GetModules() ) );
			modules.ForEach( mod => types.AddRange( mod.GetTypes() ) );
			schedTypes = types.FindAll( type => type.IsSubclassOf(typeof(Scheduler)) );

			schedTypes.ForEach( delegate (Type type)
				{
					Scheduler sched;
					sched = Activator.CreateInstance( type ) as Scheduler;
					if ( sched != null )
						schedulerList.Add( sched );
				} );

//            foreach (string f in dlls)
//            {
//                {
//                    plugin = Assembly.LoadFrom(f);

//                    foreach (Module m in plugin.GetModules())
//                    {
//                        foreach (Type type in m.GetTypes())
//                        {
//                            if (type.IsSubclassOf(typeof(Scheduler)))
//                            {
//                                Scheduler sched;
//                                sched = Activator.CreateInstance( type ) as Scheduler;
//                                if ( sched != null )
//                                {
//                                    schedulerList.Add( sched );
//                                }
//                            }
//                        }
//                    }
//                }
//            }
// <MOD> 2010/04/11 <-
			//sched = plugin.CreateInstance("mAgicScheduler");

			//MessageBox.Show(sched.ToString());

		}

// ▼ 2008/01/03
		//=========================================================================
		///	<summary>
		///		常駐しているプラグインを解放
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal static void CleanupPlugins()
		{
			if( schedulerList != null )
			{
				foreach( Scheduler sched in schedulerList )
				{
					sched.Close();
				}
				schedulerList = null;
			}
		}
// ▲ 2008/01/03

		//=========================================================================
		///	<summary>
		///		指定されたクラスのプラグインインスタンスを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/08/15 新規作成</history>
		//=========================================================================
		public static Scheduler FindScheduler( string typeName )
		{
			foreach ( Scheduler sched in schedulerList )
			{
				if ( sched.GetType().Name.Equals( typeName ) )
					return sched;
			}
			return null;
		}

// ▽ 2007/08/15 追加
		//=========================================================================
		///	<summary>
		///		デフォルトスケジューラを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public static Scheduler DefaultScheduler
		{
			get
			{
				return FindScheduler( Settings.Default.schedulerType );
			}
		}
// △ 2007/08/15 追加

// ▽ 2007/08/16 追加
		//=========================================================================
		///	<summary>
		///		SchedulerタイプからProfileタイプを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/08/16 新規作成</history>
		//=========================================================================
		public Type GetProfileType()
		{
			throw new NotImplementedException();
		}
// △ 2007/08/16 追加

		//=========================================================================
		///	<summary>
		///		番組を新規予約する
		///	</summary>
		/// <remarks>
		///		予約処理に障害が発生した場合は例外を投げる
		/// </remarks>
		/// <returns>
		///		成功した場合はtrue
		///		そもそもスケージュラに能力がない場合はfalse
		/// </returns>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		internal bool MakeReservation(
			string		title		,	// 録画タイトル
			string		uniqueID	,	// 録画の一意なID
			string		tvStation	,	// テレビ局名
			DateTime	start		,	// 開始時間
			uint		length		,	// 録画長さ
			uint		groupID		,	// 録画のグループID
			Scheduler.Profile param	)	// 付帯設定
		{
			Scheduler sched	= DefaultScheduler;

			if (sched!=null &&
				(sched.Ability & Scheduler.AbilityFlag.MakeReservation) > 0)	// スケジューラが指定されて、予約が可能か？
			{

				// ファイル名によって録画ファイルを特定？
				if (Settings.Default.specifiedFile == IdentifyFileMethod.ByFileNameWithID)
				{
					title	= string.Format(
								Settings.Default.specifiedNameFormat	,
								uniqueID								,
								title									);		// IDを含んだ録画タイトルにする
				}

				//--------------------------
				// 録画ソフトに予約をかける
				//--------------------------
				sched.MakeReservation(
					title									,
					uniqueID								,
					GetRegisteredStationName( tvStation )	,
					start									,
					(int)length								,
					title									,
					groupID									,
					param									);

				return true;
			}

			return false;
		}


		//=========================================================================
		///	<summary>
		///		予約を変更する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/05/08 CancelReservationに対応</history>
		//=========================================================================
		internal ChangeResult ChangeReservation(
			string		title		,	// 録画タイトル
			string		uniqueID	,	// 録画の一意なID
			string		tvStation	,	// テレビ局名
			DateTime	start		,	// 開始時間
			uint		length		,	// 録画長さ
			uint		groupID		,	// 録画のグループID
			Scheduler.Profile param	)	// 録画設定
		{
			Scheduler sched = DefaultScheduler;

			if ( sched == null ) return ChangeResult.Dontcare;

			Logger.Output("(予約)時間変更操作 " + start.ToString() + "(" + title + ")" );

			//----------------------------
			// 可能なら直接予約を変更
			//----------------------------
			if ((sched.Ability & Scheduler.AbilityFlag.ChangeReservation) > 0)
			{
				try
				{
					sched.ChangeReservation( title, uniqueID, start, groupID, param );
				}
				catch ( Exception ex )
				{
					Logger.Output( "(予約管理)" + ex.Message );
					return ChangeResult.Denied;
				}

				return ChangeResult.OK;
			}

			//-----------------------------
			// キャンセルと登録を行う
			//-----------------------------
			if ( ( sched.Ability & Scheduler.AbilityFlag.CancelReservation ) > 0 )
			{
				//-----------------------
				// 一旦、古い予約を削除
				//-----------------------
				try
				{
					sched.CancelReservation( title, uniqueID );
				}
				catch ( Exception ex )
				{
					Logger.Output("(予約管理)" + ex.Message);
					return ChangeResult.Denied;
				}

				//-----------------------
				// 改めて予約を登録
				//-----------------------
				try
				{
					// ファイル名によって録画ファイルを特定？
					if ( Settings.Default.specifiedFile == IdentifyFileMethod.ByFileNameWithID )
					{
						title = string.Format(
							Settings.Default.specifiedNameFormat	,
							uniqueID								,
							title									); // IDを含んだ録画タイトルにする
					}

					sched.MakeReservation(
						title									,
						uniqueID								,
						GetRegisteredStationName( tvStation )	,
						start									,
						(int)length								,
						title									,
						groupID									,
						param									);
				}
				catch ( Scheduler.DoubleBookingException dex )
				{
					throw;
//					return ChangeResult.Lost;
				}
				catch ( Exception ex )
				{
					throw;
				}

				return ChangeResult.OK;
			}

// ▼ 2007/10/28 プラグインが処理しない場合はDon't careが正しい
//			return ChangeResult.OK;
			return ChangeResult.Dontcare;
// ▲ 2007/10/28
		}

		//=========================================================================
		///	<summary>
		///		指定されたタイトル・IDの予約が存在するかチェックする
		///	</summary>
		/// <remarks>
		///		戻り値はチェックの可否。
		/// </remarks>
		/// <history>2007/08/16 新規作成</history>
		//=========================================================================
// <MOD> 2008/10/21 ->
		internal bool ExistReservation(
			string		title		,	// [i] タイトル
			string		uniqueID	,	// [i] 予約ID
			out bool	exist		)	// [o] 予約が存在する
		//internal bool ExistReservation(
		//    string title	,
		//    string uniqueID)
// <MOD> 2008/10/21 <-
		{
			Scheduler sched;

// <MOD> 2008/10/21 ->
			sched = DefaultScheduler;

// <ADD> 2008/11/16 デフォルトスケジューラが指定されていない場合スルー ->
			if( sched == null )
			{
				exist  = false;
				return false;
			}
// <ADD> 2008/11/16 <-

			bool existReservation = (0 < (sched.Ability & Scheduler.AbilityFlag.ExistReservation));

			if( (sched != null) && existReservation )
			{
				exist = sched.ExistReservation(title, uniqueID);

				return true;
			}

			exist = false;
			return false;
			//sched = DefaultScheduler;
			//if (sched == null) return true;

			//return sched.ExistReservation(title, uniqueID);
// <MOD> 2008/10/21 <-
		}

		//=========================================================================
		///	<summary>
		///		予約を削除する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/05/08 新規作成</history>
		//=========================================================================
// <MOD> 2008/10/22 ->
		internal bool CancelReservation(
//		internal void CancelReservation(
// <MOD> 2008/10/22 <-
			string title		,
			string uniqueID		)
		{
			Scheduler sched;

			sched = DefaultScheduler;
// <MOD> 2008/10/22 ->
			if ( sched == null ) return false;
//			if ( sched == null ) return;
// <MOD> 2008/10/22 <-

			if ( ( sched.Ability & Scheduler.AbilityFlag.CancelReservation ) == 0 )
// <MOD> 2008/10/22 ->
				return false;
//				return;
// <MOD> 2008/10/22 <-

			sched.CancelReservation( title, uniqueID );
// <ADD> 2008/10/22 ->
			return true;
// <ADD> 2008/10/22 <-
		}


		//=========================================================================
		///	<summary>
		///		録画ソフトに予約のコミットをかける
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public void Flush()
		{
			Scheduler sched = DefaultScheduler;

			if (sched != null)
			{
				sched.Flush();
			}
		}


		//=========================================================================
		///	<summary>
		///		テレビ局名テーブルXMLパスを返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static public string TvStationsXml	// <MOD> 2009/12/30
//		static private string TvStationsXml
		{
			get
			{
				string filePath;
// <MOD> 2009/12/27 ->
                filePath = Path.Combine( Program.AppDataPath, "tvStationsII.xml" );
//				filePath = Path.GetDirectoryName(Application.ExecutablePath);
//				filePath += Path.DirectorySeparatorChar;
//				filePath += "tvStationsII.xml";
// <MOD> 2009/12/27 <-
				return filePath;
			}
		}

		internal class StationTable
		{
			public string Name;			// テレビ局名
			public string RegName;			// 登録局名
		};

		//=========================================================================
		///	<summary>
		///		テレビ局名テーブルを読み込む
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static internal List<StationTable> LoadStationTable()
		{
			XmlReader	r;
			List<StationTable>		list	= new List<StationTable>();
			StationTable			t		= null;

			try
			{
				r = XmlReader.Create(TvStationsXml);

				while (r.Read())
				{
					if (r.NodeType == XmlNodeType.Element)
					{
						if (r.LocalName.ToUpper().Equals("STATION"))
						{
							t = new StationTable();
						}
						if (r.LocalName.ToUpper().Equals("NAME"))
						{
							t.Name = r.ReadElementContentAsString();
						}
						if (r.LocalName.ToUpper().Equals("REGNAME"))
						{
							t.RegName = r.ReadElementContentAsString();
						}

					}
					else if (r.NodeType == XmlNodeType.EndElement)
					{
						if (r.LocalName.ToUpper().Equals("STATION"))
						{
							list.Add(t);
						}
					}

				}

				r.Close();

			}catch (Exception)
			{
				list.Clear();
			}

			return list;
		}

		//=========================================================================
		///	<summary>
		///		テレビ局名テーブルに登録する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static internal void ModifyStationTable(
			string		n	,	// テレビ局名
			string		r	)	// 登録局名
		{
			List<StationTable>		list	= LoadStationTable();
			bool					w		= true;				// 新規追加フラグ

			foreach (StationTable e in list)
			{
				if (e.Name.ToUpper().Equals(n.ToUpper()))
				{
					e.RegName	= r;
					w			= false;
				}
			}

			if ( w )
			{
				StationTable e = new StationTable();

				e.Name			= n;
				e.RegName		= r;

				list.Add( e );
			}

			SaveStationTable( list );

		}

		//=========================================================================
		///	<summary>
		///		テレビ局名テーブルを保存
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static internal void SaveStationTable(List<StationTable> list)
		{
			XmlTextWriter w;

			w				= new XmlTextWriter( TvStationsXml, System.Text.Encoding.UTF8);
			w.Formatting	= Formatting.Indented;

			w.WriteStartDocument();
			w.WriteStartElement("REPLACETABLE");

			foreach (StationTable e in list)
			{
				w.WriteStartElement( "STATION" );

				w.WriteElementString("NAME", e.Name );
				w.WriteElementString("REGNAME", e.RegName);

				w.WriteEndElement();
			}

			w.WriteEndElement();
			w.WriteEndDocument();
			w.Close();

		}
	
		//=========================================================================
		///	<summary>
		///		テレビ局名テーブルを元に、テレビ局名を置き換える
		///	</summary>
		/// <remarks>
		///		定義がなければ元の文字列をそのまま返す
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		static public string GetRegisteredStationName(string tvStation)
		{
			List<StationTable> list = LoadStationTable();

			foreach (StationTable e in list)
			{
				if (e.Name.ToUpper().Equals(tvStation.ToUpper()))
				{
					return e.RegName;
				}
			}

			return tvStation;
		}


		////////////////////////////////////////////////////////////////
		// FUNCTION	:	CreateDefaultScheduler
		// ABSTRACT	:	
		////////////////////////////////////////////////////////////////
// ▽ 2007/08/15 削除
		//internal static Scheduler CreateDefaultScheduler()
		//{
		//    Scheduler s;

		//    try
		//    {
		//        if (Settings.Default.schedulerType.Equals(""))
		//            return null;

		//        //
		//        // オプションで指定されたSchedulerオブジェクトを作成
		//        //
		//        s = (Scheduler)System.Activator.CreateInstance(
		//                        FindScheduler(Settings.Default.schedulerType)
		//                    );
		//    }
		//    catch (Exception)
		//    {
		//        s = null;
		//    }

		//    return s;
		//}
// △ 2007/08/15 削除



	}
}
