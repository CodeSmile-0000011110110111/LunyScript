using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for condition blocks that evaluate to a boolean result.
	/// </summary>
	public abstract class ScriptConditionBlock : ScriptBlock
	{
		public static ScriptConditionBlock operator !(ScriptConditionBlock block) => NotBlock.Create(block);

		public static ScriptConditionBlock operator &(ScriptConditionBlock left, ScriptConditionBlock right) => AndBlock.Create(left, right);

		public static ScriptConditionBlock operator |(ScriptConditionBlock left, ScriptConditionBlock right) => OrBlock.Create(left, right);

		// Return false to force always calling operators & | which then return a new block instance
		public static Boolean operator true(ScriptConditionBlock _) => false;
		public static Boolean operator false(ScriptConditionBlock _) => false;

		protected internal abstract Boolean Evaluate(IScriptRuntimeContext runtimeContext);
	}
}
