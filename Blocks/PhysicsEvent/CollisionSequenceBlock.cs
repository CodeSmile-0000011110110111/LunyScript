using Luny.Engine.Bridge;
using LunyScript.Blocks.Guards;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks.PhysicsEvent
{
	/// <summary>
	/// Physics event sequence block for 3D collision events.
	/// Reads <see cref="LunyCollision"/> from <see cref="IScriptRuntimeContext.EventArgs"/>.
	/// Guards (cooldown) are checked first, then per-kind collision predicates.
	/// All must pass (AND logic) for child blocks to execute.
	/// Does not cache engine objects; uses IScratchContext for lookups.
	/// </summary>
	internal sealed class CollisionSequenceBlock : PhysicsEventSequenceBlock
	{
		private readonly Predicate<LunyCollider>[] _predicates;

		public CollisionSequenceBlock(IReadOnlyList<ActionBlock> blocks, EventGuard[] guards, Predicate<LunyCollider>[] predicates)
			: base(blocks, guards) => _predicates = predicates;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (context == null)
				return;

			if (!PassesAllGuards())
				return;

			if (_predicates != null)
			{
				var collision = (LunyCollision)context.EventArgs;
				var collider = collision.Collider;
				foreach (var predicate in _predicates)
				{
					if (!predicate(collider))
						return;
				}
			}

			WillExecute();

			foreach (var block in Blocks)
				block?.Execute(context);
		}
	}
}
