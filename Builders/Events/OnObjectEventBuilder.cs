using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	/// <summary>
	/// Handles object centered/targeted events, eg state, update, collision events.
	/// </summary>
	public readonly struct OnObjectEventBuilder
	{
		private readonly Script _script;
		private readonly StackTrace _trace;

		internal OnObjectEventBuilder(Script script, StackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		private ScriptEventScheduler Scheduler => _script.Scheduler;

		/// <summary>
		/// Runs once the moment when the object is instantiated.
		/// </summary>
		public ISequenceBlock Created(params ActionBlock[] blocks)
		{
			_trace.Add(nameof(Created));
			return Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.Created, _trace);
		}

		/// <summary>
		/// Runs every time the object's state changes to 'enabled' (visible and participating).
		/// Runs directly after 'Created' if the object was just instantiated.
		/// </summary>
		public ISequenceBlock Enabled(params ActionBlock[] blocks)
		{
			_trace.Add(nameof(Enabled));
			return Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.Enabled, _trace);
		}

		/// <summary>
		/// Runs once per lifetime just before the object starts processing frame/time-step events.
		/// </summary>
		public ISequenceBlock Ready(params ActionBlock[] blocks)
		{
			_trace.Add(nameof(Ready));
			return Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.Ready, _trace);
		}

		/// <summary>
		/// Runs every time the object's state changes to 'disabled' (not visible, not participating).
		/// Runs directly before 'Destroyed' if the object was enabled as it got destroyed.
		/// </summary>
		public ISequenceBlock Disabled(params ActionBlock[] blocks)
		{
			_trace.Add(nameof(Disabled));
			return Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.Disabled, _trace);
		}

		/// <summary>
		/// Runs once when the object gets destroyed. The object is already disabled, the native engine instance still exists.
		/// </summary>
		public ISequenceBlock Destroyed(params ActionBlock[] blocks)
		{
			_trace.Add(nameof(Destroyed));
			return Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.Destroyed, _trace);
		}

		/// <summary>
		/// Runs on fixed-rate stepping while object is enabled.
		/// Scheduling depends on engine and Time settings, but typically runs 30 or 50 times per second.
		/// May run multiple times per frame and may not run in every frame.
		/// </summary>
		public ISequenceBlock Heartbeat(params ActionBlock[] blocks)
		{
			_trace.Add(nameof(Heartbeat));
			return Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.Heartbeat, _trace);
		}

		/// <summary>
		/// Runs every frame while object is enabled.
		/// </summary>
		public ISequenceBlock FrameUpdate(params ActionBlock[] blocks)
		{
			_trace.Add(nameof(FrameUpdate));
			return Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.FrameUpdate, _trace);
		}

		/// <summary>
		/// Runs after frame update while object is enabled.
		/// </summary>
		public ISequenceBlock AfterFrameUpdate(params ActionBlock[] blocks)
		{
			_trace.Add(nameof(AfterFrameUpdate));
			return Scheduler?.ScheduleObjectEventSequence(blocks, LunyObjectEvent.AfterFrameUpdate, _trace);
		}

		/// <summary>
		/// Starts a filtered 3D collision event builder.
		/// Chain filter methods (Tagged, Named, Layered, Masked, Typed, Cooldown) then event handlers
		/// (Begins, Updates, Ends) and finalize with Do().
		/// </summary>
		public CollisionEventBuilder<CollisionStart> Collision() => new(_script, _trace.Add(nameof(Collision)));

		/// <summary>
		/// Starts a filtered 3D trigger event builder.
		/// Chain filter methods (Tagged, Named, Layered, Masked, Typed, Cooldown) then event handlers
		/// (Begins, Updates, Ends) and finalize with Do().
		/// </summary>
		public TriggerEventBuilder<TriggerStart> Trigger() => new(_script, _trace.Add(nameof(Trigger)));
	}
}
