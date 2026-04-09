using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class RigidbodyKinematicRotateBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly LunyVector3 _eulerDelta;
		private readonly Boolean _useVector;
		private readonly LunyTransformSpace _space;

		internal static RigidbodyKinematicRotateBlock CreateAxisRelative(VariableBlock amount, LunyAxis axis, LunyTransformSpace space,
			StackTrace trace) => new(amount, axis, default, false, space, trace);

		internal static RigidbodyKinematicRotateBlock CreateVector(LunyVector3 eulerDelta, LunyTransformSpace space, StackTrace trace) =>
			new(null, default, eulerDelta, true, space, trace);

		private static LunyVector3 AxisToVector(LunyAxis axis)
		{
			if (axis == LunyAxis.X)
				return LunyVector3.Right;
			if (axis == LunyAxis.Y)
				return LunyVector3.Up;

			return LunyVector3.Forward;
		}

		private RigidbodyKinematicRotateBlock(VariableBlock amount, LunyAxis axis, LunyVector3 eulerDelta, Boolean useVector,
			LunyTransformSpace space, StackTrace trace)
			: base(trace)
		{
			_amount = amount;
			_axis = axis;
			_eulerDelta = eulerDelta;
			_useVector = useVector;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var rigidbody = context.LunyObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning($"{nameof(RigidbodyKinematicRotateBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyObject.Name}'",
					context.LunyObject);
				return;
			}
			var euler = _useVector
				? _eulerDelta * LunyTime.DeltaTime
				: AxisToVector(_axis) * (_amount.Value * LunyTime.DeltaTime);
			rigidbody.MoveRotation(euler, _space);
		}

		public override String ToString() => $"{GetType().Name}({(_useVector ? _eulerDelta.ToString() : $"{_amount},{_axis}")}, {_space})";
	}
}
