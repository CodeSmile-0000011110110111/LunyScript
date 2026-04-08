using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;
namespace LunyScript
{
	public readonly struct TransformSetRotationTerminalBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly VariableBlock<LunyQuaternion> _rotation;
		private readonly LunyTransformSpace _space;
		private readonly StackTrace _trace;

		internal static TransformSetRotationTerminalBuilder Create(Script script, VariableBlock<LunyQuaternion> rotation,
			LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetRotationTerminalBuilder), "Transform.SetRotation()");
			return new TransformSetRotationTerminalBuilder(script, token, rotation, space, trace);
		}

		private TransformSetRotationTerminalBuilder(Script script, BuilderToken token, VariableBlock<LunyQuaternion> rotation,
			LunyTransformSpace space, StackTrace trace)
		{
			_script = script;
			_token = token;
			_rotation = rotation;
			_space = space;
			_trace = trace;

			var self = this;
			token.AutoFinish = () => self.Finish();
		}

		/// <summary> Override only the X euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAxisBlock X(Double value) => FinishAxis(LunyAxis.X, value);

		/// <summary> Override only the Y euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAxisBlock Y(Double value) => FinishAxis(LunyAxis.Y, value);

		/// <summary> Override only the Z euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAxisBlock Z(Double value) => FinishAxis(LunyAxis.Z, value);

		/// <summary> Apply rotation set in world space instead of local space. </summary>
		public TransformSetRotationTerminalBuilder InWorldSpace() => new(_script, _token, _rotation, LunyTransformSpace.World, _trace);

		internal TransformRotationSetBlock Finish() => Finish(_space);

		private TransformRotationSetAxisBlock FinishAxis(LunyAxis axis, Double value)
		{
			_script.MarkBuilderTokenFinished(_token);
			return TransformRotationSetAxisBlock.Create(axis, value, _space, _trace);
		}

		private TransformRotationSetBlock Finish(LunyTransformSpace space)
		{
			_script.MarkBuilderTokenFinished(_token);
			return TransformRotationSetBlock.Create(_rotation, space, _trace);
		}
	}
}
