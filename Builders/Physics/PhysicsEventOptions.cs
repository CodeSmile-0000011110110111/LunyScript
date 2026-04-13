using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Options DTO for collision/trigger event builders.
	/// Holds filter options, event handler blocks, and optional cooldown.
	/// </summary>
	internal record PhysicsEventOptions
	{
		internal Script Script;
		internal BuilderToken Token;
		internal LunyStackTrace Trace;

		public ActionBlock[] StartedOrEnteredBlocks;
		public ActionBlock[] TouchingOrOverlappingBlocks;
		public ActionBlock[] EndedOrExitedBlocks;

		/// <summary>Minimum seconds between successive reactions. Zero means no cooldown.</summary>
		public Double Cooldown;

		// Raw filter data (kept for diagnostics and for re-compiling trigger predicates from collision filter)
		public String[] TagsFilter;
		public String[] NameFilter;
		public String[] LayerNameFilter;
		public Int32? LayerMaskFilter;
		public Type[] ComponentTypeFilter;
	}
}
