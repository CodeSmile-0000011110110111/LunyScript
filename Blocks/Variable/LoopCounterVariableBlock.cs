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

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => throw new NotImplementedException($"{nameof(LoopCounterVariableBlock)}.GetValue()");
		}

		private LoopCounterVariableBlock() {}
		//runtimeContext.LoopCount;
	}
}
