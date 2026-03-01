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
		internal readonly Double Duration;

		internal CoroutineForBuilder(Script script, BuilderToken token, String name, Double duration)
		{
			Script = script;
			Token = token;
			Options = new CoroutineOptions { Name = name };
			Duration = duration;
		}

		internal CoroutineForBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
			Duration = 0;
		}
	}

	public interface ICoroutineForWhen : ICoroutineForBuilderState {}
	public interface ICoroutineForFrameUnit : ICoroutineForWhen {}
	public struct CoroutineForFrameUnit : ICoroutineForFrameUnit {}

	public static class ForBuilderFrameUpdateExtensions
	{
		/// <summary>Duration in seconds (frame-update coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForFrameUnit> Seconds<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => CreateFrameUnit(b, (Int32)b.Duration);

		/// <summary>Duration in milliseconds (frame-update coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForFrameUnit> Milliseconds<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => CreateFrameUnit(b, (Int32)(b.Duration / 1000));

		/// <summary>Duration in minutes (frame-update coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForFrameUnit> Minutes<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => CreateFrameUnit(b, (Int32)(b.Duration * 60));

		/// <summary>Duration in frame counts (frame-update coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForFrameUnit> Frames<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart => CreateFrameUnit(b, (Int32)b.Duration);

		private static CoroutineForBuilder<CoroutineForFrameUnit> CreateFrameUnit<T>(CoroutineForBuilder<T> b, Int32 duration)
			where T : struct, ICoroutineForBuilderState
		{
			var options = CoroutineOptions.ForCounterCoroutine(b.Options.Name, duration, Coroutine.Continuation.Finite,
				Coroutine.Process.FrameUpdate);
			return new CoroutineForBuilder<CoroutineForFrameUnit>(b.Script, b.Token, options);
		}
	}

	public interface ICoroutineForHeartbeatUnit : ICoroutineForWhen {}
	public struct CoroutineForHeartbeatUnit : ICoroutineForHeartbeatUnit {}

	public static class ForBuilderHeartbeatExtensions
	{
		/// <summary>Duration in heartbeat counts (heartbeat coroutine).</summary>
		public static CoroutineForBuilder<CoroutineForHeartbeatUnit> Heartbeats<T>(this CoroutineForBuilder<T> b)
			where T : struct, ICoroutineForBuilderStart
		{
			var options = CoroutineOptions.ForCounterCoroutine(b.Options.Name, (Int32)b.Duration, Coroutine.Continuation.Finite,
				Coroutine.Process.Heartbeat);
			return new CoroutineForBuilder<CoroutineForHeartbeatUnit>(b.Script, b.Token, options);
		}
	}

	public static class ForBuilderWhenExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineForBuilder<T> WhenStarted<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token, b.Options with { OnStarted = blocks });

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineForBuilder<T> WhenStopped<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token, b.Options with { OnStopped = blocks });

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineForBuilder<T> WhenPaused<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token, b.Options with { OnPaused = blocks });

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineForBuilder<T> WhenResumed<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token, b.Options with { OnResumed = blocks });

		/// <summary>Blocks to run when the coroutine elapsed.</summary>
		public static CoroutineForBuilder<T> WhenElapsed<T>(this CoroutineForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineForWhen => new(b.Script, b.Token, b.Options with { OnElapsed = blocks });
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
