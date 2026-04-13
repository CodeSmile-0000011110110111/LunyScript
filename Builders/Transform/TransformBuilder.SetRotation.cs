using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Instantly set the local rotation. Append <c>.InWorldSpace()</c> to set world rotation. </summary>
		public TransformSetRotationBuilder SetRotation(Double xAngle, Double yAngle, Double zAngle) => TransformSetRotationBuilder.Create(
			_script, LunyQuaternion.Euler(new LunyVector3(xAngle, yAngle, zAngle)),
			LunyTransformSpace.Local, _trace.Add(nameof(SetRotation)));

		/// <summary> Instantly set the local rotation. Append <c>.InWorldSpace()</c> to set world rotation. </summary>
		public TransformSetRotationBuilder SetRotation(LunyVector3 eulerAngles) => TransformSetRotationBuilder.Create(_script,
			LunyQuaternion.Euler(eulerAngles), LunyTransformSpace.Local, _trace.Add(nameof(SetRotation)));

		/// <summary> Instantly set the local rotation. Append <c>.InWorldSpace()</c> to set world rotation. </summary>
		public TransformSetRotationBuilder SetRotation(VariableBlock<LunyQuaternion> rotation) =>
			TransformSetRotationBuilder.Create(_script, rotation, LunyTransformSpace.Local, _trace.Add(nameof(SetRotation)));
	}

	public readonly struct TransformSetRotationBuilder
	{
		public static implicit operator ActionBlock(TransformSetRotationBuilder b) => Finish(b.Options);

		internal readonly TransformSetRotationOptions Options;

		internal static TransformSetRotationBuilder Create(Script script, VariableBlock<LunyQuaternion> rotation,
			LunyTransformSpace space, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetRotationBuilder), "Transform.SetRotation()");
			var options = new TransformSetRotationOptions
			{
				Script = script, Token = token, Trace = trace, Rotation = rotation, Space = space,
			};
			return new TransformSetRotationBuilder(options);
		}

		internal TransformSetRotationBuilder(in TransformSetRotationOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		/// <summary> Override only the X euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAngleBlock X(Double value) => FinishAxis(LunyAxis.X, value);

		/// <summary> Override only the Y euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAngleBlock Y(Double value) => FinishAxis(LunyAxis.Y, value);

		/// <summary> Override only the Z euler angle; other axes remain unchanged. </summary>
		public TransformRotationSetAngleBlock Z(Double value) => FinishAxis(LunyAxis.Z, value);

		/// <summary> Apply rotation set in world space instead of local space. </summary>
		public TransformSetRotationBuilder InWorldSpace() => new(Options with { Space = LunyTransformSpace.World });

		internal TransformRotationSetBlock Finish() => Finish(Options);

		private TransformRotationSetAngleBlock FinishAxis(LunyAxis axis, Double value)
		{
			Options.Script.MarkBuilderTokenFinished(Options.Token);
			return TransformRotationSetAngleBlock.Create(axis, value, Options.Space, Options.Trace);
		}

		private static TransformRotationSetBlock Finish(in TransformSetRotationOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return TransformRotationSetBlock.Create(options.Rotation, options.Space, options.Trace);
		}
	}

	internal record TransformSetRotationOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;
		public VariableBlock<LunyQuaternion> Rotation;
		public LunyTransformSpace Space;
	}
}
