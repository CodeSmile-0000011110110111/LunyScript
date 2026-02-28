using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface IEveryBuilder {}
	public interface IEveryBuilderStart : IEveryBuilder {}
	public struct EveryBuilderStart : IEveryBuilderStart {}

	/// <summary>
	/// Fluent builder for time-sliced coroutines.
	/// Usage: Every(3).Frames().Do(blocks);
	///        Every(Even).Heartbeats().DelayBy(1).Do(blocks);
	/// </summary>
	public readonly struct EveryBuilder<T> where T : struct, IEveryBuilder
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly EveryOptions Options;

		internal EveryBuilder(Script script, BuilderToken token, in EveryOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		/// <summary>Entry-point factory. Creates the builder token.</summary>
		internal static EveryBuilder<EveryBuilderStart> Create(Script script, Int32 interval)
		{
			if (script == null)
				throw new ArgumentNullException(nameof(script));

			if (interval < 2)
			{
				throw new ArgumentException($"Every({interval}): interval must be at least 2. Interval 1 or 0 would run every " +
				                            "Heartbeat/Update regardless of DelayBy. Use a Counter() if this is intentional.");
			}

			var options = new EveryOptions { Interval = interval };
			var token = script.CreateBuilderToken("<N/A>", "Every()");
			return new EveryBuilder<EveryBuilderStart>(script, token, in options);
		}
	}

	public interface IEveryUnitSet : IEveryBuilder {}
	public struct EveryUnitSet : IEveryUnitSet {}

	public static class EveryBuilderUnitExtensions
	{
		/// <summary>Selects frame-based execution.</summary>
		public static EveryBuilder<EveryUnitSet> Frames(this EveryBuilder<EveryBuilderStart> b)
		{
			var options = b.Options;
			options.Process = Coroutine.Process.FrameUpdate;
			return new EveryBuilder<EveryUnitSet>(b.Script, b.Token, in options);
		}

		/// <summary>Selects heartbeat-based execution.</summary>
		public static EveryBuilder<EveryUnitSet> Heartbeats(this EveryBuilder<EveryBuilderStart> b)
		{
			var options = b.Options;
			options.Process = Coroutine.Process.Heartbeat;
			return new EveryBuilder<EveryUnitSet>(b.Script, b.Token, in options);
		}
	}

	public interface IEveryOffsetSet : IEveryUnitSet {}
	public struct EveryOffsetSet : IEveryOffsetSet {}

	public static class EveryBuilderOffsetExtensions
	{
		/// <summary>Sets the phase offset (delay) for time-sliced execution.</summary>
		public static EveryBuilder<EveryOffsetSet> Offset<T>(this EveryBuilder<T> b, Int32 offset)
			where T : struct, IEveryUnitSet
		{
			if (b.Options.Offset != 0)
				throw new ArgumentException("DelayBy() can't be used twice");

			var options = b.Options;
			options.Offset = offset;
			return new EveryBuilder<EveryOffsetSet>(b.Script, b.Token, in options);
		}
	}

	public static class EveryBuilderFinalExtensions
	{
		/// <summary>Completes the builder and specifies blocks to run at each interval.</summary>
		public static ICoroutineBlock Do<T>(this EveryBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IEveryUnitSet
		{
			var opt = b.Options;
			var co = CoroutineOptions.ForIntervalCoroutine(null, opt.Interval, opt.Offset, opt.Process, blocks);
			return CoroutineBuilder.Finalize(b.Script, in co, b.Token);
		}
	}

	internal struct EveryOptions
	{
		internal Int32 Interval;
		internal Int32 Offset;
		internal Coroutine.Process Process;
	}
}
