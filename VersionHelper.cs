//=========================================================================
///	<summary>
///		バージョンヘルパ
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2010/01/XX 新規作成	Dr.Kurusugawa</history>
//=========================================================================
using System;

namespace Helpers
{
	static class VersionHelper
	{
		// ターゲットバージョンが古いか判定
		static public bool isVersionOlder(
			int major, int minor, int revision,					// [i] 基準のバージョン
            int targetMajor, int targetMinor, int targetRev)	// [i] 対象のバージョン
        {
            if( targetMajor < major )
                return true;
            else if( targetMajor == major )
            {
                if( targetMinor < minor )
                    return true;
                else if( targetMinor == minor )
                {
                    if( targetRev < revision )
                        return true;
                }
            }

            return false;
        }

		static public bool ParseVersion(
							string		ver			,
							out int		majorVer	,
							out int		minorVer	,
							out int		revision	)
		{
			if( !string.IsNullOrEmpty( ver ) )
            {
                string[]    verParts;

                verParts = ver.Split('.');

                if( 3 <= verParts.Length )
                {
                    majorVer = int.Parse( verParts[0] );
                    minorVer = int.Parse( verParts[1] );
                    revision = int.Parse( verParts[2] );
					return true;
                }
            }
			majorVer = 0;
			minorVer = 0;
			revision = 0;
			return false;
		}
	}
}