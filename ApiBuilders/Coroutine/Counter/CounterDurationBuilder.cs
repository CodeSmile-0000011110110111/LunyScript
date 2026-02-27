using System;

namespace LunyScript.ApiBuilders.Coroutine.Counter
{
	/// <summary>
	/// Builder step after counter amount is set. Next: specify unit (Frames/Heartbeats).
	/// </summary>
	public readonly struct CounterDurationBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;
		private readonly Int32 _amount;
		private readonly Coroutines.Coroutine.Continuation _continuation;

		internal CounterDurationBuilder(Script script, String name, BuilderToken token, Int32 amount, Coroutines.Coroutine.Continuation continuation)
		{
			_script = script;
			_name = name;
			_token = token;
			_amount = amount;
			_continuation = continuation;

			if (amount < 0)
				throw new ArgumentException($"Counter duration must be 0 or greater, got: {amount}");
		}

		/// <summary>
		/// Duration in frames (count-based).
		/// </summary>
		public CounterFinalBuilder Frames() => new(_script, _token,
			CoroutineOptions.ForCounter(_name, _amount, _continuation, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based).
		/// </summary>
		public CounterFinalBuilder Heartbeats() => new(_script, _token,
			CoroutineOptions.ForCounter(_name, _amount, _continuation, Coroutines.Coroutine.Process.Heartbeat));
	}
}
