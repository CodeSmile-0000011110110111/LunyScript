using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Services;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the last known axis value (LunyVector2) for a named input action from the input service.
	/// </summary>
	internal sealed class InputAxisBlock : ComputedVariableBlock
	{
		private readonly String _actionName;

		internal static InputAxisBlock Create(String actionName) => new(actionName);
		private InputAxisBlock(String actionName) => _actionName = actionName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue(IScriptRuntimeContext runtimeContext) =>
			Variable.FromVector2(LunyEngine.Instance.Input.GetAxisValue(_actionName));

		internal override T GetValue<T>(IScriptRuntimeContext runtimeContext)
		{
			if (typeof(T) == typeof(LunyVector2))
			{
				var val = LunyEngine.Instance.Input.GetAxisValue(_actionName);
				return Unsafe.As<LunyVector2, T>(ref val);
			}
			return base.GetValue<T>(runtimeContext);
		}

		public override String ToString() => $"Input.Axis(\"{_actionName}\")";
	}
}
