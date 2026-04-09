using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript.Blocks
{
	public sealed class TransformRotationSetBlock : ActionBlock
	{
		private readonly VariableBlock<LunyQuaternion> _rotation;
		private readonly LunyTransformSpace _space;

		public static TransformRotationSetBlock Create(VariableBlock<LunyQuaternion> rotation, LunyTransformSpace space, LunyStackTrace trace) =>
			new(rotation, space, trace);

		private TransformRotationSetBlock(VariableBlock<LunyQuaternion> rotation, LunyTransformSpace space, LunyStackTrace trace)
			: base(trace)
		{
			_rotation = rotation;
			_space = space;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var transform = context.LunyObject.Transform;
			if (_space == LunyTransformSpace.World)
				transform.Rotation = _rotation.Value;
			else
				transform.LocalRotation = _rotation.Value;
		}

		public override String ToString() => $"{GetType().Name}({_rotation}, {_space})";
	}
}
