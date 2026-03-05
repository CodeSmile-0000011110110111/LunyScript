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

		internal override Table.ScalarVarHandle VarHandle => _handle;

		public String Name => _handle.Name;
		public Variable Value => _handle.Value;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _handle.Value;
		}

		internal static TableVariableBlock Create(Table.ScalarVarHandle handle) => new(handle);

		private TableVariableBlock(Table.ScalarVarHandle handle) => _handle = handle;

		public override String ToString() => _handle.ToString();
	}
}
