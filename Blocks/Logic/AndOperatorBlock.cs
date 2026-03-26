using Luny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical AND condition block.
	/// </summary>
	internal sealed class AndOperatorBlock : VariableBlock, ILogicalOperatorBlock, IBlockContainer
	{
		private readonly ConditionBlock[] _conditions;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Evaluate(null);
		}

		public static AndOperatorBlock Create(params ConditionBlock[] conditions) => new(conditions);

		private AndOperatorBlock(ConditionBlock[] conditions)
		{
			_conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

#if DEBUG || LUNYSCRIPT_DEBUG
			if (_conditions.Length <= 1)
				LunyLogger.LogWarning($"{nameof(AndOperatorBlock)} with {_conditions.Length} condition(s) can be removed");
			if (_conditions.All(condition => condition == null))
				throw new ArgumentNullException(nameof(conditions), $"{nameof(AndOperatorBlock)}: Conditions cannot be null");
#endif
		}

		// ── IBlockContainer ───────────────────────────────────────────────

		Int32 IBlockContainer.ConditionSequenceCount => 1;
		String IBlockContainer.GetConditionSequenceName(Int32 index) => "AND";
		IEnumerable<IScriptBlock> IBlockContainer.GetConditionSequence(Int32 index) => _conditions;

		// ── Evaluate ──────────────────────────────────────────────────────

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext)
		{
			foreach (var condition in _conditions)
			{
				if (!condition.Evaluate(runtimeContext))
					return false;
			}

			return true;
		}
	}
}
