using System;

namespace LunyScript.ApiBuilders.Coroutine.Counter
{
	/// <summary>
	/// Entry point for the Counter fluent builder chain.
	/// Usage: Counter("name").In(5).Frames().Do(blocks);
	/// </summary>
	public readonly struct CounterBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;

		internal CounterBuilder(Script script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Counter name is null or empty", nameof(name));
			_token = script.CreateBuilderToken(_name, "Counter");
		}

		/// <summary>
		/// Sets the counter to fire once after the specified count.
		/// </summary>
		public CounterDurationBuilder In(Int32 targetCount) => new(_script, _token, new CounterOptions { Name = _name, Amount = targetCount, Continuation = Coroutines.Coroutine.Continuation.Finite });

		/// <summary>
		/// Sets the counter to fire repeatedly at the specified interval.
		/// </summary>
		public CounterDurationBuilder Every(Int32 interval) => new(_script, _token, new CounterOptions { Name = _name, Amount = interval, Continuation = Coroutines.Coroutine.Continuation.Repeating });
	}
}
