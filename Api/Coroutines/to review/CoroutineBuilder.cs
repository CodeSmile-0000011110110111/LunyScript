using Luny;
using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ICoroutineBuilderState {}
	public interface ICoroutineFrameUnit : ICoroutineBuilderState {}
	public interface ICoroutineHeartbeatUnit : ICoroutineBuilderState {}
	public interface ICoroutineReadyUnit : ICoroutineBuilderState {}
	public struct CoroutineFrameUnit : ICoroutineFrameUnit, ICoroutineReadyUnit {}
	public struct CoroutineHeartbeatUnit : ICoroutineHeartbeatUnit, ICoroutineReadyUnit {}

	/// <summary>
	/// Entry point for the Coroutine fluent builder chain.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).WhenElapsed(blocks);
	///        Coroutine("name").OnFrameUpdate(blocks).WhenStopped(blocks).Do(blocks);
	///        Coroutine("name").OnHeartbeat(blocks).Do(blocks);
	/// </summary>
	public readonly struct CoroutineBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;

		internal CoroutineBuilder(Script script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Coroutine name is null or empty", nameof(name));
			_token = script.CreateBuilderToken(_name, "Coroutine()");
		}

		/// <summary>Sets the coroutine duration. Returns a builder to specify the time unit.</summary>
		public ForBuilder<ForBuilderStart> For(Double duration) => new(_script, _token, _name, duration);

		/// <summary>Creates an open-ended coroutine (runs until stopped) which runs the blocks every frame.</summary>
		public CoroutineUpdateBuilder<CoroutineFrameUnit> OnFrameUpdate(params ScriptActionBlock[] blocks) => new(_script, _token,
			CoroutineOptions.ForOpenEndedCoroutine(_name, Coroutine.Process.FrameUpdate) with { OnFrameUpdate = blocks });

		/// <summary>Creates an open-ended coroutine (runs until stopped) which runs the blocks every heartbeat (fixed step).</summary>
		public CoroutineUpdateBuilder<CoroutineHeartbeatUnit> OnHeartbeat(params ScriptActionBlock[] blocks) => new(_script, _token,
			CoroutineOptions.ForOpenEndedCoroutine(_name, Coroutine.Process.Heartbeat) with { OnHeartbeat = blocks });

		internal static ICoroutineBlock Finalize(Script script, BuilderToken token, in CoroutineOptions options)
		{
			WarnIfAllSequencesEmpty(script, token, options);
			var block = script.RuntimeContext.Coroutines.Register(in options);
			script.FinalizeBuilderToken(token);
			return block;
		}

		private static void WarnIfAllSequencesEmpty(Script script, BuilderToken token, in CoroutineOptions options)
		{
			if (options.OnFrameUpdate == null && options.OnHeartbeat == null && options.OnElapsed == null &&
			    options.OnStarted == null && options.OnStopped == null && options.OnPaused == null && options.OnResumed == null)
				LunyLogger.LogWarning($"{token.Type} '{options.Name}' has no blocks. Add blocks or remove it.", script);
		}
	}

	/// <summary>
	/// Generic step-builder for open-ended coroutines (frame-update or heartbeat).
	/// Returned by <c>CoroutineBuilder.OnFrameUpdate()</c> and <c>CoroutineBuilder.OnHeartbeat()</c>.
	/// </summary>
	public readonly struct CoroutineUpdateBuilder<T> where T : struct, ICoroutineBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly CoroutineOptions Options;

		internal CoroutineUpdateBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			Script = script;
			Token = token;
			Options = options;

			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, token, capturedOptions));
		}
	}

	public static class CoroutineUpdateBuilderExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineUpdateBuilder<T> WhenStarted<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit => new(b.Script, b.Token,
			b.Options with { OnStarted = BuilderUtility.Append(b.Options.OnStarted, blocks) });

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineUpdateBuilder<T> WhenStopped<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit => new(b.Script, b.Token,
			b.Options with { OnStopped = BuilderUtility.Append(b.Options.OnStopped, blocks) });

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineUpdateBuilder<T> WhenPaused<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit => new(b.Script, b.Token,
			b.Options with { OnPaused = BuilderUtility.Append(b.Options.OnPaused, blocks) });

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineUpdateBuilder<T> WhenResumed<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit => new(b.Script, b.Token,
			b.Options with { OnResumed = BuilderUtility.Append(b.Options.OnResumed, blocks) });

		/// <summary>Additional frame-update blocks to run each frame.</summary>
		public static CoroutineUpdateBuilder<CoroutineFrameUnit> OnFrameUpdate<T>(this CoroutineUpdateBuilder<T> b,
			params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineFrameUnit => new(b.Script, b.Token,
			b.Options with { OnFrameUpdate = BuilderUtility.Append(b.Options.OnFrameUpdate, blocks) });

		/// <summary>Additional heartbeat blocks to run each fixed step.</summary>
		public static CoroutineUpdateBuilder<CoroutineHeartbeatUnit> OnHeartbeat<T>(this CoroutineUpdateBuilder<T> b,
			params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineHeartbeatUnit => new(b.Script, b.Token,
			b.Options with { OnHeartbeat = BuilderUtility.Append(b.Options.OnHeartbeat, blocks) });

		/// <summary>Additional update blocks. Finalizes the builder.</summary>
		public static ICoroutineBlock Do<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit
		{
			if (b.Options.ProcessMode == Coroutine.Process.Heartbeat)
				return CoroutineBuilder.Finalize(b.Script, b.Token,
					b.Options with { OnHeartbeat = BuilderUtility.Append(b.Options.OnHeartbeat, blocks) });

			return CoroutineBuilder.Finalize(b.Script, b.Token,
				b.Options with { OnFrameUpdate = BuilderUtility.Append(b.Options.OnFrameUpdate, blocks) });
		}
	}
}
