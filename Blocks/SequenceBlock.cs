using Luny;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for sequence blocks that contain child action blocks.
	/// </summary>
	public sealed class SequenceBlock : ActionBlock, ISequenceBlock
	{
		public ScriptBlockId Id { get; }
		public IReadOnlyList<ActionBlock> Blocks { get; }

		public static SequenceBlock TryCreate(IReadOnlyList<ActionBlock> blocks, LunyStackTrace trace = null) =>
			blocks?.Count > 0 ? new SequenceBlock(blocks, trace) : null;

		private SequenceBlock(IReadOnlyList<ActionBlock> blocks, LunyStackTrace trace)
			: base(trace)
		{
			Id = ScriptBlockId.Generate();
			Blocks = blocks;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (context == null)
				return;

			foreach (var block in Blocks)
				block?.Execute(context);
		}
	}
}
