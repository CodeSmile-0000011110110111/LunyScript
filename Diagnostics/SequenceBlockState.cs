using LunyScript.Blocks;
using System;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Pairs an ISequenceBlock with diagnostics-only state for use in the Script Blocks window.
	/// </summary>
	internal sealed class SequenceBlockState
	{
		private readonly ISequenceBlock _sequence;

		public Int32 FrameStamp { get; set; }
		public ISequenceBlock Sequence => _sequence;

		public SequenceBlockState(ISequenceBlock sequence) => _sequence = sequence;
	}
}
