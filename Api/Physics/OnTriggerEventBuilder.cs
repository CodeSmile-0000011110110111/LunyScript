using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Blocks.PhysicsEvent;
using System;

namespace LunyScript.Api
{
	// ── Trigger Builder States ───────────────────────────────────────────────
	public interface ITriggerBuilderState {}
	/// <summary>Marker: filter methods (Tagged, With, Layered, Masked, Typed, Cooldown) are still available.</summary>
	public interface ITriggerFilterable : ITriggerBuilderState {}
	public struct TriggerStart   : ITriggerFilterable {}
	public struct TriggerReady   : ITriggerFilterable {}
	public struct TriggerLayered : ITriggerFilterable {}
	public struct TriggerMasked  : ITriggerFilterable {}
	/// <summary>State after at least one event handler is set — filters are no longer available.</summary>
	public struct TriggerEventSet : ITriggerBuilderState {}

	/// <summary>
	/// Fluent builder for filtered trigger event sequences.
	/// Filters (Tagged, Named, Layered, Masked, Typed) are order-independent and accumulate.
	/// Event handlers (Entered, Staying, Exited) are also order-independent.
	/// Parameters within a filter kind are OR-combined; different kinds are AND-combined.
	/// </summary>
	public readonly struct TriggerEventBuilder<T> where T : struct, ITriggerBuilderState
	{
		internal readonly PhysicsEventOptions Options;

		internal TriggerEventBuilder(Script script)
		{
			var token = script.CreateBuilderToken("Physics Event", "Trigger");
			Options = new PhysicsEventOptions { Script = script, Token = token };
		}

		internal TriggerEventBuilder(in PhysicsEventOptions options) => Options = options;
	}

