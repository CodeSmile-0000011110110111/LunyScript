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
		private readonly LunyObjectRef _offsetChild;
		private LunyVector3 _offsetOrWorldPos;

		internal static RigidbodyDynamicAddForceAtPositionBlock CreateAxisWithLocalOffset(VariableBlock amount, LunyAxis axis,
			LunyForceMode forceMode, LunyVector3 localOffset, LunyStackTrace trace) =>
			new(amount, axis, default, false, forceMode, localOffset, null, true, trace);

		internal static RigidbodyDynamicAddForceAtPositionBlock CreateVectorWithLocalOffset(LunyVector3 force, LunyForceMode forceMode,
			LunyVector3 localOffset, LunyStackTrace trace) => new(null, default, force, true, forceMode, localOffset, null, true, trace);

		internal static RigidbodyDynamicAddForceAtPositionBlock CreateAxisWithWorldPosition(VariableBlock amount, LunyAxis axis,
			LunyForceMode forceMode, LunyObjectRef offsetChild, LunyStackTrace trace) => new(amount, axis, default, false, forceMode,
			LunyVector3.Zero, offsetChild, false, trace);

		internal static RigidbodyDynamicAddForceAtPositionBlock CreateVectorWithWorldPosition(LunyVector3 force, LunyForceMode forceMode,
			LunyObjectRef offsetChild, LunyStackTrace trace) =>
			new(null, default, force, true, forceMode, LunyVector3.Zero, offsetChild, false, trace);

		private RigidbodyDynamicAddForceAtPositionBlock(VariableBlock amount, LunyAxis axis, LunyVector3 force, Boolean useForce,
			LunyForceMode forceMode, LunyVector3 offsetOrWorldPos, LunyObjectRef offsetChild, Boolean useLocalOffset, LunyStackTrace trace)
			: base(trace)
		{
			_amount = amount;
			_axis = axis;
			_force = force;
			_useForce = useForce;
			_forceMode = forceMode;
			_offsetOrWorldPos = offsetOrWorldPos;
			_useLocalOffset = useLocalOffset;
			_offsetChild = offsetChild;
			_offsetChild?.TryResolveReference(out var _);
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var rigidbody = context.LunyObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning(
					$"{nameof(RigidbodyDynamicAddForceAtPositionBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyObject.Name}'",
					context.LunyObject);
				return;
			}

			var worldPosition = _useLocalOffset
				? context.LunyObject.Transform.TransformPoint(_offsetOrWorldPos)
				: _offsetOrWorldPos;

			var offsetChild = _offsetChild?.Value;
			if (offsetChild != null && offsetChild.IsValid)
				worldPosition = offsetChild.Transform.Position;

			var force = _useForce ? _force : _axis.ToVector3() * _amount.Value;
			rigidbody.AddForceAtPosition(force, worldPosition, _forceMode);
		}

		public override String ToString()
		{
			var force = _useForce ? _force.ToString() : $"{_amount},{_axis}";
			var offset = "";
			var offsetChild = _offsetChild?.Value;
			offset = offsetChild != null && offsetChild.IsValid
				? $"ChildOffset={offsetChild.Transform.LocalPosition}"
				: _useLocalOffset
					? $"Offset={_offsetOrWorldPos}"
					: $"Pos={_offsetOrWorldPos}";
			return $"{force}, {_forceMode}, {offset}";
		}
	}
}
