using System;

namespace LunyScript.Blocks
{
	public static class ControlFlow
	{
		public static Boolean EvaluateAll(IScriptRuntimeContext runtimeContext, ScriptConditionBlock[] conditions)
		{
			foreach (var condition in conditions)
			{
				if (condition == null || !condition.Evaluate(runtimeContext))
					return false;
			}

			return true;
		}

		public static void ExecuteAll(IScriptRuntimeContext runtimeContext, ScriptActionBlock[] actions)
		{
			foreach (var block in actions)
				block?.Execute(runtimeContext);
		}
	}
}
