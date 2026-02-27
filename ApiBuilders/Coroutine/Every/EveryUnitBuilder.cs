using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Coroutine.Every
{
	/// <summary>
	/// Builder step after unit (Frames/Heartbeats) is selected.
	/// </summary>
	public readonly struct EveryUnitBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly Int32 _interval;
		private readonly Int32 _delay;
		private readonly Coroutines.Coroutine.Process _process;

		internal EveryUnitBuilder(Script script, BuilderToken token, Int32 interval, Coroutines.Coroutine.Process process, Int32 delay = 0)
		{
			_script = script;
			_token = token;
			_interval = Math.Max(0, interval);
			_delay = delay;
			_process = process;

			if (interval < 0)
				throw new ArgumentException($"Every duration must be 0 or greater, got: {interval}");
		}

		/// <summary>
		/// Sets the phase offset (delay) for time-sliced execution.
		/// </summary>
		public EveryUnitBuilder DelayBy(Int32 delay)
		{
			if (_delay != 0)
				throw new ArgumentException($"{nameof(DelayBy)}() can't be used twice");

			return new EveryUnitBuilder(_script, _token, _interval, _process, delay);
		}

		/// <summary>
		/// Completes the builder and specifies blocks to run.
		/// </summary>
		public ICounterCoroutineBlock Do(params ScriptActionBlock[] blocks)
		{
			// name = null => generates a unique name for a time-sliced coroutine
			var options = CoroutineOptions.ForEveryInterval(null, _interval, _delay, _process, blocks);
			return (ICounterCoroutineBlock)CoroutineBuilder.Finalize(_script, in options, _token);
		}
	}
}
