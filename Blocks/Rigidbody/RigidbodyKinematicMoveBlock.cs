using Luny;
using Luny.Engine.Bridge;
using System;
namespace LunyScript.Blocks
{
	internal sealed class RigidbodyKinematicMoveBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly LunyVector3 _vector;
		private readonly Boolean _useVector;
		private readonly LunyTransformSpace _space;

		internal static RigidbodyKinematicMoveBlock CreateAxisRelative(VariableBlock amount, LunyAxis axis, LunyTransformSpace space, StackTrace trace) =>
			new(amount, axis, default, useVector: false, space, trace);

		internal static RigidbodyKinematicMoveBlock CreateVector(LunyVector3 delta, LunyTransformSpace space, StackTrace trace) =>
			new(null, default, delta, useVector: true, space, trace);

		private RigidbodyKinematicMoveBlock(VariableBlock amount, LunyAxis axis, LunyVector3 vector, Boolean useVector, LunyTransformSpace space, StackTrace trace)
			: base(trace)
		{
			_amount = amount;
			_axis = axis;
			_vector = vector;
			_useVector = useVector;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var rigidbody = context.LunyObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning($"{nameof(RigidbodyKinematicMoveBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyObject.Name}'", context.LunyObject);
				return;
			}
			var delta = _useVector
				? _vector * LunyTime.DeltaTime
				: AxisToVector(_axis) * (_amount.Value * LunyTime.DeltaTime);
			rigidbody.MovePosition(delta, _space);
		}

		private static LunyVector3 AxisToVector(LunyAxis axis)
		{
			if (axis == LunyAxis.X)
				return LunyVector3.Right;
			if (axis == LunyAxis.Y)
				return LunyVector3.Up;
			return LunyVector3.Forward;
		}

		public override String ToString() => $"{GetType().Name}({(_useVector ? _vector.ToString() : $"{_amount},{_axis}")}, {_space})";
	}
}
