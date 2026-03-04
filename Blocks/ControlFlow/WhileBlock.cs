using LunyScript.Exceptions;

namespace LunyScript.Blocks
{
	/// <summary>
	/// While loop execution block with safety limits.
	/// </summary>
	internal sealed class WhileBlock : ScriptActionBlock
	{
		private readonly ScriptConditionBlock[] _conditions;
		private readonly ScriptActionBlock[] _actions;

		public static WhileBlock Create(ScriptConditionBlock[] conditions, ScriptActionBlock[] actions) => new(conditions, actions);

		private WhileBlock(ScriptConditionBlock[] conditions, ScriptActionBlock[] actions)
		{
			_conditions = conditions;
			_actions = actions;
		}

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
