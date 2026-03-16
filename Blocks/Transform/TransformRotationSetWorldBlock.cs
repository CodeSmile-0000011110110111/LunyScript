using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationSetWorldBlock : ActionBlock
	{
		private readonly VariableBlock<LunyQuaternion> _rotation;

		public static TransformRotationSetWorldBlock Create(VariableBlock<LunyQuaternion> rotation) => new(rotation);

		private TransformRotationSetWorldBlock(VariableBlock<LunyQuaternion> rotation) => _rotation = rotation;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.Rotation = _rotation.Value;

		public override String ToString() => $"{GetType().Name}({_rotation})";
	}
}
