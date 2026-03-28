using Luny;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Object = System.Object;

namespace LunyScript
{
	/// <summary>
	/// Used to detect and report any 'unfinished' builders after a script's Build() method returns.
	/// </summary>
	internal sealed class BuilderToken : IEquatable<BuilderToken>
	{
		private static Int32 s_NextId;

		private readonly Int32 _id;
		private readonly String _name;
		private readonly String _type;
		private readonly String _file;
		private readonly Int32 _line;
		private Boolean _isFinished;
		private Action _autoFinish;
		public String Name => _name;
		public String Type => _type;

		public static Boolean operator ==(BuilderToken left, BuilderToken right) => Equals(left, right);
		public static Boolean operator !=(BuilderToken left, BuilderToken right) => !Equals(left, right);

		/// <summary>
		/// Registers an action to be called automatically when the builder is in a finalizable state.
		/// </summary>
		internal Action AutoFinish { set => _autoFinish = value; }

		[DebuggerHidden]
		internal static void LogUnfinishedBuilder(BuilderToken token) => LunyLogger.LogWarning(
			$"{Path.GetFileName(token._file)}, line {token._line}: {token._name} ('{token._type}') is incomplete or unused.");

		internal BuilderToken(String name, String type, [CallerFilePath] String file = "", [CallerLineNumber] Int32 lineNumber = -1)
		{
			_id = s_NextId++;
			_name = name;
			_type = type;
			_file = file;
			_line = lineNumber;
		}

		public Boolean Equals(BuilderToken other)
		{
			if (other is null)
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return _id == other._id;
		}

		/// <summary>
		/// Invokes the auto-finalizer action and marks the token finished.
		/// Returns true if the builder was auto-finalized, false if no finalizer was registered.
		/// </summary>
		internal Boolean FinishBuilder()
		{
			if (_autoFinish == null)
				return false;

			_autoFinish.Invoke();
			MarkFinished();
			return true;
		}

		internal void MarkFinished()
		{
			_isFinished = true;
			GC.SuppressFinalize(this);
		}

		~BuilderToken()
		{
			if (!_isFinished)
				LogUnfinishedBuilder(this);
		}

		public override Boolean Equals(Object obj) => ReferenceEquals(this, obj) || obj is BuilderToken other && Equals(other);

		public override Int32 GetHashCode() => _id;
	}
}
