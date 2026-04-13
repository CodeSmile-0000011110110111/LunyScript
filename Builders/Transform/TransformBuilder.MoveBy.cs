using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Movement based on a 2D direction vector. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveByBuilder MoveBy(VariableBlock<LunyVector2> direction) =>
			TransformMoveByBuilder.CreateMoveBy(_script, direction, LunyTransformSpace.Local, _trace.Add(nameof(MoveBy)));

		/// <summary> Forward based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveByBuilder MoveForward(VariableBlock amount) => TransformMoveByBuilder.CreateMoveAxis(_script, amount,
			LunyVector3.Forward, LunyTransformSpace.Local,
			_trace.Add(nameof(MoveForward)));

		/// <summary> Backward based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveByBuilder MoveBack(VariableBlock amount) => TransformMoveByBuilder.CreateMoveAxis(_script, amount,
			LunyVector3.Back, LunyTransformSpace.Local,
			_trace.Add(nameof(MoveBack)));

		/// <summary> Right based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveByBuilder MoveRight(VariableBlock amount) => TransformMoveByBuilder.CreateMoveAxis(_script, amount,
			LunyVector3.Right, LunyTransformSpace.Local,
			_trace.Add(nameof(MoveRight)));

		/// <summary> Left based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveByBuilder MoveLeft(VariableBlock amount) => TransformMoveByBuilder.CreateMoveAxis(_script, amount,
			LunyVector3.Left, LunyTransformSpace.Local,
			_trace.Add(nameof(MoveLeft)));

		/// <summary> Up based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveByBuilder MoveUp(VariableBlock amount) => TransformMoveByBuilder.CreateMoveAxis(_script, amount, LunyVector3.Up,
			LunyTransformSpace.Local,
			_trace.Add(nameof(MoveUp)));

		/// <summary> Down based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveByBuilder MoveDown(VariableBlock amount) => TransformMoveByBuilder.CreateMoveAxis(_script, amount,
			LunyVector3.Down, LunyTransformSpace.Local,
			_trace.Add(nameof(MoveDown)));
	}

	public static class TransformMoveByBuilderExtensions
	{
		/// <summary> Apply movement in world space instead of local space. </summary>
		public static TransformMoveByBuilder InWorldSpace(this TransformMoveByBuilder b) =>
			new(b.Options with { Space = LunyTransformSpace.World });

		/// <summary>
		/// Set speed of movement.
		/// </summary>
		/// <param name="speed"></param>
		/// <returns></returns>
		public static TransformMoveByBuilder Speed(this TransformMoveByBuilder b, VariableBlock speed) => new(b.Options with { Speed = speed });
	}

	public readonly struct TransformMoveByBuilder
	{
		internal readonly TransformMoveByOptions Options;

		internal static TransformMoveByBuilder CreateMoveBy(Script script, VariableBlock<LunyVector2> direction,
			LunyTransformSpace space, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformMoveByBuilder), "Transform." + nameof(TransformBuilder.MoveBy));
			var options = new TransformMoveByOptions
			{
				Script = script, Token = token, Trace = trace, Space = space,
				Direction = direction, UseDirection = true,
			};
			return new TransformMoveByBuilder(options);
		}

		internal static TransformMoveByBuilder CreateMoveAxis(Script script, VariableBlock amount, LunyVector3 axis,
			LunyTransformSpace space, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformMoveByBuilder), "Transform." + nameof(TransformBuilder.MoveBy) + "(Axis)");
			var options = new TransformMoveByOptions
			{
				Script = script, Token = token, Trace = trace, Space = space,
				Amount = amount, Axis = axis, UseDirection = false,
			};
			return new TransformMoveByBuilder(options);
		}

		internal TransformMoveByBuilder(in TransformMoveByOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformMoveByBuilder b) => Finish(b.Options);

		private static TransformMoveByBlock Finish(in TransformMoveByOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			return options.UseDirection
				? TransformMoveByBlock.CreatePlaneMove(options.Direction, options.Speed, options.Space, options.Trace)
				: TransformMoveByBlock.CreateAxisMove(options.Amount, options.Axis, options.Speed, options.Space, options.Trace);
		}
	}

	internal record TransformMoveByOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public LunyTransformSpace Space;
		public VariableBlock<LunyVector2> Direction;
		public VariableBlock Amount;
		public LunyVector3 Axis;
		public VariableBlock Speed;
		public Boolean UseDirection;
	}
}
