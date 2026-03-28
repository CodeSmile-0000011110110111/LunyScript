using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical NOT condition block.
	/// </summary>
	internal sealed class NegationOperatorBlock : VariableBlock, ILogicalOperator
	{
		private readonly ConditionBlock _condition;

		internal override Table.VarHandle VarHandle => (_condition as VariableBlock)?.VarHandle;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Evaluate(null);
		}

		public static NegationOperatorBlock Create(ConditionBlock condition, StackTrace trace = null) => new(condition, trace);

		private NegationOperatorBlock(ConditionBlock condition, StackTrace trace) : base(trace)
		{
			if (condition == null)
				throw new ArgumentNullException(nameof(condition), $"{nameof(NegationOperatorBlock)}: Condition cannot be null");

			_condition = condition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_condition == null || !_condition.Evaluate(runtimeContext);

		public override String ToString() => $"NOT({_condition})";
	}
}
