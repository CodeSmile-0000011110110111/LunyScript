using Luny;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical AND condition block.
	/// </summary>
	internal sealed class AndBlock : VariableBlock
	{
		private readonly ConditionBlock[] _conditions;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Evaluate(null);
		}

		public static AndBlock Create(params ConditionBlock[] conditions) => new(conditions);

		private AndBlock(ConditionBlock[] conditions)
		{
			_conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

#if DEBUG || LUNYSCRIPT_DEBUG
			if (_conditions.Length <= 1)
				LunyLogger.LogWarning($"{nameof(AndBlock)} with {_conditions.Length} condition(s) can be removed");
			if (_conditions.All(condition => condition == null))
				throw new ArgumentNullException(nameof(conditions), $"{nameof(AndBlock)}: Conditions cannot be null");
#endif
		}

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
