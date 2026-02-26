using Luny;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace LunyScript.BlockBuilders
{
	/// <summary>
	/// Used to detect and report any 'unfinished' builders after a script's Build() method returns.
	/// </summary>
	internal sealed class BuilderToken
	{
		private readonly String _name;
		private readonly String _type;
		private readonly String _file;
		private readonly Int32 _line;
		private Boolean _isFinished;
		private Action _autoFinalizeAction;

		public static void LogUnfinishedBuilder(BuilderToken token) => LunyLogger.LogWarning(
			$"{Path.GetFileName(token._file)}({token._line}) Unfinished {token._type} builder: '{token._name}' was never finalized.");

		public BuilderToken(String name, String type, [CallerFilePath] String file = "", [CallerLineNumber] Int32 lineNumber = -1)
		{
			_name = name;
			_type = type;
			_file = file;
			_line = lineNumber;
		}

		/// <summary>
		/// Registers an action to be called automatically when the builder is in a finalizable state.
		/// Replaces any previously registered finalizer.
		/// </summary>
		internal void SetAutoFinalizer(Action finalizeAction) => _autoFinalizeAction = finalizeAction;

		/// <summary>
		/// Invokes the auto-finalizer action and marks the token finished.
		/// Returns true if the builder was auto-finalized, false if no finalizer was registered.
		/// </summary>
		public Boolean FinalizeBuilder()
		{
			if (_autoFinalizeAction == null)
				return false;

			_autoFinalizeAction.Invoke();
			MarkFinished();
			return true;
		}

		public void MarkFinished()
		{
			_isFinished = true;
			GC.SuppressFinalize(this);
		}

		~BuilderToken()
		{
			if (!_isFinished)
				LogUnfinishedBuilder(this);
		}
	}
}
