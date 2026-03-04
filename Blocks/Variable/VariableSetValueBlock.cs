using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	internal sealed class VariableSetValueBlock : ScriptActionBlock
	{
		private readonly Table.ScalarVarHandle _handle;
		private readonly VariableBlock _value;

		public static VariableSetValueBlock Create(Table.ScalarVarHandle handle, VariableBlock value) => new(handle, value);

		private VariableSetValueBlock(Table.ScalarVarHandle handle, VariableBlock value)
		{
			_handle = handle ?? throw new ArgumentNullException(nameof(handle));
			_value = value ?? throw new ArgumentNullException(nameof(value));
			Execute(null); // instant set
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => _handle.Value = _value.GetValue();

		public override String ToString() => $"{_handle} = {_value}";
	}
}
