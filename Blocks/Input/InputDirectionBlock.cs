using Luny;
using Luny.Engine.Bridge;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the last known axis value (LunyVector2) for a named input action from the input service.
	/// </summary>
	internal sealed class InputBlock : ComputedVariableBlock
	{
		private readonly String _actionName;

		internal static InputBlock Create(String actionName) => new(actionName);
		private InputBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() =>
			Variable.FromVector2(LunyEngine.Instance.Input.GetDirection(_actionName));

		internal override T GetValue<T>()
		{
			if (typeof(T) == typeof(LunyVector2))
			{
				var val = LunyEngine.Instance.Input.GetDirection(_actionName);
				return Unsafe.As<LunyVector2, T>(ref val);
			}
			return base.GetValue<T>();
		}

		public override String ToString() => $"Input.Axis(\"{_actionName}\")";
	}
}
