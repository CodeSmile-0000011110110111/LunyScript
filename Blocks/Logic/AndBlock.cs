using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical AND condition block.
	/// </summary>
	internal sealed class AndBlock : VariableBlock
	{
		private readonly ScriptConditionBlock[] _conditions;

		public static AndBlock Create(params ScriptConditionBlock[] conditions) => new(conditions);

		private AndBlock(ScriptConditionBlock[] conditions) => _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() => Evaluate(null);

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
