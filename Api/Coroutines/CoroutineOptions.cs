using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript
{
	public interface ICoroutineBuilderState {}

	/// <summary>Frame-update unit — <c>WhenStarted/Stopped/Paused/Resumed</c> and <c>Do()</c> available.</summary>
	public interface ICoroutineFrameUnit : ICoroutineBuilderState {}

	/// <summary>Heartbeat unit — <c>WhenStarted/Stopped/Paused/Resumed</c> and <c>Do()</c> available.</summary>
	public interface ICoroutineHeartbeatUnit : ICoroutineBuilderState {}

	/// <summary>Shared base for both coroutine unit states — enables shared lifecycle extension methods.</summary>
	public interface ICoroutineReadyUnit : ICoroutineBuilderState {}

	public struct CoroutineFrameUnit : ICoroutineFrameUnit, ICoroutineReadyUnit {}
	public struct CoroutineHeartbeatUnit : ICoroutineHeartbeatUnit, ICoroutineReadyUnit {}

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
		public Coroutine.Continuation ContinuationMode { get; init; } = Coroutine.Continuation.Finite;
		public Coroutine.Process ProcessMode { get; init; } = Coroutine.Process.Always;

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

		public static CoroutineOptions ForOpenEnded(String name, Coroutine.Process processMode) =>
			new() { Name = name, ProcessMode = processMode };

		public static CoroutineOptions ForTimer(String name, Double duration, Coroutine.Continuation continuationMode,
			Coroutine.Process processMode) => new()
		{
			Name = name,
			TimerDurationInSeconds = duration,
			ContinuationMode = continuationMode,
			ProcessMode = processMode,
		};

		public static CoroutineOptions ForCounter(String name, Int32 countTarget, Coroutine.Continuation continuationMode,
			Coroutine.Process processMode) => new()
		{
			Name = name,
			CounterTarget = countTarget + 1, // since we increment before evaluate, actual target is +1 than user provides
			ContinuationMode = continuationMode,
			ProcessMode = processMode,
		};

		public static CoroutineOptions ForEveryInterval(String name, Int32 interval, Int32 offset, Coroutine.Process processMode,
			ScriptActionBlock[] doBlocks) => new()
		{
			Name = name ?? GenerateUniqueName(interval, offset, processMode),
			CounterTarget = interval, // time-sliced intervals are always counters
			TimeSliceInterval = Math.Max(1, interval),
			TimeSliceOffset = Math.Max(0, offset),
			ProcessMode = processMode,
			OnFrameUpdate = processMode == Coroutine.Process.FrameUpdate ? doBlocks : null,
			OnHeartbeat = processMode == Coroutine.Process.Heartbeat ? doBlocks : null,
		};

		private static String GenerateUniqueName(Int32 interval, Int32 delay, Coroutine.Process process) =>
			$"[{++s_UniqueNameID}]__Every({interval}).{process}().DelayBy({delay})";
	}
}
