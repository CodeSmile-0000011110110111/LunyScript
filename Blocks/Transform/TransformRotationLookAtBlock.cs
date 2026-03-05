using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationLookAtBlock : ScriptActionBlock
	{
		private readonly ILunyObject _target;
		private readonly LunyVector3 _worldUp;
		private readonly LunyVector3 _axisLock;

		public static TransformRotationLookAtBlock Create(ILunyObject target, LunyVector3 worldUp, LunyVector3 axisLock) =>
			new(target, worldUp, axisLock);

		private TransformRotationLookAtBlock(ILunyObject target, LunyVector3 worldUp, LunyVector3 axisLock)
		{
			_target = target;
			_worldUp = worldUp;
			_axisLock = axisLock;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			if (!VectorUtil.TryGetMaskedDirection(transform.Position, _target.Transform.Position, _axisLock, out var maskedDirection))
				return;

			var lookTarget = transform.Position + maskedDirection;
			transform.LookAt(lookTarget, _worldUp);
		}

		public override String ToString() => $"{GetType().Name}({_target}, worldUp={_worldUp}, axisLock={_axisLock})";
	}
}
