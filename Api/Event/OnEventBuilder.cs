using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Events;

namespace LunyScript.Api.Event
{
	/// <summary>
	/// Handles object centered/targeted events, eg state, update, collision events.
	/// </summary>
	public readonly struct OnEventBuilder
	{
		private readonly Script _script;
		internal OnEventBuilder(Script script) => _script = script;
		private ScriptEventScheduler Scheduler => _script.Scheduler;

		/// <summary>
		/// Runs once the moment when the object is instantiated.
		/// </summary>
		public ISequenceBlock Created(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.OnCreated);

		/// <summary>
		/// Runs every time the object's state changes to 'enabled' (visible and participating).
		/// Runs directly after 'Created' if the object was just instantiated.
		/// </summary>
		public ISequenceBlock Enabled(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.OnEnabled);

		/// <summary>
		/// Runs once per lifetime just before the object starts processing frame/time-step events.
		/// </summary>
		public ISequenceBlock Ready(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.OnReady);

		/// <summary>
		/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
		/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
		/// </summary>
		public ISequenceBlock Disabled(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.OnDisabled);

		/// <summary>
		/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
		/// </summary>
		public ISequenceBlock Destroyed(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.OnDestroyed);

		/// <summary>
		/// Runs every frame while object is enabled.
		/// </summary>
		public ISequenceBlock FrameUpdate(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.OnFrameUpdate);

		/// <summary>
		/// Runs after frame update while object is enabled.
		/// </summary>
		public ISequenceBlock AfterFrameUpdate(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.OnFrameLateUpdate);

		/// <summary>
		/// Runs on fixed-rate stepping while object is enabled.
		/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
		/// May run multiple times per frame and may not run in every frame.
		/// </summary>
		public ISequenceBlock Heartbeat(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.OnHeartbeat);

		/// <summary>
		/// Starts a filtered 3D collision event builder.
		/// Chain filter methods (Tagged, Named, Layered, Masked, Typed, Cooldown) then event handlers
		/// (Begins, Updates, Ends) and finalize with Do().
		/// </summary>
		public OnPhysicsEventBuilder<CollisionBuilderStart> Collision
		{
			get
			{
				var options = new PhysicsEventOptions { IsTrigger = false };
				var token = _script.CreateBuilderToken(nameof(Collision), "CollisionBuilder (3D)");
				return new OnPhysicsEventBuilder<CollisionBuilderStart>(_script, options, token);
			}
		}

		/// <summary>
		/// Starts a filtered 3D trigger event builder.
		/// Chain filter methods (Tagged, Named, Layered, Masked, Typed, Cooldown) then event handlers
		/// (Begins, Updates, Ends) and finalize with Do().
		/// </summary>
		public OnPhysicsEventBuilder<CollisionBuilderStart> Trigger
		{
			get
			{
				var options = new PhysicsEventOptions { IsTrigger = true };
				var token = _script.CreateBuilderToken(nameof(Trigger), "CollisionBuilder (Trigger, 3D)");
				return new OnPhysicsEventBuilder<CollisionBuilderStart>(_script, options, token);
			}
		}
	}
}
