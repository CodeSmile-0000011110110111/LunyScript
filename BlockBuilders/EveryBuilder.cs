using LunyScript.Api;
using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript.BlockBuilders
{
	/// <summary>
	/// Entry point for the Every fluent builder chain.
	/// Usage: Every(3).Frames().Do(blocks); or Every(Even).Heartbeats().DelayBy(1).Do(blocks);
	/// </summary>
	public readonly struct EveryBuilder
	{
		private readonly Script _script;
		private readonly Int32 _interval;
		private readonly BuilderToken _token;

		internal EveryBuilder(Script script, Int32 interval)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_interval = interval;
			_token = script.CreateToken($"Every({interval})", "EveryBuilder");
		}

		/// <summary>
		/// Selects frame-based execution.
		/// </summary>
		public EveryUnitBuilder Frames() => new(_script, _token, _interval, Coroutine.Process.FrameUpdate);

		/// <summary>
		/// Selects heartbeat-based execution.
		/// </summary>
		public EveryUnitBuilder Heartbeats() => new(_script, _token, _interval, Coroutine.Process.Heartbeat);
	}

	/// <summary>
	/// Builder step after unit (Frames/Heartbeats) is selected.
	/// </summary>
	public readonly struct EveryUnitBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly Int32 _interval;
		private readonly Int32 _delay;
		private readonly Coroutine.Process _process;

		internal EveryUnitBuilder(Script script, BuilderToken token, Int32 interval, Coroutine.Process process, Int32 delay = 0)
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
			var options = Coroutine.Options.ForEveryInterval(null, _interval, _delay, _process, blocks);
			return (ICounterCoroutineBlock)BuilderUtility.Finalize(_script, in options, _token);
		}
	}
}
