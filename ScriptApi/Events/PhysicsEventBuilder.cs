using Luny;
using Luny.Engine.Bridge.Physics;
using LunyScript.Blocks;
using LunyScript.Blocks.Guards;
using LunyScript.Blocks.PhysicsEvent;
using LunyScript.Exceptions;
using System;

namespace LunyScript
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
	/// Call <see cref="Do"/> to finalize and schedule the event sequences.
	/// </summary>
	public readonly struct PhysicsEventBuilder<T> where T : struct, ICollisionBuilderState
	{
		internal readonly Script Script;
		internal readonly PhysicsEventOptions Options;
		internal readonly BuilderToken Token;

		internal PhysicsEventBuilder(Script script, Boolean isTrigger)
		{
			Script = script;
			Options = new PhysicsEventOptions { IsTrigger = isTrigger };
			Token = script.CreateBuilderToken("Physics Event", isTrigger ? "Trigger" : "Collision");
		}

		internal PhysicsEventBuilder(Script script, PhysicsEventOptions options, BuilderToken token)
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
		public PhysicsEventBuilder<CollisionBuilderReady> Tagged(params String[] tags)
		{
			var options = Options;
			options.Filter.Tags = tags;
			return new PhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object's name matches one of the given names (OR logic).
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderReady> With(params String[] names)
		{
			var options = Options;
			options.Filter.Names = names;
			return new PhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object is on one of the given layers (OR logic).
		/// Mutually exclusive with Masked() — Masked() becomes unavailable after calling this.
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderLayered> Layered(params String[] layerNames)
		{
			var options = Options;
			options.Filter.Layers = layerNames;
			return new PhysicsEventBuilder<CollisionBuilderLayered>(Script, options, Token);
		}

		/// <summary>
		/// Only react when the other object's component list contains at least one of the given types (OR logic).
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Typed(params Type[] componentTypes)
		{
			var options = Options;
			options.Filter.ComponentTypes = componentTypes;
			return new PhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		// ── Event handlers ────────────────────────────────────────────────────

		/// <summary>Blocks to run when the collision/trigger begins.</summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Begins(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.BeginsBlocks = blocks;
			SetAutoFinalizer(Script, Token, options);
			return new PhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>Blocks to run each physics step while the collision/trigger persists.</summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Continues(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.UpdatesBlocks = blocks;
			SetAutoFinalizer(Script, Token, options);
			return new PhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>Blocks to run when the collision/trigger ends.</summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Ends(params ScriptActionBlock[] blocks)
		{
			var options = Options;
			options.EndsBlocks = blocks;
			SetAutoFinalizer(Script, Token, options);
			return new PhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		/// <summary>
		/// Minimum seconds between successive reactions. Zero (default) means no cooldown.
		/// The cooldown is checked before collision predicates; evaluated per event sequence.
		/// </summary>
		public PhysicsEventBuilder<CollisionBuilderReady> Cooldown(Double seconds)
		{
			var options = Options;
			options.Cooldown = Math.Max(0.0, seconds);
			return new PhysicsEventBuilder<CollisionBuilderReady>(Script, options, Token);
		}

		// ── Finalize ──────────────────────────────────────────────────────────
		private void SetAutoFinalizer(Script script, BuilderToken token, PhysicsEventOptions options) =>
			token?.SetAutoFinalizer(() => Finalize(script, token, options));

		internal static void Finalize(Script script, BuilderToken token, in PhysicsEventOptions options)
		{
			if (options.BeginsBlocks == null && options.UpdatesBlocks == null && options.EndsBlocks == null)
				throw new LunyScriptException($"{script}: Physics Event without any blocks");

			var guards = BuildGuards(options.Cooldown);
			var predicates = BuildPredicates(options.Filter);

			if (options.IsTrigger)
			{
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

		private static EventGuard[] BuildGuards(Double cooldownInSeconds)
		{
			if (cooldownInSeconds <= 0.0)
				return null;

			return new EventGuard[] { new CooldownGuard<T>(cooldownInSeconds, LunyEngine.Instance.Time) };
		}

		private static Predicate<LunyCollider>[] BuildPredicates(in PhysicsEventFilterOptions filter)
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
				predicates[i++] = PhysicsEventPredicates.ForNames(filter.Names);
			if (filter.Tags != null)
				predicates[i++] = PhysicsEventPredicates.ForTags(filter.Tags);
			if (filter.Layers != null)
				predicates[i++] = PhysicsEventPredicates.ForLayers(filter.Layers);
			else if (filter.LayerMask.HasValue)
				predicates[i++] = PhysicsEventPredicates.ForLayerMask(filter.LayerMask.Value);
			if (filter.ComponentTypes != null)
				predicates[i++] = PhysicsEventPredicates.ForComponentTypes(filter.ComponentTypes);
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
		public static PhysicsEventBuilder<CollisionBuilderMasked> Masked<T>(
			this PhysicsEventBuilder<T> b, params String[] layerNames)
			where T : struct, ICollisionBuilderStart
		{
			var options = b.Options;
			options.Filter.Layers = layerNames;
			return new PhysicsEventBuilder<CollisionBuilderMasked>(b.Script, options, b.Token);
		}

		/// <summary>
		/// Only react when the other object's layer is included in the given bitmask.
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static PhysicsEventBuilder<CollisionBuilderMasked> Masked<T>(
			this PhysicsEventBuilder<T> b, Int32 layerMask)
			where T : struct, ICollisionBuilderStart
		{
			var options = b.Options;
			options.Filter.LayerMask = layerMask;
			return new PhysicsEventBuilder<CollisionBuilderMasked>(b.Script, options, b.Token);
		}
	}

	/// <summary>
	/// Options DTO for collision/trigger event builders.
	/// Holds filter options, event handler blocks, and optional cooldown.
	/// </summary>
	internal struct PhysicsEventOptions
	{
		public PhysicsEventFilterOptions Filter;
		public ScriptActionBlock[] BeginsBlocks;
		public ScriptActionBlock[] UpdatesBlocks;
		public ScriptActionBlock[] EndsBlocks;
		public Boolean IsTrigger;
		/// <summary>Minimum seconds between successive reactions. Zero means no cooldown.</summary>
		public Double Cooldown;
	}

	/// <summary>
	/// Immutable filter options for collision/trigger event builders.
	/// Each predicate is compiled once when the corresponding filter method is called.
	/// Null predicate means that filter kind is inactive (no check performed).
	/// Parameters within a kind are OR-combined; different kinds are AND-combined.
	/// </summary>
	internal struct PhysicsEventFilterOptions
	{
		// Raw filter data (kept for diagnostics and for re-compiling trigger predicates from collision filter)
		public String[] Tags;
		public String[] Names;
		public String[] Layers;
		public Int32? LayerMask;
		public Type[] ComponentTypes;
	}
}
