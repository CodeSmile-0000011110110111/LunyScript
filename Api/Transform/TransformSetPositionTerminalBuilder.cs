using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;
namespace LunyScript
{
	public readonly struct TransformSetPositionTerminalBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly VariableBlock<LunyVector3> _position;
		private readonly LunyTransformSpace _space;
		private readonly StackTrace _trace;

		internal static TransformSetPositionTerminalBuilder Create(Script script, VariableBlock<LunyVector3> position,
			LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetPositionTerminalBuilder), "Transform.SetPosition()");
			return new TransformSetPositionTerminalBuilder(script, token, position, space, trace);
		}

		private TransformSetPositionTerminalBuilder(Script script, BuilderToken token, VariableBlock<LunyVector3> position,
			LunyTransformSpace space, StackTrace trace)
		{
			_script = script;
			_token = token;
			_position = position;
			_space = space;
			_trace = trace;

			var self = this;
			token.AutoFinish = () => self.Finish();
		}

		/// <summary> Override only the X component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetAxisBlock X(Double value) => FinishAxis(LunyAxis.X, value);

		/// <summary> Override only the Y component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetAxisBlock Y(Double value) => FinishAxis(LunyAxis.Y, value);

		/// <summary> Override only the Z component of the position; other axes remain unchanged. </summary>
		public TransformPositionSetAxisBlock Z(Double value) => FinishAxis(LunyAxis.Z, value);

		/// <summary> Apply position set in world space instead of local space. </summary>
		public TransformSetPositionTerminalBuilder InWorldSpace() => new(_script, _token, _position, LunyTransformSpace.World, _trace);

		internal TransformPositionSetBlock Finish() => Finish(_space);

		private TransformPositionSetAxisBlock FinishAxis(LunyAxis axis, Double value)
		{
			_script.MarkBuilderTokenFinished(_token);
			return TransformPositionSetAxisBlock.Create(axis, value, _space, _trace);
		}

		private TransformPositionSetBlock Finish(LunyTransformSpace space)
		{
			_script.MarkBuilderTokenFinished(_token);
			return TransformPositionSetBlock.Create(_position, space, _trace);
		}
	}
}
