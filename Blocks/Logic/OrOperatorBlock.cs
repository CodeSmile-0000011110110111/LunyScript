using Luny;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical OR condition block.
	/// </summary>
	internal sealed class OrOperatorBlock : VariableBlock, ILogicalOperator //, IBlockContainer
	{
		private readonly ConditionBlock[] _conditions;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Evaluate(null);
		}

		// ── IBlockContainer ───────────────────────────────────────────────

		//Int32 IBlockContainer.ConditionSequenceCount => 1;

		public static OrOperatorBlock Create(ConditionBlock[] conditions, LunyStackTrace trace = null) => new(conditions, trace);

		private OrOperatorBlock(ConditionBlock[] conditions, LunyStackTrace trace)
			: base(trace)
		{
			_conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

#if DEBUG || LUNYSCRIPT_DEBUG
			if (_conditions.Length <= 1)
				LunyLogger.LogWarning($"{nameof(OrOperatorBlock)} with {_conditions.Length} condition(s) can be removed");
			if (_conditions.All(condition => condition == null))
				throw new ArgumentNullException(nameof(conditions), $"{nameof(OrOperatorBlock)}: Conditions cannot be null");
#endif
		}

		//String IBlockContainer.GetConditionSequenceName(Int32 index) => "OR";
		//IEnumerable<IScriptBlock> IBlockContainer.GetConditionSequence(Int32 index) => _conditions;

		// ── Evaluate ──────────────────────────────────────────────────────

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext)
		{
			foreach (var condition in _conditions)
			{
				if (condition.Evaluate(runtimeContext))
					return true;
			}

			return false;
		}

		public override String ToString()
		{
			if (_conditions == null || _conditions.Length == 0)
				return $"OR({Emoji.NullReference})";

			var sb = new StringBuilder();
			var conditionCount = _conditions.Length;
			for (var i = 0; i < conditionCount; i++)
			{
				if (i != 0)
					sb.Append(Emoji.LogicalOr);
				sb.Append(_conditions[i]);
			}
			return sb.ToString();
		}
	}
}
