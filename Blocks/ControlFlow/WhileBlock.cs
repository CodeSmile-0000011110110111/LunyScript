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

		public static WhileBlock Create(ConditionBlock[] conditions, ActionBlock[] actions) => new(conditions, actions);

		private WhileBlock(ConditionBlock[] conditions, ActionBlock[] actions)
		{
			_conditions = conditions;
			_actions = actions;
		}

		// ── IBlockContainer ───────────────────────────────────────────────

		Int32 IBlockContainer.ConditionSequenceCount => 1;
		Int32 IBlockContainer.ActionSequenceCount => 1;
		String IBlockContainer.GetConditionSequenceName(Int32 index) => "While";
		String IBlockContainer.GetActionSequenceName(Int32 index) => "Do";
		IEnumerable<IScriptBlock> IBlockContainer.GetConditionSequence(Int32 index) => _conditions;
		IEnumerable<IScriptBlock> IBlockContainer.GetActionSequence(Int32 index) => _actions;

		// ── Execute ───────────────────────────────────────────────────────

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || UNITY_EDITOR
			var iterations = 0;
			var limit = ScriptEngine.MaxLoopIterations;
#endif

			while (ControlFlow.EvaluateAll(runtimeContext, _conditions))
			{
#if DEBUG || UNITY_EDITOR
				if (++iterations > limit)
					throw new LunyScriptMaxIterationException(runtimeContext, nameof(WhileBlock), limit);
#endif
				ControlFlow.ExecuteAll(runtimeContext, _actions);
			}
		}
	}
}
