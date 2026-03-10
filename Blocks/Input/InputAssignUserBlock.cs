using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Pairs object with the most recently used input device.
	/// </summary>
	internal sealed class InputAssignUserBlock : ScriptActionBlock
	{
		private String _userName;
		internal static InputAssignUserBlock Create(String userName)
		{
			if (string.IsNullOrEmpty(userName))
				throw new ArgumentException($"{nameof(userName)} cannot be null or empty.", nameof(userName));

			return new InputAssignUserBlock(userName);
		}

		private InputAssignUserBlock(String userName) => _userName = userName;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.AssignUserToLastDevice(_userName, runtimeContext.LunyObject);
	}
}
