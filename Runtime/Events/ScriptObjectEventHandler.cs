using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript
{
	/// <summary>
	/// Manages object lifecycle events by attaching hooks to LunyObjects and handling event dispatch.
	/// Coordinates enable/disable state changes and deferred object destruction.
	/// </summary>
	internal sealed class ScriptObjectEventHandler
	{
		[NotNull] private readonly ScriptRuntimeContextRegistry _contexts;
		private readonly Dictionary<ScriptRuntimeContext, ObjectEventHandler> _subscribers = new();

		internal ScriptObjectEventHandler(ScriptRuntimeContextRegistry runtimeContextRegistry) =>
			_contexts = runtimeContextRegistry ?? throw new ArgumentNullException(nameof(runtimeContextRegistry));

		~ScriptObjectEventHandler() => LunyTraceLogger.LogInfoFinalized(this);

		/// <summary>
		/// Registers lifecycle hooks on a LunyObject for the given context.
		/// Called during ScriptContext construction.
		/// </summary>
		internal void Register(ScriptRuntimeContext runtimeContext)
		{
			var subscriber = new ObjectEventHandler(this, runtimeContext);
			_subscribers[runtimeContext] = subscriber;
		}

		/// <summary>
		/// Unregisters lifecycle hooks from a LunyObject.
		/// Called during context cleanup or shutdown.
		/// </summary>
		private void Unregister(ScriptRuntimeContext runtimeContext)
		{
			_subscribers.Remove(runtimeContext);
			_contexts.Unregister(runtimeContext);
		}

		public void OnHeartbeat(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext.LunyGameObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetObjectEventSequences(LunyObjectEvent.Heartbeat);
				LunyScriptRunner.Run(sequences, runtimeContext);
			}
		}

		public void OnFrameUpdate(ScriptRuntimeContext runtimeContext)
		{
			var lunyObject = runtimeContext.LunyGameObject;
			if (lunyObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetObjectEventSequences(LunyObjectEvent.FrameUpdate);
				LunyScriptRunner.Run(sequences, runtimeContext);
			}
		}

		public void OnFrameLateUpdate(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext.LunyGameObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetObjectEventSequences(LunyObjectEvent.AfterFrameUpdate);
				LunyScriptRunner.Run(sequences, runtimeContext);
			}
		}

		public void Shutdown()
		{
			// remove all subscribers and their events
			foreach (var subscriber in _subscribers.Values)
				subscriber.UnregisterAllCallbacks();

			_subscribers.Clear();
			GC.SuppressFinalize(this);
		}

		private struct ObjectEventHandler
		{
			private readonly ScriptObjectEventHandler _objectEventHandler;
			private readonly ScriptRuntimeContext _runtimeContext;

			public ObjectEventHandler(ScriptObjectEventHandler objectEventHandler, ScriptRuntimeContext runtimeContext)
			{
				_objectEventHandler = objectEventHandler;
				_runtimeContext = runtimeContext;
				RegisterAllCallbacks();
			}

			private void RegisterAllCallbacks()
			{
				var lunyObject = _runtimeContext.LunyGameObject;
				lunyObject.OnCreated += OnCreated;
				lunyObject.OnDestroyed += OnDestroyed;
				lunyObject.OnReady += OnReady;
				lunyObject.OnEnabled += OnEnabled;
				lunyObject.OnDisabled += OnDisabled;
				lunyObject.OnCollisionEntered += OnCollisionEntered;
				lunyObject.OnCollisionExited += OnCollisionExited;
				lunyObject.OnCollisionUpdate += OnCollisionUpdate;
				lunyObject.OnTriggerEntered += OnTriggerEntered;
				lunyObject.OnTriggerExited += OnTriggerExited;
				lunyObject.OnTriggerUpdate += OnTriggerUpdate;
				lunyObject.OnCollisionEntered2D += OnCollisionEntered2D;
				lunyObject.OnCollisionExited2D += OnCollisionExited2D;
				lunyObject.OnCollisionUpdate2D += OnCollisionUpdate2D;
				lunyObject.OnTriggerEntered2D += OnTriggerEntered2D;
				lunyObject.OnTriggerExited2D += OnTriggerExited2D;
				lunyObject.OnTriggerUpdate2D += OnTriggerUpdate2D;
			}

			internal void UnregisterAllCallbacks()
			{
				var lunyObject = _runtimeContext.LunyGameObject;
				lunyObject.OnCreated -= OnCreated;
				lunyObject.OnDestroyed -= OnDestroyed;
				lunyObject.OnReady -= OnReady;
				lunyObject.OnEnabled -= OnEnabled;
				lunyObject.OnDisabled -= OnDisabled;
				lunyObject.OnCollisionEntered -= OnCollisionEntered;
				lunyObject.OnCollisionExited -= OnCollisionExited;
				lunyObject.OnCollisionUpdate -= OnCollisionUpdate;
				lunyObject.OnTriggerEntered -= OnTriggerEntered;
				lunyObject.OnTriggerExited -= OnTriggerExited;
				lunyObject.OnTriggerUpdate -= OnTriggerUpdate;
				lunyObject.OnCollisionEntered2D -= OnCollisionEntered2D;
				lunyObject.OnCollisionExited2D -= OnCollisionExited2D;
				lunyObject.OnCollisionUpdate2D -= OnCollisionUpdate2D;
				lunyObject.OnTriggerEntered2D -= OnTriggerEntered2D;
				lunyObject.OnTriggerExited2D -= OnTriggerExited2D;
				lunyObject.OnTriggerUpdate2D -= OnTriggerUpdate2D;
			}

			private void RunObjectEventSequences(LunyObjectEvent objectEvent) =>
				LunyScriptRunner.Run(_runtimeContext.Scheduler?.GetObjectEventSequences(objectEvent), _runtimeContext);

			private void RunSequencesWithEventArgs(IEnumerable<ISequenceBlock> collisionEventSequences, Object eventArgs)
			{
				if (collisionEventSequences != null)
				{
					_runtimeContext.SetEventArgs(eventArgs);
					LunyScriptRunner.Run(collisionEventSequences, _runtimeContext);
					_runtimeContext.SetEventArgs(null);
				}
			}

			private void RunCollisionEventSequences(LunyCollisionEvent collisionEvent, LunyCollision collision)
			{
				var collisionEventSequences = _runtimeContext.Scheduler?.GetCollisionEventSequences(collisionEvent);
				RunSequencesWithEventArgs(collisionEventSequences, collision);
			}

			private void RunTriggerEventSequences(LunyTriggerEvent triggerEvent, LunyCollider collider)
			{
				var triggerEventSequences = _runtimeContext.Scheduler?.GetTriggerEventSequences(triggerEvent);
				RunSequencesWithEventArgs(triggerEventSequences, collider);
			}

			private void RunCollision2DEventSequences(LunyCollision2DEvent collision2DEvent, LunyCollision2D collision)
			{
				var collision2DEventSequences = _runtimeContext.Scheduler?.GetCollision2DEventSequences(collision2DEvent);
				RunSequencesWithEventArgs(collision2DEventSequences, collision);
			}

			private void RunTrigger2DEventSequences(LunyTrigger2DEvent trigger2DEvent, LunyCollider2D collider)
			{
				var trigger2DEventSequences = _runtimeContext.Scheduler?.GetTrigger2DEventSequences(trigger2DEvent);
				RunSequencesWithEventArgs(trigger2DEventSequences, collider);
			}

			private void UnscheduleOnceOnlyEvent(LunyObjectEvent objectEvent)
			{
				// Note: during the event, the script may have run Object.Destroy() on its object, thus invalidating it
				var lunyObject = _runtimeContext.LunyGameObject;
				if (!lunyObject.IsValid)
					return;

				if (objectEvent == LunyObjectEvent.Created || objectEvent == LunyObjectEvent.Ready)
				{
					// event never fires again for this object
					if (objectEvent == LunyObjectEvent.Created)
						lunyObject.OnCreated -= OnCreated;
					else if (objectEvent == LunyObjectEvent.Ready)
						lunyObject.OnReady -= OnReady;
				}
			}

			private void OnCreated()
			{
				RunObjectEventSequences(LunyObjectEvent.Created);
				UnscheduleOnceOnlyEvent(LunyObjectEvent.Created);
			}

			private void OnDestroyed()
			{
				RunObjectEventSequences(LunyObjectEvent.Destroyed);
				UnregisterAllCallbacks(); // no more events
				_objectEventHandler.Unregister(_runtimeContext);
			}

			private void OnReady()
			{
				RunObjectEventSequences(LunyObjectEvent.Ready);
				UnscheduleOnceOnlyEvent(LunyObjectEvent.Ready);
			}

			private void OnEnabled() => RunObjectEventSequences(LunyObjectEvent.Enabled);
			private void OnDisabled() => RunObjectEventSequences(LunyObjectEvent.Disabled);

			private void OnCollisionEntered(LunyCollision collision) =>
				RunCollisionEventSequences(LunyCollisionEvent.OnCollisionStarted, collision);

			private void OnCollisionExited(LunyCollision collision) =>
				RunCollisionEventSequences(LunyCollisionEvent.OnCollisionEnded, collision);

			private void OnCollisionUpdate(LunyCollision collision) =>
				RunCollisionEventSequences(LunyCollisionEvent.OnCollisionTouching, collision);

			private void OnTriggerEntered(LunyCollider collider) => RunTriggerEventSequences(LunyTriggerEvent.OnTriggerEntered, collider);

			private void OnTriggerExited(LunyCollider collider) => RunTriggerEventSequences(LunyTriggerEvent.OnTriggerExited, collider);

			private void OnTriggerUpdate(LunyCollider collider) => RunTriggerEventSequences(LunyTriggerEvent.OnTriggerOverlapping, collider);

			private void OnCollisionEntered2D(LunyCollision2D collision) =>
				RunCollision2DEventSequences(LunyCollision2DEvent.OnCollisionEntered2D, collision);

			private void OnCollisionExited2D(LunyCollision2D collision) =>
				RunCollision2DEventSequences(LunyCollision2DEvent.OnCollisionExited2D, collision);

			private void OnCollisionUpdate2D(LunyCollision2D collision) =>
				RunCollision2DEventSequences(LunyCollision2DEvent.OnCollisionUpdate2D, collision);

			private void OnTriggerEntered2D(LunyCollider2D collider) =>
				RunTrigger2DEventSequences(LunyTrigger2DEvent.OnTriggerEntered2D, collider);

			private void OnTriggerExited2D(LunyCollider2D collider) =>
				RunTrigger2DEventSequences(LunyTrigger2DEvent.OnTriggerExited2D, collider);

			private void OnTriggerUpdate2D(LunyCollider2D collider) => RunTrigger2DEventSequences(LunyTrigger2DEvent.OnTriggering2D, collider);
		}
	}
}
