using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly struct TransformRotateBuilder<T> where T : struct, ITransformBuilderState
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly VariableBlock _amount;
		private readonly LunyAxis _axis;
		private readonly Double _minAngle;
		private readonly Double _maxAngle;
		private readonly LunyTransformSpace _space;
		private readonly StackTrace _trace;

		internal static TransformRotateBuilder<T> Create(Script script, VariableBlock amount, LunyAxis axis, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformRotateBuilder<T>), "Transform.Rotate()");
			return new TransformRotateBuilder<T>(script, token, amount, axis, Double.NegativeInfinity, Double.PositiveInfinity,
				LunyTransformSpace.Local, trace);
		}

		private TransformRotateBuilder(Script script, BuilderToken token, VariableBlock amount, LunyAxis axis,
			Double minAngle, Double maxAngle, LunyTransformSpace space, StackTrace trace)
		{
			_script = script;
			_token = token;
			_amount = amount;
			_axis = axis;
			_minAngle = minAngle;
			_maxAngle = maxAngle;
			_space = space;
			_trace = trace;
			var self = this;
			token.AutoFinish = () => self.Finish();
		}

		public static implicit operator ActionBlock(TransformRotateBuilder<T> b) => b.Finish();

		/// <summary> Clamp the accumulated rotation angle between <paramref name="min"/> and <paramref name="max"/> degrees. </summary>
		public TransformRotateBuilder<TransformBuilderReady> Clamp(Double min, Double max) =>
			new(_script, _token, _amount, _axis, min, max, _space, _trace);

		/// <summary> Apply rotation in world space instead of local space. </summary>
		public TransformRotationAddAngleBlock InWorldSpace() => Finish(LunyTransformSpace.World);

		internal TransformRotationAddAngleBlock Finish() => Finish(_space);

		private TransformRotationAddAngleBlock Finish(LunyTransformSpace space)
		{
			_script.MarkBuilderTokenFinished(_token);
			return TransformRotationAddAngleBlock.Create(_amount, _axis, space, _minAngle, _maxAngle, _trace);
		}
	}
}
