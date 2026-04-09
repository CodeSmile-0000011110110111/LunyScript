using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	// ── Collision Builder States ─────────────────────────────────────────────
	public interface ICollisionBuilderState {}

	/// <summary>Marker: filter methods (Tagged, With, Layered, Masked, Typed, Cooldown) are still available.</summary>
	public interface ICollisionFilterable : ICollisionBuilderState {}

	public struct CollisionStart : ICollisionFilterable {}
	public struct CollisionReady : ICollisionFilterable {}
	public struct CollisionLayered : ICollisionFilterable {}
	public struct CollisionMasked : ICollisionFilterable {}

	/// <summary>State after at least one event handler is set — filters are no longer available.</summary>
	public struct CollisionEventSet : ICollisionBuilderState {}

	/// <summary>
	/// Fluent builder for filtered collision event sequences.
	/// Filters (Tagged, Named, Layered, Masked, Typed) are order-independent and accumulate.
	/// Event handlers (Started, Continuing, Ended) are also order-independent.
	/// Parameters within a filter kind are OR-combined; different kinds are AND-combined.
	/// </summary>
	public readonly struct CollisionEventBuilder<T> where T : struct, ICollisionBuilderState
	{
		internal readonly PhysicsEventOptions Options;

		internal CollisionEventBuilder(Script script, StackTrace trace)
		{
			var token = script.CreateBuilderToken("Physics Event", "Collision");
			Options = new PhysicsEventOptions { Script = script, Token = token, Trace = trace };
		}

		internal CollisionEventBuilder(in PhysicsEventOptions options) => Options = options;
	}

	// ── Collision Filter Extensions ──────────────────────────────────────────
	public static class CollisionEventFilterExtensions
	{
		/// <summary>
		/// Only react when the other object has one of the given tags (OR logic).
		/// Combine with other filter kinds for AND logic across kinds.
		/// </summary>
		public static CollisionEventBuilder<CollisionReady> Tagged<T>(this CollisionEventBuilder<T> b, params String[] tags)
			where T : struct, ICollisionFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.TagsFilter);
			return new CollisionEventBuilder<CollisionReady>(b.Options with { TagsFilter = tags });
		}

		/// <summary>
		/// Only react when the other object's name matches one of the given names (OR logic).
		/// </summary>
		public static CollisionEventBuilder<CollisionReady> With<T>(this CollisionEventBuilder<T> b, params String[] names)
			where T : struct, ICollisionFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.NameFilter);
			return new CollisionEventBuilder<CollisionReady>(b.Options with { NameFilter = names });
		}

		/// <summary>
		/// Only react when the other object is on one of the given layers (OR logic).
		/// Mutually exclusive with Masked() — Masked() becomes unavailable after calling this.
		/// </summary>
		public static CollisionEventBuilder<CollisionLayered> Layered<T>(this CollisionEventBuilder<T> b, params String[] layerNames)
			where T : struct, ICollisionFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.LayerNameFilter);
			return new CollisionEventBuilder<CollisionLayered>(b.Options with { LayerNameFilter = layerNames });
		}

		/// <summary>
		/// Only react when the other object's component list contains at least one of the given types (OR logic).
		/// </summary>
		public static CollisionEventBuilder<CollisionReady> Typed<T>(this CollisionEventBuilder<T> b, params Type[] componentTypes)
			where T : struct, ICollisionFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.ComponentTypeFilter);
			return new CollisionEventBuilder<CollisionReady>(b.Options with { ComponentTypeFilter = componentTypes });
		}

		/// <summary>
		/// Minimum seconds between successive reactions. Zero (default) means no cooldown.
		/// The cooldown is checked before collision predicates; evaluated per event sequence.
		/// </summary>
		public static CollisionEventBuilder<CollisionReady> Cooldown<T>(this CollisionEventBuilder<T> b, Double seconds)
			where T : struct, ICollisionFilterable => new(b.Options with { Cooldown = Math.Max(0.0, seconds) });

		/// <summary>
		/// Only react when the other object's layer name matches one of the given names (OR logic).
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static CollisionEventBuilder<CollisionMasked> Masked<T>(this CollisionEventBuilder<T> b, params String[] layerNames)
			where T : struct, ICollisionFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.LayerNameFilter);
			var options = b.Options with { LayerNameFilter = layerNames };
			return new CollisionEventBuilder<CollisionMasked>(options);
		}

		/// <summary>
		/// Only react when the other object's layer is included in the given bitmask.
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static CollisionEventBuilder<CollisionMasked> Masked<T>(this CollisionEventBuilder<T> b, Int32 layerMask)
			where T : struct, ICollisionFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.LayerMaskFilter);
			var options = b.Options with { LayerMaskFilter = layerMask };
			return new CollisionEventBuilder<CollisionMasked>(options);
		}
	}

	// ── Collision Event Extensions ───────────────────────────────────────────
	public static class CollisionEventHandlerExtensions
	{
		/// <summary>Blocks to run when the collision began.</summary>
		public static CollisionEventBuilder<CollisionEventSet> Started<T>(this CollisionEventBuilder<T> b, params ActionBlock[] blocks)
			where T : struct, ICollisionBuilderState
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.StartedBlocks);
			var options = b.Options with { StartedBlocks = blocks };
			options.Token.AutoFinish = () => CollisionEventBuilderHelper.Finish(options.Script, options.Token, options);
			return new CollisionEventBuilder<CollisionEventSet>(options);
		}

		/// <summary>Blocks to run each physics step while the collision persists.</summary>
		public static CollisionEventBuilder<CollisionEventSet> Touching<T>(this CollisionEventBuilder<T> b, params ActionBlock[] blocks)
			where T : struct, ICollisionBuilderState
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.ContinuingBlocks);
			var options = b.Options with { ContinuingBlocks = blocks };
			options.Token.AutoFinish = () => CollisionEventBuilderHelper.Finish(options.Script, options.Token, options);
			return new CollisionEventBuilder<CollisionEventSet>(options);
		}

		/// <summary>Blocks to run when the collision ended.</summary>
		public static CollisionEventBuilder<CollisionEventSet> Ended<T>(this CollisionEventBuilder<T> b, params ActionBlock[] blocks)
			where T : struct, ICollisionBuilderState
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.EndedBlocks);
			var options = b.Options with { EndedBlocks = blocks };
			options.Token.AutoFinish = () => CollisionEventBuilderHelper.Finish(options.Script, options.Token, options);
			return new CollisionEventBuilder<CollisionEventSet>(options);
		}
	}

	public static class CollisionEventBuilderHelper
	{
		internal static void Finish(Script script, BuilderToken token, in PhysicsEventOptions options)
		{
			if (options.StartedBlocks == null && options.ContinuingBlocks == null && options.EndedBlocks == null)
				throw new LunyScriptException($"{script}: Collision Event without any blocks");

			var guards = PhysicsEventBuilderHelper.BuildGuards(options.Cooldown);
			var predicates = PhysicsEventBuilderHelper.BuildPredicates(options);

			if (options.StartedBlocks != null)
			{
				var block = new CollisionSequenceBlock(options.StartedBlocks, guards, predicates, options.Trace);
				script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionStarted);
			}

			if (options.ContinuingBlocks != null)
			{
				var block = new CollisionSequenceBlock(options.ContinuingBlocks, guards, predicates, options.Trace);
				script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionTouching);
			}

			if (options.EndedBlocks != null)
			{
				var block = new CollisionSequenceBlock(options.EndedBlocks, guards, predicates, options.Trace);
				script.Scheduler?.ScheduleCollisionEventSequence(block, LunyCollisionEvent.OnCollisionEnded);
			}

			script.MarkBuilderTokenFinished(token);
		}
	}
}
