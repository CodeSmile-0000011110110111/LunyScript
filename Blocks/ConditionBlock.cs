using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for condition blocks that evaluate to a boolean result.
	/// </summary>
	public abstract class ConditionBlock : ScriptBlock
	{
		public static ConditionBlock operator !(ConditionBlock block) => NegationOperatorBlock.Create(block);

		public static ConditionBlock operator &(ConditionBlock left, ConditionBlock right) => AndOperatorBlock.Create(new[] { left, right });

		public static ConditionBlock operator |(ConditionBlock left, ConditionBlock right) => OrOperatorBlock.Create(new[] { left, right });

		// this may be confusing
		//public static implicit operator ConditionBlock(Boolean value) => LiteralVariableBlock.Create(value, ScriptTrace.TryCreateStackTrace());

		// Return false to force always calling operators & | which then return a new block instance
		public static Boolean operator true(ConditionBlock _) => false;
		public static Boolean operator false(ConditionBlock _) => false;

		protected ConditionBlock(LunyStackTrace trace = null)
			: base(trace) {}

		protected internal abstract Boolean Evaluate(IScriptRuntimeContext runtimeContext);
	}
}
