using Luny;
using Luny.Engine.Bridge.Physics;
using LunyScript.Blocks;
using LunyScript.Blocks.Guards;
using LunyScript.Blocks.PhysicsEvent;
using LunyScript.Blocks.Predicates;
using System;

namespace LunyScript
{
	// ── State tokens & interfaces ─────────────────────────────────────────────

	public interface ICollisionBuilderState {}

	/// <summary>Initial state: no filters or event handlers set yet.</summary>
	public interface ICollisionBuilderStart : ICollisionBuilderState {}

	/// <summary>At least one filter or handler has been set.</summary>
	public interface ICollisionBuilderReady : ICollisionBuilderState {}

	/// <summary>Layered() has been called — Masked() is now blocked.</summary>
	public interface ICollisionBuilderLayered : ICollisionBuilderReady {}

	/// <summary>Masked() has been called — Layered() is now blocked.</summary>
	public interface ICollisionBuilderMasked : ICollisionBuilderReady {}

	public struct CollisionBuilderStart : ICollisionBuilderStart {}
	public struct CollisionBuilderReady : ICollisionBuilderReady {}
	public struct CollisionBuilderLayered : ICollisionBuilderLayered {}
	public struct CollisionBuilderMasked : ICollisionBuilderMasked {}

	/// <summary>
	/// Fluent builder for filtered collision/trigger event sequences.
	/// Filters (Tagged, Named, Layered, Masked, Typed) are order-independent and accumulate.
	/// Event handlers (Begins, Updates, Ends) are also order-independent.
	/// Parameters within a filter kind are OR-combined; different kinds are AND-combined.
	/// Call <see cref="Do"/> to finalize and schedule the event sequences.
	/// </summary>
	public readonly struct OnPhysicsEventBuilder<T> where T : struct, ICollisionBuilderState
	{
		internal readonly Script Script;
		internal readonly PhysicsEventOptions Options;
		internal readonly BuilderToken Token;

		internal OnPhysicsEventBuilder(Script script, PhysicsEventOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => Finalize(capturedScript, capturedOptions, token));
		}

		// ── Filters ───────────────────────────────────────────────────────────

