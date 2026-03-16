using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Blocks.Guards;
using LunyScript.Blocks.PhysicsEvent;
using System;

namespace LunyScript.Api
{
	public interface ICollisionBuilderState {}
	public interface ICollisionBuilderStart : ICollisionBuilderState {}
	public struct CollisionBuilderStart : ICollisionBuilderStart {}
	public interface ICollisionBuilderReady : ICollisionBuilderState {}
	public struct CollisionBuilderReady : ICollisionBuilderReady {}
	public interface ICollisionBuilderLayered : ICollisionBuilderReady {}
	public struct CollisionBuilderLayered : ICollisionBuilderLayered {}
	public interface ICollisionBuilderMasked : ICollisionBuilderReady {}
	public struct CollisionBuilderMasked : ICollisionBuilderMasked {}

	/// <summary>
	/// Fluent builder for filtered collision/trigger event sequences.
	/// Filters (Tagged, Named, Layered, Masked, Typed) are order-independent and accumulate.
	/// Event handlers (Begins, Updates, Ends) are also order-independent.
	/// Parameters within a filter kind are OR-combined; different kinds are AND-combined.
	/// </summary>
	public readonly struct PhysicsEventBuilder<T> where T : struct, ICollisionBuilderState
	{
		internal readonly PhysicsEventOptions Options;

		internal PhysicsEventBuilder(Script script, Boolean isTrigger)
		{
			var token = script.CreateBuilderToken("Physics Event", isTrigger ? "Trigger" : "Collision");
			Options = new PhysicsEventOptions { Script = script, Token = token, IsTrigger = isTrigger };
		}

		internal PhysicsEventBuilder(in PhysicsEventOptions options) => Options = options;

		// ── Filters ───────────────────────────────────────────────────────────

		/// <summary>
		/// Only react when the other object has one of the given tags (OR logic).
		/// Combine with other filter kinds for AND logic across kinds.
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Tagged(params String[] tags)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(Options.Script, Options.TagsFilter);
			return new PhysicsEventBuilder<CollisionBuilderReady>(Options with { TagsFilter = tags });
		}

