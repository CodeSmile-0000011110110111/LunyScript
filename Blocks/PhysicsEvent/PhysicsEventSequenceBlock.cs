using LunyScript.Blocks.Guards;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Abstract base for physics event sequence blocks (collision and trigger).
	/// Holds child blocks and context-free guards (e.g. cooldown).
	/// Guards are evaluated first (no event args needed); concrete subclasses evaluate their typed predicates second.
	/// All guards and predicates must pass (AND logic) for child blocks to execute.
	/// </summary>
	internal abstract class PhysicsEventSequenceBlock : ScriptActionBlock, ISequenceBlock
	{
		private readonly EventGuard[] _guards;
		public ScriptBlockID ID { get; }
		public IReadOnlyList<ScriptActionBlock> Blocks { get; }
		public Boolean IsEmpty => Blocks.Count == 0;

		protected PhysicsEventSequenceBlock(IReadOnlyList<ScriptActionBlock> blocks, EventGuard[] guards)
		{
			if (blocks == null || blocks.Count == 0)
				throw new ArgumentException("Sequence must contain at least one block", nameof(blocks));

			ID = ScriptBlockID.Generate();
			Blocks = blocks;
			_guards = guards;
		}

		protected Boolean PassesAllGuards()
		{
			if (_guards == null)
				return true;

			foreach (var guard in _guards)
			{
				if (!guard.CanExecute())
					return false;
			}

			return true;
		}

		protected void WillExecute()
		{
			if (_guards == null)
				return;

			foreach (var guard in _guards)
				guard.WillExecute();
		}
	}
}
