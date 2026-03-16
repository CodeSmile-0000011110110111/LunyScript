using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationSetLocalBlock : ActionBlock
	{
		private readonly VariableBlock<LunyQuaternion> _rotation;

		public static TransformRotationSetLocalBlock Create(VariableBlock<LunyQuaternion> rotation) => new(rotation);

		private TransformRotationSetLocalBlock(VariableBlock<LunyQuaternion> rotation) => _rotation = rotation;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalRotation = _rotation.Value;

		public override String ToString() => $"{GetType().Name}({_rotation})";
	}
}
