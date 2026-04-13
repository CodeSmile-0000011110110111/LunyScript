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

		public static TransformLookAtBlock Create(LunyObjectRef target, LunyVector3 worldUp, LunyVector3 lockAxis,
			LunyStackTrace trace) => new(target, worldUp, lockAxis, trace);

		private TransformLookAtBlock(LunyObjectRef target, LunyVector3 worldUp, LunyVector3 lockAxis, LunyStackTrace trace)
			: base(trace)
		{
			_target = target;
			_worldUp = worldUp;
			_lockAxis = lockAxis;
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
			transform.LookAt(lookTarget, _worldUp);
		}

		public override String ToString() => $"{_target}, Up={_worldUp}, Lock={_lockAxis}";
	}
}
