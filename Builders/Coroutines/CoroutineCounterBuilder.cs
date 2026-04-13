using LunyScript.Blocks;

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

		public ActionBlock Start()
		{
			var coroutine = CoroutineBuilder.Finish(Options);
			return coroutine.Start();
		}

		public ActionBlock Stop()
		{
			var coroutine = CoroutineBuilder.Finish(Options);
			return coroutine.Stop();
		}

		public ActionBlock Pause()
		{
			var coroutine = CoroutineBuilder.Finish(Options);
			return coroutine.Pause();
		}

		public ActionBlock Resume()
		{
			var coroutine = CoroutineBuilder.Finish(Options);
			return coroutine.Resume();
		}
	}

	public static class CounterBuilderWhenExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineCounterBuilder<T> WhenStarted<T>(this CoroutineCounterBuilder<T> b, params ActionBlock[] startedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.WhenStarted);
			return ActionBlock.IsNullOrEmpty(startedBlocks) ? b : NextBuilder(b, b.Options with { WhenStarted = startedBlocks });
		}

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineCounterBuilder<T> WhenStopped<T>(this CoroutineCounterBuilder<T> b, params ActionBlock[] stoppedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.WhenStopped);
			return ActionBlock.IsNullOrEmpty(stoppedBlocks) ? b : NextBuilder(b, b.Options with { WhenStopped = stoppedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineCounterBuilder<T> WhenPaused<T>(this CoroutineCounterBuilder<T> b, params ActionBlock[] pausedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.WhenPaused);
			return ActionBlock.IsNullOrEmpty(pausedBlocks) ? b : NextBuilder(b, b.Options with { WhenPaused = pausedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineCounterBuilder<T> WhenResumed<T>(this CoroutineCounterBuilder<T> b, params ActionBlock[] resumedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.WhenResumed);
			return ActionBlock.IsNullOrEmpty(resumedBlocks) ? b : NextBuilder(b, b.Options with { WhenResumed = resumedBlocks });
		}

		/// <summary>
		/// Blocks to run every time the coroutine updates. Counter coroutines update in Heartbeat.
		/// </summary>
		/// <param name="processingBlocks"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static CoroutineCounterBuilder<T> WhenProcessing<T>(this CoroutineCounterBuilder<T> b, params ActionBlock[] processingBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.WhenProcessing);
			return ActionBlock.IsNullOrEmpty(processingBlocks) ? b : NextBuilder(b, b.Options with { WhenProcessing = processingBlocks });
		}

		/// <summary>Completes the counter and specifies blocks to run when elapsed.</summary>
		public static CoroutineCounterBuilder<T> WhenElapsed<T>(this CoroutineCounterBuilder<T> b, params ActionBlock[] elapsedBlocks)
			where T : struct, ICoroutineCounterBuilderUnitSet
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Options.Script, b.Options.WhenElapsed);
			return ActionBlock.IsNullOrEmpty(elapsedBlocks) ? b : NextBuilder(b, b.Options with { WhenElapsed = elapsedBlocks });
		}

		private static CoroutineCounterBuilder<T> NextBuilder<T>(CoroutineCounterBuilder<T> b, CoroutineOptions options)
			where T : struct, ICoroutineCounterWhen
		{
			b.Options.Token.AutoFinish = () => CoroutineBuilder.Finish(options);
			return new CoroutineCounterBuilder<T>(options);
		}
	}
}
