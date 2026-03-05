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
		private readonly ScriptConditionBlock _condition;

		internal override Table.ScalarVarHandle VarHandle => (_condition as VariableBlock)?.VarHandle;

		public static NotBlock Create(ScriptConditionBlock condition) => new(condition);

		private NotBlock(ScriptConditionBlock condition) => _condition = condition;

		internal override Variable Variable { [MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Evaluate(null); }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			_condition == null || !_condition.Evaluate(runtimeContext);
	}
}
