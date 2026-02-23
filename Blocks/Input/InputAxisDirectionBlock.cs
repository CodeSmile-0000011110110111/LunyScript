using Luny;
using Luny.Engine.Bridge;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the last known axis value (LunyVector2) for a named input action from the input service.
	/// </summary>
	internal sealed class InputAxisDirectionBlock : ComputedVariableBlock
	{
		private readonly String _actionName;

		internal static InputAxisDirectionBlock Create(String actionName) => new(actionName);
		private InputAxisDirectionBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() => Variable.FromVector2(GetInputDirection());

		internal override T GetValue<T>()
		{
			if (typeof(T) == typeof(LunyVector2))
			{
				var val = GetInputDirection();
				return Unsafe.As<LunyVector2, T>(ref val);
			}
			return base.GetValue<T>();
		}

		private LunyVector2 GetInputDirection() => LunyEngine.Instance.Input.GetDirection(_actionName);

		public override String ToString() => $"Input.Axis(\"{_actionName}\")";
	}
}
