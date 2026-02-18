using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Condition block: true while the named button is held down.
	/// </summary>
	internal sealed class InputIsPressedBlock : ScriptConditionBlock
	{
		private readonly String _actionName;

		internal static InputIsPressedBlock Create(String actionName) => new(actionName);
		private InputIsPressedBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.GetButtonPressed(_actionName);

		public override String ToString() => $"Input.Button(\"{_actionName}\").IsPressed";
	}
}
