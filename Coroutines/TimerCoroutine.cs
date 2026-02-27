using Luny;
using LunyScript.Api.Coroutine;
using System;

namespace LunyScript.Coroutines
{
	/// <summary>
	/// Coroutine that elapses after a specific duration in seconds.
	/// </summary>
	internal sealed class TimerCoroutine : Coroutine
	{
		private Timer _timer;
		private Boolean _elapsedThisTick;

		internal Double TimeScale
		{
			get => _timer.TimeScale;
			set => _timer.TimeScale = Math.Max(0.0, value);
		}

		public TimerCoroutine(in CoroutineOptions options)
			: base(options)
		{
			var duration = Math.Max(0.0, options.TimerDurationInSeconds);
			_timer = Timer.FromSeconds(duration);
			_timer.AutoRepeat = options.ContinuationMode == Continuation.Repeating;
			_timer.OnElapsed += () => _elapsedThisTick = true;
		}

		protected override void OnStarted() => _timer.Start();
		protected override void OnStopped() => _timer.Stop();
		protected override void OnPaused() {} // no need to pause timer: Consume* methods won't be called when paused
		protected override void OnResumed() {}

		protected override Boolean ConsumeFrameUpdate()
		{
			_elapsedThisTick = false;
			_timer.Tick(LunyEngine.Instance.Time.DeltaTime);
			return _elapsedThisTick;
		}

		protected override Boolean ConsumeHeartbeat() => throw new NotImplementedException(nameof(ConsumeHeartbeat));

		public override String ToString()
		{
			var progress = $"{_timer.Current:F2}s/{_timer.Duration:F2}s";
			return $"{GetType().Name}({Name}, {State}, {progress})";
		}
	}
}
