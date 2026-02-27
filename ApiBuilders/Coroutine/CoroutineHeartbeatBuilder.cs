using LunyScript.Blocks;

namespace LunyScript.ApiBuilders.Coroutine
{
	/// <summary>
	/// Builder for open-ended coroutines running on heartbeats.
	/// </summary>
	public readonly struct CoroutineHeartbeatBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly CoroutineOptions _options;

		internal CoroutineHeartbeatBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			_script = script;
			_token = token;
			_options = options;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}

		public CoroutineHeartbeatBuilder WhenStarted(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public CoroutineHeartbeatBuilder WhenStopped(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public CoroutineHeartbeatBuilder WhenPaused(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public CoroutineHeartbeatBuilder WhenResumed(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public ICoroutineBlock Do(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) }, _token);
	}
}
