using Luny;
using System;

namespace LunyScript.Blocks
{
	internal sealed class InputActionEnableBlock : ActionBlock
	{
		private String _actionName;

		internal static InputActionEnableBlock Create(String actionName)
		{
			if (String.IsNullOrEmpty(actionName))
				throw new ArgumentException($"{nameof(actionName)} cannot be null or empty.", nameof(actionName));

			return new InputActionEnableBlock(actionName);
		}

		private InputActionEnableBlock(String actionName) => _actionName = actionName;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.EnableInputAction(_actionName);
	}

	internal sealed class InputActionDisableBlock : ActionBlock
	{
		private String _actionName;

		internal static InputActionDisableBlock Create(String actionName)
		{
			if (String.IsNullOrEmpty(actionName))
				throw new ArgumentException($"{nameof(actionName)} cannot be null or empty.", nameof(actionName));

			return new InputActionDisableBlock(actionName);
		}

		private InputActionDisableBlock(String actionName) => _actionName = actionName;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.DisableInputAction(_actionName);
	}
}
