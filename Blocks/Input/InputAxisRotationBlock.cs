using Luny;
using Luny.Engine.Bridge;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the last known axis value (LunyVector2) for a named input action from the input service.
	/// </summary>
	internal sealed class InputAxisRotationBlock : ComputedVariableBlock
	{
		private readonly String _actionName;
		private LunyVector3 _worldUp;

		internal static InputAxisRotationBlock Create(String actionName) => new(actionName, LunyVector3.Up);
		internal static InputAxisRotationBlock Create(String actionName, LunyVector3 worldUp) => new(actionName, worldUp);

		private InputAxisRotationBlock(String actionName, LunyVector3 worldUp)
		{
			_actionName = actionName;
			_worldUp = worldUp;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal override Variable GetValue() => Variable.FromQuaternion(GetInputRotation());

		internal override T GetValue<T>()
		{
			if (typeof(T) == typeof(LunyQuaternion))
			{
				var val = GetInputRotation();
				return Unsafe.As<LunyQuaternion, T>(ref val);
			}
			return base.GetValue<T>();
		}

		private LunyQuaternion GetInputRotation() => LunyEngine.Instance.Input.GetRotation(_actionName, _worldUp);

		public override String ToString() => $"Input.Axis(\"{_actionName}\")";
	}
}
