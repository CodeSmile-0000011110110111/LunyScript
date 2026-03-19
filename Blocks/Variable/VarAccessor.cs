using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Provides access to script variables.
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

		/// <summary>
		/// Defines (or gets) a variable with the given name and the default value 0.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public TableVariableBlock Define(String name) => TableVariableBlock.Create(_table.DefineVariable(name, 0.0));
		/// <summary>
		/// Defines (or gets) a variable with the given name and assigns the provided Variable.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="variable"></param>
		/// <returns></returns>
		public TableVariableBlock Define(String name, Variable variable) => TableVariableBlock.Create(_table.DefineVariable(name, variable));
		public TableVariableBlock Constant(String name, Variable variable) => TableVariableBlock.Create(_table.DefineConstant(name, variable));
	}
}
