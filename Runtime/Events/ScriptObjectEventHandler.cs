using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Bridge.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript.Events
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
			if (runtimeContext.LunyObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetSequences(LunyObjectEvent.OnHeartbeat);
				LunyScriptRunner.Run(sequences, runtimeContext);

				runtimeContext.Coroutines?.OnHeartbeat(runtimeContext);
			}
		}

		public void OnFrameUpdate(ScriptRuntimeContext runtimeContext)
		{
			var lunyObject = runtimeContext.LunyObject;
			if (lunyObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetSequences(LunyObjectEvent.OnFrameUpdate);
				LunyScriptRunner.Run(sequences, runtimeContext);

				runtimeContext.Coroutines?.OnFrameUpdate(runtimeContext);
			}
		}

		public void OnFrameLateUpdate(ScriptRuntimeContext runtimeContext)
		{
			if (runtimeContext.LunyObject.IsEnabled)
			{
				var sequences = runtimeContext.Scheduler.GetSequences(LunyObjectEvent.OnFrameLateUpdate);
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
				var lunyObject = _runtimeContext.LunyObject;
				lunyObject.OnCreate += OnCreate;
				lunyObject.OnDestroy += OnDestroy;
				lunyObject.OnReady += OnReady;
				lunyObject.OnEnable += OnEnable;
				lunyObject.OnDisable += OnDisable;
				lunyObject.OnCollisionStarted += OnCollisionStarted;
				lunyObject.OnCollisionEnded += OnCollisionEnded;
				lunyObject.OnColliding += OnColliding;
				lunyObject.OnTriggerEntered += OnTriggerEntered;
				lunyObject.OnTriggerExited += OnTriggerExited;
				lunyObject.OnTriggering += OnTriggering;
				lunyObject.OnCollisionStarted2D += OnCollisionStarted2D;
				lunyObject.OnCollisionEnded2D += OnCollisionEnded2D;
				lunyObject.OnColliding2D += OnColliding2D;
				lunyObject.OnTriggerEntered2D += OnTriggerEntered2D;
				lunyObject.OnTriggerExited2D += OnTriggerExited2D;
				lunyObject.OnTriggering2D += OnTriggering2D;
			}

			internal void UnregisterAllCallbacks()
			{
				var lunyObject = _runtimeContext.LunyObject;
				lunyObject.OnCreate -= OnCreate;
				lunyObject.OnDestroy -= OnDestroy;
				lunyObject.OnReady -= OnReady;
				lunyObject.OnEnable -= OnEnable;
				lunyObject.OnDisable -= OnDisable;
				lunyObject.OnCollisionStarted -= OnCollisionStarted;
				lunyObject.OnCollisionEnded -= OnCollisionEnded;
				lunyObject.OnColliding -= OnColliding;
				lunyObject.OnTriggerEntered -= OnTriggerEntered;
				lunyObject.OnTriggerExited -= OnTriggerExited;
				lunyObject.OnTriggering -= OnTriggering;
				lunyObject.OnCollisionStarted2D -= OnCollisionStarted2D;
				lunyObject.OnCollisionEnded2D -= OnCollisionEnded2D;
				lunyObject.OnColliding2D -= OnColliding2D;
				lunyObject.OnTriggerEntered2D -= OnTriggerEntered2D;
				lunyObject.OnTriggerExited2D -= OnTriggerExited2D;
				lunyObject.OnTriggering2D -= OnTriggering2D;
			}

			private void RunObjectEventSequences(LunyObjectEvent objectEvent) =>
				LunyScriptRunner.Run(_runtimeContext.Scheduler?.GetSequences(objectEvent), _runtimeContext);

			private void RunCollisionEventSequences(LunyCollisionEvent collisionEvent, LunyCollision collision)
			{
				LunyScriptRunner.Run(_runtimeContext.Scheduler?.GetSequences(collisionEvent), _runtimeContext);

				var physicsSequences = _runtimeContext.Scheduler?.GetPhysicsSequences(collisionEvent);
				if (physicsSequences != null)
				{
					_runtimeContext.SetEventArgs(collision);
					try
					{
						LunyScriptRunner.Run(physicsSequences, _runtimeContext);
					}
					finally
					{
						_runtimeContext.SetEventArgs(null);
					}
				}
			}

			private void RunTriggerEventSequences(LunyTriggerEvent triggerEvent, LunyCollider collider)
			{
				LunyScriptRunner.Run(_runtimeContext.Scheduler?.GetSequences(triggerEvent), _runtimeContext);

				var physicsSequences = _runtimeContext.Scheduler?.GetPhysicsSequences(triggerEvent);
				if (physicsSequences != null)
				{
					_runtimeContext.SetEventArgs(collider);
					try
					{
						LunyScriptRunner.Run(physicsSequences, _runtimeContext);
					}
					finally
					{
						_runtimeContext.SetEventArgs(null);
					}
				}
			}

			private void RunCollision2DEventSequences(LunyCollision2DEvent collision2DEvent) =>
				LunyScriptRunner.Run(_runtimeContext.Scheduler?.GetSequences(collision2DEvent), _runtimeContext);

			private void RunTrigger2DEventSequences(LunyTrigger2DEvent trigger2DEvent) =>
				LunyScriptRunner.Run(_runtimeContext.Scheduler?.GetSequences(trigger2DEvent), _runtimeContext);

			private void UnscheduleOnceOnlyEvent(LunyObjectEvent objectEvent)
			{
				// Note: during the event, the script may have run Object.Destroy() on its object, thus invalidating it
				var lunyObject = _runtimeContext.LunyObject;
				if (!lunyObject.IsValid)
					return;

				if (objectEvent == LunyObjectEvent.OnCreated || objectEvent == LunyObjectEvent.OnReady)
				{
					// event never fires again for this object
					_runtimeContext.Scheduler.Unschedule(objectEvent);

					if (objectEvent == LunyObjectEvent.OnCreated)
						lunyObject.OnCreate -= OnCreate;
					else if (objectEvent == LunyObjectEvent.OnReady)
						lunyObject.OnReady -= OnReady;
				}
			}

			private void OnCreate()
			{
				RunObjectEventSequences(LunyObjectEvent.OnCreated);
				UnscheduleOnceOnlyEvent(LunyObjectEvent.OnCreated);
			}

			private void OnDestroy()
			{
				RunObjectEventSequences(LunyObjectEvent.OnDestroyed);
				UnregisterAllCallbacks(); // no more events
				_runtimeContext.Shutdown();
				_objectEventHandler.Unregister(_runtimeContext);
			}

			private void OnReady()
			{
				RunObjectEventSequences(LunyObjectEvent.OnReady);
				UnscheduleOnceOnlyEvent(LunyObjectEvent.OnReady);
			}

			private void OnEnable() => RunObjectEventSequences(LunyObjectEvent.OnEnabled);
			private void OnDisable() => RunObjectEventSequences(LunyObjectEvent.OnDisabled);

 		private void OnCollisionStarted(LunyCollision collision) => RunCollisionEventSequences(LunyCollisionEvent.OnCollisionEntered, collision);

			private void OnCollisionEnded(LunyCollision collision) => RunCollisionEventSequences(LunyCollisionEvent.OnCollisionExited, collision);

			private void OnColliding(LunyCollision collision) => RunCollisionEventSequences(LunyCollisionEvent.OnCollisionUpdate, collision);

			private void OnTriggerEntered(LunyCollider collider) => RunTriggerEventSequences(LunyTriggerEvent.OnTriggerEntered, collider);

			private void OnTriggerExited(LunyCollider collider) => RunTriggerEventSequences(LunyTriggerEvent.OnTriggerExited, collider);

			private void OnTriggering(LunyCollider collider) => RunTriggerEventSequences(LunyTriggerEvent.OnTriggerUpdate, collider);

			private void OnCollisionStarted2D(LunyCollision2D collision) =>
				RunCollision2DEventSequences(LunyCollision2DEvent.OnCollisionStarted2D);

			private void OnCollisionEnded2D(LunyCollision2D collision) => RunCollision2DEventSequences(LunyCollision2DEvent.OnCollisionEnded2D);

			private void OnColliding2D(LunyCollision2D collision) => RunCollision2DEventSequences(LunyCollision2DEvent.OnColliding2D);

			private void OnTriggerEntered2D(LunyCollider2D collider) => RunTrigger2DEventSequences(LunyTrigger2DEvent.OnTriggerEntered2D);

			private void OnTriggerExited2D(LunyCollider2D collider) => RunTrigger2DEventSequences(LunyTrigger2DEvent.OnTriggerExited2D);

			private void OnTriggering2D(LunyCollider2D collider) => RunTrigger2DEventSequences(LunyTrigger2DEvent.OnTriggering2D);
		}
	}
}
