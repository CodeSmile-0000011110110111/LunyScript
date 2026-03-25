using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Implemented by blocks that contain named sequences of condition and/or action sub-blocks.
	/// </summary>
	public interface IBlockContainer
	{
		Int32 ConditionSequenceCount => 0;
		Int32 ActionSequenceCount => 0;
		String GetConditionSequenceName(Int32 index) => String.Empty;
		String GetActionSequenceName(Int32 index) => String.Empty;
		IEnumerable<IScriptBlock> GetConditionSequence(Int32 index) => Array.Empty<IScriptBlock>();
		IEnumerable<IScriptBlock> GetActionSequence(Int32 index) => Array.Empty<IScriptBlock>();
	}
}
