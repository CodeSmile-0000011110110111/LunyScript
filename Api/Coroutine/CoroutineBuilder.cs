using Luny;
using LunyScript.Blocks;
using System;

namespace LunyScript.Api.Coroutine
{
	/// <summary>
	/// Entry point for the Coroutine fluent builder chain.
	/// Usage: Coroutine("name").For(3).Seconds().OnFrameUpdate(blocks).WhenElapsed(blocks);
	/// </summary>
	public readonly struct CoroutineBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;

		internal CoroutineBuilder(Script script, String name)
		{
			_script = script ?? throw new ArgumentNullException(nameof(script));
			_name = !String.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Coroutine name is null or empty", nameof(name));
			_token = script.CreateToken(_name, "Coroutine");
		}

		/// <summary>
		/// Sets the coroutine duration. Returns a builder to specify the time unit.
		/// </summary>
		public CoroutineDurationBuilder For(Double duration) => new(_script, _name, _token, duration);

		/// <summary>
		/// Creates an open-ended coroutine (runs until stopped) which runs the blocks every frame.
		/// </summary>
		public OpenEndedFrameCoroutineBuilder OnFrameUpdate(params ScriptActionBlock[] blocks) => new(_script, _token,
			Coroutines.Coroutine.Options.ForOpenEnded(_name, Coroutines.Coroutine.Process.FrameUpdate) with { OnFrameUpdate = blocks });

		/// <summary>
		/// Creates an open-ended coroutine (runs until stopped) which runs the blocks every heartbeat (fixed step).
		/// </summary>
		public OpenEndedHeartbeatCoroutineBuilder OnHeartbeat(params ScriptActionBlock[] blocks) => new(_script, _token,
			Coroutines.Coroutine.Options.ForOpenEnded(_name, Coroutines.Coroutine.Process.Heartbeat) with { OnHeartbeat = blocks });


