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
		private readonly LunyStackTrace _trace;
		public LunyStackTrace Trace => _trace;

		public ScriptBlock(LunyStackTrace trace) => _trace = trace;

		public override String ToString() => $"[FIXME] {GetType().Name}";
	}
}
