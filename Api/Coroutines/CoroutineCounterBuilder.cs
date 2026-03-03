using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ICoroutineCounterBuilderState {}
	public interface ICoroutineCounterBuilderStart : ICoroutineCounterBuilderState {}
	public struct CoroutineCounterBuilderStart : ICoroutineCounterBuilderStart {}

	/// <summary>
	/// Counter runs blocks either once or repeatedly after a given number of frames/heartbeats.
	/// Usage: Counter("name").In(5).Frames().Do(blocks);
	///        Counter("name").Every(10).Heartbeats().Do(blocks);
	/// </summary>
	public readonly struct CoroutineCounterBuilder<T> where T : struct, ICoroutineCounterBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CoroutineOptions Options;

		internal CoroutineCounterBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}

		internal static CoroutineCounterBuilder<CoroutineCounterBuilderStart> Create(Script script, String name)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Counter name is null or empty", nameof(name));

			var options = new CoroutineOptions { Name = name };
			var token = script.CreateBuilderToken(name, "Counter()");
			return new CoroutineCounterBuilder<CoroutineCounterBuilderStart>(script, token, options);
		}
	}

	public interface ICoroutineCounterBuilderContinuationSet : ICoroutineCounterBuilderState {}
	public struct CoroutineCounterBuilderContinuationSet : ICoroutineCounterBuilderContinuationSet {}

	public static class CounterBuilderContinuationExtensions
	{
		/// <summary>Sets the counter to fire once after the specified count.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderContinuationSet>
			In(this CoroutineCounterBuilder<CoroutineCounterBuilderStart> b, Int32 targetCount) => new(b.Script, b.Token,
			CoroutineOptions.ForCounterCoroutine(b.Options.Name, targetCount, Coroutine.Continuation.Finite, Coroutine.Process.Always));

		/// <summary>Sets the counter to fire repeatedly at the specified interval.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderContinuationSet>
			Every(this CoroutineCounterBuilder<CoroutineCounterBuilderStart> b, Int32 interval) => new(b.Script, b.Token,
			CoroutineOptions.ForCounterCoroutine(b.Options.Name, interval, Coroutine.Continuation.Repeating, Coroutine.Process.Always));
	}

	public interface ICoroutineCounterWhen : ICoroutineCounterBuilderState {}
	public interface ICoroutineCounterBuilderUnitSet : ICoroutineCounterWhen {}
	public struct CoroutineCounterBuilderUnitSet : ICoroutineCounterBuilderUnitSet {}

	public static class CounterBuilderUnitExtensions
	{
		/// <summary>Counts frame updates.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderUnitSet> Frames<T>(this CoroutineCounterBuilder<T> b)
			where T : struct, ICoroutineCounterBuilderContinuationSet =>
			new(b.Script, b.Token, b.Options with { ProcessMode = Coroutine.Process.FrameUpdate });

		/// <summary>Counts heartbeat (fixed step) updates.</summary>
		public static CoroutineCounterBuilder<CoroutineCounterBuilderUnitSet> Heartbeats<T>(this CoroutineCounterBuilder<T> b)
			where T : struct, ICoroutineCounterBuilderContinuationSet =>
			new(b.Script, b.Token, b.Options with { ProcessMode = Coroutine.Process.Heartbeat });
	}

	public static class CounterBuilderWhenExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineCounterBuilder<T> WhenStarted<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] startedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnStarted);
			return ScriptActionBlock.IsNullOrEmpty(startedBlocks) ? b : NextBuilder(b, b.Options with { OnStarted = startedBlocks });
		}

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineCounterBuilder<T> WhenStopped<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] stoppedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnStopped);
			return ScriptActionBlock.IsNullOrEmpty(stoppedBlocks) ? b : NextBuilder(b, b.Options with { OnStopped = stoppedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineCounterBuilder<T> WhenPaused<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] pausedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnPaused);
			return ScriptActionBlock.IsNullOrEmpty(pausedBlocks) ? b : NextBuilder(b, b.Options with { OnPaused = pausedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineCounterBuilder<T> WhenResumed<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] resumedBlocks)
			where T : struct, ICoroutineCounterWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnResumed);
			return ScriptActionBlock.IsNullOrEmpty(resumedBlocks) ? b : NextBuilder(b, b.Options with { OnResumed = resumedBlocks });
		}

		private static CoroutineCounterBuilder<T> NextBuilder<T>(CoroutineCounterBuilder<T> b, in CoroutineOptions options)
			where T : struct, ICoroutineCounterWhen
		{
			CoroutineBuilder.SetAutoFinalizer(b.Script, b.Token, options);
			return new CoroutineCounterBuilder<T>(b.Script, b.Token, options);
		}
	}

	public static class CounterBuilderFinalExtensions
	{
		/// <summary>Completes the counter and specifies blocks to run when elapsed.</summary>
		public static ICoroutineBlock Do<T>(this CoroutineCounterBuilder<T> b, params ScriptActionBlock[] elapsedBlocks)
			where T : struct, ICoroutineCounterBuilderUnitSet =>
			CoroutineBuilder.Finalize(b.Script, b.Token, b.Options with { OnElapsed = elapsedBlocks });
	}
}
