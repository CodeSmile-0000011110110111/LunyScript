using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ICoroutineForBuilderState {}
	public interface ICoroutineForBuilderStart : ICoroutineForBuilderState {}
	public struct CoroutineForBuilderStart : ICoroutineForBuilderStart {}

	/// <summary>
	/// Fluent builder for finite-duration coroutines.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).WhenElapsed(blocks);
	///        Coroutine("name").For(5).Frames().Do(blocks);
	///        Coroutine("name").For(60).Heartbeats().OnHeartbeat(blocks).WhenElapsed(blocks);
	/// </summary>
	public readonly struct CoroutineForBuilder<T> where T : struct, ICoroutineForBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CoroutineOptions Options;

		internal CoroutineForBuilder(Script script, BuilderToken token, String name, Double duration)
		{
			Script = script;
			Token = token;
			Options = new CoroutineOptions { Name = name, Duration = duration };
		}

		internal CoroutineForBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
		}
	}

	public interface ICoroutineForWhen : ICoroutineForBuilderState {}
	public interface ICoroutineForFrameUnit : ICoroutineForWhen {}
	public struct CoroutineForFrameUnit : ICoroutineForFrameUnit {}

	public static class ForBuilderFrameUpdateExtensions
	{
		/// <summary>Duration in seconds (frame-update coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForFrameUnit> Seconds<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => CreateFrameUnit(b, b.Options.Duration);

		/// <summary>Duration in milliseconds (frame-update coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForFrameUnit> Milliseconds<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => CreateFrameUnit(b, b.Options.Duration / 1000.0);

		/// <summary>Duration in minutes (frame-update coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForFrameUnit> Minutes<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => CreateFrameUnit(b, b.Options.Duration * 60.0);

		/// <summary>Duration in frame counts (frame-update coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForFrameUnit> Frames<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => new(b.Script, b.Token,
			CoroutineOptions.ForCounterCoroutine(b.Options.Name, b.Options.CounterTarget, Coroutine.Continuation.Finite,
				Coroutine.Process.FrameUpdate));

		private static CoroutineForBuilder<CoroutineForFrameUnit> CreateFrameUnit<T>(CoroutineForBuilder<T> b, Double duration)
			where T : struct, ICoroutineForBuilderState => new(b.Script, b.Token,
			CoroutineOptions.ForTimerCoroutine(b.Options.Name, duration, Coroutine.Continuation.Finite));
	}

	public interface ICoroutineForHeartbeatUnit : ICoroutineForWhen {}
	public struct CoroutineForHeartbeatUnit : ICoroutineForHeartbeatUnit {}

	public static class ForBuilderHeartbeatExtensions
	{
		/// <summary>Duration in heartbeat counts (heartbeat coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForHeartbeatUnit> Heartbeats<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => new(b.Script, b.Token, CoroutineOptions.ForCounterCoroutine(b.Options.Name,
			b.Options.CounterTarget, Coroutine.Continuation.Finite, Coroutine.Process.Heartbeat));
	}

	public static class ForBuilderWhenExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineForBuilder<T> WhenStarted<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] startedBlocks)
			where T : struct, ICoroutineForWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnStarted);
			return ScriptActionBlock.IsNullOrEmpty(startedBlocks) ? b : NextBuilder(b, b.Options with { OnStarted = startedBlocks });
		}

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineForBuilder<T> WhenStopped<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] stoppedBlocks)
			where T : struct, ICoroutineForWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnStopped);
			return ScriptActionBlock.IsNullOrEmpty(stoppedBlocks) ? b : NextBuilder(b, b.Options with { OnStopped = stoppedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineForBuilder<T> WhenPaused<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] pausedBlocks)
			where T : struct, ICoroutineForWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnPaused);
			return ScriptActionBlock.IsNullOrEmpty(pausedBlocks) ? b : NextBuilder(b, b.Options with { OnPaused = pausedBlocks });
		}

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineForBuilder<T> WhenResumed<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] resumedBlocks)
			where T : struct, ICoroutineForWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnResumed);
			return ScriptActionBlock.IsNullOrEmpty(resumedBlocks) ? b : NextBuilder(b, b.Options with { OnResumed = resumedBlocks });
		}

		/// <summary>Blocks to run when the coroutine elapsed. Also runs when the coroutine automatically restarts.</summary>
		public static CoroutineForBuilder<T> WhenElapsed<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] elapsedBlocks)
			where T : struct, ICoroutineForWhen
		{
			BuilderUtility.ThrowIfUnaryMethodUsedAgain(b.Script, b.Options.OnElapsed);
			return ScriptActionBlock.IsNullOrEmpty(elapsedBlocks) ? b : NextBuilder(b, b.Options with { OnElapsed = elapsedBlocks });
		}

		private static CoroutineForBuilder<T> NextBuilder<T>(CoroutineForBuilder<T> b, in CoroutineOptions options)
			where T : struct, ICoroutineForWhen
		{
			CoroutineBuilder.SetAutoFinalizer(b.Script, b.Token, options);
			return new CoroutineForBuilder<T>(b.Script, b.Token, options);
		}
	}

	public static class ForBuilderFinalExtensions
	{
		/// <summary>
		/// Primary update blocks. Finalizes the builder. May remain empty if the coroutine only needs to run blocks in When* methods.
		/// </summary>
		public static ICoroutineBlock Do<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen
		{
			if (b.Options.ProcessMode == Coroutine.Process.Heartbeat)
				return CoroutineBuilder.Finalize(b.Script, b.Token, b.Options with { OnHeartbeat = blocks });

			if (b.Options.ProcessMode == Coroutine.Process.FrameUpdate)
				return CoroutineBuilder.Finalize(b.Script, b.Token, b.Options with { OnFrameUpdate = blocks });

			throw new ArgumentOutOfRangeException(nameof(b.Options.ProcessMode), b.Options.ProcessMode.ToString());
		}
	}
}
