using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public interface ITransformBuilderState {}
	public interface ITransformBuilderReady : ITransformBuilderState {}

	public struct TransformBuilderReady : ITransformBuilderReady {}

	public readonly struct TransformBuilder
	{
		private readonly Script _script;
		private readonly StackTrace _trace;

		internal TransformBuilder(Script script, StackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Rotate by axis angle per frame. </summary>
		public TransformRotationAddAngleBlock RotateByAxisAngle(VariableBlock deltaAngle, VariableBlock speed, LunyVector3 angleAxis,
			Double minAngle = Double.NegativeInfinity, Double maxAngle = Double.PositiveInfinity) =>
			TransformRotationAddAngleBlock.Create(deltaAngle, speed, angleAxis, LunyTransformSpace.Local, minAngle, maxAngle, _trace.Add(nameof(RotateByAxisAngle)));

		// --- Set (Absolute Snap) ---

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the World position. </summary>
		public TransformPositionSetBlock SetPosition(VariableBlock<LunyVector3> position) =>
			TransformPositionSetBlock.Create(position, LunyTransformSpace.World, _trace.Add(nameof(SetPosition)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the Local position. </summary>
		public TransformPositionSetBlock SetLocalPosition(VariableBlock<LunyVector3> position) =>
			TransformPositionSetBlock.Create(position, LunyTransformSpace.Local, _trace.Add(nameof(SetLocalPosition)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary>Instantly set the World rotation.</summary>
		public TransformRotationSetBlock SetRotation(LunyVector3 eulerAngles) =>
			TransformRotationSetBlock.Create(LunyQuaternion.Euler(eulerAngles), LunyTransformSpace.World, _trace.Add(nameof(SetRotation)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the World rotation. </summary>
		public TransformRotationSetBlock SetRotation(VariableBlock<LunyQuaternion> rotation) =>
			TransformRotationSetBlock.Create(rotation, LunyTransformSpace.World, _trace.Add(nameof(SetRotation)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the Local rotation. </summary>
		public TransformRotationSetBlock SetLocalRotation(LunyVector3 eulerAngles) =>
			TransformRotationSetBlock.Create(LunyQuaternion.Euler(eulerAngles), LunyTransformSpace.Local, _trace.Add(nameof(SetLocalRotation)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the Local rotation. </summary>
		public TransformRotationSetBlock SetLocalRotation(VariableBlock<LunyQuaternion> rotation) =>
			TransformRotationSetBlock.Create(rotation, LunyTransformSpace.Local, _trace.Add(nameof(SetLocalRotation)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the Local scale. </summary>
		public TransformScaleSetLocalBlock SetLocalScale(Double uniformScale) =>
			TransformScaleSetLocalBlock.Create(LunyVector3.Uniform(uniformScale));

		[NeedsReview, NeedsSmokeTest]
		public TransformScaleSetLocalUniformBlock SetLocalScale(VariableBlock uniformScale) =>
			TransformScaleSetLocalUniformBlock.Create(uniformScale);

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the Local scale. </summary>
		public TransformScaleSetLocalBlock SetLocalScale(VariableBlock<LunyVector3> scale) => TransformScaleSetLocalBlock.Create(scale);

		// --- Look At ---

		[NeedsReview, NeedsSmokeTest]
		/// <summary>
		/// Instantly orient to face the target.
		/// Chain <c>.WorldUp(v)</c>, <c>.LockX()</c>, <c>.LockY()</c>, <c>.LockZ()</c> then call <c>.Do()</c>.
		/// </summary>
		public TransformLookAtBuilder<TransformBuilderReady> LookAt(ILunyObject target)
		{
			var options = new TransformBuilderOptions { Target = target, WorldUp = LunyVector3.Up, AxisLock = LunyVector3.One };
			var token = _script.CreateBuilderToken(nameof(LookAt), "Transform.LookAt()");
			return new TransformLookAtBuilder<TransformBuilderReady>(options);
		}

		// --- Move Towards ---

		[NeedsReview, NeedsSmokeTest]
		/// <summary>
		/// Move toward the target position each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformPositionBuilder<TransformBuilderReady> MoveTowards(ILunyObject target)
		{
			var token = _script.CreateBuilderToken(nameof(MoveTowards), "Transform.Move()");
			var options = new TransformBuilderOptions
			{
				Script = _script, Token = token, Target = target, Speed = 3.0, DeadZone = 0.1, Responsiveness = 1.0, AxisLock = LunyVector3.One,
			};
			return new TransformPositionBuilder<TransformBuilderReady>(options);
		}

		// --- Rotate Towards ---

		[NeedsReview, NeedsSmokeTest]
		/// <summary>
		/// Rotate toward the target orientation each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (degrees/sec), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformRotationBuilder<TransformBuilderReady> RotateTowards(ILunyObject target)
		{
			var token = _script.CreateBuilderToken(nameof(RotateTowards), "Transform.Rotate()");
			var options = new TransformBuilderOptions
			{
				Script = _script, Token = token, Target = target, Speed = 90.0, DeadZone = 0.1, Responsiveness = 1.0,
				AxisLock = LunyVector3.One,
			};
			return new TransformRotationBuilder<TransformBuilderReady>(options);
		}

		// --- Scale Towards ---

		[NeedsReview, NeedsSmokeTest]
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
				Script = _script, Token = token, TargetScale = targetScale, Speed = 1.0, DeadZone = 0.1, Responsiveness = 1.0,
				AxisLock = LunyVector3.One,
			};
			return new TransformScaleBuilder<TransformBuilderReady>(options);
		}

		/*
		Category		Relative (Delta)	Absolute (Target)
		-----------------------------------------------------
		Vector			MoveBy(Vector2)		MoveTo(Vector2)
		Directional		MoveForward(5)		MoveForwardTo(10)
		Grid-based		ShiftRight(2)		ShiftRightTo(5)
		*/

		// --- Local Scalar Movement (Relative to "Nose") ---

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformPositionMoveBlock MoveBy(VariableBlock<LunyVector2> direction, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateDirectional(direction, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveBy)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformPositionMoveBlock MoveForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Forward, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveForward)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Sideways relative to orientation. </summary>
		public TransformPositionMoveBlock MoveRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Right, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveRight)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Up/Down relative to orientation. </summary>
		public TransformPositionMoveBlock MoveUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Up, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveUp)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformPositionMoveBlock MoveBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Back, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveBack)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Sideways relative to orientation. </summary>
		public TransformPositionMoveBlock MoveLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Left, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveLeft)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Up/Down relative to orientation. </summary>
		public TransformPositionMoveBlock MoveDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Down, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveDown)));

		// --- World Scalar Movement (Relative to "Map") ---

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformPositionMoveBlock ShiftBy(VariableBlock<LunyVector2> direction, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateDirectional(direction, speed, LunyTransformSpace.World, _trace.Add(nameof(ShiftBy)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Forward/backward on the World forward axis. </summary>
		public TransformPositionMoveBlock ShiftForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Forward, speed, LunyTransformSpace.World, _trace.Add(nameof(ShiftForward)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Left/Right on the World right axis. </summary>
		public TransformPositionMoveBlock ShiftRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Right, speed, LunyTransformSpace.World, _trace.Add(nameof(ShiftRight)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Up/Down on the World up axis. </summary>
		public TransformPositionMoveBlock ShiftUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Up, speed, LunyTransformSpace.World, _trace.Add(nameof(ShiftUp)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Forward/backward on the World forward axis. </summary>
		public TransformPositionMoveBlock ShiftBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Back, speed, LunyTransformSpace.World, _trace.Add(nameof(ShiftBack)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Left/Right on the World right axis. </summary>
		public TransformPositionMoveBlock ShiftLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Left, speed, LunyTransformSpace.World, _trace.Add(nameof(ShiftLeft)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Up/Down on the World up axis. </summary>
		public TransformPositionMoveBlock ShiftDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveBlock.CreateAxisRelative(amount, LunyVector3.Down, speed, LunyTransformSpace.World, _trace.Add(nameof(ShiftDown)));

		// --- Absolute Positioning (Targeting) ---

		/*/// <summary> Snap to an absolute World Position. </summary>
		public static TransformTeleportBlock MoveTo(VariableBlock targetPosition) =>
			TransformTeleportBlock.Create(targetPosition);

		/// <summary> Set absolute World X coordinate. </summary>
		public static TransformTeleportBlock ShiftRightTo(VariableBlock targetX) =>
			TransformTeleportBlock.CreateX(targetX);

		/// <summary> Set absolute World Y coordinate. </summary>
		public static TransformTeleportBlock ShiftUpTo(VariableBlock targetY) =>
			TransformTeleportBlock.CreateY(targetY);*/
	}

	internal record TransformBuilderOptions
	{
		public Script Script;
		public BuilderToken Token;

		public ILunyObject Target;
		public VariableBlock<LunyVector3> TargetScale;
		public Double Speed;
		public Double DeadZone;
		public Double Responsiveness;
		public LunyVector3 WorldUp;
		public LunyVector3 AxisLock;
		public Boolean Lerp;
		public Boolean SphericalLerp;
		public void LockAxisX() => AxisLock = VectorUtil.LockAxisX(AxisLock);
		public void LockAxisY() => AxisLock = VectorUtil.LockAxisY(AxisLock);
		public void LockAxisZ() => AxisLock = VectorUtil.LockAxisZ(AxisLock);
	}
}
