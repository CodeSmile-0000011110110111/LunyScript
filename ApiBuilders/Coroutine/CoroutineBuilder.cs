using Luny;
using LunyScript.ApiBuilders.Coroutine.For;
using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Coroutine
{
	/// <summary>
	/// Entry point for the Coroutine fluent builder chain.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).WhenElapsed(blocks);
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
			_token = script.CreateToken(_name, "Coroutine");
		}

		/// <summary>
		/// Sets the coroutine duration. Returns a builder to specify the time unit.
		/// </summary>
		public ForBuilder For(Double duration) => new(_script, _name, _token, duration);

		/// <summary>
		/// Creates an open-ended coroutine (runs until stopped) which runs the blocks every frame.
		/// </summary>
		public CoroutineFrameUpdateBuilder OnFrameUpdate(params ScriptActionBlock[] blocks) => new(_script, _token,
			CoroutineOptions.ForOpenEnded(_name, Coroutines.Coroutine.Process.FrameUpdate) with { OnFrameUpdate = blocks });

		/// <summary>
		/// Creates an open-ended coroutine (runs until stopped) which runs the blocks every heartbeat (fixed step).
		/// </summary>
		public CoroutineHeartbeatBuilder OnHeartbeat(params ScriptActionBlock[] blocks) => new(_script, _token,
			CoroutineOptions.ForOpenEnded(_name, Coroutines.Coroutine.Process.Heartbeat) with { OnHeartbeat = blocks });


		internal static ICoroutineBlock Finalize(Script script, in CoroutineOptions options, BuilderToken token)
		{
			if (options.OnFrameUpdate == null && options.OnHeartbeat == null && options.OnElapsed == null &&
			    options.OnStarted == null && options.OnStopped == null && options.OnPaused == null && options.OnResumed == null)
			{
				LunyLogger.LogWarning($"{nameof(Coroutines.Coroutine)} '{options.Name}' was finalized without any action blocks. " +
				                      "It will run but perform no actions.", script);
			}

			var scriptInternal = script;
			var block = scriptInternal.RuntimeContext.Coroutines.Register(in options);
			scriptInternal.FinalizeToken(token);
			return block;
		}
	}
}
