using LunyScript.Blocks;

namespace LunyScript.ApiBuilders.Coroutine
{
	/// <summary>
	/// Builder for open-ended coroutines running on frame updates.
	/// </summary>
	public readonly struct CoroutineFrameUpdateBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly CoroutineOptions _options;

		internal CoroutineFrameUpdateBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			_script = script;
			_token = token;
			_options = options;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}

		public CoroutineFrameUpdateBuilder WhenStarted(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public CoroutineFrameUpdateBuilder WhenStopped(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public CoroutineFrameUpdateBuilder WhenPaused(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public CoroutineFrameUpdateBuilder WhenResumed(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public ICoroutineBlock Do(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) }, _token);
	}
}
