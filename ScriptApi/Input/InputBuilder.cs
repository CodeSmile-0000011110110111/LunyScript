using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	/// <summary>
	/// Provides access to input action values. Blocks poll the input service for last known state.
	/// </summary>
	public readonly struct InputBuilder
	{
		private readonly Script _script;
		internal InputBuilder(Script script) => _script = script;

		/// <summary>
		/// Returns a VariableBlock reading the last known axis value (Vector2) for the named action.
		/// </summary>
		public VariableBlock<LunyVector2> Direction(String actionName) => InputAxisDirectionBlock.Create(actionName);

		/// <summary>
		/// Returns a VariableBlock reading the last known axis value (Vector2) for the named action.
		/// </summary>
		public VariableBlock<LunyQuaternion> Rotation(String actionName) => InputAxisRotationBlock.Create(actionName);

		public ScriptActionBlock Pair(String userName) => InputAssignUserBlock.Create(userName);
		public ScriptActionBlock Unpair(String userName) => InputUnassignUserBlock.Create(userName);

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
		/// <summary>
		/// Returns button's value while held down.
		/// </summary>
		public VariableBlock Strength => InputButtonStrengthBlock.Create(_actionName);
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
