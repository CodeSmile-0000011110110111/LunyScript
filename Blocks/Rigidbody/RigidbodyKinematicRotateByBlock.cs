using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class RigidbodyKinematicRotateByBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly LunyVector3 _eulerDelta;
		private readonly Boolean _useVector;
		private readonly LunyTransformSpace _space;

		internal static RigidbodyKinematicRotateByBlock CreateAxisRelative(VariableBlock amount, LunyAxis axis, LunyTransformSpace space,
			LunyStackTrace trace) => new(amount, axis, default, false, space, trace);

		internal static RigidbodyKinematicRotateByBlock CreateVector(LunyVector3 eulerDelta, LunyTransformSpace space, LunyStackTrace trace) =>
			new(null, default, eulerDelta, true, space, trace);

		private RigidbodyKinematicRotateByBlock(VariableBlock amount, LunyAxis axis, LunyVector3 eulerDelta, Boolean useVector,
			LunyTransformSpace space, LunyStackTrace trace)
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
				LunyLogger.LogWarning($"{nameof(RigidbodyKinematicRotateByBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyObject.Name}'",
					context.LunyObject);
				return;
			}

			// ensure we are kinematic
			if (!rigidbody.IsKinematic)
				rigidbody.IsKinematic = true;

			var euler = _useVector ? _eulerDelta : _axis.ToVector3() * _amount.Value;
			rigidbody.MoveRotation(euler, _space);
		}

		public override String ToString() => $"{(_useVector ? _eulerDelta.ToString() : $"{_axis}={_amount}")}, {_space}";
	}
}
