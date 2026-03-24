using Luny;
using System;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Pairs a VarHandleBase with diagnostics-only state (frame stamp) for use in the Script Variables window.
	/// </summary>
	internal sealed class ScriptVariableState
	{
		public Table.VarHandleBase Handle { get; }
		public Int32 FrameStamp { get; set; }

		public ScriptVariableState(Table.VarHandleBase handle) => Handle = handle;
	}
}
