using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;
using System.Collections.Generic;

namespace LunyScript
{
	/// <summary>
	/// Schedules and manages sequences for various event types.
	/// </summary>
	internal sealed class ScriptEventScheduler
	{
		// ── Category registry ─────────────────────────────────────────────
		private static readonly Dictionary<Type, Int32> s_CategoryOffsets = new();
		private static readonly Dictionary<Type, Int32> s_CategoryCounts = new();
		private static Int32 s_NextOffset;

		// ── Cached per-category offsets for allocation-free hot-path gets ─
		private static readonly Int32 s_ObjectEventOffset;
		private static readonly Int32 s_SceneEventOffset;
		private static readonly Int32 s_CollisionEventOffset;
		private static readonly Int32 s_TriggerEventOffset;
		private static readonly Int32 s_Collision2DEventOffset;
		private static readonly Int32 s_Trigger2DEventOffset;
		private static readonly Int32 s_InputActionPhaseOffset;

		// ── Instance storage ─────────────────────────────────────────────
		// Generic flat store: key = categoryOffset + (Int32)enumValue
		private Dictionary<Int32, List<ISequenceBlock>> _sequences;

		// Input-action fast runtime store: action name → sequences per phase
		// References are duplicated from _sequences at setup time
		private Dictionary<String, List<InputEventSequenceBlock>[]> _inputActionSequences;

		// ── Diagnostics accessors ────────────────────────────────────────

		/// <summary>
		/// All registered event enum categories, excluding <see cref="LunyInputActionPhase"/>.
		/// Input-action events are enumerated separately via <see cref="GetInputActionNames"/>.
		/// Intended for diagnostics and tree-view tooling only.
		/// </summary>
		internal static IEnumerable<Type> RegisteredCategories
		{
			get
			{
				foreach (var type in s_CategoryOffsets.Keys)
				{
					if (type != typeof(LunyInputActionPhase))
						yield return type;
				}
			}
		}

		/// <summary>
		/// Registers an event enum category with the scheduler, assigning it a contiguous
		/// block of integer keys sized to the number of enum members.
		/// Must be called before scheduling or querying sequences for the given enum.
		/// </summary>
		private static void RegisterEventCategory<TEvent>() where TEvent : Enum
		{
			var type = typeof(TEvent);
			if (s_CategoryOffsets.ContainsKey(type))
				return;

			var count = Enum.GetNames(type).Length;
			s_CategoryOffsets[type] = s_NextOffset;
			s_CategoryCounts[type] = count;
			s_NextOffset += count;
		}

		static ScriptEventScheduler()
		{
			RegisterEventCategory<LunyObjectEvent>();
			s_ObjectEventOffset = s_CategoryOffsets[typeof(LunyObjectEvent)];

			RegisterEventCategory<LunySceneEvent>();
			s_SceneEventOffset = s_CategoryOffsets[typeof(LunySceneEvent)];

			RegisterEventCategory<LunyCollisionEvent>();
			s_CollisionEventOffset = s_CategoryOffsets[typeof(LunyCollisionEvent)];

			RegisterEventCategory<LunyTriggerEvent>();
			s_TriggerEventOffset = s_CategoryOffsets[typeof(LunyTriggerEvent)];

			RegisterEventCategory<LunyCollision2DEvent>();
			s_Collision2DEventOffset = s_CategoryOffsets[typeof(LunyCollision2DEvent)];

			RegisterEventCategory<LunyTrigger2DEvent>();
			s_Trigger2DEventOffset = s_CategoryOffsets[typeof(LunyTrigger2DEvent)];

			RegisterEventCategory<LunyInputActionPhase>();
			s_InputActionPhaseOffset = s_CategoryOffsets[typeof(LunyInputActionPhase)];
		}

		// ── Flat-store helpers ────────────────────────────────────────────

		private void AddToFlatStore(Int32 key, ISequenceBlock sequence)
		{
			_sequences ??= new Dictionary<Int32, List<ISequenceBlock>>();
			if (!_sequences.TryGetValue(key, out var list))
				_sequences[key] = list = new List<ISequenceBlock>();
			list.Add(sequence);
		}

