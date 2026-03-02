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
		internal readonly CoroutineOptions Options;

		internal CoroutineTimerBuilder(Script script, BuilderToken token, in CoroutineOptions options)
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

			var options = new CoroutineOptions { Name = name };
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
			where T : struct, ICoroutineTimerBuilderStart => new(b.Script, b.Token,
			b.Options with { Name = b.Options.Name, TimerDurationInSeconds = duration, ContinuationMode = Coroutine.Continuation.Finite });

		/// <summary>Sets the timer to fire repeatedly at the specified interval.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerAmountSet> Every<T>(this CoroutineTimerBuilder<T> b, Double interval)
			where T : struct, ICoroutineTimerBuilderStart => new(b.Script, b.Token,
			b.Options with { Name = b.Options.Name, TimerDurationInSeconds = interval, ContinuationMode = Coroutine.Continuation.Repeating });
	}

	public interface ICoroutineTimerWhen : ICoroutineTimerBuilderState {}
	public interface ICoroutineTimerUnitSet : ICoroutineTimerWhen {}
	public struct CoroutineTimerUnitSet : ICoroutineTimerUnitSet {}

	public static class TimerBuilderUnitExtensions
	{
		/// <summary>Duration in seconds.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Seconds<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet => CreateTimerUnit(b, b.Options.TimerDurationInSeconds);

		/// <summary>Duration in milliseconds.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Milliseconds<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet => CreateTimerUnit(b, b.Options.TimerDurationInSeconds / 1000.0);

		/// <summary>Duration in minutes.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Minutes<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet => CreateTimerUnit(b, b.Options.TimerDurationInSeconds * 60.0);

		private static CoroutineTimerBuilder<CoroutineTimerUnitSet> CreateTimerUnit<T>(CoroutineTimerBuilder<T> b, Double durationInSeconds)
			where T : struct, ICoroutineTimerBuilderState
		{
			if (durationInSeconds < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {durationInSeconds}");

			return new CoroutineTimerBuilder<CoroutineTimerUnitSet>(b.Script, b.Token,
				CoroutineOptions.ForTimerCoroutine(b.Options.Name, durationInSeconds, b.Options.ContinuationMode));
		}
	}

	public static class TimerBuilderWhenExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineTimerBuilder<T> WhenStarted<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineTimerWhen => new(b.Script, b.Token, b.Options with { OnStarted = blocks });

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineTimerBuilder<T> WhenStopped<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineTimerWhen => new(b.Script, b.Token, b.Options with { OnStopped = blocks });

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineTimerBuilder<T> WhenPaused<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineTimerWhen => new(b.Script, b.Token, b.Options with { OnPaused = blocks });

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineTimerBuilder<T> WhenResumed<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineTimerWhen => new(b.Script, b.Token, b.Options with { OnResumed = blocks });

		/// <summary>Blocks to run when the coroutine elapsed.</summary>
		public static CoroutineTimerBuilder<T> WhenElapsed<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineTimerWhen => new(b.Script, b.Token, b.Options with { OnElapsed = blocks });
	}

	public static class TimerBuilderFinalExtensions
	{
		/// <summary>Completes the timer and (optional) specifies blocks to run every frame.</summary>
		public static ITimerCoroutineBlock Do<T>(this CoroutineTimerBuilder<T> b, params ScriptActionBlock[] processBlocks)
			where T : struct, ICoroutineTimerUnitSet =>
			(ITimerCoroutineBlock)CoroutineBuilder.Finalize(b.Script, b.Token, b.Options with { OnFrameUpdate = processBlocks });
	}
}
