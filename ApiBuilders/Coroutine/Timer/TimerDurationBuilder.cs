using System;

namespace LunyScript.ApiBuilders.Coroutine.Timer
{
	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct TimerDurationBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;
		private readonly Double _amount;
		private readonly Coroutines.Coroutine.Continuation _continuation;

		internal TimerDurationBuilder(Script script, String name, BuilderToken token, Double amount, Coroutines.Coroutine.Continuation continuation)
		{
			_script = script;
			_name = name;
			_token = token;
			_amount = Math.Max(0, amount);
			_continuation = continuation;

			if (amount < 0)
				throw new ArgumentException($"Timer duration must be 0 or greater, got: {amount}");
		}

		private TimerFinalBuilder CreateFinal(in CoroutineOptions options) => TimerFinalBuilder.FromOptions(_script, _token, options);

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public TimerFinalBuilder Seconds() =>
			CreateFinal(CoroutineOptions.ForTimer(_name, _amount, _continuation, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public TimerFinalBuilder Milliseconds() =>
			CreateFinal(CoroutineOptions.ForTimer(_name, _amount / 1000.0, _continuation, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public TimerFinalBuilder Minutes() =>
			CreateFinal(CoroutineOptions.ForTimer(_name, _amount * 60.0, _continuation, Coroutines.Coroutine.Process.FrameUpdate));
	}
}
