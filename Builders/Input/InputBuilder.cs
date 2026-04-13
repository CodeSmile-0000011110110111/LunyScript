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
}
