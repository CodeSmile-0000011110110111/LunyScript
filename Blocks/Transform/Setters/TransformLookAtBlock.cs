using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformLookAtBlock : ActionBlock
	{
		private readonly LunyObjectRef _target;
		private readonly LunyVector3 _worldUp;
		private readonly LunyVector3 _lockAxis;
		private readonly VariableBlock _speed;
		private readonly LunyInterpolation _interpolation;

		public static TransformLookAtBlock Create(LunyObjectRef target, LunyVector3 worldUp, LunyVector3 lockAxis,
			VariableBlock speed, LunyInterpolation interpolation = LunyInterpolation.Instant,
			LunyStackTrace trace = null) => new(target, worldUp, lockAxis, speed, interpolation, trace);

		private TransformLookAtBlock(LunyObjectRef target, LunyVector3 worldUp, LunyVector3 lockAxis,
			VariableBlock speed, LunyInterpolation interpolation, LunyStackTrace trace)
			: base(trace)
		{
			_target = target;
			_worldUp = worldUp;
			_lockAxis = lockAxis;
			_speed = speed;
			_interpolation = interpolation;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			var targetTransform = _target?.Value?.Transform;
			if (targetTransform == null)
				return;

			if (!VectorUtil.TryGetMaskedDirection(transform.Position, targetTransform.Position, _lockAxis, out var maskedDirection))
				return;

			var lookTarget = transform.Position + maskedDirection;
			var targetRotation = LunyQuaternion.LookRotation(maskedDirection, _worldUp);
			var t = _speed.Value * LunyTime.DeltaTime;

			switch (_interpolation)
			{
				case LunyInterpolation.Spherical:
					transform.Rotation = LunyQuaternion.Slerp(transform.Rotation, targetRotation, t);
					break;
				case LunyInterpolation.Linear:
					transform.Rotation = LunyQuaternion.Lerp(transform.Rotation, targetRotation, t);
					break;
				default:
					transform.LookAt(lookTarget, _worldUp);
					break;
			}
		}

		public override String ToString() => _interpolation == LunyInterpolation.Instant
			? $"{_target}, Up={_worldUp}, Lock={_lockAxis}"
			: $"{_target}, Up={_worldUp}, Lock={_lockAxis}, Speed={_speed}, Interpolation={_interpolation}";
	}
}
