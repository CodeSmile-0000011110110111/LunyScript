using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class RigidbodyDynamicAddForceAtPositionBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly LunyVector3 _force;
		private readonly Boolean _useForce;
		private readonly LunyForceMode _forceMode;
		// When _useLocalOffset is true, _offsetOrWorldPos is a local-space offset resolved at runtime via TransformPoint.
		// When _useLocalOffset is false, _offsetOrWorldPos is a baked world position (from child ref resolved at build time).
		private readonly Boolean _useLocalOffset;
		private readonly LunyVector3 _offsetOrWorldPos;

		internal static RigidbodyDynamicAddForceAtPositionBlock CreateAxisWithLocalOffset(VariableBlock amount, LunyAxis axis, LunyForceMode forceMode, LunyVector3 localOffset, StackTrace trace) =>
			new(amount, axis, default, useForce: false, forceMode, localOffset, useLocalOffset: true, trace);

		internal static RigidbodyDynamicAddForceAtPositionBlock CreateVectorWithLocalOffset(LunyVector3 force, LunyForceMode forceMode, LunyVector3 localOffset, StackTrace trace) =>
			new(null, default, force, useForce: true, forceMode, localOffset, useLocalOffset: true, trace);

		internal static RigidbodyDynamicAddForceAtPositionBlock CreateAxisWithWorldPosition(VariableBlock amount, LunyAxis axis, LunyForceMode forceMode, LunyVector3 worldPosition, StackTrace trace) =>
			new(amount, axis, default, useForce: false, forceMode, worldPosition, useLocalOffset: false, trace);

		internal static RigidbodyDynamicAddForceAtPositionBlock CreateVectorWithWorldPosition(LunyVector3 force, LunyForceMode forceMode, LunyVector3 worldPosition, StackTrace trace) =>
			new(null, default, force, useForce: true, forceMode, worldPosition, useLocalOffset: false, trace);

		private RigidbodyDynamicAddForceAtPositionBlock(VariableBlock amount, LunyAxis axis, LunyVector3 force, Boolean useForce, LunyForceMode forceMode, LunyVector3 offsetOrWorldPos, Boolean useLocalOffset, StackTrace trace)
			: base(trace)
		{
			_amount = amount;
			_axis = axis;
			_force = force;
			_useForce = useForce;
			_forceMode = forceMode;
			_offsetOrWorldPos = offsetOrWorldPos;
			_useLocalOffset = useLocalOffset;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var rigidbody = context.LunyObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning($"{nameof(RigidbodyDynamicAddForceAtPositionBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyObject.Name}'", context.LunyObject);
				return;
			}
			var worldPosition = _useLocalOffset
				? context.LunyObject.Transform.TransformPoint(_offsetOrWorldPos)
				: _offsetOrWorldPos;
			var force = _useForce ? _force : AxisToVector(_axis) * _amount.Value;
			rigidbody.AddForceAtPosition(force, worldPosition, _forceMode);
		}

		private static LunyVector3 AxisToVector(LunyAxis axis)
		{
			if (axis == LunyAxis.X)
				return LunyVector3.Right;
			if (axis == LunyAxis.Y)
				return LunyVector3.Up;
			return LunyVector3.Forward;
		}

		public override String ToString() => $"{GetType().Name}({(_useForce ? _force.ToString() : $"{_amount},{_axis}")}, {_forceMode}, {(_useLocalOffset ? $"localOffset={_offsetOrWorldPos}" : $"worldPos={_offsetOrWorldPos}")})";
	}
}
