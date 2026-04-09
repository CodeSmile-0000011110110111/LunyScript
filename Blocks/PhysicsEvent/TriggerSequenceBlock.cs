using Luny;
using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Physics event sequence block for 3D trigger events.
	/// Reads <see cref="LunyCollider"/> from <see cref="IScriptRuntimeContext.EventArgs"/>.
	/// Guards (cooldown) are checked first, then per-kind collider predicates.
	/// All must pass (AND logic) for child blocks to execute.
	/// Does not cache engine objects; uses IScratchContext for lookups.
	/// </summary>
	internal sealed class TriggerSequenceBlock : PhysicsEventSequenceBlock
	{
		private readonly Predicate<LunyCollider>[] _predicates;

		public TriggerSequenceBlock(IReadOnlyList<ActionBlock> blocks, EventGuard[] guards, Predicate<LunyCollider>[] predicates, StackTrace trace)
			: base(blocks, guards, trace) => _predicates = predicates;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (context == null)
				return;

			if (!PassesAllGuards())
				return;

			if (_predicates != null)
			{
				var collider = (LunyCollider)context.EventArgs;
				foreach (var predicate in _predicates)
				{
					if (!predicate(collider))
						return;
				}
			}

			foreach (var block in Blocks)
				block?.Execute(context);
		}
	}
}
