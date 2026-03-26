using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal sealed class VariableSetValueBlock : ActionBlock
	{
		private readonly Table.VarHandle _handle;
		private readonly VariableBlock _value;

		public static VariableSetValueBlock Create(Table.VarHandle handle, VariableBlock value) => new(handle, value);

		private VariableSetValueBlock(Table.VarHandle handle, VariableBlock value)
		{
			_handle = handle ?? throw new ArgumentNullException(nameof(handle));
			_value = value ?? throw new ArgumentNullException(nameof(value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => _handle.Variable = _value.Variable;

		public override String ToString() => $"\"{_handle.Name}\" = {_value}";
	}
}
