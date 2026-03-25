using LunyScript.Blocks;
using System;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Pairs a ScriptBlock with diagnostics-only state for use in the Script Blocks window.
	/// </summary>
	internal sealed class ScriptBlockState
	{
		private readonly ScriptBlock _block;

		public Int32 FrameStamp { get; set; }
		public String DisplayString => _block?.ToString();
		public Boolean IsAction => _block is ActionBlock;
		public Boolean IsCondition => _block is ConditionBlock;
		public ActionBlock Action => _block as ActionBlock;
		public ConditionBlock Condition => _block as ConditionBlock;

		public Boolean TryGetAction(out ActionBlock action) { action = Action; return IsAction; }
		public Boolean TryGetCondition(out ConditionBlock condition) { condition = Condition; return IsCondition; }

		public ScriptBlockState(ScriptBlock block) => _block = block;

		public Boolean Contains(String filterText) => DisplayString.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
	}
}