	// ── Trigger Filter Extensions ────────────────────────────────────────────
	public static class TriggerEventFilterExtensions
	{
		/// <summary>
		/// Only react when the other object has one of the given tags (OR logic).
		/// Combine with other filter kinds for AND logic across kinds.
		/// </summary>
		public static TriggerEventBuilder<TriggerReady> Tagged<T>(this TriggerEventBuilder<T> b, params String[] tags)
			where T : struct, ITriggerFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.TagsFilter);
			return new TriggerEventBuilder<TriggerReady>(b.Options with { TagsFilter = tags });
		}

		/// <summary>
		/// Only react when the other object's name matches one of the given names (OR logic).
		/// </summary>
		public static TriggerEventBuilder<TriggerReady> With<T>(this TriggerEventBuilder<T> b, params String[] names)
			where T : struct, ITriggerFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.NameFilter);
			return new TriggerEventBuilder<TriggerReady>(b.Options with { NameFilter = names });
		}

		/// <summary>
		/// Only react when the other object is on one of the given layers (OR logic).
		/// Mutually exclusive with Masked() — Masked() becomes unavailable after calling this.
		/// </summary>
		public static TriggerEventBuilder<TriggerLayered> Layered<T>(this TriggerEventBuilder<T> b, params String[] layerNames)
			where T : struct, ITriggerFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.LayerNameFilter);
			return new TriggerEventBuilder<TriggerLayered>(b.Options with { LayerNameFilter = layerNames });
		}

		/// <summary>
		/// Only react when the other object's component list contains at least one of the given types (OR logic).
		/// </summary>
		public static TriggerEventBuilder<TriggerReady> Typed<T>(this TriggerEventBuilder<T> b, params Type[] componentTypes)
			where T : struct, ITriggerFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.ComponentTypeFilter);
			return new TriggerEventBuilder<TriggerReady>(b.Options with { ComponentTypeFilter = componentTypes });
		}

		/// <summary>
		/// Minimum seconds between successive reactions. Zero (default) means no cooldown.
		/// The cooldown is checked before trigger predicates; evaluated per event sequence.
		/// </summary>
		public static TriggerEventBuilder<TriggerReady> Cooldown<T>(this TriggerEventBuilder<T> b, Double seconds)
			where T : struct, ITriggerFilterable => new(b.Options with { Cooldown = Math.Max(0.0, seconds) });

		/// <summary>
		/// Only react when the other object's layer name matches one of the given names (OR logic).
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static TriggerEventBuilder<TriggerMasked> Masked<T>(this TriggerEventBuilder<T> b, params String[] layerNames)
			where T : struct, ITriggerFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.LayerNameFilter);
			var options = b.Options with { LayerNameFilter = layerNames };
			return new TriggerEventBuilder<TriggerMasked>(options);
		}

		/// <summary>
		/// Only react when the other object's layer is included in the given bitmask.
		/// Mutually exclusive with Layered() — unavailable after Layered() is called.
		/// </summary>
		public static TriggerEventBuilder<TriggerMasked> Masked<T>(this TriggerEventBuilder<T> b, Int32 layerMask)
			where T : struct, ITriggerFilterable
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.LayerMaskFilter);
			var options = b.Options with { LayerMaskFilter = layerMask };
			return new TriggerEventBuilder<TriggerMasked>(options);
		}
	}

	// ── Trigger Event Extensions ─────────────────────────────────────────────
	public static class TriggerEventHandlerExtensions
	{
		/// <summary>Blocks to run when the object entered a trigger collider.</summary>
		public static TriggerEventBuilder<TriggerEventSet> Entered<T>(this TriggerEventBuilder<T> b, params ActionBlock[] blocks)
			where T : struct, ITriggerBuilderState
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.StartedBlocks);
			var options = b.Options with { StartedBlocks = blocks };
			options.Token.AutoFinish = () => TriggerEventBuilderHelper.Finish(options.Script, options.Token, options);
			return new TriggerEventBuilder<TriggerEventSet>(options);
		}

		/// <summary>Blocks to run each physics step while overlapping a trigger collider.</summary>
		public static TriggerEventBuilder<TriggerEventSet> Overlapping<T>(this TriggerEventBuilder<T> b, params ActionBlock[] blocks)
			where T : struct, ITriggerBuilderState
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.ContinuingBlocks);
			var options = b.Options with { ContinuingBlocks = blocks };
			options.Token.AutoFinish = () => TriggerEventBuilderHelper.Finish(options.Script, options.Token, options);
			return new TriggerEventBuilder<TriggerEventSet>(options);
		}

		/// <summary>Blocks to run when the object exited a trigger collider.</summary>
		public static TriggerEventBuilder<TriggerEventSet> Exited<T>(this TriggerEventBuilder<T> b, params ActionBlock[] blocks)
			where T : struct, ITriggerBuilderState
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.EndedBlocks);
			var options = b.Options with { EndedBlocks = blocks };
			options.Token.AutoFinish = () => TriggerEventBuilderHelper.Finish(options.Script, options.Token, options);
			return new TriggerEventBuilder<TriggerEventSet>(options);
		}
	}

	internal static class TriggerEventBuilderHelper
	{
		internal static void Finish(Script script, BuilderToken token, in PhysicsEventOptions options)
		{
			if (options.StartedBlocks == null && options.ContinuingBlocks == null && options.EndedBlocks == null)
				throw new LunyScriptException($"{script}: Trigger Event without any blocks");

			var guards = PhysicsEventBuilderHelper.BuildGuards(options.Cooldown);
			var predicates = PhysicsEventBuilderHelper.BuildPredicates(options);

			if (options.StartedBlocks != null)
			{
				var block = new TriggerSequenceBlock(options.StartedBlocks, guards, predicates);
				script.Scheduler?.ScheduleTriggerEventSequence(block, LunyTriggerEvent.OnTriggerEntered);
			}

			if (options.ContinuingBlocks != null)
			{
				var block = new TriggerSequenceBlock(options.ContinuingBlocks, guards, predicates);
				script.Scheduler?.ScheduleTriggerEventSequence(block, LunyTriggerEvent.OnTriggerUpdate);
			}

			if (options.EndedBlocks != null)
			{
				var block = new TriggerSequenceBlock(options.EndedBlocks, guards, predicates);
				script.Scheduler?.ScheduleTriggerEventSequence(block, LunyTriggerEvent.OnTriggerExited);
			}

			script.MarkBuilderTokenFinished(token);
		}
	}
}
