using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that holds a reference to a script variable.
	/// </summary>
	public sealed class ScriptVariableBlock : VariableBlock
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

		internal static ScriptVariableBlock Create(Table.VarHandle handle, LunyStackTrace trace) => new(handle, trace);

		private ScriptVariableBlock(Table.VarHandle handle, LunyStackTrace trace)
			: base(trace) => _handle = handle;

		/// <summary>
		/// Used to set a variable's value during Build() time.
		/// </summary>
		/// <param name="value"></param>
		public void SetImmediate(Variable value) => _handle.Variable = value;
	}
}
