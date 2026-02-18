using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	/// <summary>
	/// Provides access to input action values. Blocks poll the input service for last known state.
	/// </summary>
	public readonly struct InputApi
	{
		private readonly IScript _script;
		internal InputApi(IScript script) => _script = script;

		/// <summary>
		/// Returns a VariableBlock reading the last known axis value (LunyVector2) for the named action.
		/// Works inside On.Input(), On.FrameUpdate(), or any other context.
		/// </summary>
		public VariableBlock Axis(String actionName) => InputAxisBlock.Create(actionName);

		/// <summary>
		/// Returns a button handle with condition and value accessors for the named action.
		/// </summary>
		public InputButtonApi Button(String actionName) => new(actionName);
	}

	/// <summary>
	/// Provides condition and value accessors for a named button input action.
	/// </summary>
	public readonly struct InputButtonApi
	{
		private readonly String _actionName;
		internal InputButtonApi(String actionName) => _actionName = actionName;

		/// <summary>
		/// True only on the frame the button was pressed (transition).
		/// </summary>
		public ScriptConditionBlock IsJustPressed => InputIsJustPressedBlock.Create(_actionName);

		/// <summary>
		/// True while the button is held down.
		/// </summary>
		public ScriptConditionBlock IsPressed => InputIsPressedBlock.Create(_actionName);

		/// <summary>
		/// Analog trigger value (0.0–1.0).
		/// </summary>
		public VariableBlock Value => InputButtonValueBlock.Create(_actionName);
	}
}
