using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationSetLocalBlock : ScriptActionBlock
	{
		private readonly VariableBlock _rotation;

		public static TransformRotationSetLocalBlock Create(VariableBlock rotation) => new(rotation);

		private TransformRotationSetLocalBlock(VariableBlock rotation) => _rotation = rotation;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.LocalRotation = _rotation.GetValue<LunyQuaternion>();

		public override String ToString() => $"{GetType().Name}({_rotation})";
	}
}
