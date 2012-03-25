//=========================================================================
///	<summary>
///		�o�b�`�G���R�[�_�N���X
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

namespace magicAnime
{

	//=========================================================================
	///	<summary>
	///		�G���R�[�h�W���u
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public class EncodeJob : BatchJob
	{
		public EncodeJob(AnimeEpisode enocdeTarget)
		{
			this.encodeTarget = enocdeTarget;
		}

		public override void Do()
		{
			try
			{
				encodeTarget.Encode();
			}
			catch (Exception x)
			{
				throw new Exception(x.Message + " (" + encodeTarget.ToString()+")");
			}
		}

		public override void Cancel()
		{
			encodeTarget.CancelEncode();
		}

		public override string ToString()
		{
			return encodeTarget.ToString();
		}

		public override bool Equals(object target)
		{
			if (target.GetType() == this.GetType())
			{
				if (((EncodeJob)target).encodeTarget == this.encodeTarget)
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private AnimeEpisode encodeTarget;
	}



}
