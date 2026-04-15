using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly partial struct TransformBuilder
	{
		/// <summary> Rotate around the Y axis (world up) by <paramref name="degreesPerSecond"/> degrees per second. Chain <c>.AroundX/Y/Z()</c>, <c>.Around(axis)</c>, <c>.AngleClamp(min, max)</c> and/or <c>.InWorldSpace()</c>. </summary>
		public TransformRotateByBuilder<TransformBuilderReady> RotateBy(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Up, _trace.Add(nameof(RotateBy)));

		/// <summary> Rotate by euler angles per second. Chain <c>.InWorldSpace()</c>. </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> RotateBy(VariableBlock<LunyVector3> eulerAnglesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.CreateEuler(_script, eulerAnglesPerSecond, _trace.Add(nameof(RotateBy)));

		/// <summary> Rotate around the Y axis (world up) by <paramref name="degreesPerSecond"/> degrees per second. </summary>
		[Obsolete("Use RotateBy(degreesPerSecond).AroundY() or RotateBy(degreesPerSecond) (defaults to 'up' aka Y axis).")]
		public TransformRotateByBuilder<TransformBuilderReady> RotateBy(VariableBlock degreesPerSecond, LunyAxis axis) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, axis.ToVector3(), _trace.Add(nameof(RotateBy)));

		/// <summary>
		/// Add yaw rotation (around Y/up axis) by <paramref name="degreesPerSecond"/> degrees per second.
		/// Same as LookUp/LookDown except direction is determined by sign of parameter.
		/// </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> AddYaw(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Up, _trace.Add(nameof(AddYaw)));

		/// <summary>
		/// Add pitch rotation (around X/right axis) by <paramref name="degreesPerSecond"/> degrees per second.
		/// Same as LookLeft/LookRight except direction is determined by sign of parameter.
		/// </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> AddPitch(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Right, _trace.Add(nameof(AddPitch)));

		/// <summary>
		/// Add roll rotation (around Z/forward axis) by <paramref name="degreesPerSecond"/> degrees per second.
		/// Same as LeanLeft/LeanRight except direction is determined by sign of parameter.
		/// </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> AddRoll(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Forward, _trace.Add(nameof(AddRoll)));

		/// <summary> Rotate left around the Y axis (positive yaw). </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> LookLeft(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Up, _trace.Add(nameof(LookLeft)));

		/// <summary> Rotate right around the Y axis (negative yaw). </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> LookRight(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Down, _trace.Add(nameof(LookRight)));

		/// <summary> Rotate upward around the X axis (positive pitch). </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> LookUp(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Right, _trace.Add(nameof(LookUp)));

		/// <summary> Rotate downward around the X axis (negative pitch). </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> LookDown(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Left, _trace.Add(nameof(LookDown)));

		/// <summary> Lean left around the Z axis (positive roll). </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> LeanLeft(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Forward, _trace.Add(nameof(LeanLeft)));

		/// <summary> Lean right around the Z axis (negative roll). </summary>
		[NeedsSmokeTest]
		public TransformRotateByBuilder<TransformBuilderReady> LeanRight(VariableBlock degreesPerSecond) =>
			TransformRotateByBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, LunyVector3.Back, _trace.Add(nameof(LeanRight)));
	}

	public static class TransformRotateByBuilderExtensions
	{
		/// <summary> Apply rotation in world space instead of local space. </summary>
		public static TransformRotateByBuilder<TransformBuilderReady> InWorldSpace<T>(this TransformRotateByBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Space = LunyTransformSpace.World });

		/// <summary> Clamp the accumulated rotation angle between <paramref name="min"/> and <paramref name="max"/> degrees. </summary>
		public static TransformRotateByBuilder<TransformBuilderReady> Clamp<T>(this TransformRotateByBuilder<T> b, Double min, Double max)
			where T : struct, ITransformBuilderReady => new(b.Options with { MinAngle = min, MaxAngle = max });

		/// <summary> Rotate around the X axis. </summary>
		[NeedsSmokeTest]
		public static TransformRotateByBuilder<TransformBuilderReady> AroundX<T>(this TransformRotateByBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Axis = LunyVector3.Right });

		/// <summary> Rotate around the Y axis. </summary>
		[NeedsSmokeTest]
		public static TransformRotateByBuilder<TransformBuilderReady> AroundY<T>(this TransformRotateByBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Axis = LunyVector3.Up });

		/// <summary> Rotate around the Z axis. </summary>
		[NeedsSmokeTest]
		public static TransformRotateByBuilder<TransformBuilderReady> AroundZ<T>(this TransformRotateByBuilder<T> b)
			where T : struct, ITransformBuilderReady => new(b.Options with { Axis = LunyVector3.Forward });

		/// <summary> Rotate around a custom axis. </summary>
		[NeedsSmokeTest]
		public static TransformRotateByBuilder<TransformBuilderReady> Around<T>(this TransformRotateByBuilder<T> b, LunyVector3 axis)
			where T : struct, ITransformBuilderReady => new(b.Options with { Axis = axis });
	}

	public readonly struct TransformRotateByBuilder<T> where T : struct, ITransformBuilderState
	{
		internal readonly TransformRotateByOptions Options;

		internal static TransformRotateByBuilder<T> Create(Script script, VariableBlock amount, LunyVector3 axis, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformRotateByBuilder<T>), "Transform." + nameof(TransformBuilder.RotateBy));
			var options = new TransformRotateByOptions
			{
				Script = script, Token = token, Amount = amount, Axis = axis,
				MinAngle = Double.NegativeInfinity, MaxAngle = Double.PositiveInfinity,
				Space = LunyTransformSpace.Local, Trace = trace,
			};
			return new TransformRotateByBuilder<T>(options);
		}

		internal static TransformRotateByBuilder<T> CreateEuler(Script script, VariableBlock<LunyVector3> eulerAnglesPerSecond, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformRotateByBuilder<T>), "Transform." + nameof(TransformBuilder.RotateBy) + "(Euler)");
			var options = new TransformRotateByOptions
			{
				Script = script, Token = token, EulerAngles = eulerAnglesPerSecond,
				MinAngle = Double.NegativeInfinity, MaxAngle = Double.PositiveInfinity,
				Space = LunyTransformSpace.Local, Trace = trace, UseEuler = true,
			};
			return new TransformRotateByBuilder<T>(options);
		}

		internal TransformRotateByBuilder(in TransformRotateByOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(TransformRotateByBuilder<T> b) => Finish(b.Options);

		private static TransformRotateByBlock Finish(in TransformRotateByOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			if (options.UseEuler)
				return TransformRotateByBlock.CreateEuler(options.EulerAngles, options.Space, options.Trace);
			return TransformRotateByBlock.Create(options.Amount, options.Axis, options.Space, options.MinAngle, options.MaxAngle,
				options.Trace);
		}
	}

	internal record TransformRotateByOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public LunyTransformSpace Space;
		public VariableBlock Amount;
		public LunyVector3 Axis;
		public VariableBlock<LunyVector3> EulerAngles;
		public Boolean UseEuler;
		public Double MinAngle;
		public Double MaxAngle;
	}
}
