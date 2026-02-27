using LunyScript.Blocks;

namespace LunyScript.ApiBuilders.Coroutine.Timer
{
	/// <summary>
	/// Final builder step. Provides terminal methods to complete the timer.
	/// </summary>
	public readonly struct TimerFinalBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly CoroutineOptions _options;

		private TimerFinalBuilder(Script script, BuilderToken token, in CoroutineOptions options)
		{
			_script = script;
			_token = token;
			_options = options;
		}

		internal static TimerFinalBuilder FromOptions(Script script, BuilderToken token, in CoroutineOptions options) =>
			new(script, token, options);

		/// <summary>
		/// Completes the timer and specifies blocks to run when elapsed.
		/// </summary>
		public ITimerCoroutineBlock Do(params ScriptActionBlock[] blocks)
		{
			var options = _options with { OnElapsed = blocks };
			return (ITimerCoroutineBlock)CoroutineBuilder.Finalize(_script, in options, _token);
		}
	}
}
