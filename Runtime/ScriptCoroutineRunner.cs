using Luny;
using LunyScript.Api;
using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;
using System.Collections.Generic;

namespace LunyScript
{
	/// <summary>
	/// Manages coroutines and timers for a script context.
	/// Handles registration, advancing, and lifecycle integration.
	/// Called by LunyScriptRunner after non-coroutine updates.
	/// </summary>
	internal sealed class ScriptCoroutineRunner
	{
		private readonly Dictionary<String, CoroutineBlock> _registry = new();
		private readonly List<CoroutineBlock> _heartbeatOnly = new();
		private readonly List<CoroutineBlock> _frameOnly = new();

		/// <summary>
		/// Gets the count of registered coroutines.
		/// </summary>
		internal Int32 Count => _registry.Count;

		/// <summary>
		/// Gets all registered coroutine names.
		/// </summary>
		internal IEnumerable<String> Names => _registry.Keys;

		public ScriptCoroutineRunner(ScriptRuntimeContext runtimeContext) {}

		/// <summary>
		/// Registers a new coroutine. Throws if name already exists.
		/// </summary>
		internal ICoroutineBlock Register(in CoroutineOptions options)
		{
			if (_registry.ContainsKey(options.Name))
				throw new InvalidOperationException($"Coroutine '{options.Name}' already exists. Duplicate names are not allowed.");

			var block = CoroutineBlock.Create(options);
			_registry[options.Name] = block;

			switch (options.ProcessMode)
			{
				case Coroutine.UpdateMode.Heartbeat:
					_heartbeatOnly.Add(block);
					break;
				case Coroutine.UpdateMode.FrameUpdate:
					_frameOnly.Add(block);
					break;
			}

			return block;
		}

		/// <summary>
		/// Gets an existing coroutine by name. Returns null if not found.
		/// </summary>
		internal Coroutine Get(String name) => _registry.TryGetValue(name, out var block) ? block.Coroutine : null;

		/// <summary>
		/// Checks if a coroutine with the given name exists.
		/// </summary>
		internal Boolean Exists(String name) => _registry.ContainsKey(name);

		/// <summary>
		/// Called on fixed step (heartbeat). Advances all heartbeat coroutines.
		/// </summary>
		internal void OnHeartbeat(ScriptRuntimeContext runtimeContext)
		{
			for (var i = 0; i < _heartbeatOnly.Count; i++)
				_heartbeatOnly[i].Execute(runtimeContext);
		}

		/// <summary>
		/// Called on frame update. Advances all frame-update coroutines.
		/// </summary>
		internal void OnFrameUpdate(ScriptRuntimeContext runtimeContext)
		{
			for (var i = 0; i < _frameOnly.Count; i++)
				_frameOnly[i].Execute(runtimeContext);
		}

		~ScriptCoroutineRunner() => LunyTraceLogger.LogInfoFinalized(this);

		public void Shutdown()
		{
			foreach (var block in _registry.Values)
				block.Coroutine.OnObjectDestroyed();

			_registry.Clear();
			_heartbeatOnly.Clear();
			_frameOnly.Clear();

			GC.SuppressFinalize(this);
		}
	}
}
