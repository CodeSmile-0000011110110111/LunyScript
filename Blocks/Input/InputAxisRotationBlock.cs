using Luny;
using Luny.Engine.Bridge;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Blocks
{
	/// <summary>
	/// Reads the last known rotation (LunyQuaternion) for a named input action from the input service.
	/// </summary>
	internal sealed class InputAxisRotationBlock : VariableBlock<LunyQuaternion>
	{
		private readonly String _actionName;
		private readonly LunyVector3 _worldUp;

		public override LunyQuaternion Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => LunyEngine.Instance.Input.GetRotation(_actionName, _worldUp);
		}

		internal static InputAxisRotationBlock Create(String actionName) => new(actionName, LunyVector3.Up);
		internal static InputAxisRotationBlock Create(String actionName, LunyVector3 worldUp) => new(actionName, worldUp);

		private InputAxisRotationBlock(String actionName, LunyVector3 worldUp)
		{
			_actionName = actionName;
			_worldUp = worldUp;
		}

		public override String ToString() => $"Input.Axis(\"{_actionName}\")";
	}
}
