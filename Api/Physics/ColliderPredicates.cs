using Luny.Engine.Bridge.Physics;
using System;

namespace LunyScript.Api.Physics
{
	/// <summary>
	/// Factory methods for per-kind collision predicates (Predicate&lt;LunyCollision&gt;).
	/// Each predicate encodes OR logic within its kind.
	/// Predicates are compiled once at builder call time and evaluated at each physics event.
	/// </summary>
	internal static class ColliderPredicates
	{
		public static Predicate<LunyCollider> ForTags(String[] tags)
		{
			var captured = tags;
			return collider =>
			{
				foreach (var tag in captured)
				{
					if (collider.Tag == tag)
						return true;
				}
				return false;
			};
		}

		public static Predicate<LunyCollider> ForNames(String[] names)
		{
			var captured = names;
			return collider =>
			{
				foreach (var name in captured)
				{
					if (collider.Name == name)
						return true;
				}
				return false;
			};
		}

		public static Predicate<LunyCollider> ForLayers(String[] layerNames)
		{
			var captured = layerNames;
			return collider =>
			{
				foreach (var layer in captured)
				{
					if (collider.LayerName == layer)
						return true;
				}
				return false;
			};
		}

		public static Predicate<LunyCollider> ForLayerMask(Int32 mask) => mask != 0 ? collider => (mask & 1 << collider.Layer) != 0 : null;

		public static Predicate<LunyCollider> ForComponentTypes(Type[] types)
		{
			var captured = types;
			return collider =>
			{
				foreach (var type in captured)
				{
					if (collider.HasComponent(type))
						return true;
				}
				return false;
			};
		}
	}
}
