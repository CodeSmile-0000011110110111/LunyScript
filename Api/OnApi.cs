using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Events;

namespace LunyScript.Api
{
	/// <summary>
	/// Handles object lifecycle and update events.
	/// </summary>
	public readonly struct OnApi
	{
		private readonly Script _script;
		internal OnApi(Script script) => _script = script;
		private ScriptEventScheduler Scheduler => _script.Scheduler;

		/// <summary>
		/// Runs once the moment when the object is instantiated.
		/// </summary>
		public SequenceBlock Created(params ScriptActionBlock[] blocks) => Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnCreated);

		/// <summary>
		/// Runs every time the object's state changes to 'enabled' (visible and participating).
		/// Runs directly after 'Created' if the object was just instantiated.
		/// </summary>
		public SequenceBlock Enabled(params ScriptActionBlock[] blocks) => Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnEnabled);

		/// <summary>
		/// Runs once per lifetime just before the object starts processing frame/time-step events.
		/// </summary>
		public SequenceBlock Ready(params ScriptActionBlock[] blocks) => Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnReady);

		/// <summary>
		/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
		/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
		/// </summary>
		public SequenceBlock Disabled(params ScriptActionBlock[] blocks) => Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnDisabled);

		/// <summary>
		/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
		/// </summary>
		public SequenceBlock Destroyed(params ScriptActionBlock[] blocks) => Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnDestroyed);

		/// <summary>
		/// Runs every frame while object is enabled.
		/// </summary>
		public SequenceBlock FrameUpdate(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnFrameUpdate);

		/// <summary>
		/// Runs after frame update while object is enabled.
		/// </summary>
		public SequenceBlock AfterFrameUpdate(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnFrameLateUpdate);

		/// <summary>
		/// Runs on fixed-rate stepping while object is enabled.
		/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
		/// May run multiple times per frame and may not run in every frame.
		/// </summary>
		public SequenceBlock Heartbeat(params ScriptActionBlock[] blocks) => Scheduler?.ScheduleSequence(blocks, LunyObjectEvent.OnHeartbeat);

		// Collision events
		public SequenceBlock CollisionStarted(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyCollisionEvent.OnCollisionStarted);

		public SequenceBlock CollisionEnded(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyCollisionEvent.OnCollisionEnded);

		public SequenceBlock Colliding(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyCollisionEvent.OnColliding);

		// Trigger events
		public SequenceBlock TriggerEntered(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyTriggerEvent.OnTriggerEntered);

		public SequenceBlock TriggerExited(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyTriggerEvent.OnTriggerExited);

		public SequenceBlock Triggering(params ScriptActionBlock[] blocks) =>
			Scheduler?.ScheduleSequence(blocks, LunyTriggerEvent.OnTriggering);
	}
}
