using Luny;
using Luny.Engine.Bridge;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the last known axis value (LunyVector2) for a named input action from the input service.
	/// </summary>
	internal sealed class InputVector2Block : VariableBlock<LunyVector2>
	{
		private readonly String _actionName;

		public override LunyVector2 Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => LunyEngine.Instance.Input.GetDirection(_actionName);
		}

		internal static InputVector2Block Create(String actionName) => new(actionName);

		private InputVector2Block(String actionName) => _actionName = actionName;

		public override String ToString() => $"Input.Axis(\"{_actionName}\")";
	}
}
