using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	public sealed class VariableSetValueBlock : ActionBlock
	{
		private readonly Table.VarHandle _handle;
		private readonly VariableBlock _variable;

		public static VariableSetValueBlock Create(Table.VarHandle handle, VariableBlock value, LunyStackTrace trace) =>
			new(handle, value, trace);

		private VariableSetValueBlock(Table.VarHandle handle, VariableBlock value, LunyStackTrace trace)
			: base(trace)
		{
			_handle = handle ?? throw new ArgumentNullException(nameof(handle));
			_variable = value ?? throw new ArgumentNullException(nameof(value));
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var value = _variable.Variable;
			_handle.Variable = value;
		}

		public override String ToString()
		{
			if (_variable is LiteralVariableBlock)
				return $"({_handle}){Emoji.Equality}{_variable}";

			return _variable.ToString();
		}
	}
}
