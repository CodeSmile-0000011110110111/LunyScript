using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformLookAtBlock : ScriptActionBlock
	{
		private readonly ILunyObject _target;
		private readonly LunyVector3 _worldUp;
		private readonly LunyVector3 _axisLock;

		public static TransformLookAtBlock Create(
			ILunyObject target,
			LunyVector3 worldUp,
			LunyVector3 axisLock) =>
			new(target, worldUp, axisLock);

		private TransformLookAtBlock(ILunyObject target, LunyVector3 worldUp, LunyVector3 axisLock)
		{
			_target = target;
			_worldUp = worldUp;
			_axisLock = axisLock;
		}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var transform = runtimeContext.LunyObject.Transform;
			var direction = _target.Transform.Position - transform.Position;
			var maskedDirection = direction * _axisLock;
			if (maskedDirection.SqrMagnitude < Single.Epsilon)
				return;

			var lookTarget = transform.Position + maskedDirection;
			transform.LookAt(lookTarget, _worldUp);
		}

		public override String ToString() => $"{GetType().Name}({_target}, worldUp={_worldUp}, axisLock={_axisLock})";
	}
}
