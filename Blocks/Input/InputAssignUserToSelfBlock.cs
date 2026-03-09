using Luny;
using System;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Pairs object with the most recently used input device.
	/// </summary>
	internal sealed class InputAssignUserToSelfBlock : ScriptActionBlock
	{
		private String _userName;
		internal static InputAssignUserToSelfBlock Create(String userName)
		{
			if (string.IsNullOrEmpty(userName))
				throw new ArgumentException($"{nameof(userName)} cannot be null or empty.", nameof(userName));

			return new InputAssignUserToSelfBlock(userName);
		}

		private InputAssignUserToSelfBlock(String userName) => _userName = userName;

		public override String ToString() => "Input.PairDevice()";

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.AssignUserToLastDevice(_userName, runtimeContext.LunyObject);
	}
}
