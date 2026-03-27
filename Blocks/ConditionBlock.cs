using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for condition blocks that evaluate to a boolean result.
	/// </summary>
	public abstract class ConditionBlock : ScriptBlock
	{
		public static ConditionBlock operator !(ConditionBlock block) => NegationOperatorBlock.Create(block);

		public static ConditionBlock operator &(ConditionBlock left, ConditionBlock right) => AndOperatorBlock.Create(left, right);

		public static ConditionBlock operator |(ConditionBlock left, ConditionBlock right) => OrOperatorBlock.Create(left, right);

		// Return false to force always calling operators & | which then return a new block instance
		public static Boolean operator true(ConditionBlock _) => false;
		public static Boolean operator false(ConditionBlock _) => false;

		protected ConditionBlock([CallerMemberName] String name = "", [CallerFilePath] String path = "", [CallerLineNumber] Int32 line = 0)
			: base(new StackTrace($"{nameof(ConditionBlock)}.{name}: missing stack trace", path, line)) {}

		protected internal abstract Boolean Evaluate(IScriptRuntimeContext runtimeContext);
	}
}
