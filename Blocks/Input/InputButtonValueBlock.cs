using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the analog trigger value (0.0–1.0) for a named button action from the input service.
	/// </summary>
	internal sealed class InputButtonValueBlock : ComputedVariableBlock
	{
		private readonly String _actionName;

		internal static InputButtonValueBlock Create(String actionName) => new(actionName);
		private InputButtonValueBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Variable GetValue(IScriptRuntimeContext runtimeContext) =>
			(Variable)(Double)LunyEngine.Instance.Input.GetButtonValue(_actionName);

		public override String ToString() => $"Input.Button(\"{_actionName}\").Value";
	}
}
