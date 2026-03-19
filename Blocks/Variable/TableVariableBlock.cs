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
		private readonly Table.VarHandle _handle;

		internal override Table.VarHandle VarHandle => _handle;

		public String Name => _handle.Name;
		public new Variable Value => _handle.Variable;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _handle.Variable;
		}

		internal static TableVariableBlock Create(Table.VarHandle handle) => new(handle);

		private TableVariableBlock(Table.VarHandle handle) => _handle = handle;

		public override String ToString() => _handle.ToString();
	}
}
