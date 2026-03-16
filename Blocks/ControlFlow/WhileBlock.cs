namespace LunyScript.Blocks
{
	/// <summary>
	/// While loop execution block with safety limits.
	/// </summary>
	internal sealed class WhileBlock : ActionBlock
	{
		private readonly ConditionBlock[] _conditions;
		private readonly ActionBlock[] _actions;

		public static WhileBlock Create(ConditionBlock[] conditions, ActionBlock[] actions) => new(conditions, actions);

		private WhileBlock(ConditionBlock[] conditions, ActionBlock[] actions)
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
