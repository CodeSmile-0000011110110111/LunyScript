using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	public interface ISequenceBlock
	{
		ScriptBlockId Id { get; }
		IReadOnlyList<ActionBlock> Blocks => throw new NotImplementedException(nameof(Blocks));
		Int32 BlockCount => Blocks?.Count ?? 0;
		Boolean IsEmpty => BlockCount == 0;
	}
}
