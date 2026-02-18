using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that returns a constant variable value.
	/// </summary>
	public sealed class ConstantVariableBlock : VariableBlock
	{
		private readonly Variable _value;

		public static ConstantVariableBlock Create(Variable value) => new(value);

		private ConstantVariableBlock(Variable value) => _value = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue(IScriptRuntimeContext runtimeContext) => _value;

		public override String ToString() => _value.ToString();
	}
}
