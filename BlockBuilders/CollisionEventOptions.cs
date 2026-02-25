using LunyScript.Blocks;
using System;

namespace LunyScript.BlockBuilders
{
	/// <summary>
	/// Options DTO for collision/trigger event builders.
	/// Holds filter options, event handler blocks, and optional cooldown.
	/// </summary>
	public struct CollisionEventOptions
	{
		public CollisionFilterOptions Filter;
		public ScriptActionBlock[] BeginsBlocks;
		public ScriptActionBlock[] UpdatesBlocks;
		public ScriptActionBlock[] EndsBlocks;
		public Boolean IsTrigger;
		/// <summary>Minimum seconds between successive reactions. Zero means no cooldown.</summary>
		public Double Cooldown;
	}
}
