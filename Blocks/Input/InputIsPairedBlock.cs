using Luny;
using Luny.Engine.Bridge;
using LunyScript.Exceptions;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Pairs object with the most recently used input device.
	/// </summary>
	internal sealed class InputIsPairedBlock : ScriptConditionBlock
	{
		private String _userName;

		internal static InputIsPairedBlock Create(String userName)
		{
			if (String.IsNullOrEmpty(userName))
				throw new ArgumentException($"{nameof(userName)} cannot be null or empty.", nameof(userName));

			return new InputIsPairedBlock(userName);
		}

		private InputIsPairedBlock(String userName) => _userName = userName;

		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext)
		{
			var inputEvent = runtimeContext.EventArgs as LunyInputActionEvent;
			if (inputEvent == null)
				throw new LunyScriptException($"{nameof(InputIsPairedBlock)} can only be used in Input event sequences.");

			return LunyEngine.Instance.Input.IsUserPairedWithDevice(_userName, inputEvent.DeviceId);
		}
	}
}
