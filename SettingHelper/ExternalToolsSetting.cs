//=========================================================================
///	<summary>
///		�O���c�[���ݒ�A�C�e�� �N���X
/// </summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬</history>
//=========================================================================
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace magicAnime
{
	[Serializable()]
	public class ExternalToolItem
	{
		public string	toolName		= "";
		public string	toolPath		= "";
		public string	toolCommandLine = "";
	}

	//=========================================================================
	///	<summary>
	///		�O���c�[���ݒ�A�C�e�����X�g
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>2009/08/19 �V�K�쐬</history>
	//=========================================================================
	[Serializable()]
	public class ExternalToolsSetting : System.ICloneable
	{
		public ExternalToolsSetting()
		{
		}

		public ExternalToolsSetting(ExternalToolsSetting src)
		{
			if( src != null )
				this.tools = new List<ExternalToolItem>( src.tools );
		}

		public object Clone()
		{
			ExternalToolsSetting coppied = new ExternalToolsSetting();
			
			coppied.tools = new List<ExternalToolItem>( tools );

			return coppied;
		}

		public List<ExternalToolItem> tools = new List<ExternalToolItem>();
	}

}

