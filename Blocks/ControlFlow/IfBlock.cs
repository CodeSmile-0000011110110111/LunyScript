using Luny;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Conditional execution block supporting If / ElseIf / Else branches.
	/// </summary>
	internal sealed class IfBlock : ActionBlock, IBlockContainer
	{
		private readonly (ConditionBlock[] conditions, ActionBlock[] actions)[] _branches;
		private readonly ActionBlock[] _elseBranch;

		public static IfBlock Create(
			(ConditionBlock[] conditions, ActionBlock[] actions)[] branches,
			ActionBlock[] elseBlocks,
			StackTrace trace) => new(branches, elseBlocks, trace);

		private IfBlock(
			(ConditionBlock[] conditions, ActionBlock[] actions)[] branches,
			ActionBlock[] elseBranch,
			StackTrace trace) : base(trace)
		{
			_branches = branches;
			_elseBranch = elseBranch;
		}

		// ── IBlockContainer ───────────────────────────────────────────────

		Int32 IBlockContainer.ConditionSequenceCount => _branches.Length;
		Int32 IBlockContainer.ActionSequenceCount => _branches.Length + (_elseBranch != null ? 1 : 0);

		String IBlockContainer.GetConditionSequenceName(Int32 index) => index == 0 ? "If" : "ElseIf";
		String IBlockContainer.GetActionSequenceName(Int32 index) => index < _branches.Length ? "Then" : "Else";

		IEnumerable<IScriptBlock> IBlockContainer.GetConditionSequence(Int32 index) =>
			index < _branches.Length ? _branches[index].conditions : Array.Empty<IScriptBlock>();

		IEnumerable<IScriptBlock> IBlockContainer.GetActionSequence(Int32 index) =>
			index < _branches.Length ? _branches[index].actions : _elseBranch ?? Array.Empty<ActionBlock>();

		// ── Execute ───────────────────────────────────────────────────────

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			foreach (var (conditions, actions) in _branches)
			{
				if (ControlFlow.EvaluateAll(context, conditions))
				{
					ControlFlow.ExecuteAll(context, actions);
					return;
				}
			}

			if (_elseBranch != null)
				ControlFlow.ExecuteAll(context, _elseBranch);
		}

		public override String ToString()
		{
			var sb = new StringBuilder();
			var container = (IBlockContainer)this;
			var conditionCount = container.ConditionSequenceCount;
			var actionCount = container.ActionSequenceCount;
			for (var i = 0; i < conditionCount; i++)
			{
				var name = container.GetConditionSequenceName(i);
				sb.Append(name);
				sb.Append('/');
				// sb.Append('(');
				// sb.Append(_branches[i].conditions.Length);
				// sb.Append(')');
				// if (i < actionCount)
				// 	sb.Append('.');
			}

			var hasElse = conditionCount < container.ActionSequenceCount;
			if (hasElse)
			{
				var lastIndex = actionCount - 1;
				sb.Append(container.GetActionSequenceName(lastIndex));
				// sb.Append('/');
				// sb.Append('(');
				// sb.Append(_elseBranch?.Length);
				// sb.Append(')');
			}

			return sb.ToString();
		}
	}
}
