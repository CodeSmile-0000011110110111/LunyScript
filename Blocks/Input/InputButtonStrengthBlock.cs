using Luny;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Condition block: true while the named button is held down.
	/// </summary>
	internal sealed class InputButtonStrengthBlock : VariableBlock
	{
		private readonly String _actionName;

		internal override Luny.Variable Variable => LunyEngine.Instance.Input.GetButtonStrength(_actionName);

		internal static InputButtonStrengthBlock Create(String actionName) => new(actionName);
		private InputButtonStrengthBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal override Boolean Evaluate(IScriptRuntimeContext runtimeContext) =>
			LunyEngine.Instance.Input.GetButtonStrength(_actionName) > Single.Epsilon;

		public override String ToString() => $"Input.Button(\"{_actionName}\").Strength";
	}
}
