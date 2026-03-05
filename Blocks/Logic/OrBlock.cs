using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Logical OR condition block.
	/// </summary>
	internal sealed class OrBlock : VariableBlock
	{
		private readonly ScriptConditionBlock[] _conditions;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Evaluate(null);
		}

		public static OrBlock Create(params ScriptConditionBlock[] conditions) => new(conditions);

		private OrBlock(ScriptConditionBlock[] conditions) => _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

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
	}
}
