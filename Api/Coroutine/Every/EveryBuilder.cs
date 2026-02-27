using LunyScript.Blocks;
using System;

namespace LunyScript.Api.Coroutine.Every
{
	/// <summary>
	/// Fluent builder for time-sliced coroutines.
	/// Usage: Every(3).Frames().Do(blocks);
	///        Every(Even).Heartbeats().DelayBy(1).Do(blocks);
	/// </summary>
	public readonly struct EveryBuilder<T> where T : struct, IEveryBuilderState
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
			if (script == null) throw new ArgumentNullException(nameof(script));
			var options = new EveryOptions { Interval = interval };
			var token = script.CreateBuilderToken($"Every({interval})", "EveryBuilder");
			return new EveryBuilder<EveryBuilderStart>(script, token, in options);
		}
	}

	public static class EveryBuilderExtensions
	{
		/// <summary>Selects frame-based execution.</summary>
		public static EveryBuilder<EveryUnitSet> Frames<T>(this EveryBuilder<T> b)
			where T : struct, IEveryBuilderStart
		{
			var options = b.Options;
			options.Process = Coroutines.Coroutine.Process.FrameUpdate;
			return RegisterAutoFinalizer(b, options);
		}

		/// <summary>Selects heartbeat-based execution.</summary>
		public static EveryBuilder<EveryUnitSet> Heartbeats<T>(this EveryBuilder<T> b)
			where T : struct, IEveryBuilderStart
		{
			var options = b.Options;
			options.Process = Coroutines.Coroutine.Process.Heartbeat;
			return RegisterAutoFinalizer(b, options);
		}

		/// <summary>Sets the phase offset (delay) for time-sliced execution.</summary>
		public static EveryBuilder<EveryUnitSet> DelayBy<T>(this EveryBuilder<T> b, Int32 delay)
			where T : struct, IEveryUnitSet
		{
			if (b.Options.Delay != 0)
				throw new ArgumentException($"{nameof(DelayBy)}() can't be used twice");
			var options = b.Options;
			options.Delay = delay;
			return RegisterAutoFinalizer(b, options);
		}

		/// <summary>Completes the builder and specifies blocks to run at each interval.</summary>
		public static ICounterCoroutineBlock Do<T>(this EveryBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IEveryUnitSet
		{
			var co = CoroutineOptions.ForEveryInterval(null, b.Options.Interval, b.Options.Delay, b.Options.Process, blocks);
			return (ICounterCoroutineBlock)CoroutineBuilder.Finalize(b.Script, in co, b.Token);
		}

		private static EveryBuilder<EveryUnitSet> RegisterAutoFinalizer<T>(EveryBuilder<T> b, EveryOptions options)
			where T : struct, IEveryBuilderState
		{
			if (options.Interval < 0)
				throw new ArgumentException($"Every duration must be 0 or greater, got: {options.Interval}");
			var capturedScript = b.Script;
			var capturedOptions = options;
			var capturedToken = b.Token;
			b.Token?.SetAutoFinalizer(() =>
			{
				var co = CoroutineOptions.ForEveryInterval(null, capturedOptions.Interval, capturedOptions.Delay, capturedOptions.Process, null);
				CoroutineBuilder.Finalize(capturedScript, in co, capturedToken);
			});
			return new EveryBuilder<EveryUnitSet>(b.Script, b.Token, in options);
		}
	}
}
