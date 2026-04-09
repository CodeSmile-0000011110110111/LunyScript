using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	// ── Shared Helpers ────────────────────────────────────────────────────────
	internal static class PhysicsEventBuilderHelper
	{
		internal static EventGuard[] BuildGuards(Double cooldownInSeconds) => cooldownInSeconds <= 0.0
			? null
			: new EventGuard[] { new CooldownGuard(cooldownInSeconds, LunyEngine.Instance.Time) };

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
