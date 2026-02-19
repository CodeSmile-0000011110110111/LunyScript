using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript.BlockBuilders
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
			_token = script.CreateToken(_name, "Counter");
		}

		/// <summary>
		/// Sets the counter to fire once after the specified count.
		/// </summary>
		public CounterDurationBuilder In(Int32 targetCount) => new(_script, _name, _token, targetCount, Coroutine.Continuation.Finite);

		/// <summary>
		/// Sets the counter to fire repeatedly at the specified interval.
		/// </summary>
		public CounterDurationBuilder Every(Int32 interval) => new(_script, _name, _token, interval, Coroutine.Continuation.Repeating);
	}

	/// <summary>
	/// Builder step after counter amount is set. Next: specify unit (Frames/Heartbeats).
	/// </summary>
	public readonly struct CounterDurationBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;
		private readonly Int32 _amount;
		private readonly Coroutine.Continuation _continuation;

		internal CounterDurationBuilder(Script script, String name, BuilderToken token, Int32 amount, Coroutine.Continuation continuation)
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
			Coroutine.Options.ForCounter(_name, _amount, _continuation, Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based).
		/// </summary>
		public CounterFinalBuilder Heartbeats() => new(_script, _token,
			Coroutine.Options.ForCounter(_name, _amount, _continuation, Coroutine.Process.Heartbeat));
	}

	/// <summary>
	/// Final builder step for counters.
	/// </summary>
	public readonly struct CounterFinalBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly Coroutine.Options _options;

		internal CounterFinalBuilder(Script script, BuilderToken token, in Coroutine.Options options)
		{
			_script = script;
			_token = token;
			_options = options;
		}

		/// <summary>
		/// Completes the counter and specifies blocks to run when elapsed.
		/// </summary>
		public ICounterCoroutineBlock Do(params ScriptActionBlock[] blocks)
		{
			var options = _options with { OnElapsed = blocks };
			return (ICounterCoroutineBlock)BuilderUtility.Finalize(_script, in options, _token);
		}
	}
}
