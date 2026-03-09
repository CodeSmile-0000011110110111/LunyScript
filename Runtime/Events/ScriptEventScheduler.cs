using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Blocks.PhysicsEvent;
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
		private static readonly Int32 s_InputActionEventCount = Enum.GetNames(typeof(LunyInputActionPhase)).Length;

		// Fast array-based storage for lifecycle events (hot path)
		private List<ISequenceBlock>[] _objectSequences;
		private List<ISequenceBlock>[] _sceneSequences;
		private List<ISequenceBlock>[] _collisionSequences;
		private List<ISequenceBlock>[] _triggerSequences;
		private List<ISequenceBlock>[] _collision2DSequences;
		private List<ISequenceBlock>[] _trigger2DSequences;
		private Dictionary<String, List<InputEventSequenceBlock>[]> _inputActionSequences;

		private static ISequenceBlock SchedulePhysicsSequence(ref List<ISequenceBlock>[] sequencesRef, ISequenceBlock sequence,
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

		// Scheduling
		internal ISequenceBlock ScheduleObjectEventSequence(ScriptActionBlock[] blocks, LunyObjectEvent objectEvent) =>
			ScheduleSequence(ref _objectSequences, SequenceBlock.TryCreate(blocks), (Int32)objectEvent, s_ObjectEventCount);

		internal ISequenceBlock ScheduleSceneEventSequence(ScriptActionBlock[] blocks, LunySceneEvent sceneEvent) =>
			ScheduleSequence(ref _sceneSequences, SequenceBlock.TryCreate(blocks), (Int32)sceneEvent, s_SceneEventCount);

		internal ISequenceBlock ScheduleCollisionEventSequence(CollisionSequenceBlock blocks, LunyCollisionEvent collisionEvent) =>
			SchedulePhysicsSequence(ref _collisionSequences, blocks, (Int32)collisionEvent, s_CollisionEventCount);

		internal ISequenceBlock ScheduleTriggerEventSequence(TriggerSequenceBlock blocks, LunyTriggerEvent triggerEvent) =>
			SchedulePhysicsSequence(ref _triggerSequences, blocks, (Int32)triggerEvent, s_TriggerEventCount);

		internal ISequenceBlock ScheduleInputActionEventSequence(String actionName, LunyInputActionPhase phase,
			InputEventSequenceBlock sequence)
		{
			if (sequence == null || sequence.IsEmpty)
				return null;

			_inputActionSequences ??= new Dictionary<String, List<InputEventSequenceBlock>[]>();
			if (!_inputActionSequences.TryGetValue(actionName, out var sequences))
				_inputActionSequences[actionName] = sequences = new List<InputEventSequenceBlock>[s_InputActionEventCount];

			var eventIndex = (Int32)phase;
			sequences[eventIndex] ??= new List<InputEventSequenceBlock>();
			sequences[eventIndex].Add(sequence);
			return sequence;
		}

		internal void Unschedule(LunyObjectEvent objectEvent)
		{
			if (_objectSequences == null)
				return;

			_objectSequences[(Int32)objectEvent] = null;
		}

		// Get scheduled sequences
		internal IEnumerable<ISequenceBlock> GetObjectEventSequences(LunyObjectEvent objectEvent) =>
			IsObserving((Int32)objectEvent, ref _objectSequences) ? _objectSequences[(Int32)objectEvent] : null;

		internal IEnumerable<ISequenceBlock> GetSceneEventSequences(LunySceneEvent sceneEvent) =>
			IsObserving((Int32)sceneEvent, ref _sceneSequences)
				? _sceneSequences[(Int32)sceneEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetCollisionEventSequences(LunyCollisionEvent collisionEvent) =>
			IsObserving((Int32)collisionEvent, ref _collisionSequences)
				? _collisionSequences[(Int32)collisionEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetTriggerEventSequences(LunyTriggerEvent triggerEvent) =>
			IsObserving((Int32)triggerEvent, ref _triggerSequences)
				? _triggerSequences[(Int32)triggerEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetCollision2DEventSequences(LunyCollision2DEvent collisionEvent) =>
			IsObserving((Int32)collisionEvent, ref _collisionSequences)
				? _collisionSequences[(Int32)collisionEvent]
				: null;

		internal IEnumerable<ISequenceBlock> GetTrigger2DEventSequences(LunyTrigger2DEvent triggerEvent) =>
			IsObserving((Int32)triggerEvent, ref _triggerSequences)
				? _triggerSequences[(Int32)triggerEvent]
				: null;

		internal IEnumerable<InputEventSequenceBlock> GetInputActionEventSequences(String actionName, LunyInputActionPhase phase) =>
			IsObservingInputAction(actionName, phase, out var sequences) ? sequences : null;

		// Observing queries
		private Boolean IsObserving(Int32 eventIndex, ref List<ISequenceBlock>[] sequencesRef) =>
			sequencesRef != null && sequencesRef[eventIndex] != null && sequencesRef[eventIndex].Count > 0;

		private Boolean IsObservingInputAction(String actionName, LunyInputActionPhase phase, out List<InputEventSequenceBlock> sequencesRef)
		{
			sequencesRef = _inputActionSequences.TryGetValue(actionName, out var sequences) ? sequences[(Int32)phase] : null;
			return sequencesRef != null && sequencesRef.Count > 0;
		}

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
				case not null when enumType == typeof(LunyInputActionEvent):
					return _inputActionSequences != null;
				default:
					throw new ArgumentOutOfRangeException(nameof(enumType), enumType?.ToString());
			}
		}

		internal void Shutdown() => GC.SuppressFinalize(this);
		~ScriptEventScheduler() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
