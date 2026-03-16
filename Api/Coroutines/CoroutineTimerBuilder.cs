using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript.Api
{
	public interface ICoroutineTimerBuilderState {}
	public interface ICoroutineTimerAmountSet : ICoroutineTimerBuilderState {}
	public struct CoroutineTimerAmountSet : ICoroutineTimerAmountSet {}
	public interface ICoroutineTimerWhen : ICoroutineTimerBuilderState {}
	public interface ICoroutineTimerUnitSet : ICoroutineTimerWhen {}
	public struct CoroutineTimerUnitSet : ICoroutineTimerUnitSet {}

	/// <summary>
	/// Fluent builder for timer coroutines.
	/// Usage: Timer("name").In(3).Seconds().Do(blocks);
	///        Timer("name").Every(1.5).Minutes().Do(blocks);
	/// </summary>
	public readonly struct CoroutineTimerBuilder<T> where T : struct, ICoroutineTimerBuilderState
	{
		internal readonly CoroutineOptions Options;

		internal CoroutineTimerBuilder(in CoroutineOptions options) => Options = options;

		internal CoroutineTimerBuilder(Script script, BuilderToken token, String name, Double duration, Boolean repeating)
		{
			if (duration < 0)
				throw new ArgumentException($"Coroutine duration cannot be negative, got: {duration}");

			Options = CoroutineOptions.ForCoroutine(name, duration, repeating) with { Script = script, Token = token };
		}
	}

	public static class TimerBuilderUnitExtensions
	{
		/// <summary>Duration in seconds.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Seconds<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet => new(b.Options);

		/// <summary>Duration in milliseconds.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Milliseconds<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet => new(b.Options with { Duration = b.Options.Duration / 1000.0 });

		/// <summary>Duration in minutes.</summary>
		public static CoroutineTimerBuilder<CoroutineTimerUnitSet> Minutes<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet => new(b.Options with { Duration = b.Options.Duration * 60.0 });
	}

	public static class TimerBuilderCounterUnitExtensions
	{
		/// <summary>Counts frame updates.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderUnitSet> Frames<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet => new(b.Options with { IsCounter = true, ProcessMode = Coroutine.Process.FrameUpdate });

		/// <summary>Counts heartbeat (fixed step) updates.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderUnitSet> Heartbeats<T>(this CoroutineTimerBuilder<T> b)
			where T : struct, ICoroutineTimerAmountSet => new(b.Options with { IsCounter = true, ProcessMode = Coroutine.Process.Heartbeat });
	}

	public static class TimerBuilderWhenExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineTimerBuilder<T> WhenStarted<T>(this CoroutineTimerBuilder<T> b, params ActionBlock[] startedBlocks)
			where T : struct, ICoroutineTimerWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnStarted);
			return ActionBlock.IsNullOrEmpty(startedBlocks) ? b : NextBuilder(b, b.Options with { OnStarted = startedBlocks });
		}

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineTimerBuilder<T> WhenStopped<T>(this CoroutineTimerBuilder<T> b, params ActionBlock[] stoppedBlocks)
			where T : struct, ICoroutineTimerWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnStopped);
			return ActionBlock.IsNullOrEmpty(stoppedBlocks) ? b : NextBuilder(b, b.Options with { OnStopped = stoppedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineTimerBuilder<T> WhenPaused<T>(this CoroutineTimerBuilder<T> b, params ActionBlock[] pausedBlocks)
			where T : struct, ICoroutineTimerWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnPaused);
			return ActionBlock.IsNullOrEmpty(pausedBlocks) ? b : NextBuilder(b, b.Options with { OnPaused = pausedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineTimerBuilder<T> WhenResumed<T>(this CoroutineTimerBuilder<T> b, params ActionBlock[] resumedBlocks)
			where T : struct, ICoroutineTimerWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnResumed);
			return ActionBlock.IsNullOrEmpty(resumedBlocks) ? b : NextBuilder(b, b.Options with { OnResumed = resumedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineTimerBuilder<T> WhenProcessed<T>(this CoroutineTimerBuilder<T> b, params ActionBlock[] processBlocks)
			where T : struct, ICoroutineTimerWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnFrameUpdate);
			return ActionBlock.IsNullOrEmpty(processBlocks) ? b : NextBuilder(b, b.Options with { OnFrameUpdate = processBlocks });
		}

		private static CoroutineTimerBuilder<T> NextBuilder<T>(CoroutineTimerBuilder<T> b, CoroutineOptions options)
			where T : struct, ICoroutineTimerWhen
		{
			b.Options.Token.AutoFinish = () => CoroutineBuilder.Finish(b.Options.Script, b.Options.Token, options);
			return new CoroutineTimerBuilder<T>(options);
		}
	}

	public static class TimerBuilderFinalExtensions
	{
		/// <summary>Completes the timer and (optional) specifies blocks to run every frame.</summary>
		public static ITimerCoroutineBlock WhenElapsed<T>(this CoroutineTimerBuilder<T> b, params ActionBlock[] elapsedBlocks)
			where T : struct, ICoroutineTimerUnitSet =>
			(ITimerCoroutineBlock)CoroutineBuilder.Finish(b.Options.Script, b.Options.Token, b.Options with { OnElapsed = elapsedBlocks });
	}
}
