using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class RigidbodyKinematicMoveByBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly LunyVector3 _vector;
		private readonly Boolean _useVector;
		private readonly LunyTransformSpace _space;

		internal static RigidbodyKinematicMoveByBlock CreateAxisRelative(VariableBlock amount, LunyAxis axis, LunyTransformSpace space,
			LunyStackTrace trace) => new(amount, axis, default, false, space, trace);

		internal static RigidbodyKinematicMoveByBlock CreateVector(LunyVector3 delta, LunyTransformSpace space, LunyStackTrace trace) =>
			new(null, default, delta, true, space, trace);

		private RigidbodyKinematicMoveByBlock(VariableBlock amount, LunyAxis axis, LunyVector3 vector, Boolean useVector,
			LunyTransformSpace space, LunyStackTrace trace)
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
			var rigidbody = context.LunyGameObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning($"{nameof(RigidbodyKinematicMoveByBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyGameObject.Name}'",
					context.LunyGameObject);
				return;
			}

			// ensure we are kinematic
			if (!rigidbody.IsKinematic)
				rigidbody.IsKinematic = true;

			var delta = _useVector ? _vector : _axis.ToVector3() * _amount.Value;
			rigidbody.MovePosition(delta, _space);
		}

		public override String ToString()
		{
			var vectorOrAmount = _useVector ? _vector.ToString() : $"{_axis}={_amount}";
			return $"{vectorOrAmount}, {_space}";
		}
	}
}
