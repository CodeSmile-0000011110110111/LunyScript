using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Coroutines
{
	[Flags]
	internal enum CoroutineEvents
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

	internal static class CoroutineEventsExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean Has(this CoroutineEvents events, CoroutineEvents flag) => (events & flag) != 0;
	}

	/// <summary>
	/// Base class for coroutines and timers with runtime state and control methods.
	/// </summary>
	internal partial class Coroutine
	{
		private static readonly String[] s_StateNames = Enum.GetNames(typeof(CoroutineState));

		private readonly String _name;
		private CoroutineState _state = CoroutineState.New;
		private CoroutineEvents _pendingEvents = CoroutineEvents.None;

		internal String Name => _name;
		internal String State => s_StateNames[(Int32)_state];
		private Continuation ContinuationMode { get; } = Continuation.Finite;
		private Boolean IsStopped => _state == CoroutineState.Stopped;
		private Boolean IsRunning => _state == CoroutineState.Running;
		private Boolean IsPaused => _state == CoroutineState.Paused;
		private Boolean IsNew => _state == CoroutineState.New;

		/// <summary>
		/// Factory method to create specialized coroutine instances.
		/// </summary>
		public static Coroutine Create(in Options options) => options.IsTimer ? new TimerCoroutine(options) :
			options.IsCounter ? new CounterCoroutine(options) : new Coroutine(options);

		// return options.IsTimer ? new TimerCoroutine(options) :
		// 	options.IsCounter ? new CounterCoroutine(options) :
		// 	new Coroutine(options);
		private Coroutine() {} // hide default ctor

		protected Coroutine(in Options options)
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
				_pendingEvents |= CoroutineEvents.Started;
			OnStarted();
		}

		private void StartWithoutEvents() => Start(false);

		/// <summary>
		/// Stops the coroutine and resets state.
		/// Returns true if the coroutine was running or paused (indicating Stopped event should fire).
		/// </summary>
		internal void Stop(Boolean fireStopEvent = true)
		{
			StartIfNew();
			if (IsStopped)
				return;

			//LunyLogger.LogInfo($"{nameof(Stop)}({_name})", this);
			_state = CoroutineState.Stopped;
			if (fireStopEvent)
				_pendingEvents |= CoroutineEvents.Stopped;
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
			if (IsPaused)
				return;

			//LunyLogger.LogInfo($"{nameof(Pause)}({_name})", this);
			_state = CoroutineState.Paused;
			_pendingEvents |= CoroutineEvents.Paused;
			OnPaused();
		}

		/// <summary>
		/// Resumes a paused coroutine.
		/// Returns true if the coroutine was paused (indicating Resumed event should fire).
		/// </summary>
		internal void Resume()
		{
			if (IsRunning || IsNew)
				return;

			//LunyLogger.LogInfo($"{nameof(Resume)}({_name})", this);
			_state = CoroutineState.Running;
			_pendingEvents |= CoroutineEvents.Resumed;
			OnResumed();
		}

		/// <summary>
		/// Stop coroutine when object is destroyed.
		/// </summary>
		internal void OnObjectDestroyed() => StopWithoutEvent();

		internal CoroutineEvents GetAndClearPendingEvents()
		{
			var events = _pendingEvents;
			_pendingEvents = CoroutineEvents.None;
			return events;
		}

		/// <summary>
		/// Updates coroutine heartbeat state. Returns events that occurred.
		/// </summary>
		internal CoroutineEvents ProcessHeartbeat()
		{
			StartIfNew();
			if (IsRunning)
			{
				_pendingEvents |= CoroutineEvents.Heartbeat;
				if (ConsumeHeartbeat())
				{
					_pendingEvents |= CoroutineEvents.Elapsed;
					ApplyContinuation();
				}
			}

			return GetAndClearPendingEvents();
		}

		/// <summary>
		/// Updates coroutine frame update state. Returns events that occurred.
		/// </summary>
		internal CoroutineEvents ProcessFrameUpdate()
		{
			StartIfNew();
			if (IsRunning)
			{
				_pendingEvents |= CoroutineEvents.FrameUpdate;
				if (ConsumeFrameUpdate())
				{
					_pendingEvents |= CoroutineEvents.Elapsed;
					ApplyContinuation();
				}
			}

			return GetAndClearPendingEvents();
		}

		private void StartIfNew()
		{
			if (IsNew)
				Start();
		}

		private void ApplyContinuation()
		{
			if (ContinuationMode == Continuation.Repeating)
				StartWithoutEvents();
			else
				StopWithoutEvent();
		}

		protected virtual void OnStarted() {}
		protected virtual void OnStopped() {}
		protected virtual void OnPaused() {}
		protected virtual void OnResumed() {}
		protected virtual Boolean ConsumeFrameUpdate() => false;
		protected virtual Boolean ConsumeHeartbeat() => false;
		public override String ToString() => $"{GetType().Name}({Name}, {State})";
	}
}
