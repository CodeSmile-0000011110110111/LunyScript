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

		public static void LogWarning(BuilderToken token) => LunyLogger.LogWarning(
			$"{Path.GetFileName(token._file)}({token._line}): Unfinished {token._type} '{token._name}' => " +
			"Did you forget to call a terminal method like .Do() or .WhenElapsed()?");

		public BuilderToken(String name, String type, [CallerFilePath] String file = "", [CallerLineNumber] Int32 lineNumber = -1)
		{
			_name = name;
			_type = type;
			_file = file;
			_line = lineNumber;
		}

		public void MarkFinished()
		{
			_isFinished = true;
			GC.SuppressFinalize(this);
		}

		~BuilderToken()
		{
			if (!_isFinished)
				LogWarning(this);
		}
	}
}
