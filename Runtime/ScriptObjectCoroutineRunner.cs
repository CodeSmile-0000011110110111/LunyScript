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
	internal sealed class ScriptObjectCoroutineRunner
	{
		private readonly Dictionary<String, CoroutineEntry> _registry = new();
		private readonly List<CoroutineEntry> _heartbeatOnly = new();
		private readonly List<CoroutineEntry> _frameOnly = new();
		private readonly List<CoroutineEntry> _always = new();

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

		private static void RunSequences(in CoroutineEntry entry, CoroutineEvents events, ScriptRuntimeContext context)
		{
			if (events == CoroutineEvents.None)
				return;

			// intentional order
			if (events.Has(CoroutineEvents.Started))
				LunyScriptRunner.Run(entry.Sequences[0], context);
			if (events.Has(CoroutineEvents.Resumed))
				LunyScriptRunner.Run(entry.Sequences[1], context);
			if (events.Has(CoroutineEvents.Heartbeat))
				LunyScriptRunner.Run(entry.Sequences[2], context);
			if (events.Has(CoroutineEvents.FrameUpdate))
				LunyScriptRunner.Run(entry.Sequences[3], context);
			if (events.Has(CoroutineEvents.Paused))
				LunyScriptRunner.Run(entry.Sequences[4], context);
			if (events.Has(CoroutineEvents.Stopped))
				LunyScriptRunner.Run(entry.Sequences[5], context);
			if (events.Has(CoroutineEvents.Elapsed))
				LunyScriptRunner.Run(entry.Sequences[6], context);
		}

		public ScriptObjectCoroutineRunner(ScriptRuntimeContext runtimeContext) => _time = LunyEngine.Instance.Time;

		/// <summary>
		/// Registers a new coroutine. Throws if name already exists.
		/// </summary>
		internal ICoroutineBlock Register(IScript script, in Coroutine.Options options)
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
				case Coroutine.Process.Always:
					_always.Add(entry);
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
					RunSequences(entry, entry.Coroutine.ProcessHeartbeat(), runtimeContext);
			}

			for (var i = 0; i < _always.Count; i++)
			{
				var entry = _always[i];
				RunSequences(entry, entry.Coroutine.ProcessHeartbeat(), runtimeContext);
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
					RunSequences(entry, entry.Coroutine.ProcessFrameUpdate(), runtimeContext);
			}

			for (var i = 0; i < _always.Count; i++)
			{
				var entry = _always[i];
				RunSequences(entry, entry.Coroutine.ProcessFrameUpdate(), runtimeContext);
			}
		}

		~ScriptObjectCoroutineRunner() => LunyTraceLogger.LogInfoFinalized(this);

		public void Shutdown()
		{
			foreach (var entry in _registry.Values)
				entry.Coroutine.OnObjectDestroyed();

			// TODO: shouldn't clear, move collections to registry (same with Scheduler)
			_registry.Clear();
			_heartbeatOnly.Clear();
			_frameOnly.Clear();
			_always.Clear();
			_time = null;

			GC.SuppressFinalize(this);
		}

		private sealed class CoroutineEntry
		{
			public readonly Coroutine Coroutine;
			public readonly SequenceBlock[] Sequences;
			public readonly Int32 TimeSliceInterval;
			public readonly Int32 TimeSliceOffset;
			public readonly Coroutine.Process ProcessMode;
			public Boolean IsTimeSliced => TimeSliceInterval > 0;

			public CoroutineEntry(Coroutine coroutine, in Coroutine.Options options)
			{
				Coroutine = coroutine;
				TimeSliceInterval = options.TimeSliceInterval;
				TimeSliceOffset = options.TimeSliceOffset;
				ProcessMode = options.ProcessMode;

				Sequences = new SequenceBlock[7];
				Sequences[0] = SequenceBlock.TryCreate(options.OnStarted);
				Sequences[1] = SequenceBlock.TryCreate(options.OnResumed);
				Sequences[2] = SequenceBlock.TryCreate(options.OnHeartbeat);
				Sequences[3] = SequenceBlock.TryCreate(options.OnFrameUpdate);
				Sequences[4] = SequenceBlock.TryCreate(options.OnPaused);
				Sequences[5] = SequenceBlock.TryCreate(options.OnStopped);
				Sequences[6] = SequenceBlock.TryCreate(options.OnElapsed);
			}
		}
	}
}
