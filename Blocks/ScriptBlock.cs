using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Base interface for LunyScript blocks.
	/// </summary>
	public interface IScriptBlock {}

	/// <summary>
	/// Abstract base for all LunyScript blocks.
	/// </summary>
	public abstract class ScriptBlock : IScriptBlock
	{
		public StackTrace Trace { get; private init; }

		public ScriptBlock(StackTrace trace) => Trace = trace;

		public ScriptBlock([CallerMemberName] String name = "", [CallerFilePath] String path = "", [CallerLineNumber] Int32 line = 0) =>
			Trace = new StackTrace(name, path, line);

		public override String ToString() => $"{GetType().Name} <-- should override ToString()!";
	}
}
