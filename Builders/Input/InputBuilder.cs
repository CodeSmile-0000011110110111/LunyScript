using Luny;
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
		private readonly LunyStackTrace _trace;

		internal InputBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		/// <summary>
		/// Returns a VariableBlock reading the last known axis value (Vector2) for the named action.
		/// </summary>
		public VariableBlock<LunyVector2> Direction(String actionName) => InputVector2Block.Create(actionName);

		/// <summary>
		/// Returns a VariableBlock reading the last known axis value (Vector2) for the named action.
		/// </summary>
		public VariableBlock<LunyQuaternion> Rotation(String actionName) => InputRotationBlock.Create(actionName);

		/// <summary>
		/// Pairs a named input user with the most recently used input device. Only pairs with unused devices. Should be used within an Input.Action event.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public ActionBlock Pair(String userName) => InputAssignUserBlock.Create(userName);

		/// <summary>
		/// Unpairs a named input user from input devices.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public ActionBlock Unpair(String userName) => InputUnassignUserBlock.Create(userName);

		/// <summary>
		/// Checks if the named input user has an input device assigned.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public ConditionBlock IsPaired(String userName) => InputIsPairedBlock.Create(userName);

		/// <summary>
		/// Returns a button handle with condition and value accessors for the named action.
		/// </summary>
		public InputButtonBuilder Button(String actionName) => new(actionName);

		/// <summary>
		/// Returns a axis handle with condition and value accessors for the named action.
		/// </summary>
		public InputAxisBuilder Axis(String actionName) => new(actionName);

		/// <summary>
		/// Used with Input Action maps.
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public InputActionBuilder Action(String actionName) => new(actionName);
	}

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
		public VariableBlock Value => InputAxisBlock.Create(_actionName);

		public VariableBlock Horizontal => InputAxisHorizontalBlock.Create(_actionName);
		public VariableBlock Vertical => InputAxisVerticalBlock.Create(_actionName);

		public static implicit operator VariableBlock(InputAxisBuilder axis) => axis.Value;
	}
}
