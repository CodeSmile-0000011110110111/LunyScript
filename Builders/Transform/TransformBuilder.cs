using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public interface ITransformBuilderState {}
	public interface ITransformBuilderReady : ITransformBuilderState {}

	public struct TransformBuilderReady : ITransformBuilderReady {}

	public readonly partial struct TransformBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal TransformBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		[NeedsReview] [NeedsSmokeTest]
		/// <summary>
		/// Move toward the target position each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformMoveTowardsBuilder<TransformBuilderReady> MoveTowards(LunyObjectRef target)
		{
			var options = new TransformBuilderOptions
			{
				Script = _script,
				Token = _script.CreateBuilderToken(nameof(MoveTowards), "Transform.MoveTowards()"),
				Trace = _trace.Add(nameof(MoveTowards)),
				Target = target,
				Speed = 3.0,
				DeadZone = 0.1,
				Responsiveness = 1.0,
				AxisLock = LunyVector3.One,
			};
			return new TransformMoveTowardsBuilder<TransformBuilderReady>(options);
		}

		// --- Movement relative to orientation ---

		/*
		Category		Relative (Delta)	Absolute (Target)
		-----------------------------------------------------
		Vector			MoveBy(Vector2)		MoveTo(Vector2)
		Directional		MoveForward(5)		MoveForwardTo(10)
		*/

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Movement based on a 2D direction vector. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveBuilder MoveOnPlane(VariableBlock<LunyVector2> direction, VariableBlock speed = null) =>
			TransformMoveBuilder.CreateDirectional(_script, direction, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveOnPlane)));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Forward based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveBuilder MoveForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveBuilder.CreateAxisRelative(_script, amount, LunyVector3.Forward, speed, LunyTransformSpace.Local,
				_trace.Add(nameof(MoveForward)));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Backward based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveBuilder MoveBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveBuilder.CreateAxisRelative(_script, amount, LunyVector3.Back, speed, LunyTransformSpace.Local,
				_trace.Add(nameof(MoveBack)));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Right based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveBuilder MoveRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveBuilder.CreateAxisRelative(_script, amount, LunyVector3.Right, speed, LunyTransformSpace.Local,
				_trace.Add(nameof(MoveRight)));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Left based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveBuilder MoveLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveBuilder.CreateAxisRelative(_script, amount, LunyVector3.Left, speed, LunyTransformSpace.Local,
				_trace.Add(nameof(MoveLeft)));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Up based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveBuilder MoveUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveBuilder.CreateAxisRelative(_script, amount, LunyVector3.Up, speed, LunyTransformSpace.Local,
				_trace.Add(nameof(MoveUp)));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary> Down based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveBuilder MoveDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveBuilder.CreateAxisRelative(_script, amount, LunyVector3.Down, speed, LunyTransformSpace.Local,
				_trace.Add(nameof(MoveDown)));

		[NeedsReview] [NeedsSmokeTest]
		/// <summary>
		/// Instantly orient to face the target.
		/// Chain <c>.WorldUp(v)</c>, <c>.LockX()</c>, <c>.LockY()</c>, <c>.LockZ()</c> then call <c>.Do()</c>.
		/// </summary>
		public TransformLookAtBuilder<TransformBuilderReady> LookAt(LunyObjectRef target)
		{
			var token = _script.CreateBuilderToken(nameof(LookAt), "Transform.LookAt()");
			var options = new TransformBuilderOptions
			{
				Script = _script,
				Token = token,
				Trace = _trace.Add(nameof(LookAt)),
				Target = target,
				WorldUp = LunyVector3.Up,
				AxisLock = LunyVector3.One,
			};
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}

		/// <summary> Rotate around <paramref name="axis"/> by <paramref name="degreesPerSecond"/> degrees per second. Chain <c>.Clamp(min, max)</c> and/or <c>.InWorldSpace()</c>. </summary>
		public TransformRotateBuilder<TransformBuilderReady> RotateBy(VariableBlock degreesPerSecond, LunyAxis axis) =>
			TransformRotateBuilder<TransformBuilderReady>.Create(_script, degreesPerSecond, axis, _trace.Add(nameof(RotateBy)));

		// --- Rotate Towards ---

		[NeedsReview] [NeedsSmokeTest]
		/// <summary>
		/// Rotate toward the target orientation each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (degrees/sec), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformRotationBuilder<TransformBuilderReady> RotateTowards(LunyObjectRef target)
		{
			var token = _script.CreateBuilderToken(nameof(RotateTowards), "Transform.Rotate()");
			var options = new TransformBuilderOptions
			{
				Script = _script,
				Token = token,
				Trace = _trace.Add(nameof(RotateTowards)),
				Target = target,
				Speed = 90.0,
				DeadZone = 0.1,
				Responsiveness = 1.0,
				AxisLock = LunyVector3.One,
			};
			return new TransformRotationBuilder<TransformBuilderReady>(options);
		}

		// --- Scale Towards ---

		[NeedsReview] [NeedsSmokeTest]
		/// <summary>
		/// Scale toward the target scale each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformScaleBuilder<TransformBuilderReady> ScaleTowards(VariableBlock<LunyVector3> targetScale)
		{
			var token = _script.CreateBuilderToken(nameof(ScaleTowards), "Transform.Scale()");
			var options = new TransformBuilderOptions
			{
				Script = _script,
				Token = token,
				Trace = _trace.Add(nameof(ScaleTowards)),
				TargetScale = targetScale,
				Speed = 1.0,
				DeadZone = 0.1,
				Responsiveness = 1.0,
				AxisLock = LunyVector3.One,
			};
			return new TransformScaleBuilder<TransformBuilderReady>(options);
		}
	}

	internal record TransformBuilderOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public LunyObjectRef Target;
		public VariableBlock<LunyVector3> TargetScale;
		public Double Speed;
		public Double DeadZone;
		public Double Responsiveness;
		public LunyVector3 WorldUp;
		public LunyVector3 AxisLock;
		public Boolean Lerp;
		public Boolean SphericalLerp;
		public LunyTransformSpace Space;
		public VariableBlock Amount;
		public LunyAxis Axis;
		public Double MinAngle;
		public Double MaxAngle;
		public void LockAxisX() => AxisLock = VectorUtil.LockAxisX(AxisLock);
		public void LockAxisY() => AxisLock = VectorUtil.LockAxisY(AxisLock);
		public void LockAxisZ() => AxisLock = VectorUtil.LockAxisZ(AxisLock);
	}
}
