using Luny.Engine.Bridge;
using LunyScript.BlockBuilders;
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

		// TODO:
		// only singular "with"
		// With("") => name filter
		// With(Type) => component type filter
		// WithLayer("") => layer filter (or: WithGroup)
		// On.Collision.With("").Started(blocks).Continues(blocks).Ended(blocks)
		//
		// On.Collision/Trigger.Tagged("").*
		// On.Collision/Trigger.Named("").*
		// On.Collision/Trigger.Layered("").*  (InLayer, OfLayer?)
		// On.Collision/Trigger.Masked("").*
		//
		// On.Collision.Started(blocks).With("") ??
		// On.Collision.Started(blocks)
		// On.CollisionStarted(blocks)
		// Trigger.Entered(blocks)

		/*
		On.Collision/Trigger.Tagged("tag").Named("name").Layered("Ground", "Player").Masked(string[] or int)
		   .Begins(blocks).Updates(blocks).Ends(blocks)

		   Layered and Masked are mutually exclusive
		   the parameters should be 'params string[]' to allow for multiple which are logically OR combinations
		   Masked should have an override with an int to allow passing in a layer bitmask
		*/

		// Filtered collision/trigger builders
		/// <summary>
		/// Starts a filtered 3D collision event builder.
		/// Chain filter methods (Tagged, Named, Layered, Masked, Typed, Cooldown) then event handlers
		/// (Begins, Updates, Ends) and finalize with Do().
		/// </summary>
		public CollisionBuilder<CollisionBuilderStart> Collision
		{
			get
			{
				var options = new CollisionEventOptions { IsTrigger = false };
				var token = _script.CreateToken("Collision", "CollisionBuilder");
				return new CollisionBuilder<CollisionBuilderStart>(_script, options, token);
			}
		}

		/// <summary>
		/// Starts a filtered 3D trigger event builder.
		/// Chain filter methods (Tagged, Named, Layered, Masked, Typed, Cooldown) then event handlers
		/// (Begins, Updates, Ends) and finalize with Do().
		/// </summary>
		public CollisionBuilder<CollisionBuilderStart> Trigger
		{
			get
			{
				var options = new CollisionEventOptions { IsTrigger = true };
				var token = _script.CreateToken("Trigger", "CollisionBuilder");
				return new CollisionBuilder<CollisionBuilderStart>(_script, options, token);
			}
		}
	}
}
