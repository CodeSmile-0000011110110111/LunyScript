using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// For loop execution block with 1-based indexing and safety limits.
	/// </summary>
	internal sealed class ForBlock : ActionBlock, IBlockContainer
	{
		private readonly VariableBlock _limit;
		private readonly VariableBlock _step;
		private readonly ActionBlock[] _actions;

		public static ForBlock Create(VariableBlock limit, VariableBlock step, ActionBlock[] actions) => new(limit, step, actions);

		private ForBlock(VariableBlock limit, VariableBlock step, ActionBlock[] actions)
		{
			_limit = limit;
			_step = step;
			_actions = actions;
		}

		// ── IBlockContainer ───────────────────────────────────────────────

		Int32 IBlockContainer.ActionSequenceCount => 1;
		String IBlockContainer.GetActionSequenceName(Int32 index) => "Do";
		IEnumerable<IScriptBlock> IBlockContainer.GetActionSequence(Int32 index) => _actions;

		// ── Execute ───────────────────────────────────────────────────────

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
#if DEBUG || UNITY_EDITOR
			var iterations = 0;
			var maxLimit = ScriptEngine.MaxLoopIterations;
#endif

			var step = _step.Variable.AsInt32();
			step = step == 0 ? 1 : step; // don't allow zero step => would cause an infinite loop
			var limit = _limit.Variable.AsInt32();
			var start = step > 0 ? 1 : limit;
			var end = step > 0 ? limit : 1;
			for (var i = start; step > 0 ? i <= end : i >= end; i += step)
			{
#if DEBUG || UNITY_EDITOR
				if (++iterations > maxLimit)
					throw new LunyScriptMaxIterationException(runtimeContext, nameof(ForBlock), maxLimit);
#endif

				ControlFlow.ExecuteAll(runtimeContext, _actions);
			}
		}
	}
}
