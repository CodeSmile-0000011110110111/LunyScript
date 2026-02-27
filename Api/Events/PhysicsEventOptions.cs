using Luny.Engine.Bridge.Physics;
using LunyScript.Blocks;
using System;

namespace LunyScript
{

	/// <summary>
	/// Options DTO for collision/trigger event builders.
	/// Holds filter options, event handler blocks, and optional cooldown.
	/// </summary>
	public struct PhysicsEventOptions
	{
		public PhysicsEventFilterOptions Filter;
		public ScriptActionBlock[] BeginsBlocks;
		public ScriptActionBlock[] UpdatesBlocks;
		public ScriptActionBlock[] EndsBlocks;
		public Boolean IsTrigger;
		/// <summary>Minimum seconds between successive reactions. Zero means no cooldown.</summary>
		public Double Cooldown;
	}

	/// <summary>
	/// Immutable filter options for collision/trigger event builders.
	/// Each predicate is compiled once when the corresponding filter method is called.
	/// Null predicate means that filter kind is inactive (no check performed).
	/// Parameters within a kind are OR-combined; different kinds are AND-combined.
	/// </summary>
	public struct PhysicsEventFilterOptions
	{
		// Raw filter data (kept for diagnostics and for re-compiling trigger predicates from collision filter)
		public String[] Tags;
		public String[] Names;
		public String[] Layers;
		public Int32? LayerMask;
		public Type[] ComponentTypes;

		// Per-kind compiled predicates for collision events — AND-combined at runtime by CollisionSequenceBlock.
		// Each predicate encodes OR logic within its kind.
		public Predicate<LunyCollider> TagPredicate;
		public Predicate<LunyCollider> NamePredicate;
		public Predicate<LunyCollider> LayerPredicate;
		public Predicate<LunyCollider> TypePredicate;
	}
}