		private IReadOnlyList<ISequenceBlock> GetFromFlatStore(Int32 key) =>
			_sequences != null && _sequences.TryGetValue(key, out var list) ? list.AsReadOnly() : null;

		// ── Scheduling ────────────────────────────────────────────────────

		internal ISequenceBlock ScheduleObjectEventSequence(ActionBlock[] blocks, LunyObjectEvent objectEvent, StackTrace trace = null)
		{
			var sequence = SequenceBlock.TryCreate(blocks, trace);
			if (sequence == null)
				return sequence;

			AddToFlatStore(s_ObjectEventOffset + (Int32)objectEvent, sequence);
			return sequence;
		}

		internal void ScheduleObjectEventSequence(ISequenceBlock sequence, LunyObjectEvent objectEvent) =>
			AddToFlatStore(s_ObjectEventOffset + (Int32)objectEvent, sequence);

		internal ISequenceBlock ScheduleSceneEventSequence(ActionBlock[] blocks, LunySceneEvent sceneEvent, StackTrace trace = null)
		{
			var sequence = SequenceBlock.TryCreate(blocks, trace);
			if (sequence == null)
				return sequence;

			AddToFlatStore(s_SceneEventOffset + (Int32)sceneEvent, sequence);
			return sequence;
		}

		internal ISequenceBlock ScheduleCollisionEventSequence(CollisionSequenceBlock sequence, LunyCollisionEvent collisionEvent)
		{
			if (sequence == null)
				return sequence;

			AddToFlatStore(s_CollisionEventOffset + (Int32)collisionEvent, sequence);
			return sequence;
		}

		internal ISequenceBlock ScheduleTriggerEventSequence(TriggerSequenceBlock sequence, LunyTriggerEvent triggerEvent)
		{
			if (sequence == null)
				return sequence;

			AddToFlatStore(s_TriggerEventOffset + (Int32)triggerEvent, sequence);
			return sequence;
		}

		internal ISequenceBlock ScheduleCollision2DEventSequence(CollisionSequenceBlock sequence, LunyCollision2DEvent collisionEvent)
		{
			if (sequence == null)
				return sequence;

			AddToFlatStore(s_Collision2DEventOffset + (Int32)collisionEvent, sequence);
			return sequence;
		}

		internal ISequenceBlock ScheduleTrigger2DEventSequence(TriggerSequenceBlock sequence, LunyTrigger2DEvent triggerEvent)
		{
			if (sequence == null)
				return sequence;

			AddToFlatStore(s_Trigger2DEventOffset + (Int32)triggerEvent, sequence);
			return sequence;
		}

		internal ISequenceBlock ScheduleInputActionEventSequence(String actionName, LunyInputActionPhase phase,
			InputEventSequenceBlock sequence)
		{
			if (sequence == null)
				return null;

			// 1. Generic flat store (diagnostics / tree view)
			AddToFlatStore(s_InputActionPhaseOffset + (Int32)phase, sequence);

			// 2. Fast runtime store (hot path) — references duplicated from flat store
			var phaseCount = s_CategoryCounts[typeof(LunyInputActionPhase)];
			_inputActionSequences ??= new Dictionary<String, List<InputEventSequenceBlock>[]>();
			if (!_inputActionSequences.TryGetValue(actionName, out var byPhase))
				_inputActionSequences[actionName] = byPhase = new List<InputEventSequenceBlock>[phaseCount];

			byPhase[(Int32)phase] ??= new List<InputEventSequenceBlock>();
			byPhase[(Int32)phase].Add(sequence);
			return sequence;
		}

		/// <summary>
		/// Schedules a sequence for a custom/extension event category.
		/// </summary>
		public ISequenceBlock ScheduleGeneric<TEvent>(TEvent eventMethod, ISequenceBlock sequence) where TEvent : Enum
		{
			if (sequence == null || sequence.IsEmpty)
				return null;

			// Intentional boxing — acceptable at setup time for extension categories
			var key = s_CategoryOffsets[typeof(TEvent)] + (Int32)(Object)eventMethod;
			AddToFlatStore(key, sequence);
			return sequence;
		}

