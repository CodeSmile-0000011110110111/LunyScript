using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformSetRotationBlock : ScriptActionBlock
	{
		private readonly VariableBlock _rotation;

		public static TransformSetRotationBlock Create(VariableBlock rotation) => new(rotation);

		private TransformSetRotationBlock(VariableBlock rotation) => _rotation = rotation;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			runtimeContext.LunyObject.Transform.Rotation = _rotation.GetValue<LunyQuaternion>();

		public override String ToString() => $"{GetType().Name}({_rotation})";
	}
}