		/// <summary>
		/// Only react when the other object has one of the given tags (OR logic).
		/// Combine with other filter kinds for AND logic across kinds.
		/// </summary>
		public OnPhysicsEventBuilder<CollisionBuilderReady> Tagged(params String[] tags)
		{
			var options = Options;
			options.Filter.Tags = tags;
			options.Filter.TagPredicate = BlockExecutePredicates.ForTags(tags);
			return new OnPhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object's name matches one of the given names (OR logic).
		/// </summary>
		public OnPhysicsEventBuilder<CollisionBuilderReady> With(params String[] names)
		{
			var options = Options;
			options.Filter.Names = names;
			options.Filter.NamePredicate = BlockExecutePredicates.ForNames(names);
			return new OnPhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object is on one of the given layers (OR logic).
		/// Mutually exclusive with Masked() — Masked() becomes unavailable after calling this.
		/// </summary>
		public OnPhysicsEventBuilder<CollisionBuilderLayered> Layered(params String[] layerNames)
		{
			var options = Options;
			options.Filter.Layers = layerNames;
			options.Filter.LayerPredicate = BlockExecutePredicates.ForLayers(layerNames);
			return new OnPhysicsEventBuilder<CollisionBuilderLayered>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object's component list contains at least one of the given types (OR logic).
		/// </summary>
		public OnPhysicsEventBuilder<CollisionBuilderReady> Typed(params Type[] componentTypes)
		{
			var options = Options;
			options.Filter.ComponentTypes = componentTypes;
			options.Filter.TypePredicate = BlockExecutePredicates.ForComponentTypes(componentTypes);
			return new OnPhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		// ── Event handlers ────────────────────────────────────────────────────

		/// <summary>Blocks to run when the collision/trigger begins.</summary>
		public OnPhysicsEventBuilder<CollisionBuilderReady> Begins(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.BeginsBlocks = blocks;
			return new OnPhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>Blocks to run each physics step while the collision/trigger persists.</summary>
		public OnPhysicsEventBuilder<CollisionBuilderReady> Continues(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.UpdatesBlocks = blocks;
			return new OnPhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>Blocks to run when the collision/trigger ends.</summary>
		public OnPhysicsEventBuilder<CollisionBuilderReady> Ends(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.EndsBlocks = blocks;
			return new OnPhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Minimum seconds between successive reactions. Zero (default) means no cooldown.
		/// The cooldown is checked before collision predicates; evaluated per event sequence.
		/// </summary>
		public OnPhysicsEventBuilder<CollisionBuilderReady> Cooldown(Double seconds)
		{
			var options = Options;
			options.Cooldown = seconds;
			return new OnPhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		// ── Finalize ──────────────────────────────────────────────────────────

		private static void Finalize(Script script, in PhysicsEventOptions options, BuilderToken token)
		{
			var guards = BuildGuards(options.Cooldown);

			if (options.IsTrigger)
			{
				var predicates = BuildTriggerPredicates(options.Filter);

				if (options.BeginsBlocks != null)
				{
					var block = new TriggerSequenceBlock(options.BeginsBlocks, guards, predicates);
					script.Scheduler?.ScheduleTriggerEventSequence(block, LunyTriggerEvent.OnTriggerEntered);
				}

				if (options.UpdatesBlocks != null)
				{
					var block = new TriggerSequenceBlock(options.UpdatesBlocks, guards, predicates);
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
				var predicates = BuildCollisionPredicates(options.Filter);

				if (options.BeginsBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.BeginsBlocks, guards, predicates);
					script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionEntered);
				}

				if (options.UpdatesBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.UpdatesBlocks, guards, predicates);
					script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionUpdate);
				}

				if (options.EndsBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.EndsBlocks, guards, predicates);
					script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionExited);
				}
			}

			script.FinalizeBuilderToken(token);
		}

		private static Predicate<LunyCollider>[] BuildCollisionPredicates(in PhysicsEventFilterOptions filter)
		{
			var count = 0;
			if (filter.NamePredicate != null)
				count++;
			if (filter.TagPredicate != null)
				count++;
			if (filter.LayerPredicate != null)
				count++;
			if (filter.TypePredicate != null)
				count++;

			if (count == 0)
				return null;

			var predicates = new Predicate<LunyCollider>[count];
			var i = 0;
			if (filter.NamePredicate != null)
				predicates[i++] = filter.NamePredicate;
			if (filter.TagPredicate != null)
				predicates[i++] = filter.TagPredicate;
			if (filter.LayerPredicate != null)
				predicates[i++] = filter.LayerPredicate;
			if (filter.TypePredicate != null)
				predicates[i++] = filter.TypePredicate;
			return predicates;
		}

		private static Predicate<LunyCollider>[] BuildTriggerPredicates(in PhysicsEventFilterOptions filter)
		{
			var count = 0;
			if (filter.Names != null)
				count++;
			if (filter.Tags != null)
				count++;
			if (filter.Layers != null || filter.LayerMask.HasValue)
				count++;
			if (filter.ComponentTypes != null)
				count++;

			if (count == 0)
				return null;

			var predicates = new Predicate<LunyCollider>[count];
			var i = 0;
			if (filter.Names != null)
				predicates[i++] = BlockExecutePredicates.ForNames(filter.Names);
			if (filter.Tags != null)
				predicates[i++] = BlockExecutePredicates.ForTags(filter.Tags);
			if (filter.Layers != null)
				predicates[i++] = BlockExecutePredicates.ForLayers(filter.Layers);
			else if (filter.LayerMask.HasValue)
				predicates[i++] = BlockExecutePredicates.ForLayerMask(filter.LayerMask.Value);
			if (filter.ComponentTypes != null)
				predicates[i++] = BlockExecutePredicates.ForComponentTypes(filter.ComponentTypes);
			return predicates;
		}

		private static EventGuard[] BuildGuards(Double cooldownInSeconds)
		{
			if (cooldownInSeconds <= 0.0)
				return null;

			return new EventGuard[] { new CooldownGuard<T>(cooldownInSeconds, LunyEngine.Instance.Time) };
		}
	}

	// ── Extension methods (mutual exclusivity for Masked) ─────────────────────

	public static class OnCollisionEventBuilderMaskedExtensions
	{
		/// <summary>
		/// Only react when the other object's layer is included in the given bitmask.
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static OnPhysicsEventBuilder<CollisionBuilderMasked> Masked<T>(
			this OnPhysicsEventBuilder<T> b, Int32 layerMask)
			where T : struct, ICollisionBuilderStart
		{
			var options = b.Options;
			options.Filter.LayerMask = layerMask;
			options.Filter.LayerPredicate = BlockExecutePredicates.ForLayerMask(layerMask);
			return new OnPhysicsEventBuilder<CollisionBuilderMasked>(b.Script, options, b.Token);
		}

		/// <summary>
		/// Only react when the other object's layer name matches one of the given names (OR logic).
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static OnPhysicsEventBuilder<CollisionBuilderMasked> Masked<T>(
			this OnPhysicsEventBuilder<T> b, params String[] layerNames)
			where T : struct, ICollisionBuilderStart
		{
			var options = b.Options;
			options.Filter.Layers = layerNames;
			options.Filter.LayerPredicate = BlockExecutePredicates.ForLayers(layerNames);
			return new OnPhysicsEventBuilder<CollisionBuilderMasked>(b.Script, options, b.Token);
		}
	}
}
