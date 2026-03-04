using Luny.Engine.Bridge.Physics;
using System;

namespace LunyScript
{
	/// <summary>
	/// Factory methods for per-kind trigger predicates. Each predicate encodes OR logic within its kind.
	/// Predicates are compiled once at builder call time and evaluated at each physics event.
	/// </summary>
	internal static class PhysicsEventPredicates
	{
		public static Predicate<LunyCollider> ForTags(String[] tags) => collider =>
		{
			foreach (var tag in tags)
			{
				if (collider.Tag == tag)
					return true;
			}
			return false;
		};

		public static Predicate<LunyCollider> ForNames(String[] names) => collider =>
		{
			foreach (var name in names)
			{
				if (collider.Name == name)
					return true;
			}
			return false;
		};

		public static Predicate<LunyCollider> ForLayers(String[] layerNames) => collider =>
		{
			foreach (var layer in layerNames)
			{
				if (collider.LayerName == layer)
					return true;
			}
			return false;
		};

		public static Predicate<LunyCollider> ForLayerMask(Int32 mask) => collider => (mask & 1 << collider.Layer) != 0;

		public static Predicate<LunyCollider> ForComponentTypes(Type[] types) => collider =>
		{
			foreach (var type in types)
			{
				if (collider.HasComponent(type))
					return true;
			}
			return false;
		};
	}
}
