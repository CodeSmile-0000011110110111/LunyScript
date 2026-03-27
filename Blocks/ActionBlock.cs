using System;
using System.Runtime.CompilerServices;
using StackTrace = Luny.StackTrace;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for action blocks that perform an action that may alter game state.
	/// </summary>
	public abstract class ActionBlock : ScriptBlock
	{
		public static Boolean IsNullOrEmpty(ActionBlock[] blocks) => blocks == null || blocks.Length == 0;

		protected ActionBlock(StackTrace trace)
			: base(trace) {}

		// placeholder until all blocks have stacktrace
		protected ActionBlock([CallerMemberName] String name = "", [CallerFilePath] String path = "", [CallerLineNumber] Int32 line = 0)
			: base(new StackTrace($"{nameof(ActionBlock)}.{name}: missing stack trace", path, line)) {}

		protected internal abstract void Execute(IScriptRuntimeContext runtimeContext);

		public override String ToString()
		{
			if (this is ISequenceBlock sequence)
			{
				return sequence.Blocks?.Count switch
				{
					0 => "No blocks",
					1 => "1 block",
					var _ => $"{sequence.Blocks?.Count} blocks",
				};
			}

			return base.ToString();
		}
	}
}
