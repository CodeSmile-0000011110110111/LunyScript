using Luny;
using System;

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
		private readonly StackTrace _trace;
		public StackTrace Trace => _trace;

		public ScriptBlock(StackTrace trace) => _trace = trace;

		public override String ToString() => GetType().Name;
	}
}
