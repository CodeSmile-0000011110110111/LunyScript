using Luny.Engine.Bridge.Physics;
using System;

namespace LunyScript.BlockBuilders
{
	/// <summary>
	/// Factory methods for per-kind collision predicates (Predicate&lt;LunyCollision&gt;).
	/// Each predicate encodes OR logic within its kind.
	/// Predicates are compiled once at builder call time and evaluated at each physics event.
	/// </summary>
	internal static class CollisionPredicates
	{
		public static Predicate<LunyCollision> ForTags(String[] tags)
		{
			var captured = tags;
			return collision =>
			{
				foreach (var tag in captured)
				{
					if (collision.Tag == tag)
						return true;
				}
				return false;
			};
		}

		public static Predicate<LunyCollision> ForNames(String[] names)
		{
			var captured = names;
			return collision =>
			{
				foreach (var name in captured)
				{
					if (collision.Name == name)
						return true;
				}
				return false;
			};
		}

		public static Predicate<LunyCollision> ForLayers(String[] layerNames)
		{
			var captured = layerNames;
			return collision =>
			{
				foreach (var layer in captured)
				{
					if (collision.LayerName == layer)
						return true;
				}
				return false;
			};
		}

		public static Predicate<LunyCollision> ForLayerMask(Int32 mask)
		{
			return collision => (mask & (1 << collision.LayerIndex)) != 0;
		}

		public static Predicate<LunyCollision> ForComponentTypes(Type[] types)
		{
			var captured = types;
			return collision =>
			{
				foreach (var type in captured)
				{
					if (collision.HasComponent(type))
						return true;
				}
				return false;
			};
		}
	}
}
