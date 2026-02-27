using LunyScript.Blocks;
using LunyScript.Exceptions;

namespace LunyScript.ApiBuilders.Coroutine.For
{
	/// <summary>
	/// Builder for finite coroutines running on frame updates.
	/// </summary>
	public readonly struct ForFiniteFrameBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly CoroutineOptions _options;

		internal ForFiniteFrameBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			_script = script;
			_token = token;
			_options = options;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}

		public ForFiniteFrameBuilder OnFrameUpdate(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) });

		public ForFiniteFrameBuilder WhenStarted(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public ForFiniteFrameBuilder WhenStopped(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public ForFiniteFrameBuilder WhenPaused(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public ForFiniteFrameBuilder WhenResumed(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public ICoroutineBlock WhenElapsed(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnElapsed = BuilderUtility.Append(_options.OnElapsed, blocks) }, _token);

		public ICoroutineBlock Do(params ScriptActionBlock[] blocks)
		{
			if (_options.OnFrameUpdate != null)
				throw new LunyScriptException($"{_token}: {nameof(Do)}() cannot be combined with {nameof(OnFrameUpdate)}()");

			return CoroutineBuilder.Finalize(_script,
				_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) }, _token);
		}
	}
}
