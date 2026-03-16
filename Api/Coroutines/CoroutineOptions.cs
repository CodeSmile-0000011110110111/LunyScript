using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Configuration options for creating a coroutine.
	/// </summary>
	internal record CoroutineOptions
	{
		private static Int32 s_UniqueNameID;

		internal Script Script;
		internal BuilderToken Token;

		public String Name { get; init; }
		public Double Duration { get; init; } // Used only by TimerCoroutine
		public Int32 CounterTarget => (Int32)Math.Round(Duration); // Used by CounterCoroutine
		public Int32 TimeSliceInterval { get; init; }
		public Int32 TimeSliceOffset { get; init; }
		public Coroutine.Continuation ContinuationMode { get; init; } = Coroutine.Continuation.Finite;
		public Coroutine.Process ProcessMode { get; init; } = Coroutine.Process.FrameUpdate;

		internal Boolean IsTimer { get; set; }
		internal Boolean IsCounter { get => !IsTimer; set => IsTimer = !value; }

		// Handlers
		public ActionBlock[] OnFrameUpdate { get; init; }
		public ActionBlock[] OnHeartbeat { get; init; }
		public ActionBlock[] OnElapsed { get; init; }
		public ActionBlock[] OnStarted { get; init; }
		public ActionBlock[] OnStopped { get; init; }
		public ActionBlock[] OnPaused { get; init; }
		public ActionBlock[] OnResumed { get; init; }

		public static CoroutineOptions ForCoroutine(String name, Double duration, Boolean repeating) => new()
		{
			Name = name,
			IsTimer = true,
			Duration = duration,
			ContinuationMode = repeating ? Coroutine.Continuation.Repeating : Coroutine.Continuation.Finite,
		};

		public static CoroutineOptions ForTimerCoroutine(String name, Double duration, Coroutine.Continuation continuationMode,
			ActionBlock[] processBlocks = null) => new()
		{
			Name = name,
			Duration = duration,
			ContinuationMode = continuationMode,
			ProcessMode = Coroutine.Process.FrameUpdate,
			OnFrameUpdate = processBlocks,
		};

		public static CoroutineOptions ForCounterCoroutine(String name, Int32 countTarget, Coroutine.Continuation continuationMode,
			Coroutine.Process processMode, ActionBlock[] processBlocks = null, ActionBlock[] elapsedBlocks = null) => new()
		{
			Name = name,
			Duration = countTarget,
			ContinuationMode = continuationMode,
			ProcessMode = processMode,
			OnFrameUpdate = processMode == Coroutine.Process.FrameUpdate ? processBlocks : null,
			OnHeartbeat = processMode == Coroutine.Process.Heartbeat ? processBlocks : null,
			OnElapsed = elapsedBlocks,
		};

		public static CoroutineOptions ForIntervalCoroutine(String name, Int32 interval, Int32 offset, Coroutine.Process processMode,
			ActionBlock[] processBlocks = null) => new()
		{
			Name = name ?? GenerateUniqueName(interval, offset, processMode),
			Duration = Math.Max(1, interval), // time-sliced intervals are always counters
			TimeSliceOffset = Math.Max(0, offset),
			ContinuationMode = Coroutine.Continuation.Repeating,
			ProcessMode = processMode,
			OnFrameUpdate = processMode == Coroutine.Process.FrameUpdate ? processBlocks : null,
			OnHeartbeat = processMode == Coroutine.Process.Heartbeat ? processBlocks : null,
		};

		private static String GenerateUniqueName(Int32 interval, Int32 delay, Coroutine.Process process) =>
			$"[{++s_UniqueNameID}]__Every({interval}).{process}().DelayBy({delay})";
	}
}
