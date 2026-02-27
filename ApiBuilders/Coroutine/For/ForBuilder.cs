using System;

namespace LunyScript.ApiBuilders.Coroutine.For
{
	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct ForBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;
		private readonly Double _duration;

		internal ForBuilder(Script script, String name, BuilderToken token, Double duration)
		{
			_script = script;
			_name = name;
			_token = token;
			_duration = Math.Max(0, duration);

			if (duration < 0)
				throw new ArgumentException($"Coroutine duration must be 0 or greater, got: {duration}");
		}

		/// <summary>
		/// Duration in seconds (time-based).
		/// </summary>
		public ForFiniteFrameBuilder Seconds() => new(_script, _token,
			CoroutineOptions.ForTimer(_name, _duration, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public ForFiniteFrameBuilder Milliseconds() => new(_script, _token,
			CoroutineOptions.ForTimer(_name, _duration / 1000.0, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public ForFiniteFrameBuilder Minutes() => new(_script, _token,
			CoroutineOptions.ForTimer(_name, _duration * 60.0, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in frames (count-based, counts frames).
		/// </summary>
		public ForFiniteFrameBuilder Frames() => new(_script, _token,
			CoroutineOptions.ForCounter(_name, (Int32)_duration, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public ForFiniteHeartbeatBuilder Heartbeats() => new(_script, _token,
			CoroutineOptions.ForCounter(_name, (Int32)_duration, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.Heartbeat));
	}
}
