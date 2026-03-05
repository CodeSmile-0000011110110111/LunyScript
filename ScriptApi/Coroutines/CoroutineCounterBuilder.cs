using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ICoroutineCounterBuilderState {}
	public interface ICoroutineCounterWhen : ICoroutineCounterBuilderState {}
	public interface ICoroutineCounterBuilderUnitSet : ICoroutineCounterWhen {}
	public struct CoroutineCounterBuilderUnitSet : ICoroutineCounterBuilderUnitSet {}

	/// <summary>
	/// Counter runs blocks either once or repeatedly after a given number of frames/heartbeats.
	/// Usage: Counter("name").In(5).Frames().Do(blocks);
	///        Counter("name").Every(10).Heartbeats().Do(blocks);
	/// </summary>
	public readonly struct CoroutineCounterBuilder<T> where T : struct, ICoroutineCounterBuilderState
	{
		internal readonly CoroutineOptions Options;

		internal CoroutineCounterBuilder(in CoroutineOptions options) => Options = options;
	}

	public static class CounterBuilderWhenExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineCounterBuilder<T> WhenStarted<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] startedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnStarted);
			return ScriptActionBlock.IsNullOrEmpty(startedBlocks) ? b : NextBuilder(b, b.Options with { OnStarted = startedBlocks });
		}

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineCounterBuilder<T> WhenStopped<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] stoppedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnStopped);
			return ScriptActionBlock.IsNullOrEmpty(stoppedBlocks) ? b : NextBuilder(b, b.Options with { OnStopped = stoppedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineCounterBuilder<T> WhenPaused<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] pausedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnPaused);
			return ScriptActionBlock.IsNullOrEmpty(pausedBlocks) ? b : NextBuilder(b, b.Options with { OnPaused = pausedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineCounterBuilder<T> WhenResumed<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] resumedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnResumed);
			return ScriptActionBlock.IsNullOrEmpty(resumedBlocks) ? b : NextBuilder(b, b.Options with { OnResumed = resumedBlocks });
		}

		public static CoroutineCounterBuilder<T> WhenProcessed<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] processBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			if (b.Options.ProcessMode == Coroutine.Process.Heartbeat)
			{
				BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnHeartbeat);
				return ScriptActionBlock.IsNullOrEmpty(processBlocks) ? b : NextBuilder(b, b.Options with { OnHeartbeat = processBlocks });
			}

			if (b.Options.ProcessMode == Coroutine.Process.FrameUpdate)
			{
				BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.OnFrameUpdate);
				return ScriptActionBlock.IsNullOrEmpty(processBlocks) ? b : NextBuilder(b, b.Options with { OnFrameUpdate = processBlocks });
			}

			throw new ArgumentOutOfRangeException(nameof(b.Options.ProcessMode), b.Options.ProcessMode.ToString());
		}

		private static CoroutineCounterBuilder<T> NextBuilder<T>(CoroutineCounterBuilder<T> b, CoroutineOptions options)
			where T : struct, ICoroutineCounterWhen
		{
			b.Options.Token.AutoFinish = () => CoroutineBuilder.Finish(b.Options.Script, b.Options.Token, options);
			return new CoroutineCounterBuilder<T>(options);
		}
	}

	public static class CounterBuilderFinalExtensions
	{
		/// <summary>Completes the counter and specifies blocks to run when elapsed.</summary>
		public static ICoroutineBlock WhenElapsed<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] elapsedBlocks)
			where T : struct, ICoroutineCounterBuilderUnitSet =>
			CoroutineBuilder.Finish(b.Options.Script, b.Options.Token, b.Options with { OnElapsed = elapsedBlocks });
	}
}
