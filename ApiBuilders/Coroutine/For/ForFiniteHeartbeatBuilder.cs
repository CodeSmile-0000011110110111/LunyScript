using LunyScript.Blocks;
using LunyScript.Exceptions;

namespace LunyScript.ApiBuilders.Coroutine.For
{
	/// <summary>
	/// Builder for finite coroutines running on heartbeats.
	/// </summary>
	public readonly struct ForFiniteHeartbeatBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly CoroutineOptions _options;

		internal ForFiniteHeartbeatBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			_script = script;
			_token = token;
			_options = options;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}

		public ForFiniteHeartbeatBuilder OnHeartbeat(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) });

		public ForFiniteHeartbeatBuilder WhenStarted(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public ForFiniteHeartbeatBuilder WhenStopped(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public ForFiniteHeartbeatBuilder WhenPaused(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public ForFiniteHeartbeatBuilder WhenResumed(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public ICoroutineBlock WhenElapsed(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnElapsed = BuilderUtility.Append(_options.OnElapsed, blocks) }, _token);

		public ICoroutineBlock Do(params ScriptActionBlock[] blocks)
		{
			if (_options.OnHeartbeat != null)
				throw new LunyScriptException($"{_token}: {nameof(Do)}() cannot be combined with {nameof(OnHeartbeat)}()");

			return CoroutineBuilder.Finalize(_script,
				_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) }, _token);
		}
	}
}
