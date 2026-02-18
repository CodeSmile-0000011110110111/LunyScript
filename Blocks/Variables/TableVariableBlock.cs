using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that holds a reference to a script variable.
	/// </summary>
	public sealed class TableVariableBlock : VariableBlock
	{
		private readonly Table.ScalarVarHandle _handle;

		internal override Table.ScalarVarHandle TargetHandle => _handle;
		internal Table.ScalarVarHandle ScalarVarHandle => _handle;

		public String Name => _handle.Name;
		public Variable Value => _handle.Value;

		internal static TableVariableBlock Create(Table.ScalarVarHandle handle) => new(handle);

		private TableVariableBlock(Table.ScalarVarHandle handle) => _handle = handle;

		public override String ToString() => _handle.ToString();

		// VariableBlock
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue(IScriptRuntimeContext runtimeContext) => _handle.Value;
	}
}
