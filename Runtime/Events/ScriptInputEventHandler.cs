using Luny;
using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LunyScript.Events
{
	/// <summary>
	/// Handles Input Action events.
	/// </summary>
	internal sealed class ScriptInputEventHandler
	{
		[NotNull] private readonly ScriptRuntimeContextRegistry _contexts;

		private readonly List<LunyObjectId> _subscriberObjectIDs = new();

		internal ScriptInputEventHandler(ScriptRuntimeContextRegistry runtimeContextRegistry) =>
			_contexts = runtimeContextRegistry ?? throw new ArgumentNullException(nameof(runtimeContextRegistry));

		~ScriptInputEventHandler() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Register(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext.Scheduler.IsObservingAnyOf(typeof(LunyInputActionEvent)))
			{
				if (_subscriberObjectIDs.Count == 0)
					RegisterInputActionEvent();

				_subscriberObjectIDs.Add(runtimeContext.LunyObject.LunyObjectId);
			}
		}

		private void RegisterInputActionEvent() => LunyEngine.Instance.Input.OnInputAction += OnInputAction;

		private void UnregisterInputActionEvent()
		{
			var input = LunyEngine.Instance?.Input;
			if (input != null)
				input.OnInputAction -= OnInputAction;
		}

		private void OnInputAction(LunyInputActionEvent inputActionEvent)
		{
			foreach (var subscriberID in _subscriberObjectIDs)
				TryRunForEvent(subscriberID, inputActionEvent);
		}

		private void TryRunForEvent(LunyObjectId subscriberID, LunyInputActionEvent inputEvent)
		{
			var context = _contexts.GetByLunyObjectID(subscriberID);
			if (context == null || !context.LunyObject.IsEnabled)
				return;

			var sequences = context.Scheduler?.GetInputActionEventSequences(inputEvent.ActionName, inputEvent.Phase);
			if (sequences != null)
			{
				context.SetEventArgs(inputEvent);
				LunyScriptRunner.Run(sequences, context);
				context.SetEventArgs(null);
			}
		}

		public void Shutdown()
		{
			UnregisterInputActionEvent();
			_subscriberObjectIDs.Clear();
			GC.SuppressFinalize(this);
		}
	}
}
