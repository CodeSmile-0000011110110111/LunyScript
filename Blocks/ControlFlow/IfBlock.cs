using LunyScript.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Builder for constructing 'If' blocks with 'ElseIf' and 'Else' branches.
	/// </summary>
	public sealed class IfBlock : ActionBlock, IBlockContainer
	{
		// builder-phase state (freed after Build)
		private List<(ConditionBlock[] conditions, ActionBlock[] actions)> _branchesBuilder = new();

		// runtime-phase state (used in Execute)
		private (ConditionBlock[] conditions, ActionBlock[] actions)[] _branches;
		private ActionBlock[] _elseBlocks;

		Int32 IBlockContainer.ConditionSequenceCount => _branches.Length;
		Int32 IBlockContainer.ActionSequenceCount => _branches.Length + (_elseBlocks != null ? 1 : 0);

		internal IfBlock(Script script, ConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				throw new LunyScriptException("If() conditions cannot be null or empty");

			var token = script.CreateBuilderToken(nameof(IfBlock), "If");
			token.AutoFinish = () => Build(script, token);
			_branchesBuilder.Add((conditions, Array.Empty<ActionBlock>()));
		}

		String IBlockContainer.GetConditionSequenceName(Int32 index) => index == 0 ? "If" : "ElseIf";
		String IBlockContainer.GetActionSequenceName(Int32 index) => index < _branches.Length ? "Then" : "Else";

		IEnumerable<IScriptBlock> IBlockContainer.GetConditionSequence(Int32 index) =>
			index < _branches.Length ? _branches[index].conditions : Array.Empty<IScriptBlock>();

		IEnumerable<IScriptBlock> IBlockContainer.GetActionSequence(Int32 index) =>
			index < _branches.Length ? _branches[index].actions : _elseBlocks ?? Array.Empty<ActionBlock>();

		public IfBlock Then(params ActionBlock[] actions)
		{
			if (actions == null || actions.Length == 0)
				throw new LunyScriptException("Then() blocks cannot be null or empty");

			var last = _branchesBuilder.Count - 1;
			_branchesBuilder[last] = (_branchesBuilder[last].conditions, actions);
			return this;
		}

		public IfBlock ElseIf(params ConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				throw new LunyScriptException("ElseIf() conditions cannot be null or empty");

			_branchesBuilder.Add((conditions, Array.Empty<ActionBlock>()));
			return this;
		}

		public ActionBlock Else(params ActionBlock[] actions)
		{
			if (actions == null || actions.Length == 0)
				throw new LunyScriptException("Else() blocks cannot be null or empty");

			_elseBlocks = actions;
			return this;
		}

		private void Build(Script script, BuilderToken token)
		{
			if (_branchesBuilder.Count == 0)
				throw new LunyScriptException($"{nameof(IfBlock)} has no branches");

			_branches = _branchesBuilder.ToArray();
			_branchesBuilder = null;
			script.MarkBuilderTokenFinished(token);
		}

		// ── Execute ───────────────────────────────────────────────────────

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			if (_branches == null)
				return;

			foreach (var (conditions, actions) in _branches)
			{
				if (ControlFlow.EvaluateAll(runtimeContext, conditions))
				{
					ControlFlow.ExecuteAll(runtimeContext, actions);
					return;
				}
			}

			if (_elseBlocks != null)
				ControlFlow.ExecuteAll(runtimeContext, _elseBlocks);
		}

		public override String ToString()
		{
			var container = (IBlockContainer)this;
			var sb = new StringBuilder();
			var conditionCount = container.ConditionSequenceCount;
			var actionCount = container.ActionSequenceCount;
			for (int i = 0; i < conditionCount; i++)
			{
				var name = container.GetConditionSequenceName(i);
				sb.Append(name);
				sb.Append('(');
				sb.Append(')');
				if (i < actionCount)
					sb.Append('.');
			}

			var hasElse = conditionCount < container.ActionSequenceCount;
			if (hasElse)
			{
				var lastIndex = actionCount - 1;
				sb.Append(container.GetActionSequenceName(lastIndex));
				sb.Append('(');
				sb.Append(')');
			}

			return sb.ToString();
		}
	}
}
