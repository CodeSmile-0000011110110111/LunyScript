using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for sequence blocks that contain child action blocks.
	/// </summary>
	public sealed class SequenceBlock : ScriptActionBlock
	{
		public ScriptBlockID ID { get; }
		public IReadOnlyList<ScriptActionBlock> Blocks { get; }
		public Boolean IsEmpty => Blocks.Count == 0;

		public static SequenceBlock TryCreate(IReadOnlyList<ScriptActionBlock> blocks) => blocks?.Count > 0 ? new SequenceBlock(blocks) : null;

		public SequenceBlock(IReadOnlyList<ScriptActionBlock> blocks)
		{
			if (blocks == null || blocks.Count == 0)
				throw new ArgumentException("Sequence must contain at least one block", nameof(blocks));

			ID = ScriptBlockID.Generate();
			Blocks = blocks;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext == null)
				return;

			foreach (var block in Blocks)
				block?.Execute(runtimeContext);
		}
	}
}
