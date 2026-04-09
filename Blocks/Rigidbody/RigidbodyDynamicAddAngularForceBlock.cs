using Luny;
using Luny.Engine.Bridge;
using System;
namespace LunyScript.Blocks
{
	internal sealed class RigidbodyDynamicAddAngularForceBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly LunyVector3 _vector;
		private readonly Boolean _useVector;
		private readonly LunyForceMode _forceMode;
		private readonly LunyTransformSpace _space;

		internal static RigidbodyDynamicAddAngularForceBlock CreateAxisRelative(VariableBlock amount, LunyAxis axis, LunyForceMode forceMode, LunyTransformSpace space, StackTrace trace) =>
			new(amount, axis, default, useVector: false, forceMode, space, trace);

		internal static RigidbodyDynamicAddAngularForceBlock CreateVector(LunyVector3 torque, LunyForceMode forceMode, LunyTransformSpace space, StackTrace trace) =>
			new(null, default, torque, useVector: true, forceMode, space, trace);

		private RigidbodyDynamicAddAngularForceBlock(VariableBlock amount, LunyAxis axis, LunyVector3 vector, Boolean useVector, LunyForceMode forceMode, LunyTransformSpace space, StackTrace trace)
			: base(trace)
		{
			_amount = amount;
			_axis = axis;
			_vector = vector;
			_useVector = useVector;
			_forceMode = forceMode;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var rigidbody = context.LunyObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning($"{nameof(RigidbodyDynamicAddAngularForceBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyObject.Name}'", context.LunyObject);
				return;
			}
			var torque = _useVector ? _vector : AxisToVector(_axis) * _amount.Value;
			rigidbody.AddTorque(torque, _forceMode, _space);
		}

		private static LunyVector3 AxisToVector(LunyAxis axis)
		{
			if (axis == LunyAxis.X)
				return LunyVector3.Right;
			if (axis == LunyAxis.Y)
				return LunyVector3.Up;
			return LunyVector3.Forward;
		}

		public override String ToString() => $"{GetType().Name}({(_useVector ? _vector.ToString() : $"{_amount},{_axis}")}, {_forceMode}, {_space})";
	}
}
