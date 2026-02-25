using Luny.Engine.Bridge.Physics;
using System;

namespace LunyScript.BlockBuilders
{
	/// <summary>
	/// Immutable filter options for collision/trigger event builders.
	/// Each predicate is compiled once when the corresponding filter method is called.
	/// Null predicate means that filter kind is inactive (no check performed).
	/// Parameters within a kind are OR-combined; different kinds are AND-combined.
	/// </summary>
	public struct CollisionFilterOptions
	{
		// Raw filter data (kept for diagnostics and for re-compiling trigger predicates from collision filter)
		public String[] Tags;
		public String[] Names;
		public String[] Layers;
		public Int32? LayerMask;
		public Type[] ComponentTypes;

		// Per-kind compiled predicates for collision events — AND-combined at runtime by CollisionSequenceBlock.
		// Each predicate encodes OR logic within its kind.
		public Predicate<LunyCollision> TagPredicate;
		public Predicate<LunyCollision> NamePredicate;
		public Predicate<LunyCollision> LayerPredicate;
		public Predicate<LunyCollision> TypePredicate;
	}
}
