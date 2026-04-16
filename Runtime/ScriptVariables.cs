using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Provides access to the script's Variable instances stored in a Table.
	/// </summary>
	public sealed class ScriptVariables
	{
		private readonly ITable _table;

		public ScriptVariableBlock this[String name]
		{
			get => ScriptVariableBlock.Create(_table.GetHandle(name), ScriptTrace.TryCreateStackTrace($"[{name}]"));
			set => _table.GetHandle(name).Variable = value.Variable;
		}

		internal ScriptVariables(ITable table) => _table = table;

		/// <summary>
		/// Defines (or gets) a variable with the given name and the default value 0.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ScriptVariableBlock Define(String name) =>
			ScriptVariableBlock.Create(_table.DefineVariable(name, 0.0), ScriptTrace.TryCreateStackTrace(nameof(Define)));

		/// <summary>
		/// Defines (or gets) a variable with the given name and assigns the provided Variable.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="variable"></param>
		/// <returns></returns>
		public ScriptVariableBlock Define(String name, Variable variable) => ScriptVariableBlock.Create(_table.DefineVariable(name, variable),
			ScriptTrace.TryCreateStackTrace(nameof(Define)));

		public ScriptVariableBlock Constant(String name, Variable variable) => ScriptVariableBlock.Create(_table.DefineConstant(name, variable),
			ScriptTrace.TryCreateStackTrace(nameof(Constant)));
	}
}
