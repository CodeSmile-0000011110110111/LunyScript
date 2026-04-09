using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Condition block: true while the named button is held down.
	/// </summary>
	internal sealed class InputButtonIsPressedBlock : VariableBlock
	{
		private readonly String _actionName;

		internal override Luny.Variable Variable => Evaluate(null);

		internal static InputButtonIsPressedBlock Create(String actionName) => new(actionName);
		private InputButtonIsPressedBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.GetButtonPressed(_actionName);

		public override String ToString() => $"Input.Button(\"{_actionName}\").IsPressed";
	}
}
