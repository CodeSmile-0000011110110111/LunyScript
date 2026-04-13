using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
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

	// ── Shared Helpers ────────────────────────────────────────────────────────
	internal static class PhysicsEventHelper
	{
		internal static EventGuard[] BuildGuards(Double cooldownInSeconds) => cooldownInSeconds <= 0.0
			? null
			: new[] { new CooldownGuard(cooldownInSeconds, LunyEngine.Instance.Time) };

		internal static Predicate<LunyCollider>[] BuildPredicates(in PhysicsEventOptions options)
		{
			var count = 0;
			if (options.NameFilter != null)
				count++;
			if (options.TagsFilter != null)
				count++;
			if (options.LayerNameFilter != null || options.LayerMaskFilter.HasValue)
				count++;
			if (options.ComponentTypeFilter != null)
				count++;

			if (count == 0)
				return null;

			var predicates = new Predicate<LunyCollider>[count];
			var i = 0;

			if (options.NameFilter != null)
				predicates[i++] = PhysicsEventPredicates.ForNames(options.NameFilter);
			if (options.TagsFilter != null)
				predicates[i++] = PhysicsEventPredicates.ForTags(options.TagsFilter);
			if (options.LayerNameFilter != null)
				predicates[i++] = PhysicsEventPredicates.ForLayers(options.LayerNameFilter);
			else if (options.LayerMaskFilter.HasValue)
				predicates[i++] = PhysicsEventPredicates.ForLayerMask(options.LayerMaskFilter.Value);
			if (options.ComponentTypeFilter != null)
				predicates[i++] = PhysicsEventPredicates.ForComponentTypes(options.ComponentTypeFilter);

			return predicates;
		}
	}
}
