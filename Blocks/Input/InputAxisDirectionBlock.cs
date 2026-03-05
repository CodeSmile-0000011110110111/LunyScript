using Luny;
using Luny.Engine.Bridge;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the last known axis value (LunyVector2) for a named input action from the input service.
	/// </summary>
	internal sealed class InputAxisDirectionBlock : VariableBlock<LunyVector2>
	{
		private readonly String _actionName;

		internal static InputAxisDirectionBlock Create(String actionName) => new(actionName);

		private InputAxisDirectionBlock(String actionName) => _actionName = actionName;

		internal override LunyVector2 Value { [MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => LunyEngine.Instance.Input.GetDirection(_actionName); }

		public override String ToString() => $"Input.Axis(\"{_actionName}\")";
	}
}
