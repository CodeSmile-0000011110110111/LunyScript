using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// While loop execution block with safety limits.
	/// </summary>
	internal sealed class WhileBlock : ActionBlock, IBlockContainer
	{
		private readonly ConditionBlock[] _conditions;
		private readonly ActionBlock[] _actions;

		// ── Execute ───────────────────────────────────────────────────────

#if DEBUG || UNITY_EDITOR
		private Boolean _didHitMaxLoopIterationLimit;
#endif

		// ── IBlockContainer ───────────────────────────────────────────────

		Int32 IBlockContainer.ConditionSequenceCount => 1;
		Int32 IBlockContainer.ActionSequenceCount => 1;

		public static WhileBlock Create(ConditionBlock[] conditions, ActionBlock[] actions) => new(conditions, actions);

		private WhileBlock(ConditionBlock[] conditions, ActionBlock[] actions)
		{
			_conditions = conditions;
			_actions = actions;
		}

		String IBlockContainer.GetConditionSequenceName(Int32 index) => "While";
		String IBlockContainer.GetActionSequenceName(Int32 index) => "Do";
		IEnumerable<IScriptBlock> IBlockContainer.GetConditionSequence(Int32 index) => _conditions;
		IEnumerable<IScriptBlock> IBlockContainer.GetActionSequence(Int32 index) => _actions;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
#if DEBUG || UNITY_EDITOR
			var iterations = 0;
			var limit = ScriptEngine.MaxLoopIterations;
			if (_didHitMaxLoopIterationLimit)
				return;
#endif

			while (ControlFlow.EvaluateAll(context, _conditions))
			{
#if DEBUG || UNITY_EDITOR
				if (++iterations > limit)
				{
					_didHitMaxLoopIterationLimit = true;
					throw new LunyScriptMaxIterationException(context, nameof(WhileBlock), limit);
				}
#endif
				ControlFlow.ExecuteAll(context, _actions);
			}
		}
	}
}
