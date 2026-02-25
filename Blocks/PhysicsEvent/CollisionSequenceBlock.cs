using Luny.Engine.Bridge.Physics;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Physics event sequence block for 3D collision events.
	/// Reads <see cref="LunyCollision"/> from <see cref="IScriptRuntimeContext.EventArgs"/>.
	/// Guards (cooldown) are checked first, then per-kind collision predicates.
	/// All must pass (AND logic) for child blocks to execute.
	/// Does not cache engine objects; uses IScratchContext for lookups.
	/// </summary>
	public sealed class CollisionSequenceBlock : PhysicsEventSequenceBlock
	{
		private readonly Predicate<LunyCollision>[] _predicates;

		public CollisionSequenceBlock(
			IReadOnlyList<ScriptActionBlock> blocks,
			Func<Boolean>[] guards,
			Predicate<LunyCollision>[] predicates)
			: base(blocks, guards)
		{
			_predicates = predicates;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext == null)
				return;

			if (!PassesAllGuards())
				return;

			if (_predicates != null)
			{
				var collision = runtimeContext.EventArgs as LunyCollision;
				foreach (var predicate in _predicates)
				{
					if (!predicate(collision))
						return;
				}
			}

			foreach (var block in Blocks)
				block?.Execute(runtimeContext);
		}
	}
}
