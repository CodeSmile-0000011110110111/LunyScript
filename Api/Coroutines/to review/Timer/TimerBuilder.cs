using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ITimerBuilderState {}

	/// <summary>Initial state — next: call <c>In()</c> or <c>Every()</c>.</summary>
	public interface ITimerBuilderStart : ITimerBuilderState {}

	/// <summary>Amount set — next: call <c>Seconds()</c>, <c>Milliseconds()</c>, or <c>Minutes()</c>.</summary>
	public interface ITimerAmountSet : ITimerBuilderState {}

	/// <summary>Unit chosen — ready to finalize via <c>Do()</c>.</summary>
	public interface ITimerUnitSet : ITimerBuilderState {}

	public struct TimerBuilderStart : ITimerBuilderStart {}
	public struct TimerAmountSet : ITimerAmountSet {}
	public struct TimerUnitSet : ITimerUnitSet {}

	/// <summary>
	/// Fluent builder for timer coroutines.
	/// Usage: Timer("name").In(3).Seconds().Do(blocks);
	///        Timer("name").Every(1.5).Minutes().Do(blocks);
	/// </summary>
	public readonly struct TimerBuilder<T> where T : struct, ITimerBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly TimerOptions Options;

		internal TimerBuilder(Script script, BuilderToken token, in TimerOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		/// <summary>Entry-point factory. Creates the builder token.</summary>
		internal static TimerBuilder<TimerBuilderStart> Create(Script script, String name)
		{
			if (script == null)
				throw new ArgumentNullException(nameof(script));
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Timer name is null or empty", nameof(name));

			var options = new TimerOptions { Name = name };
			var token = script.CreateBuilderToken(name, "Timer()");
			return new TimerBuilder<TimerBuilderStart>(script, token, in options);
		}
	}

	public static class TimerBuilderExtensions
	{
		/// <summary>Sets the timer to fire once after the specified duration.</summary>
		public static TimerBuilder<TimerAmountSet> In<T>(this TimerBuilder<T> b, Double duration)
			where T : struct, ITimerBuilderStart
		{
			var options = b.Options;
			options.Amount = duration;
			options.Continuation = Coroutine.Continuation.Finite;
			return new TimerBuilder<TimerAmountSet>(b.Script, b.Token, in options);
		}

		/// <summary>Sets the timer to fire repeatedly at the specified interval.</summary>
		public static TimerBuilder<TimerAmountSet> Every<T>(this TimerBuilder<T> b, Double interval)
			where T : struct, ITimerBuilderStart
		{
			var options = b.Options;
			options.Amount = interval;
			options.Continuation = Coroutine.Continuation.Repeating;
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
			var co = CoroutineOptions.ForTimerCoroutine(b.Options.Name, b.Options.DurationInSeconds, b.Options.Continuation,
					Coroutine.Process.FrameUpdate) with
				{
					OnElapsed = blocks,
				};
			return (ITimerCoroutineBlock)CoroutineBuilder.Finalize(b.Script, b.Token, in co);
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
				var co = CoroutineOptions.ForTimerCoroutine(capturedOptions.Name, capturedOptions.DurationInSeconds,
					capturedOptions.Continuation, Coroutine.Process.FrameUpdate);
				CoroutineBuilder.Finalize(capturedScript, capturedToken, in co);
			});
			return new TimerBuilder<TimerUnitSet>(b.Script, b.Token, in options);
		}
	}

	public static class TimerBuilderStartEx
	{
		/// <summary>Sets the timer to fire once after the specified duration.</summary>
		public static TimerBuilder<TimerAmountSet> In(this TimerBuilder<TimerBuilderStart> b, Double duration)
		{
			var options = b.Options;
			options.Amount = duration;
			options.Continuation = Coroutine.Continuation.Finite;
			return new TimerBuilder<TimerAmountSet>(b.Script, b.Token, in options);
		}

		/// <summary>Sets the timer to fire repeatedly at the specified interval.</summary>
		public static TimerBuilder<TimerAmountSet> Every(this TimerBuilder<TimerBuilderStart> b, Double interval)
		{
			var options = b.Options;
			options.Amount = interval;
			options.Continuation = Coroutine.Continuation.Repeating;
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
			var co = CoroutineOptions.ForTimerCoroutine(b.Options.Name, b.Options.DurationInSeconds, b.Options.Continuation,
					Coroutine.Process.FrameUpdate) with
				{
					OnElapsed = blocks,
				};
			return (ITimerCoroutineBlock)CoroutineBuilder.Finalize(b.Script, b.Token, in co);
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
				var co = CoroutineOptions.ForTimerCoroutine(capturedOptions.Name, capturedOptions.DurationInSeconds,
					capturedOptions.Continuation, Coroutine.Process.FrameUpdate);
				CoroutineBuilder.Finalize(capturedScript, capturedToken, in co);
			});
			return new TimerBuilder<TimerUnitSet>(b.Script, b.Token, in options);
		}
	}

	internal struct TimerOptions
	{
		internal String Name;
		internal Double Amount;
		internal Coroutine.Continuation Continuation;
		internal Double DurationInSeconds; // set after unit chosen
	}
}
