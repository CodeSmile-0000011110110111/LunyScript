using LunyScript.Blocks;
using System;

namespace LunyScript.Api.Input
{
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
