using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ICoroutineTimerBuilderState {}
	public interface ICoroutineTimerBuilderStart : ICoroutineTimerBuilderState {}
	public struct CoroutineTimerBuilderStart : ICoroutineTimerBuilderStart {}

	/// <summary>
	/// Fluent builder for timer coroutines.
	/// Usage: Timer("name").In(3).Seconds().Do(blocks);
	///        Timer("name").Every(1.5).Minutes().Do(blocks);
	/// </summary>
	public readonly struct CoroutineTimerBuilder<T> where T : struct, ICoroutineTimerBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly TimerOptions Options;

		internal CoroutineTimerBuilder(Script script, BuilderToken token, in TimerOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		/// <summary>Entry-point factory. Creates the builder token.</summary>
		internal static CoroutineTimerBuilder<CoroutineTimerBuilderStart> Create(Script script, String name)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Timer name is null or empty", nameof(name));

			var options = new TimerOptions { Name = name };
			var token = script.CreateBuilderToken(name, "Timer()");
			return new CoroutineTimerBuilder<CoroutineTimerBuilderStart>(script, token, options);
		}
	}

	public interface ICoroutineTimerAmountSet : ICoroutineTimerBuilderState {}
	public struct CoroutineTimerAmountSet : ICoroutineTimerAmountSet {}

	public static class TimerBuilderAmountExtensions
	{
		/// <summary>Sets the timer to fire once after the specified duration.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerAmountSet> In<T>(this CoroutineTimerBuilder<T> b, Double duration)
			where T : struct, ICoroutineTimerBuilderStart
		{
			var options = b.Options;
			options.Amount = duration;
			options.Continuation = Coroutine.Continuation.Finite;
			return new CoroutineTimerBuilder<CoroutineTimerAmountSet>(b.Script, b.Token, options);
		}

		/// <summary>Sets the timer to fire repeatedly at the specified interval.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerAmountSet> Every<T>(this CoroutineTimerBuilder<T> b, Double interval)
			where T : struct, ICoroutineTimerBuilderStart
		{
			var options = b.Options;
			options.Amount = interval;
			options.Continuation = Coroutine.Continuation.Repeating;
			return new CoroutineTimerBuilder<CoroutineTimerAmountSet>(b.Script, b.Token, options);
		}
	}

	public interface ICoroutineTimerWhen : ICoroutineTimerBuilderState {}
	public interface ICoroutineTimerUnitSet : ICoroutineTimerWhen {}
	public struct CoroutineTimerUnitSet : ICoroutineTimerUnitSet {}

	public static class TimerBuilderUnitExtensions
	{
		/// <summary>Duration in seconds.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Seconds<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {b.Options.Amount}");

			return CreateTimerUnit(b, b.Options.Amount);
		}

		/// <summary>Duration in milliseconds.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Milliseconds<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {b.Options.Amount}");

			return CreateTimerUnit(b, b.Options.Amount / 1000.0);
		}

		/// <summary>Duration in minutes.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Minutes<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet
		{
			if (b.Options.Amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {b.Options.Amount}");

			return CreateTimerUnit(b, b.Options.Amount * 60.0);
		}

		private static CoroutineTimerBuilder<CoroutineTimerUnitSet> CreateTimerUnit<T>(CoroutineTimerBuilder<T> b, Double durationInSeconds)
			where T : struct, ICoroutineTimerBuilderState
		{
			var options = b.Options;
			options.DurationInSeconds = durationInSeconds;
			return new CoroutineTimerBuilder<CoroutineTimerUnitSet>(b.Script, b.Token, options);
		}
	}

	/*public static class TimerBuilderWhenExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineTimerBuilder<T> WhenStarted<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token,
			b.Options with { OnStarted = BuilderUtility.Append(b.Options.OnStarted, blocks) });

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineTimerBuilder<T> WhenStopped<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token,
			b.Options with { OnStopped = BuilderUtility.Append(b.Options.OnStopped, blocks) });

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineTimerBuilder<T> WhenPaused<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token,
			b.Options with { OnPaused = BuilderUtility.Append(b.Options.OnPaused, blocks) });

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineTimerBuilder<T> WhenResumed<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token,
			b.Options with { OnResumed = BuilderUtility.Append(b.Options.OnResumed, blocks) });

		/// <summary>Blocks to run when the coroutine elapsed.</summary>
		public static CoroutineTimerBuilder<T> WhenElapsed<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token,
			b.Options with { OnElapsed = BuilderUtility.Append(b.Options.OnElapsed, blocks) });
	}*/

	public static class TimerBuilderFinalExtensions
	{
		/// <summary>Completes the timer and specifies blocks to run when elapsed.</summary>
		public static ITimerCoroutineBlock Do<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineTimerUnitSet
		{
			var o = b.Options;
			var options = CoroutineOptions.ForTimerCoroutine(o.Name, o.DurationInSeconds, o.Continuation) with { OnElapsed = blocks };
			return (ITimerCoroutineBlock)CoroutineBuilder.Finalize(b.Script, b.Token, in options);
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
