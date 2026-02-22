using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Block that returns the current loop counter from the context stack.
	/// </summary>
	internal sealed class LoopCounterVariableBlock : VariableBlock
	{
		public static readonly LoopCounterVariableBlock Instance = new();
		private LoopCounterVariableBlock() {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() => throw new NotImplementedException($"{nameof(LoopCounterVariableBlock)}.{nameof(GetValue)}()");
			//runtimeContext.LoopCount;
	}
}