		internal static ICoroutineBlock Finalize(Script script, in Coroutines.Coroutine.Options options, BuilderToken token)
		{
			if (options.OnFrameUpdate == null && options.OnHeartbeat == null && options.OnElapsed == null &&
			    options.OnStarted == null && options.OnStopped == null && options.OnPaused == null && options.OnResumed == null)
			{
				LunyLogger.LogWarning($"{nameof(Coroutines.Coroutine)} '{options.Name}' was finalized without any action blocks. " +
				                      "It will run but perform no actions.", script);
			}

			var scriptInternal = script;
			var block = scriptInternal.RuntimeContext.Coroutines.Register(in options);
			scriptInternal.FinalizeToken(token);
			return block;
		}
	}

	/// <summary>
	/// Builder step after duration amount is set. Next: specify time unit.
	/// </summary>
	public readonly struct CoroutineDurationBuilder
	{
		private readonly Script _script;
		private readonly String _name;
		private readonly BuilderToken _token;
		private readonly Double _duration;

		internal CoroutineDurationBuilder(Script script, String name, BuilderToken token, Double duration)
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
		public FiniteFrameCoroutineBuilder Seconds() => new(_script, _token,
			Coroutines.Coroutine.Options.ForTimer(_name, _duration, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in milliseconds (time-based).
		/// </summary>
		public FiniteFrameCoroutineBuilder Milliseconds() => new(_script, _token,
			Coroutines.Coroutine.Options.ForTimer(_name, _duration / 1000.0, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in minutes (time-based).
		/// </summary>
		public FiniteFrameCoroutineBuilder Minutes() => new(_script, _token,
			Coroutines.Coroutine.Options.ForTimer(_name, _duration * 60.0, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.FrameUpdate));

		/// <summary>
		/// Duration in heartbeats (count-based, counts fixed steps).
		/// </summary>
		public FiniteHeartbeatCoroutineBuilder Heartbeats() => new(_script, _token,
			Coroutines.Coroutine.Options.ForCounter(_name, (Int32)_duration, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.Heartbeat));

		/// <summary>
		/// Duration in frames (count-based, counts frames).
		/// </summary>
		public FiniteFrameCoroutineBuilder Frames() => new(_script, _token,
			Coroutines.Coroutine.Options.ForCounter(_name, (Int32)_duration, Coroutines.Coroutine.Continuation.Finite, Coroutines.Coroutine.Process.FrameUpdate));
	}

	/// <summary>
	/// Builder for finite coroutines running on frame updates.
	/// </summary>
	public readonly struct FiniteFrameCoroutineBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly Coroutines.Coroutine.Options _options;

		internal FiniteFrameCoroutineBuilder(Script script, BuilderToken token, in Coroutines.Coroutine.Options options)
		{
			_script = script;
			_token = token;
			_options = options;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}

		public FiniteFrameCoroutineBuilder OnFrameUpdate(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) });

		public FiniteFrameCoroutineBuilder WhenStarted(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public FiniteFrameCoroutineBuilder WhenStopped(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public FiniteFrameCoroutineBuilder WhenPaused(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public FiniteFrameCoroutineBuilder WhenResumed(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public ICoroutineBlock WhenElapsed(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnElapsed = BuilderUtility.Append(_options.OnElapsed, blocks) }, _token);

		public ICoroutineBlock Do(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) }, _token);
	}

	/// <summary>
	/// Builder for finite coroutines running on heartbeats.
	/// </summary>
	public readonly struct FiniteHeartbeatCoroutineBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly Coroutines.Coroutine.Options _options;

		internal FiniteHeartbeatCoroutineBuilder(Script script, BuilderToken token, in Coroutines.Coroutine.Options options)
		{
			_script = script;
			_token = token;
			_options = options;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}

		public FiniteHeartbeatCoroutineBuilder OnHeartbeat(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) });

		public FiniteHeartbeatCoroutineBuilder WhenStarted(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public FiniteHeartbeatCoroutineBuilder WhenStopped(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public FiniteHeartbeatCoroutineBuilder WhenPaused(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public FiniteHeartbeatCoroutineBuilder WhenResumed(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public ICoroutineBlock WhenElapsed(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnElapsed = BuilderUtility.Append(_options.OnElapsed, blocks) }, _token);

		public ICoroutineBlock Do(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) }, _token);
	}

	/// <summary>
	/// Builder for open-ended coroutines running on frame updates.
	/// </summary>
	public readonly struct OpenEndedFrameCoroutineBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly Coroutines.Coroutine.Options _options;

		internal OpenEndedFrameCoroutineBuilder(Script script, BuilderToken token, in Coroutines.Coroutine.Options options)
		{
			_script = script;
			_token = token;
			_options = options;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}

		public OpenEndedFrameCoroutineBuilder WhenStarted(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public OpenEndedFrameCoroutineBuilder WhenStopped(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public OpenEndedFrameCoroutineBuilder WhenPaused(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public OpenEndedFrameCoroutineBuilder WhenResumed(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public ICoroutineBlock Do(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnFrameUpdate = BuilderUtility.Append(_options.OnFrameUpdate, blocks) }, _token);
	}

	/// <summary>
	/// Builder for open-ended coroutines running on heartbeats.
	/// </summary>
	public readonly struct OpenEndedHeartbeatCoroutineBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly Coroutines.Coroutine.Options _options;

		internal OpenEndedHeartbeatCoroutineBuilder(Script script, BuilderToken token, in Coroutines.Coroutine.Options options)
		{
			_script = script;
			_token = token;
			_options = options;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => CoroutineBuilder.Finalize(capturedScript, capturedOptions, token));
		}

		public OpenEndedHeartbeatCoroutineBuilder WhenStarted(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStarted = BuilderUtility.Append(_options.OnStarted, blocks) });

		public OpenEndedHeartbeatCoroutineBuilder WhenStopped(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnStopped = BuilderUtility.Append(_options.OnStopped, blocks) });

		public OpenEndedHeartbeatCoroutineBuilder WhenPaused(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnPaused = BuilderUtility.Append(_options.OnPaused, blocks) });

		public OpenEndedHeartbeatCoroutineBuilder WhenResumed(params ScriptActionBlock[] blocks) => new(_script, _token,
			_options with { OnResumed = BuilderUtility.Append(_options.OnResumed, blocks) });

		public ICoroutineBlock Do(params ScriptActionBlock[] blocks) => CoroutineBuilder.Finalize(_script,
			_options with { OnHeartbeat = BuilderUtility.Append(_options.OnHeartbeat, blocks) }, _token);
	}

}
