using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	public interface ISequenceBlock
	{
		ScriptBlockId Id { get; }
		IReadOnlyList<ScriptActionBlock> Blocks { get; }
		Boolean IsEmpty => Blocks.Count == 0;
	}

	/// <summary>
	/// Abstract base for sequence blocks that contain child action blocks.
	/// </summary>
	public sealed class SequenceBlock : ScriptActionBlock, ISequenceBlock
	{
		public ScriptBlockId Id { get; }
		public IReadOnlyList<ScriptActionBlock> Blocks { get; }
		public Boolean IsEmpty => Blocks.Count == 0;

		public static SequenceBlock TryCreate(IReadOnlyList<ScriptActionBlock> blocks) => blocks?.Count > 0 ? new SequenceBlock(blocks) : null;

		public SequenceBlock(IReadOnlyList<ScriptActionBlock> blocks)
		{
			if (blocks == null || blocks.Count == 0)
				throw new ArgumentException("Sequence must contain at least one block", nameof(blocks));

			Id = ScriptBlockId.Generate();
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
