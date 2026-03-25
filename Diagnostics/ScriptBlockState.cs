using LunyScript.Blocks;
using System;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Pairs an ActionBlock with diagnostics-only state for use in the Script Blocks window.
	/// </summary>
	internal sealed class ScriptBlockState
	{
		private readonly ActionBlock _block;

		public Int32 FrameStamp { get; set; }
		public String DisplayString => _block?.ToString();

		public ScriptBlockState(ActionBlock block) => _block = block;

		public Boolean Contains(String filterText) => DisplayString.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
	}
}
