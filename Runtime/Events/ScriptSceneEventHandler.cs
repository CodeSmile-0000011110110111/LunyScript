using Luny;
using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript.Events
{
	/// <summary>
	/// Handles scene events.
	/// </summary>
	internal sealed class ScriptSceneEventHandler
	{
		[NotNull] private readonly ScriptRuntimeContextRegistry _contexts;

		private readonly List<LunyObjectId> _subscriberObjectIDs = new();

		internal ScriptSceneEventHandler(ScriptRuntimeContextRegistry runtimeContextRegistry) =>
			_contexts = runtimeContextRegistry ?? throw new ArgumentNullException(nameof(runtimeContextRegistry));

		~ScriptSceneEventHandler() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Register(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext.Scheduler.IsObservingAnyOf(typeof(LunySceneEvent)))
				_subscriberObjectIDs.Add(runtimeContext.LunyObject.LunyObjectId);
		}

		public void OnSceneUnloaded(ILunyScene scene)
		{
			foreach (var subscriberID in _subscriberObjectIDs)
				TryRunForEvent(subscriberID, LunySceneEvent.OnSceneUnloaded);
		}

		public void OnSceneLoaded(ILunyScene scene)
		{
			foreach (var subscriberID in _subscriberObjectIDs)
				TryRunForEvent(subscriberID, LunySceneEvent.OnSceneLoaded);
		}

		private void TryRunForEvent(LunyObjectId subscriberID, LunySceneEvent sceneEvent)
		{
			var context = _contexts.GetByLunyObjectID(subscriberID);
			var sequences = context?.Scheduler?.GetSceneEventSequences(sceneEvent);
			if (sequences != null)
			{
				LunyLogger.LogInfo($"Running {nameof(sceneEvent)} for {context}", this);

				LunyScriptRunner.Run(sequences, context);
			}
		}

		public void Shutdown()
		{
			_subscriberObjectIDs.Clear();
			GC.SuppressFinalize(this);
		}
	}
}
