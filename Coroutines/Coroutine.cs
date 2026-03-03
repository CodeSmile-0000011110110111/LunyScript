using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Base class for coroutines and timers with runtime state and control methods.
	/// </summary>
	internal class Coroutine
	{
		private static readonly String[] s_StateNames = Enum.GetNames(typeof(CoroutineState));

		private readonly String _name;
		private CoroutineState _state = CoroutineState.New;
		private Events _pendingEvents = Events.None;

		internal String Name => _name;
		internal String State => s_StateNames[(Int32)_state];
		private Continuation ContinuationMode { get; } = Continuation.Finite;
		private Boolean IsNew => _state == CoroutineState.New;
		private Boolean IsRunning => _state == CoroutineState.Running;
		private Boolean IsPaused => _state == CoroutineState.Paused;
		private Boolean IsStopped => _state == CoroutineState.Stopped;
		private Boolean IsElapsed => _state == CoroutineState.Elapsed;

		/// <summary>
		/// Factory method to create specialized coroutine instances.
		/// </summary>
		internal static Coroutine Create(in CoroutineOptions options) => options.IsTimer ? new TimerCoroutine(options) :
			options.IsCounter ? new CounterCoroutine(options) : new Coroutine(options);

		private Coroutine() {} // hide default ctor

		protected Coroutine(in CoroutineOptions options)
		{
			if (String.IsNullOrEmpty(options.Name))
				throw new ArgumentException("Coroutine name cannot be null or empty", nameof(options.Name));

			_name = options.Name;
			ContinuationMode = options.ContinuationMode;
		}

		/// <summary>
		/// Starts or restarts the coroutine.
		/// </summary>
		internal void Start(Boolean fireStartStopEvents = true)
		{
			if (!IsNew)
				Stop(fireStartStopEvents);

			//LunyLogger.LogInfo($"{nameof(Start)}({_name})", this);
			_state = CoroutineState.Running;
			if (fireStartStopEvents)
				_pendingEvents |= Events.Started;

			OnStarted();
		}

		private void StartWithoutEvents() => Start(false);

		private void StartIfNew()
		{
			if (IsNew)
				Start();
		}

		/// <summary>
		/// Stops the coroutine and resets state.
		/// Returns true if the coroutine was running or paused (indicating Stopped event should fire).
		/// </summary>
		internal void Stop(Boolean fireStopEvent = true)
		{
			StartIfNew();
			if (IsStopped || IsElapsed)
				return;

			//LunyLogger.LogInfo($"{nameof(Stop)}({_name})", this);
			_state = CoroutineState.Stopped;
			if (fireStopEvent)
				_pendingEvents |= Events.Stopped;

			OnStopped();
		}

		private void StopWithoutEvent() => Stop(false);

		/// <summary>
		/// Pauses the coroutine, preserving current elapsed time.
		/// Returns true if the coroutine was running (indicating Paused event should fire).
		/// </summary>
		internal void Pause()
		{
			StartIfNew();
			if (IsPaused || IsStopped || IsElapsed)
				return;

			//LunyLogger.LogInfo($"{nameof(Pause)}({_name})", this);
			_state = CoroutineState.Paused;
			_pendingEvents |= Events.Paused;

			OnPaused();
		}

		/// <summary>
		/// Resumes a paused coroutine.
		/// Returns true if the coroutine was paused (indicating Resumed event should fire).
		/// </summary>
		internal void Resume()
		{
			if (IsRunning || IsStopped || IsElapsed || IsNew)
				return;

			//LunyLogger.LogInfo($"{nameof(Resume)}({_name})", this);
			_state = CoroutineState.Running;
			_pendingEvents |= Events.Resumed;

			OnResumed();
		}

		/// <summary>
		/// Stop coroutine when object is destroyed.
		/// </summary>
		internal void OnObjectDestroyed() => StopWithoutEvent();

		internal Events GetAndClearPendingEvents()
		{
			var events = _pendingEvents;
			_pendingEvents = Events.None;
			return events;
		}

		/// <summary>
		/// Updates coroutine heartbeat state. Returns events that occurred.
		/// </summary>
		internal Events ProcessHeartbeat()
		{
			StartIfNew();
			if (IsRunning && !IsElapsed)
			{
				_pendingEvents |= Events.Heartbeat;
				if (ConsumeHeartbeat())
					SetElapsedOrRestart();
			}

			return GetAndClearPendingEvents();
		}

		/// <summary>
		/// Updates coroutine frame update state. Returns events that occurred.
		/// </summary>
		internal Events ProcessFrameUpdate()
		{
			StartIfNew();
			if (IsRunning && !IsElapsed)
			{
				_pendingEvents |= Events.FrameUpdate;
				if (ConsumeFrameUpdate())
					SetElapsedOrRestart();
			}

			return GetAndClearPendingEvents();
		}

		private void SetElapsedOrRestart()
		{
			_pendingEvents |= Events.Elapsed;

			if (ContinuationMode == Continuation.Repeating)
				StartWithoutEvents();
			else
			{
				_state = CoroutineState.Elapsed;
				OnElapsed();
			}
		}

		protected virtual void OnStarted() {}

		protected virtual void OnStopped() {}

		// reserved for future use if a subclass needs these callbacks
		protected virtual void OnPaused() {}
		protected virtual void OnResumed() {}
		protected virtual void OnElapsed() {}

		protected virtual Boolean ConsumeFrameUpdate() => false;
		protected virtual Boolean ConsumeHeartbeat() => false;
		public override String ToString() => $"{GetType().Name}({Name}, {State})";

		/// <summary>
		/// Represents the execution state of a coroutine or timer.
		/// </summary>
		private enum CoroutineState
		{
			/// <summary>
			/// Coroutine has not started yet.
			/// </summary>
			New,

			/// <summary>
			/// Coroutine is not running and has no accumulated time.
			/// </summary>
			Stopped,

			/// <summary>
			/// Coroutine is actively running and accumulating time.
			/// </summary>
			Running,

			/// <summary>
			/// Coroutine is frozen at current time, will resume when unpaused.
			/// </summary>
			Paused,

			/// <summary>
			/// Coroutine has run to end. It can be Started again, but not stopped, paused or resumed.
			/// </summary>
			Elapsed,
		}

		[Flags]
		internal enum Events
		{
			None = 0,
			Started = 1 << 0,
			Resumed = 1 << 1,
			Heartbeat = 1 << 2,
			FrameUpdate = 1 << 3,
			Paused = 1 << 4,
			Stopped = 1 << 5,
			Elapsed = 1 << 6,
		}

		/// <summary>
		/// For Counter coroutines: Whether it counts frames or heartbeats.
		/// </summary>
		internal enum Process
		{
			Always,
			FrameUpdate,
			Heartbeat,
		}

		/// <summary>
		/// Coroutine behaviour after it ran to completion.
		/// </summary>
		internal enum Continuation
		{
			Finite,
			Repeating,
		}
	}
}