		// ── Get scheduled sequences ───────────────────────────────────────

		internal IReadOnlyList<ISequenceBlock> GetObjectEventSequences(LunyObjectEvent objectEvent) =>
			GetFromFlatStore(s_ObjectEventOffset + (Int32)objectEvent);

		internal IReadOnlyList<ISequenceBlock> GetSceneEventSequences(LunySceneEvent sceneEvent) =>
			GetFromFlatStore(s_SceneEventOffset + (Int32)sceneEvent);

		internal IReadOnlyList<ISequenceBlock> GetCollisionEventSequences(LunyCollisionEvent collisionEvent) =>
			GetFromFlatStore(s_CollisionEventOffset + (Int32)collisionEvent);

		internal IReadOnlyList<ISequenceBlock> GetTriggerEventSequences(LunyTriggerEvent triggerEvent) =>
			GetFromFlatStore(s_TriggerEventOffset + (Int32)triggerEvent);

		internal IReadOnlyList<ISequenceBlock> GetCollision2DEventSequences(LunyCollision2DEvent collisionEvent) =>
			GetFromFlatStore(s_Collision2DEventOffset + (Int32)collisionEvent);

		internal IReadOnlyList<ISequenceBlock> GetTrigger2DEventSequences(LunyTrigger2DEvent triggerEvent) =>
			GetFromFlatStore(s_Trigger2DEventOffset + (Int32)triggerEvent);

		internal IReadOnlyList<InputEventSequenceBlock> GetInputActionEventSequences(String actionName, LunyInputActionPhase phase)
		{
			if (_inputActionSequences == null || !_inputActionSequences.TryGetValue(actionName, out var byPhase))
				return null;

			var list = byPhase[(Int32)phase];
			return list != null ? list : null;
		}

		/// <summary>
		/// Returns all scheduled sequences for the given event method.
		/// Intended for diagnostics and tree-view tooling; boxing is acceptable here.
		/// </summary>
		internal IReadOnlyList<ISequenceBlock> GetObjectEventSequences<TEvent>(TEvent eventMethod) where TEvent : Enum =>
			GetFromFlatStore(s_CategoryOffsets[typeof(TEvent)] + (Int32)(Object)eventMethod);

		/// <summary>
		/// Returns all scheduled sequences for the given enum category type and enum ordinal.
		/// Returns null when no sequences are registered for that key.
		/// Intended for diagnostics and tree-view tooling; boxing is acceptable here.
		/// </summary>
		internal IReadOnlyList<ISequenceBlock> GetObjectEventSequences(Type enumType, Int32 enumValue)
		{
			if (!s_CategoryOffsets.TryGetValue(enumType, out var offset))
				return null;

			return GetFromFlatStore(offset + enumValue);
		}

		/// <summary>
		/// Returns all action names that have at least one scheduled input-action sequence.
		/// Intended for diagnostics and tree-view tooling only.
		/// </summary>
		internal IEnumerable<String> GetInputActionNames() =>
			_inputActionSequences != null ? _inputActionSequences.Keys : Array.Empty<String>();

		// ── Observing queries ─────────────────────────────────────────────

		internal Boolean IsObservingAnyOf(Type enumType)
		{
			if (_sequences == null)
				return false;

			// LunyInputActionEvent is a class used as the event arg type;
			// its scheduling is keyed by LunyInputActionPhase so check the fast store directly
			if (enumType == typeof(LunyInputActionEvent))
				return _inputActionSequences != null;

			if (!s_CategoryOffsets.TryGetValue(enumType, out var offset))
				throw new ArgumentOutOfRangeException(nameof(enumType), enumType?.ToString());

			var count = s_CategoryCounts[enumType];
			for (var i = 0; i < count; i++)
			{
				if (_sequences.ContainsKey(offset + i))
					return true;
			}
			return false;
		}

		internal void Shutdown() => GC.SuppressFinalize(this);
		~ScriptEventScheduler() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
