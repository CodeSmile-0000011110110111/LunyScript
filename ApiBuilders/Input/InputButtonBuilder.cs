using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Input
{
	/// <summary>
	/// Provides condition and value accessors for a named button input action.
	/// </summary>
	public readonly struct InputButtonBuilder
	{
		private readonly String _actionName;
		internal InputButtonBuilder(String actionName) => _actionName = actionName;

		/// <summary>
		/// True only on the frame the button was pressed (transition).
		/// </summary>
		public VariableBlock IsJustPressed => InputButtonIsJustPressedBlock.Create(_actionName);

		/// <summary>
		/// True while the button is held down.
		/// </summary>
		public VariableBlock IsPressed => InputButtonIsPressedBlock.Create(_actionName);
		/// <summary>
		/// Returns button's value while held down.
		/// </summary>
		public VariableBlock Strength => InputButtonStrengthBlock.Create(_actionName);
	}
}
