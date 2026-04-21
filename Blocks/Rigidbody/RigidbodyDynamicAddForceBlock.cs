using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	internal sealed class RigidbodyDynamicAddForceBlock : ActionBlock
	{
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly LunyVector3 _vector;
		private readonly Boolean _useVector;
		private readonly LunyForceMode _forceMode;
		private readonly LunyTransformSpace _space;

		internal static RigidbodyDynamicAddForceBlock CreateAxisRelative(VariableBlock amount, LunyAxis axis, LunyForceMode forceMode,
			LunyTransformSpace space, LunyStackTrace trace) => new(amount, axis, default, false, forceMode, space, trace);

		internal static RigidbodyDynamicAddForceBlock CreateVector(LunyVector3 force, LunyForceMode forceMode, LunyTransformSpace space,
			LunyStackTrace trace) => new(null, default, force, true, forceMode, space, trace);

		private RigidbodyDynamicAddForceBlock(VariableBlock amount, LunyAxis axis, LunyVector3 vector, Boolean useVector,
			LunyForceMode forceMode, LunyTransformSpace space, LunyStackTrace trace)
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
			var rigidbody = context.LunyGameObject.Rigidbody;
			if (rigidbody == null)
			{
				LunyLogger.LogWarning($"{nameof(RigidbodyDynamicAddForceBlock)}: no {nameof(ILunyRigidbody)} on '{context.LunyGameObject.Name}'",
					context.LunyGameObject);
				return;
			}

			// ensure we are not kinematic
			if (rigidbody.IsKinematic)
				rigidbody.IsKinematic = false;

			var force = _useVector ? _vector : _axis.ToVector3() * _amount.Value;
			rigidbody.AddForce(force, _forceMode, _space);
		}

		public override String ToString() => $"{(_useVector ? _vector.ToString() : $"{_amount},{_axis}")}, {_forceMode}, {_space}";
	}
}
