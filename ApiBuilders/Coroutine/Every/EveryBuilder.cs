using System;

namespace LunyScript.ApiBuilders.Coroutine.Every
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
		public EveryUnitBuilder Frames() => new(_script, _token, _interval, Coroutines.Coroutine.Process.FrameUpdate);

		/// <summary>
		/// Selects heartbeat-based execution.
		/// </summary>
		public EveryUnitBuilder Heartbeats() => new(_script, _token, _interval, Coroutines.Coroutine.Process.Heartbeat);
	}
}
