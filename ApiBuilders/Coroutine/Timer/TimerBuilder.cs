using System;

namespace LunyScript.ApiBuilders.Coroutine.Timer
{
	/// <summary>
	/// Entry point for the Timer fluent builder chain.
	/// Usage: Timer("name").In(3).Seconds().Do(blocks);
	/// </summary>
	public readonly struct TimerBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;

		internal TimerBuilder(Script script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Timer name is null or empty", nameof(name));
			_token = script.CreateToken(_name, "Timer");
		}

		/// <summary>
		/// Sets the timer to fire once after the specified duration.
		/// </summary>
		public TimerDurationBuilder In(Double duration) => new(_script, _name, _token, duration, Coroutines.Coroutine.Continuation.Finite);

		/// <summary>
		/// Sets the timer to fire repeatedly at the specified interval.
		/// </summary>
		public TimerDurationBuilder Every(Double interval) => new(_script, _name, _token, interval, Coroutines.Coroutine.Continuation.Repeating);
	}
}
