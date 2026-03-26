using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
 public interface ISequenceBlock
 {
 	ScriptBlockId Id { get; }
 	IReadOnlyList<ActionBlock> Blocks => throw new NotImplementedException(nameof(Blocks));
 	Boolean IsEmpty => false;
 }
}
