using LunyScript.Blocks;
using System;

namespace LunyScript.BlockBuilders
{
	/// <summary>
	/// Provides access to input action values. Blocks poll the input service for last known state.
	/// </summary>
	public readonly struct InputApi
	{
		private readonly Script _script;
		internal InputApi(Script script) => _script = script;

		/// <summary>
		/// Returns a VariableBlock reading the last known axis value (LunyVector2) for the named action.
		/// Works inside On.Input(), On.FrameUpdate(), or any other context.
		/// </summary>
		public VariableBlock Direction(String actionName) => InputBlock.Create(actionName);

		/// <summary>
		/// Returns a button handle with condition and value accessors for the named action.
		/// </summary>
		public InputButtonBuilder Button(String actionName) => new(actionName);
		/// <summary>
		/// Returns a axis handle with condition and value accessors for the named action.
		/// </summary>
		public InputAxisBuilder Axis(String actionName) => new(actionName);
	}

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
	}

	/// <summary>
	/// Provides condition and value accessors for a named axis input action.
	/// </summary>
	public readonly struct InputAxisBuilder
	{
		private readonly String _actionName;
		internal InputAxisBuilder(String actionName) => _actionName = actionName;

		/// <summary>
		/// Analog trigger value (0.0–1.0).
		/// </summary>
		public VariableBlock Value => InputAxisValueBlock.Create(_actionName);
	}
}