		/// <summary>
		/// Only react when the other object's name matches one of the given names (OR logic).
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderReady> With(params String[] names)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(Options.Script, Options.NameFilter);
			return new PhysicsEventBuilder<CollisionBuilderReady>(Options with { NameFilter = names });
		}

		/// <summary>
		/// Only react when the other object is on one of the given layers (OR logic).
		/// Mutually exclusive with Masked() — Masked() becomes unavailable after calling this.
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderLayered> Layered(params String[] layerNames)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(Options.Script, Options.LayerNameFilter);
			return new PhysicsEventBuilder<CollisionBuilderLayered>(Options with { LayerNameFilter = layerNames });
		}

		/// <summary>
		/// Only react when the other object's component list contains at least one of the given types (OR logic).
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Typed(params Type[] componentTypes)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(Options.Script, Options.ComponentTypeFilter);
			return new PhysicsEventBuilder<CollisionBuilderReady>(Options with { ComponentTypeFilter = componentTypes });
		}

		// ── Event handlers ────────────────────────────────────────────────────

		/// <summary>Blocks to run when the collision/trigger begins.</summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Begins(params ActionBlock[] blocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(Options.Script, Options.BeginsBlocks);
			var options = Options with { BeginsBlocks = blocks };
			SetAutoFinish(options);
			return new PhysicsEventBuilder<CollisionBuilderReady>(options);
		}

		/// <summary>Blocks to run each physics step while the collision/trigger persists.</summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Continues(params ActionBlock[] blocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(Options.Script, Options.ContinuesBlocks);
			var options = Options with { ContinuesBlocks = blocks };
			SetAutoFinish(options);
			return new PhysicsEventBuilder<CollisionBuilderReady>(options);
		}

		/// <summary>Blocks to run when the collision/trigger ends.</summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Ends(params ActionBlock[] blocks)
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(Options.Script, Options.EndsBlocks);
			var options = Options with { EndsBlocks = blocks };
			SetAutoFinish(options);
			return new PhysicsEventBuilder<CollisionBuilderReady>(options);
		}

		/// <summary>
		/// Minimum seconds between successive reactions. Zero (default) means no cooldown.
		/// The cooldown is checked before collision predicates; evaluated per event sequence.
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Cooldown(Double seconds) => new(Options with { Cooldown = Math.Max(0.0, seconds) });

		// ── Finalize ──────────────────────────────────────────────────────────
		private void SetAutoFinish(PhysicsEventOptions options) =>
			options.Token.AutoFinish = () => Finish(options.Script, options.Token, options);

		internal static void Finish(Script script, BuilderToken token, in PhysicsEventOptions options)
		{
			if (options.BeginsBlocks == null && options.ContinuesBlocks == null && options.EndsBlocks == null)
				throw new LunyScriptException($"{script}: Physics Event without any blocks");

			var guards = BuildGuards(options.Cooldown);
			var predicates = BuildPredicates(options);

			if (options.IsTrigger)
			{
				if (options.BeginsBlocks != null)
				{
					var block = new TriggerSequenceBlock(options.BeginsBlocks, guards, predicates);
					script.Scheduler?.ScheduleTriggerEventSequence(block, LunyTriggerEvent.OnTriggerEntered);
				}

				if (options.ContinuesBlocks != null)
				{
					var block = new TriggerSequenceBlock(options.ContinuesBlocks, guards, predicates);
					script.Scheduler?.ScheduleTriggerEventSequence(block, LunyTriggerEvent.OnTriggerUpdate);
				}

				if (options.EndsBlocks != null)
				{
					var block = new TriggerSequenceBlock(options.EndsBlocks, guards, predicates);
					script.Scheduler?.ScheduleTriggerEventSequence(block, LunyTriggerEvent.OnTriggerExited);
				}
			}
			else
			{
				if (options.BeginsBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.BeginsBlocks, guards, predicates);
					script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionEntered);
				}

				if (options.ContinuesBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.ContinuesBlocks, guards, predicates);
					script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionUpdate);
				}

				if (options.EndsBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.EndsBlocks, guards, predicates);
					script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionExited);
				}
			}

			script.MarkBuilderTokenFinished(token);
		}

		private static EventGuard[] BuildGuards(Double cooldownInSeconds)
		{
			if (cooldownInSeconds <= 0.0)
				return null;

			return new EventGuard[] { new CooldownGuard<T>(cooldownInSeconds, LunyEngine.Instance.Time) };
		}

		private static Predicate<LunyCollider>[] BuildPredicates(in PhysicsEventOptions options)
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

	// ── Layer/LayerMask Extensions (mutual exclusivity) ─────────────────────
	public static class PhysicsEventLayerExtensions
	{
		/// <summary>
		/// Only react when the other object's layer name matches one of the given names (OR logic).
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static PhysicsEventBuilder<CollisionBuilderMasked> Masked<T>(this PhysicsEventBuilder<T> b, params String[] layerNames)
			where T : struct, ICollisionBuilderStart
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.LayerNameFilter);
			var options = b.Options with { LayerNameFilter = layerNames };
			return new PhysicsEventBuilder<CollisionBuilderMasked>(options);
		}

		/// <summary>
		/// Only react when the other object's layer is included in the given bitmask.
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static PhysicsEventBuilder<CollisionBuilderMasked> Masked<T>(this PhysicsEventBuilder<T> b, Int32 layerMask)
			where T : struct, ICollisionBuilderStart
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.LayerMaskFilter);
			var options = b.Options with { LayerMaskFilter = layerMask };
			return new PhysicsEventBuilder<CollisionBuilderMasked>(options);
		}
	}

	/// <summary>
	/// Options DTO for collision/trigger event builders.
	/// Holds filter options, event handler blocks, and optional cooldown.
	/// </summary>
	internal record PhysicsEventOptions
	{
		internal Script Script;
		internal BuilderToken Token;

		public ActionBlock[] BeginsBlocks;
		public ActionBlock[] ContinuesBlocks;
		public ActionBlock[] EndsBlocks;
		public Boolean IsTrigger;
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
