using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the analog trigger value (0.0–1.0) for a named button action from the input service.
	/// </summary>
	internal sealed class InputAxisBlock : VariableBlock
	{
		private readonly String _actionName;

		internal override Variable Variable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Double)LunyEngine.Instance.Input.GetAxis(_actionName);
		}

		internal static InputAxisBlock Create(String actionName) => new(actionName);
		private InputAxisBlock(String actionName) => _actionName = actionName;

		public override String ToString() => $"Input.Button(\"{_actionName}\").Value";
	}
}
