using Luny;
using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunyScript
{
	/// <summary>
	/// Manages run contexts and their binding to objects.
	/// Provides deterministic iteration order based on ObjectID.
	/// </summary>
	internal sealed class ScriptRuntimeContextRegistry
	{
		private readonly Dictionary<LunyObjectId, ScriptRuntimeContext> _contextsByObjectID = new();
		private readonly Dictionary<LunyNativeObjectId, ScriptRuntimeContext> _contextsByNativeID = new();
		private ScriptRuntimeContext[] _sortedContexts = Array.Empty<ScriptRuntimeContext>();
		private Boolean _isSortedContextsDirty;

		/// <summary>
		/// Gets all run contexts in deterministic order (sorted by ObjectID).
		/// </summary>
		public IReadOnlyList<ScriptRuntimeContext> AllContexts =>
			!_isSortedContextsDirty ? _sortedContexts : _sortedContexts = CreateSortedContexts();

		/// <summary>
		/// Gets the number of registered contexts.
		/// </summary>
		public Int32 Count => _contextsByObjectID.Count;

		~ScriptRuntimeContextRegistry() => LunyTraceLogger.LogInfoFinalized(this);

		public ScriptRuntimeContext CreateContext(ScriptDefinition scriptDef, ILunyGameObject sceneObject)
		{
			var context = new ScriptRuntimeContext(scriptDef, sceneObject);
			Register(context);
			return context;
		}

		/// <summary>
		/// Registers a new run context.
		/// </summary>
		private void Register(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext == null)
				throw new ArgumentNullException(nameof(runtimeContext));

			var lunyID = runtimeContext.LunyGameObject.LunyObjectId;
			if (_contextsByObjectID.ContainsKey(lunyID))
				throw new LunyScriptException($"Context for object {runtimeContext.LunyGameObject.Name} ({lunyID}) already registered, replacing");

			_contextsByObjectID[lunyID] = runtimeContext;
			_contextsByNativeID[runtimeContext.LunyGameObject.NativeObjectId] = runtimeContext;
			_isSortedContextsDirty = true;
		}

		/// <summary>
		/// Unregisters a context by ObjectID.
		/// </summary>
		internal Boolean Unregister(ScriptRuntimeContext runtimeContext)
		{
			//LunyLogger.LogInfo($"Unregistering ... {runtimeContext} ({runtimeContext.GetHashCode()})", this);
			var lunyID = runtimeContext.LunyGameObject.LunyObjectId;
			if (!_contextsByObjectID.Remove(lunyID))
				return false;

			_isSortedContextsDirty = true;
			_contextsByNativeID.Remove(runtimeContext.LunyGameObject.NativeObjectId);
			runtimeContext.Shutdown();

			//LunyLogger.LogInfo($"Unregistered: {runtimeContext} ({runtimeContext.GetHashCode()})", this);
			return true;
		}

		/// <summary>
		/// Gets a context by ObjectID.
		/// </summary>
		public ScriptRuntimeContext GetByLunyObjectID(LunyObjectId lunyObjectID)
		{
			_contextsByObjectID.TryGetValue(lunyObjectID, out var context);
			return context;
		}

		public ScriptRuntimeContext GetByNativeObjectID(LunyNativeObjectId lunyNativeObjectID)
		{
			_contextsByNativeID.TryGetValue(lunyNativeObjectID, out var context);
			return context;
		}

		/// <summary>
		/// Checks if a context exists for the given ObjectID.
		/// </summary>
		public Boolean HasContext(LunyObjectId lunyObjectID) => _contextsByObjectID.ContainsKey(lunyObjectID);

		private ScriptRuntimeContext[] CreateSortedContexts()
		{
			_isSortedContextsDirty = false;
			return _contextsByObjectID.Values
				.OrderBy(ctx => ctx.LunyGameObject.LunyObjectId)
				.ToArray();
		}

		internal void Shutdown()
		{
			ScriptRuntimeContext.ClearGlobalVariables();

			foreach (var context in AllContexts)
				context.Shutdown();

			_contextsByObjectID.Clear();
			_contextsByNativeID.Clear();
			_sortedContexts = Array.Empty<ScriptRuntimeContext>();

			GC.SuppressFinalize(this);
		}
	}
}
