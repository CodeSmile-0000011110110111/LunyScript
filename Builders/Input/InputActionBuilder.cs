using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly struct InputActionBuilder
	{
		private readonly String _actionName;

		public InputActionBuilder(String actionName) => _actionName = actionName;

		/// <summary>
		/// Enables an input action or action map.
		/// </summary>
		/// <returns></returns>
		public ActionBlock Enable() => InputActionEnableBlock.Create(_actionName);

		/// <summary>
		/// Disables an input action or action map.
		/// </summary>
		/// <returns></returns>
		public ActionBlock Disable() => InputActionDisableBlock.Create(_actionName);
	}
}
