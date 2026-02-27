using LunyScript.Blocks;

namespace LunyScript.ApiBuilders.Coroutine.Counter
{
	/// <summary>
	/// Final builder step for counters.
	/// </summary>
	public readonly struct CounterFinalBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly CoroutineOptions _options;

		internal CounterFinalBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			_script = script;
			_token = token;
			_options = options;
		}

		/// <summary>
		/// Completes the counter and specifies blocks to run when elapsed.
		/// </summary>
		public ICounterCoroutineBlock Do(params ScriptActionBlock[] blocks)
		{
			var options = _options with { OnElapsed = blocks };
			return (ICounterCoroutineBlock)CoroutineBuilder.Finalize(_script, in options, _token);
		}
	}
}
