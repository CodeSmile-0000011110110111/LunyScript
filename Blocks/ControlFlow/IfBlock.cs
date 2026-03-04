using LunyScript.Exceptions;
using System;
using System.Collections.Generic;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Builder for constructing 'If' blocks with 'ElseIf' and 'Else' branches.
	/// </summary>
	public sealed class IfBlock : ScriptActionBlock
	{
		// builder-phase state (freed after Build)
		private List<(ScriptConditionBlock[] conditions, ScriptActionBlock[] actions)> _branchesBuilder = new();

		// runtime-phase state (used in Execute)
		private (ScriptConditionBlock[] conditions, ScriptActionBlock[] actions)[] _branches;
		private ScriptActionBlock[] _elseBlocks;

		internal IfBlock(Script script, ScriptConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				throw new LunyScriptException("If() conditions cannot be null or empty");

			var token = script.CreateBuilderToken("If", "If");
			token.SetAutoFinish(() => Build(script, token));
			_branchesBuilder.Add((conditions, Array.Empty<ScriptActionBlock>()));
		}

		public IfBlock Then(params ScriptActionBlock[] actions)
		{
			if (actions == null || actions.Length == 0)
				throw new LunyScriptException("Then() blocks cannot be null or empty");

			var last = _branchesBuilder.Count - 1;
			_branchesBuilder[last] = (_branchesBuilder[last].conditions, actions);
			return this;
		}

		public IfBlock ElseIf(params ScriptConditionBlock[] conditions)
		{
			if (conditions == null || conditions.Length == 0)
				throw new LunyScriptException("ElseIf() conditions cannot be null or empty");

			_branchesBuilder.Add((conditions, Array.Empty<ScriptActionBlock>()));
			return this;
		}

		public ScriptActionBlock Else(params ScriptActionBlock[] actions)
		{
			if (actions == null || actions.Length == 0)
				throw new LunyScriptException("Else() blocks cannot be null or empty");

			_elseBlocks = actions;
			return this;
		}

		private void Build(Script script, BuilderToken token)
		{
			_branches = _branchesBuilder.ToArray(); // freeze to array
			_branchesBuilder = null; // release List (GC-eligible)
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
