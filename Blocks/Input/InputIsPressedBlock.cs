using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Condition block: true while the named button is held down.
	/// </summary>
	internal sealed class InputIsPressedBlock : VariableBlock
	{
		private readonly String _actionName;

		internal static InputIsPressedBlock Create(String actionName) => new(actionName);
		private InputIsPressedBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.GetButtonPressed(_actionName);

		internal override Variable GetValue(IScriptRuntimeContext runtimeContext) => Evaluate(runtimeContext);

		public override String ToString() => $"Input.Button(\"{_actionName}\").IsPressed";
	}
}
