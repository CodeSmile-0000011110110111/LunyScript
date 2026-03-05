using Luny;
using Luny.Engine;
using Luny.Engine.Bridge;
using Luny.Engine.Services;
using LunyScript.Activation;
using LunyScript.Blocks;
using LunyScript.Diagnostics;
using LunyScript.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LunyScript
{
	/// <summary>
	/// Engine-agnostic script execution runner.
	/// Implements the lifecycle contract and will be discovered/reflected by
	/// the <see cref="LunyEngine"/> at startup.
	/// Manages script discovery, object binding, and run context lifecycle.
	/// </summary>
	internal sealed class LunyScriptRunner : ILunyEngineObserver
	{
		[NotNull] private ScriptEngine _scriptEngine;
		[NotNull] private ScriptDefinitionRegistry _scripts;
		[NotNull] private ScriptRuntimeContextRegistry _contexts;
		[NotNull] private ScriptObjectEventHandler _objectEventHandler;
		[NotNull] private ScriptSceneEventHandler _sceneEventHandler;
		[NotNull] private ScriptInputEventHandler _inputEventHandler;

		private ILunyTimeService _engineTime;
		private Table.ScalarVarHandle gvar_Time_HeartbeatCount;
		private Table.ScalarVarHandle gvar_Time_FrameCount;
		private Table.ScalarVarHandle gvar_Time_ElapsedSeconds;

		internal ScriptDefinitionRegistry Scripts => _scripts;
		internal ScriptRuntimeContextRegistry Contexts => _contexts;
		internal ScriptObjectEventHandler ObjectEventHandler => _objectEventHandler;
		internal ScriptSceneEventHandler SceneEventHandler => _sceneEventHandler;
		internal ScriptInputEventHandler InputEventHandler => _inputEventHandler;

		internal static void Run(IEnumerable<SequenceBlock> sequences, ScriptRuntimeContext runtimeContext)
		{
			if (sequences == null)
				return;

			foreach (var sequence in sequences)
				Run(sequence, runtimeContext);
		}

		internal static void Run(IEnumerable<ISequenceBlock> sequences, ScriptRuntimeContext runtimeContext)
		{
			if (sequences == null)
				return;

			foreach (var sequence in sequences)
				Run(sequence, runtimeContext);
		}

		internal static void Run(ISequenceBlock sequence, ScriptRuntimeContext runtimeContext)
		{
			if (sequence == null)
				return;

			// TODO: avoid profiling overhead when not profiling
			var timeService = LunyEngine.Instance.Time;
			var blockType = sequence.GetType();
			var trace = new ScriptExecutionTrace
			{
				FrameCount = timeService?.FrameCount ?? -1,
				ElapsedSeconds = timeService?.ElapsedSeconds ?? -1.0,
				ScriptSequenceId = sequence.Id,
				BlockType = blockType,
				BlockDescription = sequence.ToString(),
			};

			// Note: explicit local vars because a script may destroy the object during OnCreate
			// In that case the context's BlockProfiler/DebugHooks would be null in catch/finally
			var debugHooks = runtimeContext.DebugHooks;
			var blockProfiler = runtimeContext.BlockProfiler;
			debugHooks.NotifyBlockExecute(trace);
			blockProfiler.BeginBlock(trace.ScriptSequenceId);

			try
			{
				((ScriptActionBlock)sequence).Execute(runtimeContext);
			}
			catch (Exception ex)
			{
				trace.Error = ex;
				blockProfiler.RecordError(trace.ScriptSequenceId, ex);
				debugHooks.NotifyBlockError(trace);
				//LunyLogger.LogError(ex.ToString(), runtimeContext);
				throw;
			}
			finally
			{
				blockProfiler.EndBlock(trace.ScriptSequenceId, blockType);
				debugHooks.NotifyBlockComplete(trace);
			}
		}

		public LunyScriptRunner()
		{
			LunyTraceLogger.LogInfoInitializing(this);

			ScriptId.Reset();
			ScriptBlockId.Reset();
			_scriptEngine = new ScriptEngine(this); // public API interface (split to ensure users don't call OnStartup etc)
			_scripts = new ScriptDefinitionRegistry(); // performs LunyScript type discovery
			_contexts = new ScriptRuntimeContextRegistry();
			_objectEventHandler = new ScriptObjectEventHandler(_contexts);
			_sceneEventHandler = new ScriptSceneEventHandler(_contexts);
			_inputEventHandler = new ScriptInputEventHandler(_contexts);
			_engineTime = LunyEngine.Instance.Time;

			LunyTraceLogger.LogInfoInitialized(this);
		}

		public void OnEngineStartup()
		{
			try
			{
				LunyTraceLogger.LogInfoStartingUp(this);

				var gvars = ScriptRuntimeContext.GetGlobalVariables();
				gvar_Time_HeartbeatCount = gvars.GetHandle("Time.HeartbeatCount");
				gvar_Time_FrameCount = gvars.GetHandle("Time.FrameCount");
				gvar_Time_ElapsedSeconds = gvars.GetHandle("Time.ElapsedSeconds");
			}
			catch (Exception)
			{
				LunyLogger.LogError($"Error during {nameof(LunyEngine)} {nameof(OnEngineStartup)}!", this);
				throw;
			}
			finally
			{
				LunyTraceLogger.LogInfoStartupComplete(this);
			}
		}

		public void OnEngineShutdown()
		{
			try
			{
				LunyTraceLogger.LogInfoShuttingDown(this);

				// ensure all objects run their OnDestroy
				foreach (var context in _contexts.AllContexts)
					context.LunyObject.Destroy();

				// final cleanup of pending object destroy
				_inputEventHandler.Shutdown();
				_sceneEventHandler.Shutdown();
				_objectEventHandler.Shutdown();
				_contexts.Shutdown();
				_scripts.Shutdown();
				_scriptEngine.Shutdown();
			}
			catch (Exception)
			{
				LunyLogger.LogError($"Error during {nameof(LunyScriptRunner)} {nameof(OnEngineShutdown)}!", this);
				throw;
			}
			finally
			{
				_scriptEngine = null;
				_objectEventHandler = null;
				GC.SuppressFinalize(this);

				LunyTraceLogger.LogInfoShutdownComplete(this);
			}
		}

		public void OnSceneLoaded(ILunyScene loadedScene)
		{
			_sceneEventHandler.OnSceneLoaded(loadedScene);

			// Process current scene to bind scripts to objects
			var lunyEngine = LunyEngine.Instance;
			var scriptNames = _scripts.GetNames();
			var scriptedObjects = lunyEngine.Scene.GetObjects(scriptNames);

			// Filter out objects that already have a context to avoid double activation
			var newScriptedObjects = scriptedObjects.Where(obj => _contexts.GetByNativeObjectID(obj.NativeObjectId) == null).ToList();

			ScriptBuilder.BuildAndActivateLunyScripts(this, newScriptedObjects);
		}

		public void OnObjectRegistered(ILunyObject lunyObject)
		{
			//LunyLogger.LogInfo($"{nameof(OnObjectRegistered)}: {lunyObject}", this);

			// Check if object is already scripted to avoid double activation
			if (_contexts.GetByNativeObjectID(lunyObject.NativeObjectId) != null)
			{
				LunyLogger.LogWarning($"{lunyObject} already runs a script", this);
				return;
			}

			// Activate script on dynamically created object
			ScriptBuilder.BuildAndActivateLunyScript(this, lunyObject);
		}

		public void OnObjectUnregistered(ILunyObject lunyObject)
		{
			// Cleanup context for engine-side auto-destroyed objects (scene load)
			var context = _contexts.GetByNativeObjectID(lunyObject.NativeObjectId);
			if (context != null)
			{
				LunyLogger.LogInfo($"{nameof(OnObjectUnregistered)}: unregistering {context} ({context.GetHashCode()})");
				// _sceneEventHandler.Unregister(context);
				_contexts.Unregister(context);
			}
		}

		public void OnSceneUnloaded(ILunyScene unloadedScene) => _sceneEventHandler.OnSceneUnloaded(unloadedScene);

		//public void OnEngineFrameBegins() {}
		//public void OnEngineFrameEnds() {}

		public void OnEngineHeartbeat()
		{
			gvar_Time_ElapsedSeconds.Value = _engineTime.ElapsedSeconds;
			gvar_Time_HeartbeatCount.Value = _engineTime.HeartbeatCount;

			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnHeartbeat(context);
		}

		public void OnEngineFrameUpdate()
		{
			gvar_Time_ElapsedSeconds.Value = _engineTime.ElapsedSeconds;
			gvar_Time_FrameCount.Value = _engineTime.FrameCount;

			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnFrameUpdate(context);
		}

		public void OnEngineFrameLateUpdate()
		{
			// Run all LateUpdate runnables
			foreach (var context in _contexts.AllContexts)
				_objectEventHandler.OnFrameLateUpdate(context);
		}

		~LunyScriptRunner() => LunyTraceLogger.LogInfoFinalized(this);

		internal void InvokeOnScriptBuilt(ScriptRuntimeContext runtimeContext) => _scriptEngine.InvokeOnScriptBuilt(runtimeContext);
	}
}
