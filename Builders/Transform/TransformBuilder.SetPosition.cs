using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Instantly set the local position. Append <c>.InWorldSpace()</c> to set world position. </summary>
		public TransformSetPositionBuilder SetPosition(Double x, Double y, Double z) => TransformSetPositionBuilder.Create(
			_script, new LunyVector3(x, y, z), LunyTransformSpace.Local, _trace.Add(nameof(SetPosition)));

		/// <summary> Instantly set the local position. Append <c>.InWorldSpace()</c> to set world position. </summary>
		public TransformSetPositionBuilder SetPosition(VariableBlock<LunyVector3> position) =>
			TransformSetPositionBuilder.Create(_script, position, LunyTransformSpace.Local, _trace.Add(nameof(SetPosition)));
	}

	public static class TransformSetPositionBuilderExtensions
	{
		/// <summary> Apply position set in world space instead of local space. </summary>
		public static TransformSetPositionBuilder InWorldSpace(this TransformSetPositionBuilder b) =>
			new(b.Options with { Space = LunyTransformSpace.World });

		/// <summary> Override only the X component of the position; other axes remain unchanged. </summary>
		public static TransformSetPositionBuilder X(this TransformSetPositionBuilder b, Double value) =>
			new(b.Options with { Axis = LunyAxis.X, UseAxisValue = true, AxisValue = value });

		/// <summary> Override only the Y component of the position; other axes remain unchanged. </summary>
		public static TransformSetPositionBuilder Y(this TransformSetPositionBuilder b, Double value) =>
			new(b.Options with { Axis = LunyAxis.Y, UseAxisValue = true, AxisValue = value });

		/// <summary> Override only the Z component of the position; other axes remain unchanged. </summary>
		public static TransformSetPositionBuilder Z(this TransformSetPositionBuilder b, Double value) =>
			new(b.Options with { Axis = LunyAxis.Z, UseAxisValue = true, AxisValue = value });
	}

	public readonly struct TransformSetPositionBuilder
	{
		public static implicit operator ActionBlock(TransformSetPositionBuilder b) => Finish(b.Options);

		internal readonly TransformSetPositionOptions Options;

		internal static TransformSetPositionBuilder Create(Script script, VariableBlock<LunyVector3> position,
			LunyTransformSpace space, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetPositionBuilder), "Transform.SetPosition()");
			var options = new TransformSetPositionOptions
			{
				Script = script, Token = token, Trace = trace, Position = position, Space = space,
			};
			return new TransformSetPositionBuilder(options);
		}

		internal TransformSetPositionBuilder(in TransformSetPositionOptions options)
		{
			Options = options;
			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		private static ActionBlock Finish(in TransformSetPositionOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseAxisValue
				? TransformPositionSetSingleAxisBlock.Create(options.Axis, options.AxisValue, options.Space, options.Trace)
				: TransformPositionSetBlock.Create(options.Position, options.Space, options.Trace);
		}
	}

	internal record TransformSetPositionOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public VariableBlock<LunyVector3> Position;
		public LunyTransformSpace Space;
		public LunyAxis Axis;
		public Double AxisValue;
		public Boolean UseAxisValue;
	}
}
