using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;
using System.Collections.Generic;

namespace LunyScript.Events
{
	/// <summary>
	/// Schedules and manages sequences for various event types.
	/// </summary>
	internal sealed class ScriptEventScheduler
	{
		private static readonly Int32 s_ObjectEventCount = Enum.GetNames(typeof(LunyObjectEvent)).Length;
		private static readonly Int32 s_SceneEventCount = Enum.GetNames(typeof(LunySceneEvent)).Length;
		private static readonly Int32 s_CollisionEventCount = Enum.GetNames(typeof(LunyCollisionEvent)).Length;
		private static readonly Int32 s_TriggerEventCount = Enum.GetNames(typeof(LunyTriggerEvent)).Length;
		private static readonly Int32 s_Collision2DEventCount = Enum.GetNames(typeof(LunyCollision2DEvent)).Length;
		private static readonly Int32 s_Trigger2DEventCount = Enum.GetNames(typeof(LunyTrigger2DEvent)).Length;

		// Fast array-based storage for lifecycle events (hot path)
		private List<ISequenceBlock>[] _objectSequences;
		private List<ISequenceBlock>[] _sceneSequences;
		private List<ISequenceBlock>[] _collisionSequences;
		private List<ISequenceBlock>[] _triggerSequences;
		private List<ISequenceBlock>[] _collision2DSequences;
		private List<ISequenceBlock>[] _trigger2DSequences;

		internal static ISequenceBlock SchedulePhysicsSequence(ref List<ISequenceBlock>[] sequencesRef, ISequenceBlock sequence,
			Int32 eventIndex, Int32 eventCount)
		{
			if (sequence != null && !sequence.IsEmpty)
			{
				sequencesRef ??= new List<ISequenceBlock>[eventCount];
				sequencesRef[eventIndex] ??= new List<ISequenceBlock>();
				sequencesRef[eventIndex].Add(sequence);
			}

			return sequence;
		}

		private static ISequenceBlock ScheduleSequence(ref List<ISequenceBlock>[] sequencesRef, ISequenceBlock sequence,
			Int32 eventIndex, Int32 eventCount)
		{
			if (sequence != null && !sequence.IsEmpty)
			{
				sequencesRef ??= new List<ISequenceBlock>[eventCount];
				sequencesRef[eventIndex] ??= new List<ISequenceBlock>();
				sequencesRef[eventIndex].Add(sequence);
			}

			return sequence;
		}

		~ScriptEventScheduler() => LunyTraceLogger.LogInfoFinalized(this);

		/// Schedule Events
		internal ISequenceBlock ScheduleObjectEventSequence(ScriptActionBlock[] blocks, LunyObjectEvent objectEvent) =>
			ScheduleSequence(ref _objectSequences, SequenceBlock.TryCreate(blocks), (Int32)objectEvent, s_ObjectEventCount);

		internal ISequenceBlock ScheduleSceneEventSequence(ScriptActionBlock[] blocks, LunySceneEvent sceneEvent) =>
			ScheduleSequence(ref _sceneSequences, SequenceBlock.TryCreate(blocks), (Int32)sceneEvent, s_SceneEventCount);

		internal ISequenceBlock ScheduleCollisionEventSequence(CollisionSequenceBlock blocks, LunyCollisionEvent collisionEvent) =>
			SchedulePhysicsSequence(ref _collisionSequences, blocks, (Int32)collisionEvent, s_CollisionEventCount);

		internal ISequenceBlock ScheduleTriggerEventSequence(TriggerSequenceBlock blocks, LunyTriggerEvent triggerEvent) =>
			SchedulePhysicsSequence(ref _triggerSequences, blocks, (Int32)triggerEvent, s_TriggerEventCount);

		// internal ISequenceBlock ScheduleCollision2DEventSequence(CollisionSequence2DBlock[] blocks, LunyCollision2DEvent collision2DEvent) =>
		// 	SchedulePhysicsSequence(ref _collision2DSequences, SequenceBlock.TryCreate(blocks), (Int32)collision2DEvent, s_Collision2DEventCount);
		//
		// internal ISequenceBlock ScheduleTrigger2DEventSequence(TriggerSequence2DBlock[] blocks, LunyTrigger2DEvent trigger2DEvent) =>
		// 	SchedulePhysicsSequence(ref _trigger2DSequences, SequenceBlock.TryCreate(blocks), (Int32)trigger2DEvent, s_Trigger2DEventCount);

		/// Gets all sequences scheduled for a specific lifecycle event.
		internal IEnumerable<ISequenceBlock> GetSequences(LunyObjectEvent objectEvent) =>
			IsObserving((Int32)objectEvent, ref _objectSequences) ? _objectSequences[(Int32)objectEvent] : null;

		internal IEnumerable<ISequenceBlock> GetSequences(LunySceneEvent sceneEvent) => IsObserving((Int32)sceneEvent, ref _objectSequences)
			? _objectSequences[(Int32)sceneEvent]
			: null;

		internal IEnumerable<ISequenceBlock> GetSequences(LunyCollisionEvent collisionEvent) =>
			IsObserving((Int32)collisionEvent, ref _collisionSequences)
				? _collisionSequences[(Int32)collisionEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetSequences(LunyTriggerEvent triggerEvent) =>
			IsObserving((Int32)triggerEvent, ref _triggerSequences)
				? _triggerSequences[(Int32)triggerEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetSequences(LunyCollision2DEvent collisionEvent) =>
			IsObserving((Int32)collisionEvent, ref _collisionSequences)
				? _collisionSequences[(Int32)collisionEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetSequences(LunyTrigger2DEvent triggerEvent) =>
			IsObserving((Int32)triggerEvent, ref _triggerSequences)
				? _triggerSequences[(Int32)triggerEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetPhysicsSequences(LunyCollisionEvent collisionEvent) => _collisionSequences != null &&
			_collisionSequences[(Int32)collisionEvent] != null &&
			_collisionSequences[(Int32)collisionEvent].Count > 0
				? _collisionSequences[(Int32)collisionEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetPhysicsSequences(LunyTriggerEvent triggerEvent) => _triggerSequences != null &&
			_triggerSequences[(Int32)triggerEvent] != null &&
			_triggerSequences[(Int32)triggerEvent].Count > 0
				? _triggerSequences[(Int32)triggerEvent]
				: null;

		internal Boolean IsObserving(Int32 eventIndex, ref List<ISequenceBlock>[] sequencesRef) =>
			sequencesRef != null && sequencesRef[eventIndex] != null && sequencesRef[eventIndex].Count > 0;

		internal Boolean IsObservingAnyOf(Type enumType)
		{
			switch (enumType)
			{
				case not null when enumType == typeof(LunyObjectEvent):
					return _objectSequences != null;
				case not null when enumType == typeof(LunySceneEvent):
					return _sceneSequences != null;
				case not null when enumType == typeof(LunyCollisionEvent):
					return _collisionSequences != null;
				case not null when enumType == typeof(LunyTriggerEvent):
					return _triggerSequences != null;
				case not null when enumType == typeof(LunyCollision2DEvent):
					return _collision2DSequences != null;
				case not null when enumType == typeof(LunyTrigger2DEvent):
					return _trigger2DSequences != null;
				default:
					throw new ArgumentOutOfRangeException(nameof(enumType), enumType?.ToString());
			}
		}

		internal void Unschedule(LunyObjectEvent objectEvent)
		{
			if (_objectSequences == null)
				return;

			_objectSequences[(Int32)objectEvent] = null;
		}

		public void Shutdown() => GC.SuppressFinalize(this);
	}
}
