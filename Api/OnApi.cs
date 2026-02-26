using Luny.Engine.Bridge;
using LunyScript.BlockBuilders;
using LunyScript.Blocks;
using LunyScript.Events;
using System;

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
				var token = _script.CreateToken(nameof(Collision), "CollisionBuilder (3D)");
				return new CollisionBuilder<CollisionBuilderStart>(_script, options, token);
			}
		}

		/// <summary>
		/// Starts a filtered 3D collision event builder.
		/// Chain filter methods (Cooldown) then event handlers
		/// (Begins, Updates, Ends) and finalize with Do().
		/// </summary>
		/*public CollisionBuilder<CollisionBuilderReady> CollisionWith(String name = null, String tag = null, String layer = null,
			Type type = null)
		{
			var options = new CollisionEventOptions { IsTrigger = false };
			var token = _script.CreateToken(nameof(CollisionWith), "CollisionBuilder (3D)");
			var startBuilder = new CollisionBuilder<CollisionBuilderStart>(_script, options, token);

			var hasName = !string.IsNullOrEmpty(name);
			var hasTag = !string.IsNullOrEmpty(tag);
			var hasLayer = !string.IsNullOrEmpty(layer);
			var hasType = type != null;
			if (!hasName && !hasTag && !hasLayer && !hasType)
				throw new ArgumentException($"{nameof(CollisionWith)}: at least one argument must be specified");

			throw new NotImplementedException(nameof(CollisionWith));

			// Bundle parameters into a tuple to match against
			var with = (name, tag, layer, type);
			CollisionBuilder<CollisionBuilderReady> readyBuilder = with switch
			{
				// 5. All parameters provided
				(not null, not null, not null, not null) => startBuilder,

				// 1. All are null
				(null, null, null, null) => startBuilder,

				// 2. Exact matches (Example: Only name is provided)
				(not null, null, null, null) => startBuilder,

				// 3. Complex combinations (Example: Tag and Type provided)
				(null, not null, null, not null) => startBuilder,

				// 4. Partial matches using discards (_)
				// This matches ANY case where 'layer' is provided, regardless of others
				(_, _, not null, _) => startBuilder,


				_ => throw new ArgumentOutOfRangeException(nameof(CollisionWith))
			};

			return readyBuilder;
		}*/

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
				var token = _script.CreateToken(nameof(Trigger), "CollisionBuilder (Trigger, 3D)");
				return new CollisionBuilder<CollisionBuilderStart>(_script, options, token);
			}
		}

		/// <summary>
		/// Starts a filtered 3D trigger event builder.
		/// Chain filter methods (Cooldown) then event handlers
		/// (Begins, Updates, Ends) and finalize with Do().
		/// </summary>
		/*public CollisionBuilder<CollisionBuilderReady> TriggerWith(String name = null, String tag = null, String layer = null,
			Type type = null)
		{
			throw new NotImplementedException(nameof(TriggerWith));

			var options = new CollisionEventOptions { IsTrigger = true };
			var token = _script.CreateToken(nameof(TriggerWith), "CollisionBuilder (Trigger, 3D)");
			return new CollisionBuilder<CollisionBuilderReady>(_script, options, token).Named(name).Tagged(tag).Layered(layer).Typed(type);
		}*/
	}
}
