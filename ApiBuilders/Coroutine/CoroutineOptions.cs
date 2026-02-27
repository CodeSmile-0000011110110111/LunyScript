using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Coroutine
{
	/// <summary>
	/// Configuration options for creating a coroutine.
	/// </summary>
	internal record CoroutineOptions
	{
		private static Int32 s_UniqueNameID;

		public String Name { get; init; }
		public Double TimerDurationInSeconds { get; init; } // Used only by TimerCoroutine
		public Int32 CounterTarget { get; init; } // Used by CounterCoroutine
		public Int32 TimeSliceInterval { get; init; }
		public Int32 TimeSliceOffset { get; init; }
		public Coroutines.Coroutine.Continuation ContinuationMode { get; init; } = Coroutines.Coroutine.Continuation.Finite;
		public Coroutines.Coroutine.Process ProcessMode { get; init; } = Coroutines.Coroutine.Process.Always;

		internal Boolean IsTimer => TimerDurationInSeconds > 0;
		internal Boolean IsCounter => CounterTarget > 0;

		// Handlers
		public ScriptActionBlock[] OnFrameUpdate { get; init; }
		public ScriptActionBlock[] OnHeartbeat { get; init; }
		public ScriptActionBlock[] OnElapsed { get; init; }
		public ScriptActionBlock[] OnStarted { get; init; }
		public ScriptActionBlock[] OnStopped { get; init; }
		public ScriptActionBlock[] OnPaused { get; init; }
		public ScriptActionBlock[] OnResumed { get; init; }

		public static CoroutineOptions ForOpenEnded(String name, Coroutines.Coroutine.Process processMode) =>
			new() { Name = name, ProcessMode = processMode };

		public static CoroutineOptions ForTimer(String name, Double duration, Coroutines.Coroutine.Continuation continuationMode,
			Coroutines.Coroutine.Process processMode) => new()
		{
			Name = name,
			TimerDurationInSeconds = duration,
			ContinuationMode = continuationMode,
			ProcessMode = processMode,
		};

		public static CoroutineOptions ForCounter(String name, Int32 countTarget, Coroutines.Coroutine.Continuation continuationMode,
			Coroutines.Coroutine.Process processMode) => new()
		{
			Name = name,
			CounterTarget = countTarget + 1, // since we increment before evaluate, actual target is +1 than user provides
			ContinuationMode = continuationMode,
			ProcessMode = processMode,
		};

		public static CoroutineOptions ForEveryInterval(String name, Int32 interval, Int32 offset, Coroutines.Coroutine.Process processMode,
			ScriptActionBlock[] doBlocks) => new()
		{
			Name = name ?? GenerateUniqueName(interval, offset, processMode),
			CounterTarget = interval, // time-sliced intervals are always counters
			TimeSliceInterval = Math.Max(1, interval),
			TimeSliceOffset = Math.Max(0, offset),
			ProcessMode = processMode,
			OnFrameUpdate = processMode == Coroutines.Coroutine.Process.FrameUpdate ? doBlocks : null,
			OnHeartbeat = processMode == Coroutines.Coroutine.Process.Heartbeat ? doBlocks : null,
		};

		private static String GenerateUniqueName(Int32 interval, Int32 delay, Coroutines.Coroutine.Process process) =>
			$"[{++s_UniqueNameID}]__Every({interval}).{process}().DelayBy({delay})";
	}
}
