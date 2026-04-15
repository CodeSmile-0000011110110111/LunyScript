using Luny;
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

		// ── Execute ───────────────────────────────────────────────────────
#if DEBUG || UNITY_EDITOR
		private Boolean _didHitMaxLoopIterationLimit;
#endif

		// ── IBlockContainer ───────────────────────────────────────────────

		Int32 IBlockContainer.ActionSequenceCount => 1;

		public static ForBlock Create(VariableBlock limit, VariableBlock step, ActionBlock[] actions, LunyStackTrace trace) =>
			new(limit, step, actions, trace);

		private ForBlock(VariableBlock limit, VariableBlock step, ActionBlock[] actions, LunyStackTrace trace)
			: base(trace)
		{
			_limit = limit;
			_step = step;
			_actions = actions;
		}

		String IBlockContainer.GetActionSequenceName(Int32 index) => ToString();
		IEnumerable<IScriptBlock> IBlockContainer.GetActionSequence(Int32 index) => _actions;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
#if DEBUG || UNITY_EDITOR
			var iterations = 0;
			var maxLimit = ScriptEngine.MaxLoopIterations;
			if (_didHitMaxLoopIterationLimit)
				return;
#endif

			var step = GetStep();
			var (start, end) = GetStartAndEnd(step);
			for (var i = start; step > 0 ? i <= end : i >= end; i += step)
			{
#if DEBUG || UNITY_EDITOR
				if (++iterations > maxLimit)
				{
					_didHitMaxLoopIterationLimit = true;
					throw new LunyScriptMaxIterationException(context, nameof(ForBlock), maxLimit);
				}
#endif

				ControlFlow.ExecuteAll(context, _actions);
			}
		}

		private (Int32 start, Int32 end) GetStartAndEnd(Int32 step)
		{
			var limit = _limit.Variable.AsInt32();
			var start = step > 0 ? 1 : limit;
			var end = step > 0 ? limit : 1;
			return (start, end);
		}

		private Int32 GetStep()
		{
			var step = _step.Variable.AsInt32();
			return step == 0 ? 1 : step; // don't allow zero step => would cause an infinite loop
		}

		public override String ToString()
		{
			var stepText = String.Empty;
			var step = GetStep();
			if (Math.Abs(step) != 1)
				stepText = $" in steps of {step}";

			var (start, end) = GetStartAndEnd(step);
			var count = Math.Max(0, (end - start) / step + 1);
			return $"For({count} times: from {start} to {end}{stepText})";
		}
	}
}
