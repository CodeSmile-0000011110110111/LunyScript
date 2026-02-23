using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks.Transform
{
	public sealed class TransformSetLocalRotationBlock : ScriptActionBlock
	{
		private readonly VariableBlock _rotation;

		public static TransformSetLocalRotationBlock Create(VariableBlock rotation) => new(rotation);

		private TransformSetLocalRotationBlock(VariableBlock rotation) => _rotation = rotation;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalRotation = _rotation.GetValue<LunyQuaternion>();

		public override String ToString() => $"{GetType().Name}({_rotation})";
	}
}
