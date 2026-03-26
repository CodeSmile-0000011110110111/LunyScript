using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Pairs object with the most recently used input device.
	/// </summary>
	internal sealed class InputIsPairedBlock : ConditionBlock
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
			var inputEvent = runtimeContext?.EventArgs as LunyInputActionEvent;
			return inputEvent != null && LunyEngine.Instance.Input.IsUserPairedWithDevice(_userName, inputEvent.DeviceId);
		}
	}
}
