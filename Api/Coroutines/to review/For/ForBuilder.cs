using LunyScript.Blocks;
using LunyScript.Exceptions;
using System;

namespace LunyScript
{
	/// <summary>
	/// Fluent builder for finite-duration coroutines.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).WhenElapsed(blocks);
	///        Coroutine("name").For(5).Frames().Do(blocks);
	///        Coroutine("name").For(60).Heartbeats().OnHeartbeat(blocks).WhenElapsed(blocks);
	/// </summary>
	public readonly struct ForBuilder<T> where T : struct, IForBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CoroutineOptions Options;
		internal readonly String Name;
		internal readonly Double Duration;

		internal ForBuilder(Script script, BuilderToken token, String name, Double duration)
		{
			Script = script;
			Token = token;
			Options = default;
			Name = name;
			Duration = duration;
		}

		internal ForBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			Script = script;
			Token = token;
			Options = options;
			Name = options.Name;
			Duration = 0;
		}
	}

	public static class ForBuilderExtensions
	{
		/// <summary>Duration in seconds (frame-update coroutine).</summary>
		public static ForBuilder<ForFrameUnit> Seconds<T>(this ForBuilder<T> b)
			where T : struct, IForAmountSet
			=> CreateFrameUnit(b, CoroutineOptions.ForTimerCoroutine(b.Name, b.Duration, LunyScript.Coroutines.Coroutine.Continuation.Finite, LunyScript.Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>Duration in milliseconds (frame-update coroutine).</summary>
		public static ForBuilder<ForFrameUnit> Milliseconds<T>(this ForBuilder<T> b)
			where T : struct, IForAmountSet
			=> CreateFrameUnit(b, CoroutineOptions.ForTimerCoroutine(b.Name, b.Duration / 1000.0, LunyScript.Coroutines.Coroutine.Continuation.Finite, LunyScript.Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>Duration in minutes (frame-update coroutine).</summary>
		public static ForBuilder<ForFrameUnit> Minutes<T>(this ForBuilder<T> b)
			where T : struct, IForAmountSet
			=> CreateFrameUnit(b, CoroutineOptions.ForTimerCoroutine(b.Name, b.Duration * 60.0, LunyScript.Coroutines.Coroutine.Continuation.Finite, LunyScript.Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>Duration in frame counts (frame-update coroutine).</summary>
		public static ForBuilder<ForFrameUnit> Frames<T>(this ForBuilder<T> b)
			where T : struct, IForAmountSet
			=> CreateFrameUnit(b, CoroutineOptions.ForCounterCoroutine(b.Name, (Int32)b.Duration, LunyScript.Coroutines.Coroutine.Continuation.Finite, LunyScript.Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>Duration in heartbeat counts (heartbeat coroutine).</summary>
		public static ForBuilder<ForHeartbeatUnit> Heartbeats<T>(this ForBuilder<T> b)
			where T : struct, IForAmountSet
			=> CreateHeartbeatUnit(b, CoroutineOptions.ForCounterCoroutine(b.Name, (Int32)b.Duration, LunyScript.Coroutines.Coroutine.Continuation.Finite, LunyScript.Coroutines.Coroutine.Process.Heartbeat));

		// Lifecycle — shared across both frame and heartbeat unit states

		/// <summary>Blocks to run on each frame update.</summary>
		public static ForBuilder<ForFrameUnit> OnFrameUpdate<T>(this ForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IForFrameUnit
			=> new(b.Script, b.Token, b.Options with { OnFrameUpdate = BuilderUtility.Append(b.Options.OnFrameUpdate, blocks) });

		/// <summary>Blocks to run on each heartbeat.</summary>
		public static ForBuilder<ForHeartbeatUnit> OnHeartbeat<T>(this ForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IForHeartbeatUnit
			=> new(b.Script, b.Token, b.Options with { OnHeartbeat = BuilderUtility.Append(b.Options.OnHeartbeat, blocks) });

		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static ForBuilder<T> WhenStarted<T>(this ForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IForReadyUnit
			=> new(b.Script, b.Token, b.Options with { OnStarted = BuilderUtility.Append(b.Options.OnStarted, blocks) });

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static ForBuilder<T> WhenStopped<T>(this ForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IForReadyUnit
			=> new(b.Script, b.Token, b.Options with { OnStopped = BuilderUtility.Append(b.Options.OnStopped, blocks) });

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static ForBuilder<T> WhenPaused<T>(this ForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IForReadyUnit
			=> new(b.Script, b.Token, b.Options with { OnPaused = BuilderUtility.Append(b.Options.OnPaused, blocks) });

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static ForBuilder<T> WhenResumed<T>(this ForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IForReadyUnit
			=> new(b.Script, b.Token, b.Options with { OnResumed = BuilderUtility.Append(b.Options.OnResumed, blocks) });

		/// <summary>Blocks to run when elapsed. Finalizes the builder.</summary>
		public static ICoroutineBlock WhenElapsed<T>(this ForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IForReadyUnit
			=> CoroutineBuilder.Finalize(b.Script, b.Options with { OnElapsed = BuilderUtility.Append(b.Options.OnElapsed, blocks) }, b.Token);

		/// <summary>
		/// Primary update blocks. Finalizes the builder.
		/// For frame-update coroutines: cannot be combined with <c>OnFrameUpdate()</c>.
		/// For heartbeat coroutines: cannot be combined with <c>OnHeartbeat()</c>.
		/// </summary>
		public static ICoroutineBlock Do<T>(this ForBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, IForReadyUnit
		{
			if (b.Options.ProcessMode == LunyScript.Coroutines.Coroutine.Process.Heartbeat)
			{
				if (b.Options.OnHeartbeat != null)
					throw new LunyScriptException($"{b.Token}: {nameof(Do)}() cannot be combined with {nameof(OnHeartbeat)}()");
				return CoroutineBuilder.Finalize(b.Script, b.Options with { OnHeartbeat = BuilderUtility.Append(b.Options.OnHeartbeat, blocks) }, b.Token);
			}

			if (b.Options.OnFrameUpdate != null)
				throw new LunyScriptException($"{b.Token}: {nameof(Do)}() cannot be combined with {nameof(OnFrameUpdate)}()");
			return CoroutineBuilder.Finalize(b.Script, b.Options with { OnFrameUpdate = BuilderUtility.Append(b.Options.OnFrameUpdate, blocks) }, b.Token);
		}

		private static ForBuilder<ForFrameUnit> CreateFrameUnit<T>(ForBuilder<T> b, in CoroutineOptions options)
			where T : struct, IForBuilderState
		{
			var capturedScript = b.Script;
			var capturedOptions = options;
			var capturedToken = b.Token;
			b.Token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, capturedToken));
			return new ForBuilder<ForFrameUnit>(b.Script, b.Token, in options);
		}

		private static ForBuilder<ForHeartbeatUnit> CreateHeartbeatUnit<T>(ForBuilder<T> b, in CoroutineOptions options)
			where T : struct, IForBuilderState
		{
			var capturedScript = b.Script;
			var capturedOptions = options;
			var capturedToken = b.Token;
			b.Token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, capturedToken));
			return new ForBuilder<ForHeartbeatUnit>(b.Script, b.Token, in options);
		}
	}
}
