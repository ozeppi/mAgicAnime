//=========================================================================
///	<summary>
///		エンコーダプラグイン基底クラス
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX 新規作成	Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace magicAnime
{

	public abstract class EncodeProfile
	{
		public abstract string TagName { get; }

		public abstract void Write(System.Xml.XmlWriter xmlWriter);
		public abstract void Read(System.Xml.XmlReader xmlReader);

	}
	public class AbortException : Exception
	{
		public override string Message	{ get { return "中断されました"; } }
	}

	/////////////////////////////////////////////////////////////////////////////
	// CLASS	:	Encoder
	// ABSTRACT	:	動画エンコーダ基底クラス
	/////////////////////////////////////////////////////////////////////////////
	public abstract class Encoder
	{

		public class TvProgramAdditionalInfo
		{
			public string Title;
// ▼ 2007/11/13 改修
//			public string StoryNumber;
			public object StoryNumber;
// ▲ 2007/11/13 改修
			public string Subtitle;
			public string TvStation;
// ▼ 2007/11/13 改修
//			public string StartDate;
//			public string StartTime;
			public object StartDate;
			public object StartTime;
// ▲ 2007/11/13 改修
		};

		public abstract string Name { get; }
		public abstract Type ProfileType { get; }
		public abstract string Extension { get; }


		public virtual Form CreatePropertyPage(
			Control parentm,
			EncodeProfile profile)
		{
			return null;
		}

		public virtual void ShowOptionDialog() {}

		public abstract void DoEncode(
			string				sourceFile,
			ref string			outputFile,
			EncodeProfile		profie,
			object				additionalInfo);

		public virtual void AbortEncodeProcess() {}


	}

}
