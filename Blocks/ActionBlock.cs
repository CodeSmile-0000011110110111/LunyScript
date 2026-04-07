using System;
using StackTrace = Luny.StackTrace;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for action blocks that perform an action that may alter game state.
	/// </summary>
	public abstract class ActionBlock : ScriptBlock
	{
		public static Boolean IsNullOrEmpty(ActionBlock[] blocks) => blocks == null || blocks.Length == 0;

		protected ActionBlock(StackTrace trace = null)
			: base(trace) {}

		protected internal abstract void Execute(IScriptRuntimeContext context);

		public override String ToString()
		{
			if (this is ISequenceBlock sequence)
			{
				return sequence.Blocks?.Count switch
				{
					0 => "Empty",
					1 => "1 block",
					var _ => $"{sequence.Blocks?.Count} blocks",
				};
			}

			return base.ToString();
		}
	}
}
