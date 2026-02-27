// Entry-step extension methods for generic coroutine builders.
// These live in the LunyScript namespace so scripts deriving from Script can
// chain them without needing an explicit 'using' directive for the builder namespaces.

using LunyScript.Api.Coroutine.Counter;
using LunyScript.Api.Coroutine.Every;
using LunyScript.Api.Coroutine.Timer;
using LunyScript.Blocks;
using System;

namespace LunyScript.Api.Coroutine
{
	// ── Counter ─────────────────────────────────────────────────────────────

	public static class CounterBuilderStartEx
	{
		/// <summary>Sets the counter to fire once after the specified count.</summary>
		public static CounterBuilder<CounterAmountSet> In(this CounterBuilder<CounterBuilderStart> b, Int32 targetCount)
		{
			var options = b.Options;
			options.Amount = targetCount;
			options.Continuation = Coroutines.Coroutine.Continuation.Finite;
			return new CounterBuilder<CounterAmountSet>(b.Script, b.Token, in options);
		}

		/// <summary>Sets the counter to fire repeatedly at the specified interval.</summary>
		public static CounterBuilder<CounterAmountSet> Every(this CounterBuilder<CounterBuilderStart> b, Int32 interval)
		{
			var options = b.Options;
			options.Amount = interval;
			options.Continuation = Coroutines.Coroutine.Continuation.Repeating;
			return new CounterBuilder<CounterAmountSet>(b.Script, b.Token, in options);
		}

		/// <summary>Counts frame updates.</summary>
		public static CounterBuilder<CounterUnitSet> Frames<T>(this CounterBuilder<T> b)
			where T : struct, ICounterAmountSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Counter duration must be 0 or greater, got: {b.Options.Amount}");
			var options = b.Options;
			options.Process = Coroutines.Coroutine.Process.FrameUpdate;
			var capturedScript = b.Script;
			var capturedOptions = options;
			b.Token?.SetAutoFinalizer(() =>
			{
				var co = CoroutineOptions.ForCounter(capturedOptions.Name, capturedOptions.Amount, capturedOptions.Continuation, capturedOptions.Process);
				CoroutineBuilder.Finalize(capturedScript, in co, b.Token);
			});
			return new CounterBuilder<CounterUnitSet>(b.Script, b.Token, in options);
		}

		/// <summary>Counts heartbeat (fixed step) updates.</summary>
		public static CounterBuilder<CounterUnitSet> Heartbeats<T>(this CounterBuilder<T> b)
			where T : struct, ICounterAmountSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Counter duration must be 0 or greater, got: {b.Options.Amount}");
			var options = b.Options;
			options.Process = Coroutines.Coroutine.Process.Heartbeat;
			var capturedScript = b.Script;
			var capturedOptions = options;
			b.Token?.SetAutoFinalizer(() =>
			{
				var co = CoroutineOptions.ForCounter(capturedOptions.Name, capturedOptions.Amount, capturedOptions.Continuation, capturedOptions.Process);
				CoroutineBuilder.Finalize(capturedScript, in co, b.Token);
			});
			return new CounterBuilder<CounterUnitSet>(b.Script, b.Token, in options);
		}

		/// <summary>Completes the counter and specifies blocks to run when elapsed.</summary>
		public static ICounterCoroutineBlock Do<T>(this CounterBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICounterUnitSet
		{
			var co = CoroutineOptions.ForCounter(b.Options.Name, b.Options.Amount, b.Options.Continuation, b.Options.Process) with { OnElapsed = blocks };
			return (ICounterCoroutineBlock)CoroutineBuilder.Finalize(b.Script, in co, b.Token);
		}
	}

	// ── Timer ────────────────────────────────────────────────────────────────

	public static class TimerBuilderStartEx
	{
		/// <summary>Sets the timer to fire once after the specified duration.</summary>
		public static TimerBuilder<TimerAmountSet> In(this TimerBuilder<TimerBuilderStart> b, Double duration)
		{
			var options = b.Options;
			options.Amount = duration;
			options.Continuation = Coroutines.Coroutine.Continuation.Finite;
			return new TimerBuilder<TimerAmountSet>(b.Script, b.Token, in options);
		}

		/// <summary>Sets the timer to fire repeatedly at the specified interval.</summary>
		public static TimerBuilder<TimerAmountSet> Every(this TimerBuilder<TimerBuilderStart> b, Double interval)
		{
			var options = b.Options;
			options.Amount = interval;
			options.Continuation = Coroutines.Coroutine.Continuation.Repeating;
			return new TimerBuilder<TimerAmountSet>(b.Script, b.Token, in options);
		}

		/// <summary>Duration in seconds.</summary>
		public static TimerBuilder<TimerUnitSet> Seconds<T>(this TimerBuilder<T> b)
			where T : struct, ITimerAmountSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {b.Options.Amount}");
			return CreateFinalStep(b, b.Options.Amount);
		}

		/// <summary>Duration in milliseconds.</summary>
		public static TimerBuilder<TimerUnitSet> Milliseconds<T>(this TimerBuilder<T> b)
			where T : struct, ITimerAmountSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {b.Options.Amount}");
			return CreateFinalStep(b, b.Options.Amount / 1000.0);
		}

		/// <summary>Duration in minutes.</summary>
		public static TimerBuilder<TimerUnitSet> Minutes<T>(this TimerBuilder<T> b)
			where T : struct, ITimerAmountSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {b.Options.Amount}");
			return CreateFinalStep(b, b.Options.Amount * 60.0);
		}

		/// <summary>Completes the timer and specifies blocks to run when elapsed.</summary>
		public static ITimerCoroutineBlock Do<T>(this TimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ITimerUnitSet
		{
			var co = CoroutineOptions.ForTimer(b.Options.Name, b.Options.DurationInSeconds, b.Options.Continuation, Coroutines.Coroutine.Process.FrameUpdate) with { OnElapsed = blocks };
			return (ITimerCoroutineBlock)CoroutineBuilder.Finalize(b.Script, in co, b.Token);
		}

		private static TimerBuilder<TimerUnitSet> CreateFinalStep<T>(TimerBuilder<T> b, Double durationInSeconds)
			where T : struct, ITimerBuilderState
		{
			var options = b.Options;
			options.DurationInSeconds = durationInSeconds;
			var capturedScript = b.Script;
			var capturedOptions = options;
			var capturedToken = b.Token;
			b.Token?.SetAutoFinalizer(() =>
			{
				var co = CoroutineOptions.ForTimer(capturedOptions.Name, capturedOptions.DurationInSeconds, capturedOptions.Continuation, Coroutines.Coroutine.Process.FrameUpdate);
				CoroutineBuilder.Finalize(capturedScript, in co, capturedToken);
			});
			return new TimerBuilder<TimerUnitSet>(b.Script, b.Token, in options);
		}
	}

	// ── Every ────────────────────────────────────────────────────────────────

	public static class EveryBuilderStartEx
	{
		/// <summary>Selects frame-based execution.</summary>
		public static EveryBuilder<EveryUnitSet> Frames(this EveryBuilder<EveryBuilderStart> b)
		{
			var options = b.Options;
			options.Process = Coroutines.Coroutine.Process.FrameUpdate;
			return RegisterAutoFinalizer(b, options);
		}

		/// <summary>Selects heartbeat-based execution.</summary>
		public static EveryBuilder<EveryUnitSet> Heartbeats(this EveryBuilder<EveryBuilderStart> b)
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
				throw new ArgumentException($"DelayBy() can't be used twice");
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
