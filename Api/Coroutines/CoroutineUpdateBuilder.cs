using LunyScript.Blocks;

namespace LunyScript
{
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
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}
	}

	public static class CoroutineUpdateBuilderExtensions
	{
		/// <summary>Blocks to run when the coroutine starts.</summary>
		public static CoroutineUpdateBuilder<T> WhenStarted<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit
			=> new(b.Script, b.Token, b.Options with { OnStarted = BuilderUtility.Append(b.Options.OnStarted, blocks) });

		/// <summary>Blocks to run when the coroutine stops.</summary>
		public static CoroutineUpdateBuilder<T> WhenStopped<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit
			=> new(b.Script, b.Token, b.Options with { OnStopped = BuilderUtility.Append(b.Options.OnStopped, blocks) });

		/// <summary>Blocks to run when the coroutine is paused.</summary>
		public static CoroutineUpdateBuilder<T> WhenPaused<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit
			=> new(b.Script, b.Token, b.Options with { OnPaused = BuilderUtility.Append(b.Options.OnPaused, blocks) });

		/// <summary>Blocks to run when the coroutine is resumed.</summary>
		public static CoroutineUpdateBuilder<T> WhenResumed<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit
			=> new(b.Script, b.Token, b.Options with { OnResumed = BuilderUtility.Append(b.Options.OnResumed, blocks) });

		/// <summary>Additional frame-update blocks to run each frame.</summary>
		public static CoroutineUpdateBuilder<CoroutineFrameUnit> OnFrameUpdate<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineFrameUnit
			=> new(b.Script, b.Token, b.Options with { OnFrameUpdate = BuilderUtility.Append(b.Options.OnFrameUpdate, blocks) });

		/// <summary>Additional heartbeat blocks to run each fixed step.</summary>
		public static CoroutineUpdateBuilder<CoroutineHeartbeatUnit> OnHeartbeat<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineHeartbeatUnit
			=> new(b.Script, b.Token, b.Options with { OnHeartbeat = BuilderUtility.Append(b.Options.OnHeartbeat, blocks) });

		/// <summary>Additional update blocks. Finalizes the builder.</summary>
		public static ICoroutineBlock Do<T>(this CoroutineUpdateBuilder<T> b, params ScriptActionBlock[] blocks)
			where T : struct, ICoroutineReadyUnit
		{
			if (b.Options.ProcessMode == LunyScript.Coroutines.Coroutine.Process.Heartbeat)
				return CoroutineBuilder.Finalize(b.Script, b.Options with { OnHeartbeat = BuilderUtility.Append(b.Options.OnHeartbeat, blocks) }, b.Token);
			return CoroutineBuilder.Finalize(b.Script, b.Options with { OnFrameUpdate = BuilderUtility.Append(b.Options.OnFrameUpdate, blocks) }, b.Token);
		}
	}
}
