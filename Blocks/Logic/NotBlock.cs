using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical NOT condition block.
	/// </summary>
	internal sealed class NotBlock : VariableBlock
	{
		private readonly ConditionBlock _condition;

		internal override Table.ScalarVarHandle VarHandle => (_condition as VariableBlock)?.VarHandle;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Evaluate(null);
		}

		public static NotBlock Create(ConditionBlock condition) => new(condition);

		private NotBlock(ConditionBlock condition)
		{
			if (condition == null)
				throw new ArgumentNullException(nameof(condition), $"{nameof(NotBlock)}: Condition cannot be null");

			_condition = condition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_condition == null || !_condition.Evaluate(runtimeContext);
	}
}
