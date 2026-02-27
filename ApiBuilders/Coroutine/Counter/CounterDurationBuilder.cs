using System;

namespace LunyScript.ApiBuilders.Coroutine.Counter
{
	/// <summary>
	/// Builder step after counter amount is set. Next: specify unit (Frames/Heartbeats).
	/// </summary>
	public readonly struct CounterDurationBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly CounterOptions _options;

		internal CounterDurationBuilder(Script script, BuilderToken token, in CounterOptions options)
		{
			_script = script;
			_token = token;
			_options = options;

			if (options.Amount < 0)
				throw new ArgumentException($"Counter duration must be 0 or greater, got: {options.Amount}");
		}

		/// <summary>
		/// Duration in frames (count-based).
		/// </summary>
		public CounterFinalBuilder Frames() => new(_script, _token,
			CoroutineOptions.ForCounter(_options.Name, _options.Amount, _options.Continuation, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based).
		/// </summary>
		public CounterFinalBuilder Heartbeats() => new(_script, _token,
			CoroutineOptions.ForCounter(_options.Name, _options.Amount, _options.Continuation, Coroutines.Coroutine.Process.Heartbeat));
	}
}
