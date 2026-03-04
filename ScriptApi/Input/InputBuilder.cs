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
		public VariableBlock Direction(String actionName) => InputAxisDirectionBlock.Create(actionName);

		/// <summary>
		/// Returns a VariableBlock reading the last known axis value (Vector2) for the named action.
		/// </summary>
		public VariableBlock Rotation(String actionName) => InputAxisRotationBlock.Create(actionName);

		/// <summary>
		/// Returns a button handle with condition and value accessors for the named action.
		/// </summary>
		public InputButtonBuilder Button(String actionName) => new(actionName);

		/// <summary>
		/// Returns a axis handle with condition and value accessors for the named action.
		/// </summary>
		public InputAxisBuilder Axis(String actionName) => new(actionName);
	}
}
