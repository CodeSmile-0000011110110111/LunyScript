using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for action blocks that perform an action that may alter game state.
	/// </summary>
	public abstract class ScriptActionBlock : ScriptBlock
	{
		public static Boolean IsNullOrEmpty(ScriptActionBlock[] blocks) => blocks == null || blocks.Length == 0;
		protected internal abstract void Execute(IScriptRuntimeContext runtimeContext);
	}
}
