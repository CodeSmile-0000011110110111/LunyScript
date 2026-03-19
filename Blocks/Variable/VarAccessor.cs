using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Provides indexed access to script variables.
	/// Getter returns a VariableBlock for use in script expressions and conditions.
	/// Setter performs immediate variable assignment during Build().
	/// </summary>
	public sealed class VarAccessor
	{
		private readonly ITable _table;

		public TableVariableBlock this[String name]
		{
			get => TableVariableBlock.Create(_table.GetHandle(name));
			set => _table.GetHandle(name).Variable = value.Variable;
		}

		internal VarAccessor(ITable table) => _table = table;

		public TableVariableBlock Define(String name, Variable value) => TableVariableBlock.Create(_table.DefineVariable(name, value));
		public TableVariableBlock Constant(String name, Variable value) => TableVariableBlock.Create(_table.DefineConstant(name, value));
	}
}
