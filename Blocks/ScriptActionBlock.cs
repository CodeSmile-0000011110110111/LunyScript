namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for action blocks that perform an action that may alter game state.
	/// </summary>
	public abstract class ScriptActionBlock : ScriptBlock
	{
		protected internal abstract void Execute(IScriptRuntimeContext runtimeContext);
	}
}
