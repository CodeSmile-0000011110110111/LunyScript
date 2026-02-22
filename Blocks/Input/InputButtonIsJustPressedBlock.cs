using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Condition block: true only on the frame the named button was pressed (transition).
	/// </summary>
	internal sealed class InputButtonIsJustPressedBlock : VariableBlock
	{
		private readonly String _actionName;

		internal static InputButtonIsJustPressedBlock Create(String actionName) => new(actionName);
		private InputButtonIsJustPressedBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.GetButtonJustPressed(_actionName);

		internal override Variable GetValue() => Evaluate(null);

		public override String ToString() => $"Input.Button(\"{_actionName}\").IsJustPressed";
	}
}
