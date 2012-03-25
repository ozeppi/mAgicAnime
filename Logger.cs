//=========================================================================
///	<summary>
///		���K�[���W���[��
///	</summary>
/// <remarks>
/// </remarks>
/// <history>2006/XX/XX �V�K�쐬 Dr.Kurusugawa</history>
//=========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace magicAnime
{
	//=========================================================================
	///	<summary>
	///		���K�[���N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	public abstract class Logger : IDisposable
	{
		public Logger()
		{
			if (instance!=null) throw new Exception("already created.");
//			instance = this;
		}

// <ADD> 2010/01/28 ->
		public void SetDefault()
		{
			instance = this;
		}
// <ADD> 2010/01/28 <-

// <MOD> 2008/03/26 Dispose��͖������� ->
//		public static void Output(string t) { GetInstance()._Output(t); }
		public static void Output(string t)
		{
			if( GetInstance().IsDisposed() )
				return;

			GetInstance()._Output(t);
		}
// <MOD> 2008/03/26 <-
		public abstract void _Output(string t);
		public virtual void Dispose()
		{
			Output("���K�[�I��");
		}
// <MOD> 2008/03/26 ->
		public virtual bool IsDisposed()
		{
			return false;
		}
// <MOD> 2008/03/26 <-

		protected string OutputFormat(string t)
		{
			return DateTime.Now.ToString() + ": " + t;
		}

		public static Logger GetInstance() { return instance;  }

		private static Logger instance;
	}

	//=========================================================================
	///	<summary>
	///		�e�L�X�g�o�̓��K�[�N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	class TextLogger : Logger
	{
		public TextLogger(string path)
		{
			this.path = path;
			writer = new StreamWriter(path,true,Encoding.Unicode);

			writer.AutoFlush = true;
		}

		public override void _Output(string t)
		{
			writer.WriteLine(OutputFormat(t));
		}

		public override void Dispose()
		{
			base.Dispose();
			writer.Flush();
			writer.Close();
			writer.Dispose();
// <MOD> 2008/03/26 ->
			writer = null;
// <MOD> 2008/03/26 <-
		}

// <MOD> 2008/03/26 ->
		public override bool IsDisposed()
		{
			return (writer == null);
		}
// <MOD> 2008/03/26 <-

		public void ShowLog()
		{
			writer.Flush();

			Process.Start(path);
		}

		private string path;
		private StreamWriter writer;
	}

// <ADD> 2010/01/28 ->
	//=========================================================================
	///	<summary>
	///		�I�������� ���K�[�N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	class MemoryLogger : Logger
	{
		public MemoryLogger()
		{
		}

		public override void _Output(string t)
		{
			mStrings.Add( OutputFormat( t ) );
		}

		// �I���������ɋL�^�������O���z���o��
		public List<string> GetLog()
		{
			var copied = new List<string>();
			mStrings.ForEach( line => copied.Add( line ) );
			mStrings.Clear();
			return copied;
		}

		public override void Dispose()
		{
			mStrings = null;
		}

		public override bool IsDisposed()
		{
			return (mStrings == null);
		}

		private List<string> mStrings = new List<string>();
	}

	//=========================================================================
	///	<summary>
	///		�e�B�[ ���K�[�N���X
	///	</summary>
	/// <remarks>
	/// </remarks>
	/// <history>2006/XX/XX �V�K�쐬</history>
	//=========================================================================
	class TeeLogger : Logger
	{
		public TeeLogger( List<Logger> loggers )
		{
			mLoggers = loggers;
		}

		public override void _Output(string t)
		{
			mLoggers.ForEach( logger => logger._Output(t) );
		}

		public override void Dispose()
		{
			mLoggers = null;
		}

		public override bool IsDisposed()
		{
			return (mLoggers == null);
		}

		private List<Logger> mLoggers;
	}
// <ADD> 2010/01/28 <-
}
