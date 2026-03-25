using Luny;
using System;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Pairs a VarHandleBase with diagnostics-only state (frame stamp) for use in the Script Variables window.
	/// </summary>
	internal sealed class ScriptVariableState
	{
		private readonly Table.VarHandleBase _handle;

		public String Name => _handle.Name;
		public Boolean IsConstant => _handle.IsConstant;
		public Int32 FrameStamp { get; set; }
		public Int32 ValueTypeOrdinal => _handle is Table.VarHandle h ? (Int32)h.Variable.Type : -1;

		public ScriptVariableState(Table.VarHandleBase handle) => _handle = handle;

		public Boolean HasName(String name) => _handle.Name == name;

		public Boolean Contains(String filterText) => _handle.Name.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;

		public Int32 CompareNameTo(ScriptVariableState other) =>
			String.Compare(_handle.Name, other._handle.Name, StringComparison.OrdinalIgnoreCase);

		public Boolean TryGetVarHandle(out Table.VarHandle varHandle)
		{
			varHandle = _handle as Table.VarHandle;
			return varHandle != null;
		}
	}
}
