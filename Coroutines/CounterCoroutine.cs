using Luny;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific number of heartbeats/ticks.
	/// </summary>
	internal sealed class CounterCoroutine : Coroutine
	{
		private Counter _counter;
		private Boolean _elapsedThisTick;

		public CounterCoroutine(in CoroutineOptions options)
			: base(options)
		{
			_counter = new Counter(Math.Max(0, options.CounterTarget));
			_counter.AutoRepeat = options.ContinuationMode == Continuation.Repeating;
			_counter.OnElapsed += () =>
			{
				_elapsedThisTick = true;
				_counter.Start(); // Reset counter
			};
		}

		protected override void OnStarted() => _counter.Start();

		protected override void OnStopped() => _counter.Stop();

		//protected override void OnPaused() {} // no need to pause counter: Consume* methods won't be called when paused
		//protected override void OnResumed() {}
		protected override Boolean ConsumeProcess() => IncrementCounter();

		private Boolean IncrementCounter()
		{
			_elapsedThisTick = false;
			_counter.Increment(); // might fire OnElapsed event
			return _elapsedThisTick;
		}

		public override String ToString()
		{
			var progress = $"{_counter.Current}/{_counter.Target}";
			return $"{GetType().Name}({Name}, {State}, {progress})";
		}
	}
}
