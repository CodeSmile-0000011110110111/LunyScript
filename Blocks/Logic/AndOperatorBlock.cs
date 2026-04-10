using Luny;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical AND condition block.
	/// </summary>
	internal sealed class AndOperatorBlock : VariableBlock, ILogicalOperator //, IBlockContainer
	{
		private readonly ConditionBlock[] _conditions;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Evaluate(null);
		}

		// ── IBlockContainer ───────────────────────────────────────────────

		//Int32 IBlockContainer.ConditionSequenceCount => 1;

		public static AndOperatorBlock Create(ConditionBlock[] conditions, LunyStackTrace trace = null) => new(conditions, trace);

		private AndOperatorBlock(ConditionBlock[] conditions, LunyStackTrace trace)
			: base(trace)
		{
			_conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

#if DEBUG || LUNYSCRIPT_DEBUG
			if (_conditions.Length <= 1)
				LunyLogger.LogWarning($"{nameof(AndOperatorBlock)} with {_conditions.Length} condition(s) can be removed");
			if (_conditions.All(condition => condition == null))
				throw new ArgumentNullException(nameof(conditions), $"{nameof(AndOperatorBlock)}: Conditions cannot be null");
#endif
		}

		// String IBlockContainer.GetConditionSequenceName(Int32 index) => "AND";
		// IEnumerable<IScriptBlock> IBlockContainer.GetConditionSequence(Int32 index) => _conditions;

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

		public override String ToString()
		{
			if (_conditions == null || _conditions.Length == 0)
				return $"AND({Emoji.NullReference})";

			var sb = new StringBuilder("("); // brackets are required to correctly represent order of operations
			var conditionCount = _conditions.Length;
			for (var i = 0; i < conditionCount; i++)
			{
				if (i != 0)
					sb.Append(Emoji.LogicalAnd);
				sb.Append(_conditions[i]);
			}
			sb.Append(")");
			return sb.ToString();
		}
	}
}
