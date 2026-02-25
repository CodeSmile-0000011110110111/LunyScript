using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Bridge.Physics;
using LunyScript.Blocks;
using System;

namespace LunyScript.BlockBuilders
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

	// ── Builder ───────────────────────────────────────────────────────────────

	/// <summary>
	/// Fluent builder for filtered collision/trigger event sequences.
	/// Filters (Tagged, Named, Layered, Masked, Typed) are order-independent and accumulate.
	/// Event handlers (Begins, Updates, Ends) are also order-independent.
	/// Parameters within a filter kind are OR-combined; different kinds are AND-combined.
	/// Call <see cref="Do"/> to finalize and schedule the event sequences.
	/// </summary>
	public readonly struct CollisionBuilder<T> where T : struct, ICollisionBuilderState
	{
		internal readonly Script Script;
		internal readonly CollisionEventOptions Options;
		internal readonly BuilderToken Token;

		internal CollisionBuilder(Script script, CollisionEventOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;
		}

		// ── Filters ───────────────────────────────────────────────────────────

		/// <summary>
		/// Only react when the other object has one of the given tags (OR logic).
		/// Combine with other filter kinds for AND logic across kinds.
		/// </summary>
		public CollisionBuilder<CollisionBuilderReady> Tagged(params String[] tags)
		{
			var options = Options;
			options.Filter.Tags = tags;
			options.Filter.TagPredicate = CollisionPredicates.ForTags(tags);
			return new CollisionBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object's name matches one of the given names (OR logic).
		/// </summary>
		public CollisionBuilder<CollisionBuilderReady> Named(params String[] names)
		{
			var options = Options;
			options.Filter.Names = names;
			options.Filter.NamePredicate = CollisionPredicates.ForNames(names);
			return new CollisionBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object is on one of the given layers (OR logic).
		/// Mutually exclusive with Masked() — Masked() becomes unavailable after calling this.
		/// </summary>
		public CollisionBuilder<CollisionBuilderLayered> Layered(params String[] layerNames)
		{
			var options = Options;
			options.Filter.Layers = layerNames;
			options.Filter.LayerPredicate = CollisionPredicates.ForLayers(layerNames);
			return new CollisionBuilder<CollisionBuilderLayered>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object's component list contains at least one of the given types (OR logic).
		/// </summary>
		public CollisionBuilder<CollisionBuilderReady> Typed(params Type[] componentTypes)
		{
			var options = Options;
			options.Filter.ComponentTypes = componentTypes;
			options.Filter.TypePredicate = CollisionPredicates.ForComponentTypes(componentTypes);
			return new CollisionBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		// ── Event handlers ────────────────────────────────────────────────────

		/// <summary>Blocks to run when the collision/trigger begins.</summary>
		public CollisionBuilder<CollisionBuilderReady> Begins(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.BeginsBlocks = blocks;
			return new CollisionBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>Blocks to run each physics step while the collision/trigger persists.</summary>
		public CollisionBuilder<CollisionBuilderReady> Updates(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.UpdatesBlocks = blocks;
			return new CollisionBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>Blocks to run when the collision/trigger ends.</summary>
		public CollisionBuilder<CollisionBuilderReady> Ends(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.EndsBlocks = blocks;
			return new CollisionBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Minimum seconds between successive reactions. Zero (default) means no cooldown.
		/// The cooldown is checked before collision predicates; evaluated per event sequence.
		/// </summary>
		public CollisionBuilder<CollisionBuilderReady> Cooldown(Double seconds)
		{
			var options = Options;
			options.Cooldown = seconds;
			return new CollisionBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		// ── Finalize ──────────────────────────────────────────────────────────

		/// <summary>Finalizes the builder and schedules all configured event handlers.</summary>
		public void Do() => Finalize(Script, Options, Token);

		private static void Finalize(Script script, in CollisionEventOptions options, BuilderToken token)
		{
			var guards = BuildGuards(options.Cooldown);

			if (options.IsTrigger)
			{
				var predicates = BuildTriggerPredicates(options.Filter);

				if (options.BeginsBlocks != null)
				{
					var block = new TriggerSequenceBlock(options.BeginsBlocks, guards, predicates);
					script.Scheduler?.SchedulePhysicsSequence(block, LunyTriggerEvent.OnTriggerEntered);
				}

				if (options.UpdatesBlocks != null)
				{
					var block = new TriggerSequenceBlock(options.UpdatesBlocks, guards, predicates);
					script.Scheduler?.SchedulePhysicsSequence(block, LunyTriggerEvent.OnTriggerUpdate);
				}

				if (options.EndsBlocks != null)
				{
					var block = new TriggerSequenceBlock(options.EndsBlocks, guards, predicates);
					script.Scheduler?.SchedulePhysicsSequence(block, LunyTriggerEvent.OnTriggerExited);
				}
			}
			else
			{
				var predicates = BuildCollisionPredicates(options.Filter);

				if (options.BeginsBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.BeginsBlocks, guards, predicates);
					script.Scheduler?.SchedulePhysicsSequence(block, LunyCollisionEvent.OnCollisionEntered);
				}

				if (options.UpdatesBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.UpdatesBlocks, guards, predicates);
					script.Scheduler?.SchedulePhysicsSequence(block, LunyCollisionEvent.OnCollisionUpdate);
				}

				if (options.EndsBlocks != null)
				{
					var block = new CollisionSequenceBlock(options.EndsBlocks, guards, predicates);
					script.Scheduler?.SchedulePhysicsSequence(block, LunyCollisionEvent.OnCollisionExited);
				}
			}

			script.FinalizeToken(token);
		}

		private static Func<Boolean>[] BuildGuards(Double cooldownSeconds)
		{
			if (cooldownSeconds <= 0.0)
				return null;

			// Capture the time service once at build time to avoid repeated instance lookups at runtime
			var time = LunyEngine.Instance.Time;
			var state = new CooldownState();

			return new Func<Boolean>[]
			{
				() =>
				{
					var now = time.ElapsedSeconds;
					if (now - state.LastExecutionTime < cooldownSeconds)
						return false;

					state.LastExecutionTime = now;
					return true;
				},
			};
		}

		private static Predicate<LunyCollision>[] BuildCollisionPredicates(in CollisionFilterOptions filter)
		{
			var count = 0;
			if (filter.TagPredicate != null) count++;
			if (filter.NamePredicate != null) count++;
			if (filter.LayerPredicate != null) count++;
			if (filter.TypePredicate != null) count++;

			if (count == 0)
				return null;

			var predicates = new Predicate<LunyCollision>[count];
			var i = 0;
			if (filter.TagPredicate != null) predicates[i++] = filter.TagPredicate;
			if (filter.NamePredicate != null) predicates[i++] = filter.NamePredicate;
			if (filter.LayerPredicate != null) predicates[i++] = filter.LayerPredicate;
			if (filter.TypePredicate != null) predicates[i++] = filter.TypePredicate;
			return predicates;
		}

		private static Predicate<LunyCollider>[] BuildTriggerPredicates(in CollisionFilterOptions filter)
		{
			var count = 0;
			if (filter.Tags != null) count++;
			if (filter.Names != null) count++;
			if (filter.Layers != null || filter.LayerMask.HasValue) count++;
			if (filter.ComponentTypes != null) count++;

			if (count == 0)
				return null;

			var predicates = new Predicate<LunyCollider>[count];
			var i = 0;
			if (filter.Tags != null) predicates[i++] = TriggerPredicates.ForTags(filter.Tags);
			if (filter.Names != null) predicates[i++] = TriggerPredicates.ForNames(filter.Names);
			if (filter.Layers != null)
				predicates[i++] = TriggerPredicates.ForLayers(filter.Layers);
			else if (filter.LayerMask.HasValue)
				predicates[i++] = TriggerPredicates.ForLayerMask(filter.LayerMask.Value);
			if (filter.ComponentTypes != null) predicates[i++] = TriggerPredicates.ForComponentTypes(filter.ComponentTypes);
			return predicates;
		}

		/// <summary>Tiny heap object to hold mutable last-execution time for the cooldown guard closure.</summary>
		private sealed class CooldownState
		{
			public Double LastExecutionTime = Double.MinValue;
		}
	}

	// ── Extension methods (mutual exclusivity for Masked) ─────────────────────

	public static class CollisionBuilderExtensions
	{
		/// <summary>
		/// Only react when the other object's layer is included in the given bitmask.
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static CollisionBuilder<CollisionBuilderMasked> Masked<T>(
			this CollisionBuilder<T> b, Int32 layerMask)
			where T : struct, ICollisionBuilderStart
		{
			var options = b.Options;
			options.Filter.LayerMask = layerMask;
			options.Filter.LayerPredicate = CollisionPredicates.ForLayerMask(layerMask);
			return new CollisionBuilder<CollisionBuilderMasked>(b.Script, options, b.Token);
		}

		/// <summary>
		/// Only react when the other object's layer name matches one of the given names (OR logic).
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static CollisionBuilder<CollisionBuilderMasked> Masked<T>(
			this CollisionBuilder<T> b, params String[] layerNames)
			where T : struct, ICollisionBuilderStart
		{
			var options = b.Options;
			options.Filter.Layers = layerNames;
			options.Filter.LayerPredicate = CollisionPredicates.ForLayers(layerNames);
			return new CollisionBuilder<CollisionBuilderMasked>(b.Script, options, b.Token);
		}
	}
}
