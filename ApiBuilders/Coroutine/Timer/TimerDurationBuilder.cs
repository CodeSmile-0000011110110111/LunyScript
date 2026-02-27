using System;

namespace LunyScript.ApiBuilders.Coroutine.Timer
{
	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct TimerDurationBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly TimerOptions _options;

		internal TimerDurationBuilder(Script script, BuilderToken token, in TimerOptions options)
		{
			_script = script;
			_token = token;
			_options = options;

			if (options.Amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {options.Amount}");
		}

		private TimerFinalBuilder CreateFinal(in CoroutineOptions options) => TimerFinalBuilder.FromOptions(_script, _token, options);

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public TimerFinalBuilder Seconds() =>
			CreateFinal(CoroutineOptions.ForTimer(_options.Name, _options.Amount, _options.Continuation, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public TimerFinalBuilder Milliseconds() =>
			CreateFinal(CoroutineOptions.ForTimer(_options.Name, _options.Amount / 1000.0, _options.Continuation, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public TimerFinalBuilder Minutes() =>
			CreateFinal(CoroutineOptions.ForTimer(_options.Name, _options.Amount * 60.0, _options.Continuation, Coroutines.Coroutine.Process.FrameUpdate));
	}
}
