using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Condition block: true only on the frame the named button was pressed (transition).
	/// </summary>
	internal sealed class InputIsJustPressedBlock : ScriptConditionBlock
	{
		private readonly String _actionName;

		internal static InputIsJustPressedBlock Create(String actionName) => new(actionName);
		private InputIsJustPressedBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.GetButtonJustPressed(_actionName);

		public override String ToString() => $"Input.Button(\"{_actionName}\").IsJustPressed";
	}
}
