using LunyScript.Api;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Builder for constructing 'If' blocks with 'ElseIf' and 'Else' branches.
	/// </summary>
	public sealed class IfBlock : ActionBlock
	{
		// builder-phase state (freed after Build)
		private List<(ConditionBlock[] conditions, ActionBlock[] actions)> _branchesBuilder = new();

		// runtime-phase state (used in Execute)
		private (ConditionBlock[] conditions, ActionBlock[] actions)[] _branches;
		private ActionBlock[] _elseBlocks;

		internal IfBlock(Script script, ConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				throw new LunyScriptException("If() conditions cannot be null or empty");

			var token = script.CreateBuilderToken("If", "If");
			token.AutoFinish = () => Build(script, token);
			_branchesBuilder.Add((conditions, Array.Empty<ActionBlock>()));
		}

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
			_branches = _branchesBuilder.ToArray();
			_branchesBuilder = null;
			script.MarkBuilderTokenFinished(token);
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
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
	}
}
