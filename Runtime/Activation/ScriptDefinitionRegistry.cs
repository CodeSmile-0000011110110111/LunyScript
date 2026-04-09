using Luny;
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace LunyScript
{
	/// <summary>
	/// Discovers and manages LunyScript definitions.
	/// Supports both reflection-based discovery and manual registration.
	/// </summary>
	internal sealed class ScriptDefinitionRegistry
	{
		private readonly Dictionary<ScriptId, ScriptDefinition> _scriptsById = new();
		private readonly Dictionary<String, ScriptDefinition> _scriptsByName = new();

		public ScriptDefinitionRegistry() => DiscoverScripts();
		~ScriptDefinitionRegistry() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Discovers all LunyScript subclasses via reflection and registers them.
		/// </summary>
		public void DiscoverScripts()
		{
			var sw = Stopwatch.StartNew();

			var scriptTypes = TypeDiscovery.FindAll<Script>();
			foreach (var type in scriptTypes)
				RegisterScript(type);

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Registered {_scriptsById.Count} LunyScript(s) in {ms} ms.", this);
		}

		/// <summary>
		/// Manually registers a LunyScript type.
		/// </summary>
		private void RegisterScript(Type scriptType)
		{
			if (scriptType == null)
				throw new ArgumentNullException(nameof(scriptType));

			if (_scriptsByName.ContainsKey(scriptType.Name))
				throw new LunyScriptException($"{scriptType.Name}: duplicate type name");

			var definition = new ScriptDefinition(scriptType);
			_scriptsById[definition.ScriptId] = definition;
			_scriptsByName[definition.Name] = definition;

			LunyLogger.LogInfo($"{definition} registered", this);
		}

		/// <summary>
		/// Gets a script definition by ID.
		/// </summary>
		public ScriptDefinition GetById(ScriptId id)
		{
			_scriptsById.TryGetValue(id, out var definition);
			return definition;
		}

		/// <summary>
		/// Gets a script definition by name (for object binding).
		/// </summary>
		public ScriptDefinition GetByName(String name)
		{
			_scriptsByName.TryGetValue(name, out var definition);
			return definition;
		}

		/// <summary>
		/// Checks if a script with the given name exists.
		/// </summary>
		public Boolean HasScript(String name) => _scriptsByName.ContainsKey(name);

		internal void Shutdown()
		{
			_scriptsById.Clear();
			_scriptsByName.Clear();
			GC.SuppressFinalize(this);
		}

		public IReadOnlyCollection<String> GetNames() => _scriptsByName.Keys;
	}
}
