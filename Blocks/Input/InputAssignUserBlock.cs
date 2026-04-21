using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Pairs object with the most recently used input device.
	/// </summary>
	internal sealed class InputAssignUserBlock : ActionBlock
	{
		private String _userName;

		internal static InputAssignUserBlock Create(String userName)
		{
			if (String.IsNullOrEmpty(userName))
				throw new ArgumentException($"{nameof(userName)} cannot be null or empty.", nameof(userName));

			return new InputAssignUserBlock(userName);
		}

		private InputAssignUserBlock(String userName) => _userName = userName;

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var inputEvent = context.EventArgs as LunyInputActionEvent;
			if (inputEvent == null)
				throw new LunyScriptException($"{nameof(InputAssignUserBlock)} can only be used in Input event sequences.");

			LunyEngine.Instance.Input.AssignUserToLastDevice(_userName, inputEvent.DeviceId, context.LunyGameObject);
		}
	}

	/// <summary>
	/// Pairs object with the most recently used input device.
	/// </summary>
	internal sealed class InputUnassignUserBlock : ActionBlock
	{
		private String _userName;

		internal static InputUnassignUserBlock Create(String userName)
		{
			if (String.IsNullOrEmpty(userName))
				throw new ArgumentException($"{nameof(userName)} cannot be null or empty.", nameof(userName));

			return new InputUnassignUserBlock(userName);
		}

		private InputUnassignUserBlock(String userName) => _userName = userName;

		protected internal override void Execute(IScriptRuntimeContext context) => LunyEngine.Instance.Input.UnassignUser(_userName);
	}
}
