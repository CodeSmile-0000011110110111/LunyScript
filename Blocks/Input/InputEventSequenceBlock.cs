using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for input event sequences.
	/// Holds child blocks and context-free guards (e.g. cooldown).
	/// Guards are evaluated first (no event args needed); concrete subclasses evaluate their typed predicates second.
	/// All guards and predicates must pass (AND logic) for child blocks to execute.
	/// </summary>
	internal sealed class InputEventSequenceBlock : ActionBlock, ISequenceBlock
	{
		public ScriptBlockId Id { get; }
		public IReadOnlyList<ActionBlock> Blocks { get; }
		public Boolean IsEmpty => Blocks.Count == 0;
		public String ActionName { get; }
		public String UserName { get; }
		public LunyInputActionPhase Phase { get; }

		public static InputEventSequenceBlock Create(String actionName, String userName, LunyInputActionPhase phase, ActionBlock[] blocks) =>
			blocks == null || blocks.Length == 0 ? null : new InputEventSequenceBlock(actionName, userName, phase, blocks);

		private InputEventSequenceBlock(String actionName, String userName, LunyInputActionPhase phase, IReadOnlyList<ActionBlock> blocks)
		{
			Id = ScriptBlockId.Generate();
			Blocks = blocks;
			ActionName = actionName;
			UserName = userName;
			Phase = phase;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			foreach (var block in Blocks)
				block.Execute(runtimeContext);
		}
	}
}
