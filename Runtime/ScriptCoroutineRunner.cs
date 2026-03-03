using Luny;
using Luny.Engine.Services;
using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;
using System.Collections.Generic;
using CoroutineBlock = LunyScript.Blocks.CoroutineBlock;

namespace LunyScript
{
	/// <summary>
	/// Manages coroutines and timers for a script context.
	/// Handles registration, advancing, and lifecycle integration.
	/// Called by LunyScriptRunner after non-coroutine updates.
	/// </summary>
	internal sealed class ScriptCoroutineRunner
	{
		private readonly Dictionary<String, CoroutineEntry> _registry = new();
		private readonly List<CoroutineEntry> _heartbeatOnly = new();
		private readonly List<CoroutineEntry> _frameOnly = new();

		private ILunyTimeService _time;

		/// <summary>
		/// Gets the count of registered coroutines.
		/// </summary>
		internal Int32 Count => _registry.Count;

		/// <summary>
		/// Gets all registered coroutine names.
		/// </summary>
		internal IEnumerable<String> Names => _registry.Keys;

		private static Boolean ShouldProcess(in CoroutineEntry entry, Int64 tickCount, Coroutine.Process mode)
		{
			if (!entry.IsTimeSliced)
				return true;

			if (entry.ProcessMode != mode)
				return true; // slicing applies only to the designated mode

			return (tickCount - entry.TimeSliceOffset) % entry.TimeSliceInterval == 0;
		}

		public ScriptCoroutineRunner(ScriptRuntimeContext runtimeContext) => _time = LunyEngine.Instance.Time;

		/// <summary>
		/// Registers a new coroutine. Throws if name already exists.
		/// </summary>
		internal ICoroutineBlock Register(in CoroutineOptions options)
		{
			if (_registry.ContainsKey(options.Name))
				throw new InvalidOperationException($"Coroutine '{options.Name}' already exists. Duplicate names are not allowed.");

			var coroutine = Coroutine.Create(options);
			var entry = new CoroutineEntry(coroutine, options);
			_registry[options.Name] = entry;

			switch (options.ProcessMode)
			{
				case Coroutine.Process.Heartbeat:
					_heartbeatOnly.Add(entry);
					break;
				case Coroutine.Process.FrameUpdate:
					_frameOnly.Add(entry);
					break;
			}

			return CoroutineBlock.Create(coroutine);
		}

		/// <summary>
		/// Gets an existing coroutine by name. Returns null if not found.
		/// </summary>
		internal Coroutine Get(String name) => _registry.TryGetValue(name, out var entry) ? entry.Coroutine : null;

		/// <summary>
		/// Checks if a coroutine with the given name exists.
		/// </summary>
		internal Boolean Exists(String name) => _registry.ContainsKey(name);

		/// <summary>
		/// Called on fixed step (heartbeat). Advances all running coroutines with OnHeartbeat sequences.
		/// Also advances count-based (heartbeat) coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnHeartbeat(ScriptRuntimeContext runtimeContext)
		{
			var heartbeatCount = _time?.HeartbeatCount ?? 0;
			for (var i = 0; i < _heartbeatOnly.Count; i++)
			{
				var entry = _heartbeatOnly[i];
				if (ShouldProcess(entry, heartbeatCount, Coroutine.Process.Heartbeat))
					CoroutineEntry.RunSequences(entry, entry.Coroutine.ProcessHeartbeat(), runtimeContext);
			}
		}

		/// <summary>
		/// Called on frame update. Advances all running time-based coroutines.
		/// Should be called from LunyScriptRunner AFTER non-coroutine updates.
		/// </summary>
		internal void OnFrameUpdate(ScriptRuntimeContext runtimeContext)
		{
			var frameCount = _time?.FrameCount ?? 0;
			for (var i = 0; i < _frameOnly.Count; i++)
			{
				var entry = _frameOnly[i];
				if (ShouldProcess(entry, frameCount, Coroutine.Process.FrameUpdate))
					CoroutineEntry.RunSequences(entry, entry.Coroutine.ProcessFrameUpdate(), runtimeContext);
			}
		}

		~ScriptCoroutineRunner() => LunyTraceLogger.LogInfoFinalized(this);

		public void Shutdown()
		{
			foreach (var entry in _registry.Values)
				entry.Coroutine.OnObjectDestroyed();

			// TODO: shouldn't clear, move collections to registry (same with Scheduler)
			_registry.Clear();
			_heartbeatOnly.Clear();
			_frameOnly.Clear();
			_time = null;

			GC.SuppressFinalize(this);
		}

		private sealed class CoroutineEntry
		{
			private const Int32 OnStarted = 0;
			private const Int32 OnResumed = 1;
			private const Int32 OnHeartbeat = 2;
			private const Int32 OnFrameUpdate = 3;
			private const Int32 OnPaused = 4;
			private const Int32 OnStopped = 5;
			private const Int32 OnElapsed = 6;

			public readonly Coroutine Coroutine;
			public readonly SequenceBlock[] Sequences;
			public readonly Int32 TimeSliceInterval;
			public readonly Int32 TimeSliceOffset;
			public readonly Coroutine.Process ProcessMode;
			public Boolean IsTimeSliced => TimeSliceInterval > 0;

			public static void RunSequences(in CoroutineEntry entry, Coroutine.Events events, ScriptRuntimeContext context)
			{
				if (events == Coroutine.Events.None)
					return;

				// intentional order in which events should fire within a single frame
				if (events.Has(Coroutine.Events.Started))
					LunyScriptRunner.Run(entry.Sequences[OnStarted], context);
				if (events.Has(Coroutine.Events.Resumed))
					LunyScriptRunner.Run(entry.Sequences[OnResumed], context);

				if (events.Has(Coroutine.Events.Heartbeat))
					LunyScriptRunner.Run(entry.Sequences[OnHeartbeat], context);
				if (events.Has(Coroutine.Events.FrameUpdate))
					LunyScriptRunner.Run(entry.Sequences[OnFrameUpdate], context);

				if (events.Has(Coroutine.Events.Elapsed))
					LunyScriptRunner.Run(entry.Sequences[OnElapsed], context);

				if (events.Has(Coroutine.Events.Paused))
					LunyScriptRunner.Run(entry.Sequences[OnPaused], context);
				if (events.Has(Coroutine.Events.Stopped))
					LunyScriptRunner.Run(entry.Sequences[OnStopped], context);
			}

			public CoroutineEntry(Coroutine coroutine, in CoroutineOptions options)
			{
				Coroutine = coroutine;
				TimeSliceInterval = options.TimeSliceInterval;
				TimeSliceOffset = options.TimeSliceOffset;
				ProcessMode = options.ProcessMode;

				Sequences = new SequenceBlock[7];
				Sequences[OnStarted] = SequenceBlock.TryCreate(options.OnStarted);
				Sequences[OnResumed] = SequenceBlock.TryCreate(options.OnResumed);
				Sequences[OnHeartbeat] = SequenceBlock.TryCreate(options.OnHeartbeat);
				Sequences[OnFrameUpdate] = SequenceBlock.TryCreate(options.OnFrameUpdate);
				Sequences[OnPaused] = SequenceBlock.TryCreate(options.OnPaused);
				Sequences[OnStopped] = SequenceBlock.TryCreate(options.OnStopped);
				Sequences[OnElapsed] = SequenceBlock.TryCreate(options.OnElapsed);
			}
		}
	}
}
