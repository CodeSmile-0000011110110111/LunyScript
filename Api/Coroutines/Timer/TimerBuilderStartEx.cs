using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public static class TimerBuilderStartEx
	{
		/// <summary>Sets the timer to fire once after the specified duration.</summary>
		public static TimerBuilder<TimerAmountSet> In(this TimerBuilder<TimerBuilderStart> b, Double duration)
		{
			var options = b.Options;
			options.Amount = duration;
			options.Continuation = LunyScript.Coroutines.Coroutine.Continuation.Finite;
			return new TimerBuilder<TimerAmountSet>(b.Script, b.Token, in options);
		}

		/// <summary>Sets the timer to fire repeatedly at the specified interval.</summary>
		public static TimerBuilder<TimerAmountSet> Every(this TimerBuilder<TimerBuilderStart> b, Double interval)
		{
			var options = b.Options;
			options.Amount = interval;
			options.Continuation = LunyScript.Coroutines.Coroutine.Continuation.Repeating;
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
			var co = CoroutineOptions.ForTimer(b.Options.Name, b.Options.DurationInSeconds, b.Options.Continuation, LunyScript.Coroutines.Coroutine.Process.FrameUpdate) with { OnElapsed = blocks };
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
				var co = CoroutineOptions.ForTimer(capturedOptions.Name, capturedOptions.DurationInSeconds, capturedOptions.Continuation, LunyScript.Coroutines.Coroutine.Process.FrameUpdate);
				CoroutineBuilder.Finalize(capturedScript, in co, capturedToken);
			});
			return new TimerBuilder<TimerUnitSet>(b.Script, b.Token, in options);
		}
	}
}
