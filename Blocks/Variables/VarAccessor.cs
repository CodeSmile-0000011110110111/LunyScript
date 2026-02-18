using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Provides indexed access to script variables.
	/// Getter returns a VariableBlock for use in script expressions and conditions.
	/// Setter performs immediate variable assignment during Build().
	/// </summary>
	public sealed class VarAccessor
	{
		private readonly ITable _table;

		public VariableBlock this[String name]
		{
			get => TableVariableBlock.Create(_table.GetHandle(name));
			set => _table.GetHandle(name).Value = value.GetValue(null);
		}

		internal VarAccessor(ITable table) => _table = table;
	}
}
