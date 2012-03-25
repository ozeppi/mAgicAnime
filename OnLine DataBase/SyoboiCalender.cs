//=========================================================================
///	<summary>
///		しょぼいカレンダー オンラインデータ取得クラス
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成 Dr.Kurusugawa</history>
/// <history>2010/05/01 Subversionで管理するため不要なコメント削除</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections;
using System.Web;
using  System.Windows.Forms;
using magicAnime.Properties;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		しょぼいカレンダーデータ取得クラス
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX 新規作成</history>
	//=========================================================================
	class SyoboiCalender
	{
		DateTime?	prevUpdateListGetTime	= null;			// 前回の更新リスト取得時刻

		// タイトルリストテーブル(http://cal.syoboi.jp/titlelist.php)
		// タイトル	初回放送	初回終了	Point	TID	更新
		
		//=========================================================================
		///	<summary>
		///		しょぼかるの番組データを保持するクラス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public class SyoboiProgram
		{
			public string title;
			public 	struct SeasonOnAir
			{
				public const	uint YearDecided		= 1;
				public const	uint MonthDecided		= 2;
				public uint	RecordState;
				public int		year;
				public int		month;
				public override string ToString()
				{
					return string.Format(
						"{0:0}-{1:0}",
						((RecordState&YearDecided)>0)?year.ToString():"????",
						((RecordState&MonthDecided)>0)?month.ToString():"??");
				}
			};
			public SeasonOnAir seasonOnAir;
			
			public int	tid;
			
		}

		//=========================================================================
		///	<summary>
		///		しょぼかるのエピソードデータを保持するクラス
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public class SyoboiRecord : ICloneable
		{
			public int number;
			public string subtitle;
			public string tvStation;
			public int length;
			
			public DateTime onAirDateTime;

			public System.Object Clone()
			{
				return this.MemberwiseClone();
			}
		}

		public class SyoboiUpdate
		{
			public int tid;
			public DateTime updateDate;
		}
		
		//=========================================================================
		///	<summary>
		///		mAgicAnime固有のUser-Agent文字列を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2007/12/22 新規作成</history>
		//=========================================================================
		public string UserAgent
		{
			get
			{
				return "mAgicAnime " + Application.ProductVersion.ToString();
			}
		}

		//=========================================================================
		///	<summary>
		///		しょぼかるRSSから更新された番組リストを取得
		///	</summary>
		/// <remarks>
		///		更新がない場合は空のリストを返す。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2009/02/11 しょぼかる負荷対策のためif-modified-since追加</history>
		//=========================================================================
		public List<SyoboiUpdate> DownloadUpdateList()
		{
			List<SyoboiUpdate> updateList = new List<SyoboiUpdate>();

			try
			{
				//------------------------
				// しょぼかるRSSを開く
				//------------------------
				HttpWebRequest webRequest =
					HttpWebRequest.Create(Settings.Default.syoboiProg) as HttpWebRequest;

				webRequest.UserAgent		= UserAgent;
				// 前回の取得時刻
				if( prevUpdateListGetTime.HasValue )
					webRequest.IfModifiedSince	= prevUpdateListGetTime.Value;

				WebResponse	webResponse	= webRequest.GetResponse();
				XmlReader	xmlReader	= new XmlTextReader(webResponse.GetResponseStream());

				//------------------------
				// RSSエントリをParse
				//------------------------
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element &&
						xmlReader.LocalName.Equals("item"))
					{
						// <item>
						SyoboiUpdate u = new SyoboiUpdate();

						while (xmlReader.Read())
						{
							if (xmlReader.NodeType == XmlNodeType.Element &&
								xmlReader.LocalName.Equals("link"))
							{
								string t;
								// <link>
								t = xmlReader.ReadElementContentAsString();
								t = t.Substring(t.IndexOf("tid/") + 4);

								u.tid = System.Convert.ToInt32(t);
							}
							else if (xmlReader.NodeType == XmlNodeType.Element &&
									   xmlReader.LocalName.Equals("pubDate"))
							{
								string t;
								t = xmlReader.ReadElementContentAsString();

								u.updateDate = ParsePubDate(t);
							}
							else if (xmlReader.NodeType == XmlNodeType.EndElement &&
									  xmlReader.LocalName.Equals("item"))
								break;
						}

						updateList.Add(u);
					}
				}
				// 次回取得時のため、今回取得時刻を記憶
				prevUpdateListGetTime = DateTime.Now;
			}
			catch(WebException ex)
			{
				bool	ignoreException = false;

				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse	httpRes = ex.Response as HttpWebResponse;

					// 前回から更新なし(304エラー)なら例外を無視
					bool	notModfied;
					notModfied	=	(httpRes != null)
								&&	(httpRes.StatusCode == HttpStatusCode.NotModified);

//#if DEBUG
					if( notModfied )
					{
						if( prevUpdateListGetTime.HasValue )
							Logger.Output(	"(しょぼかる)前回取得時("
										+	prevUpdateListGetTime.Value.ToShortTimeString()
										+	")から更新なし"	);
					}
//#endif

					ignoreException |= notModfied;
				}

				if (!ignoreException)
					throw;
			}

			return updateList;
		}

		//=========================================================================
		///	<summary>
		///		更新日付時刻を取得
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		DateTime ParsePubDate(string t)
		{
			int			y, m, d, hour, minute;
			string		[]subStrings;

			subStrings = t.Split(' ');

			//--------------
			// 月を取得
			//--------------

			string[] Months = {	"JAN","FEB","MAR","APR","MAY","JUN",
								"JUL","AUG","SEP","OCT","NOV","DEC"};

			m = 0;
			foreach (string Month in Months)
			{
				if (Month.Equals( subStrings[2].ToUpper() ))
				{
					m = Array.IndexOf(Months, Month) + 1;
					break;
				}
			}

			//--------------
			// 年日を取得
			//--------------
			y = System.Convert.ToInt32( subStrings[3] );
			d = System.Convert.ToInt32( subStrings[1] );

			//--------------
			// 時分を取得
			//--------------

			t = subStrings[4];
			hour	= Convert.ToInt32(t.Substring(0, t.IndexOf(':')));

			t = t.Substring(t.IndexOf(':') + 1);
			minute	= Convert.ToInt32(t.Substring(0, t.IndexOf(':')));

			return new DateTime(y, m, d, hour, minute, 0);
		}

		//=========================================================================
		///	<summary>
		///		番組リストをダウンロード
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2009/10/08 しょぼかる書式変更に合わせた改修</history>
		//=========================================================================
		public List<SyoboiProgram> DownloadPrograms()
		{
			WebClient		wc				= new WebClient();

			wc.Headers.Add( "User-Agent", UserAgent );

			Stream			s				= wc.OpenRead( Properties.Settings.Default.syoboiTitleList );
			StreamReader	streamRender	= new StreamReader(s);
			List<SyoboiProgram>	animeDataBase = new List<SyoboiProgram>();
			string			allText			= "";

			while( !streamRender.EndOfStream )
			{
				allText += streamRender.ReadLine();
			}
			allText = allText.Replace('\t', ' ');

			// 番組データを取り出す
			// <tr><td><a href="/tid/1234"> ～ </td></tr>
			Regex reg = new Regex("\\<tr\\>" +
								  "(\\s|\\n)*?(?<CONTAIN>\\<td\\>(\\s|\\n)*?<a\\s+href=\\\"/tid/[0-9]+\\\">" +
								  "(.*?)" +
								  "\\</td\\>)" +
								  "(\\s|\\n)*\\</tr\\>" );
			Match		matched = reg.Match( allText );

			for (; matched.Success; )
			{
				string strRow = matched.Groups["CONTAIN"].Value;
				ArrayList Cols = ParseTableRow( strRow );

				animeDataBase.Add(ConvertToSyoboiProgram(Cols));

				matched = matched.NextMatch();
			}

			return animeDataBase;
		}
		
		//=========================================================================
		///	<summary>
		///		指定されたTIDの番組の放送リストをダウンロードする
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		/// <history>2009/04/13 しょぼかる新仕様に合わせて改修</history>
		/// <history>2010/06/01 しょぼかる新仕様に合わせて改修</history>
		//=========================================================================
		public List<SyoboiRecord> DownloadOnAirList(
			int					tid		,	// [i] しょぼかるTID
			out string			title	)	// [o] 番組タイトル
		{
			List<String>	source;
			return DownloadOnAirList( tid, out title, out source );
		}

		public List<SyoboiRecord> DownloadOnAirList(
			int					tid		,	// [i] しょぼかるTID
			out string			title	,	// [o] 番組タイトル
			out List<String>	source	)	// [o] データソース(html)
		{
			WebClient			wc			= new WebClient();

			wc.Headers.Add("User-Agent", UserAgent);

			List<SyoboiRecord>	recordList	= new List<SyoboiRecord>();
			
			//-------------------------------------
			// URL: (しょぼかるTID)/timeを開く
			//-------------------------------------
			Stream				s			= wc.OpenRead(
												Properties.Settings.Default.syoboiTid	+
												Convert.ToString(tid)					+
												"/time"									);
			
			StreamReader		streamRender = new StreamReader(s);

			title = null;

			string strRow = "";

			// 一括取得しておく
			List<string>	lines	= new List<string>();
			string			allLine	= "";

			for (; !streamRender.EndOfStream; )
			{
				lines.Add( streamRender.ReadLine() );
			}

			foreach( string line in lines )
			{
                //---------------------------------
                // タイトルを<keywords>タグから取得
                //---------------------------------
				if (title == null &&
					line.IndexOf("\"keywords\"") >= 0)
				{
					// <meta name="keywords" content="タイトル,・・・">
					Regex	parseTitle	= new Regex(
						"(<meta)(\\s*?)(name=\\\"keywords\\\")(\\s*?)(content=\\\")(?<Title>.*?)(\\s*?)(,)(.*\\\">){1}" );
					Match	matchTitle	= parseTitle.Match(line);

					if( matchTitle.Success )
						title = matchTitle.Groups["Title"].Value;
				}

				allLine += line;
			}

			//--------------------------------------
			// 放送データのあるテーブルを切り出す
			//--------------------------------------	
			const string	progComment	= @"(<!)(.*?)(プログラム一覧){1}(.*?)(->)";
			Regex			regex		= new Regex( progComment + "(?<Content>.*)" + progComment );
			Match			match		= regex.Match( allLine );
			Group			group		= match.Groups["Content"];

			if (group.Success)
			{
				string tableData = group.Value;

				//------------------------
				// 放送時間テーブルの解析
				//------------------------

                // <tr...id="PIDxxxxx">...</tr>で囲まれたデータ(テーブルの1行)を切り出す
				// (<tr class="past" id="PIDxxxxx">を含む)
				string	tableFirst		= "(<tr)(\\s*?)(class=\\\"past\\\"(\\s*?))?(id=\\\"PID[0-9]{1,}\\\"){1}(.*?)(>)";
                Regex	findTableCH		= new Regex("(?<FirstPos>" + tableFirst + "(.*?)</tr>" + ")");
				Match	matchTableCH	= findTableCH.Match(tableData);

                for(; matchTableCH.Success ;)
                {
                    strRow = matchTableCH.Groups["FirstPos"].Value;

                    ParseProgramTable( strRow, recordList );

					matchTableCH = matchTableCH.NextMatch();
				}

			}

			streamRender.Close();

			source = lines;

			return recordList;
		}


		//=========================================================================
		///	<summary>
		///		放送テーブルの行データをパースして放送データを取り出す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2009/04/13 しょぼかる新仕様に合わせて改修</history>
		//=========================================================================
		private void ParseProgramTable(
			string				strRow		,	// [i] 放送テーブルの<td></td>内の文字列
			List<SyoboiRecord>	recordList	)	// [o] 放送データ
		{
			SyoboiRecord syoboiRecord = null;
		
			try
			{
				ArrayList Cols;
				string temp;

				//---------------------------------------------------------------------------------
				// チャンネル	開始日時	分	回数	サブタイトル/コメント	フラグ	更新日	-
				//---------------------------------------------------------------------------------
				Cols = ParseTableRow(strRow);
				
				syoboiRecord = new SyoboiRecord();

				syoboiRecord.tvStation = (string)Cols[0]; // TV局名

				if ( Cols[ 3 ].Equals( "" ) )
				{
					//-----------------------------
					// 複数話連続放送の対策
					//-----------------------------

					// #nn～#mm
					Regex parser	= new Regex( "^#(?<FirstEpisode>[0-9]+)～#(?<LastEpisode>[0-9]+)" );
					Match match		= parser.Match( (string)Cols[ 4 ] );

					if (match.Success)
					{
						int firstEpisode, lastEpisode;
						int episodeCount = 1;
						DateTime onairDateTime;

						firstEpisode = int.Parse(match.Groups["FirstEpisode"].Value);
						lastEpisode = int.Parse(match.Groups["LastEpisode"].Value);

						syoboiRecord.number = firstEpisode;
						episodeCount = (lastEpisode - firstEpisode + 1); // 連続放送数

						// 尺=全放送時間/話数
						syoboiRecord.length = int.Parse((string)Cols[2]) / episodeCount;

						onairDateTime = ConvertToDateTime( (string)Cols[1] );

						//-----------------------------
						// 連続話数分、データを追加
						//-----------------------------
						for (int i = firstEpisode; i <= lastEpisode; ++i)
						{
							SyoboiRecord newRecord;

							newRecord = (SyoboiRecord)syoboiRecord.Clone();

							newRecord.number = i;		// 話番号
							newRecord.subtitle = "";	// サブタイトル

							// 開始時刻
							newRecord.onAirDateTime = onairDateTime.AddMinutes(newRecord.length * (i - firstEpisode));

							recordList.Add(newRecord);
						}
					}
					else
					{
						//------------------------------------------------
						// 2話連続「#1、#2」
						//------------------------------------------------
						DateTime		onairDateTime;
						string			dateTag;
						List<int>		episodeNums		= new List<int>();
						List<string>	subTitles		= new List<string>();
						
						// 連続放送時間
						int				totalLen		= int.Parse((string)Cols[2]);

						onairDateTime = ConvertToDateTime( (string)Cols[1] );

						// 前後編の場合、サブタイトルが一緒になっている
						// #nn「サブタイトル」、#mm「サブタイトル」...
						parser = new Regex("#(?<EpisodeNumber>[0-9]+)((｢|「)(?<Subtitle>.*?)(｣|」))?(、)?");
						match	= parser.Match((string)Cols[4]);

						// エピソード番号とタイトルを全て切り出す
						while(match.Success)
						{
							int		episodeNum	= int.Parse(match.Groups["EpisodeNumber"].Value);
							string	subTitle	= (string)match.Groups["Subtitle"].Value;

							episodeNums.Add( episodeNum );
							subTitles.Add( subTitle );

							match = match.NextMatch();
						}

						// 前後編の場合のサブタイトル処理
						if( subTitles.Count == 2 )
						{
							if( string.IsNullOrEmpty( subTitles[0] ) &&
								!string.IsNullOrEmpty( subTitles[1] ) )
							{
								subTitles[0] = subTitles[1];
							}
						}

						// 各エピソードを追加
						for( int i = 0 ; i < episodeNums.Count ; ++i )
						{
							int		episodeNum	= episodeNums[i];
							string	subTitle	= subTitles[i];

							SyoboiRecord newRecord = (SyoboiRecord)syoboiRecord.Clone();

							// 尺=全放送時間/話数
							newRecord.length	= totalLen / episodeNums.Count;
							newRecord.number	= episodeNum;	// エピソード番号
							newRecord.subtitle	= subTitle;		// サブタイトル

							// 開始時刻
							newRecord.onAirDateTime = onairDateTime.AddMinutes(newRecord.length * i);

							recordList.Add(newRecord);
						}
					}
				}
				else
				{
					syoboiRecord.length = int.Parse( (string)Cols[ 2 ] ); // 尺
					syoboiRecord.number = int.Parse( (string)Cols[ 3 ] ); // 話番号

					// HTMLエンコード文字をデコード
					syoboiRecord.subtitle = HttpUtility.HtmlDecode( MakeNaked( (string)Cols[ 4 ] ) ); // サブタイトル
					syoboiRecord.onAirDateTime = ConvertToDateTime( (string)Cols[1] );	// 開始日時

					recordList.Add( syoboiRecord );
				}

			}
			catch(Exception)
			{
#if _DEBUG
//				if (syoboiRecord!=null)
//					Console.WriteLine(title + " " + syoboiRecord.number);
#endif
			}
		}

		//=========================================================================
		///	<summary>
		///		放送リストから放送局をリストアップする
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public ArrayList ListupTvStation(List<SyoboiCalender.SyoboiRecord> recordList)
		{
			ArrayList tvStationList = new ArrayList();
		
			foreach(SyoboiCalender.SyoboiRecord record in recordList)
			{
				bool addToList = true;

				foreach(string tvStation in tvStationList)
				{
					if (record.tvStation.Equals(tvStation))
					{
						addToList = false;
						break;
					}
				}
				
				if (addToList)
					tvStationList.Add(record.tvStation);
			}
		
			return 	tvStationList;
		}

		//=========================================================================
		///	<summary>
		///		番組のhtmlテーブルから内部の構造体形式に変換
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private SyoboiProgram ConvertToSyoboiProgram(ArrayList Cols)
		{
			SyoboiProgram newProgram = new SyoboiProgram();

			newProgram.title = (string)Cols[0];
			newProgram.tid = int.Parse((string)Cols[3]);

			newProgram.seasonOnAir.RecordState = 0;
			
			string seasonOnAir = (string)Cols[1];
			
			seasonOnAir = seasonOnAir.Trim();
			if( seasonOnAir.Equals("-------") )
			{
			}else{
				string year, month;

				year = seasonOnAir.Substring( 0, 4 );
				if( !year.Equals("----") )
				{
					newProgram.seasonOnAir.RecordState |= SyoboiProgram.SeasonOnAir.YearDecided;
					newProgram.seasonOnAir.year = int.Parse(year);
				}
				
				month	= seasonOnAir.Substring( seasonOnAir.Length - 2 );
				if(! month.Equals("--") )
				{
					newProgram.seasonOnAir.RecordState |= SyoboiProgram.SeasonOnAir.MonthDecided;
					newProgram.seasonOnAir.month	= int.Parse( month );
				}

			}
		
			return newProgram;
		}
		
		//=========================================================================
		///	<summary>
		///		しょぼかるの日付書式からDateTimeに変換
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private DateTime ConvertToDateTime(
			string strDateTime)	// [i] 日時文字列「2009-02-02 (月) 10:30」
		{
			DateTime dateTime;
			int		year;
			int		month;
			int		day;
			int		hour;
			int		minute;
			bool incDay = false;

			Regex	regex = new Regex(
				"(?<Year>[0-9]{4})-(?<Month>[0-9]{1,})-(?<Day>[0-9]{1,})( *?)(.*?)( *?)" +
				"(?<Hour>[0-9]{1,}):(?<Minute>[0-9]{1,})");
			Match	match = regex.Match( strDateTime );

			year	= int.Parse( match.Groups["Year"].Value );
			month	= int.Parse(match.Groups["Month"].Value);
			day		= int.Parse(match.Groups["Day"].Value);
			hour	= int.Parse(match.Groups["Hour"].Value);
			minute	= int.Parse(match.Groups["Minute"].Value);


			// 24:00以降なら+1日
			if (hour >= 24)
			{
				hour -= 24;
				incDay = true;
			}

			dateTime = new DateTime(
				year	,
				month	,
				day		,
				hour	,
				minute	,
				0		);

			if (incDay)
				dateTime = dateTime.AddDays(1.0);

			return dateTime;
		}

		//=========================================================================
		///	<summary>
		///		htmlタグを排除する
		///	</summary>
		/// <remarks>
		///		とりあえず、頻繁に出てくる<a><div>を削除する。
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private string MakeNaked(string context)
		{
			string nakedContext = context;

			{
				Regex regex = new Regex("<a(.*?)>(?<Content>(.*?))</a>");
				Match match = regex.Match(nakedContext);

				if (match.Groups["Content"].Success)
				{
					string content = match.Groups["Content"].Value;

					nakedContext = regex.Replace(context, content);
				}
			}

			// <TAG*>xxxx</TAG>形式のタグを除去
			string[]	targetTags = {"div", "span"};

			foreach( string target in targetTags )
			{
				Regex regex = new Regex("<" + target +"(.*?)>(.*?)</" + target + ">");
				Match match = regex.Match(nakedContext);

				if (match.Success)
				{
					nakedContext = regex.Replace(nakedContext, "");
				}
			}

			return nakedContext;
		}
		
		//=========================================================================
		///	<summary>
		///		HTMLのテーブルの行を<TD>...</TD>列ごとに分解する
		///	</summary>
		/// <remarks>
		///		<tr></tr>セクションで囲まれた内側の文字列を渡す
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private ArrayList ParseTableRow(string strRow)
		{
			ArrayList Cols = new ArrayList();
			string strCol;

			Regex	regex	= new Regex("(<td)(.*?)(>)(?<Content>(.*?))(</td>)");
			Match	match	= regex.Match( strRow );

			for (; match.Success; )
			{
				string	content = (string)match.Groups["Content"].Value;
				Cols.Add( MakeNaked( content ) );
				match = match.NextMatch();
			}
			
			return Cols;
		}
		
		//=========================================================================
		///	<summary>
		///		指定されたタグで囲まれた範囲の文字列を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private string ExtractTagContain(
			string		line	,
			string		tagName	)
		{
			string dummy;
			
			return ExtractTagContain( line, tagName , out dummy );
		}

		//=========================================================================
		///	<summary>
		///		指定されたタグで囲まれた範囲の文字列と残りの文字列を返す
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		private string ExtractTagContain(
			string		line		,	// [i] 文字列
			string		tagName		,	// [i] タグ名
			out string	remain		)	// [o] 残り文字列
		{
			try
			{
				Regex reg = new Regex(@"\<" + tagName + @"\>(?<CONTAIN>.*?)\</" + tagName + @"\>");
				Match matched = reg.Match(line);

				if (matched.Success)
				{
					string contain = matched.Groups["CONTAIN"].Value;
					remain = reg.Replace(line, "");
					return contain;
				}
			}
			catch (Exception ex)
			{
			}

			remain = line;
			return "";
		}
		
	}

}
