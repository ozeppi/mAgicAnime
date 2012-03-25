using System;
using System.IO;

namespace Helpers
{
	public class PathHelper
	{
		static PathHelper()
		{
			mFileNameRule = 0;
		}

	//=========================================================================
		///	<summary>
		///		文字列をファイル名に使える文字列に変換する
		///	</summary>
		/// <remarks>
		/// </remarks>
		/// <history>2006/XX/XX 新規作成</history>
		//=========================================================================
		public static string ToFileTitle(string name)
		{
			// 無効な文字を適当な文字に変換する
			if( 0 < (mFileNameRule & 1) )
				name = name.Replace('!', '！');	// ?と合わせるため
			name = name.Replace('?', '？');
			name = name.Replace('/', '／');
			name = name.Replace('\\', '￥');
			name = name.Replace('*', '＊');
			name = name.Replace(',', '，');
			name = name.Replace('<', '＜');
			name = name.Replace('>', '＞');
			name = name.Replace( ':', '：' );
			name = name.Replace( ':', '：' );
			name = name.Replace( '\"', '”' );

			// 無効な文字を削除する
			name = name.TrimStart( Path.GetInvalidFileNameChars() );

			return name;
		}

		// ファイル名変換ルールを設定
		static public void SetFileNameRule(uint rule)
		{
			mFileNameRule = rule;
		}

		static private uint mFileNameRule;	// ファイル名変換ルール
											// Bit0:特定文字を全角にする
	}
}