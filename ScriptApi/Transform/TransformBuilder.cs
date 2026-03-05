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

		internal TransformBuilder(Script script) => _script = script;

		// --- Set (Absolute Snap) ---

		/// <summary> Instantly set the World position. </summary>
		public TransformPositionSetWorldBlock SetPosition(VariableBlock position) => TransformPositionSetWorldBlock.Create(position);

		/// <summary> Instantly set the Local position. </summary>
		public TransformPositionSetLocalBlock SetLocalPosition(VariableBlock position) => TransformPositionSetLocalBlock.Create(position);

		/// <summary> Instantly set the World rotation. </summary>
		public TransformRotationSetWorldBlock SetRotation(LunyVector3 eulerAngles) => TransformRotationSetWorldBlock.Create(LunyQuaternion.Euler(eulerAngles));

		/// <summary> Instantly set the World rotation. </summary>
		public TransformRotationSetWorldBlock SetRotation(VariableBlock rotation) => TransformRotationSetWorldBlock.Create(rotation);

		/// <summary> Instantly set the Local rotation. </summary>
		public TransformRotationSetLocalBlock SetLocalRotation(LunyVector3 eulerAngles) => TransformRotationSetLocalBlock.Create(LunyQuaternion.Euler(eulerAngles));

		/// <summary> Instantly set the Local rotation. </summary>
		public TransformRotationSetLocalBlock SetLocalRotation(VariableBlock rotation) => TransformRotationSetLocalBlock.Create(rotation);

		/// <summary> Instantly set the Local scale. </summary>
		public TransformScaleSetLocalBlock SetLocalScale(Double scale) => TransformScaleSetLocalBlock.Create(LunyVector3.Uniform(scale));

		/// <summary> Instantly set the Local scale. </summary>
		public TransformScaleSetLocalBlock SetLocalScale(VariableBlock scale) => TransformScaleSetLocalBlock.Create(scale);

		// --- Look At ---

		/// <summary>
		/// Instantly orient to face the target.
		/// Chain <c>.WorldUp(v)</c>, <c>.LockX()</c>, <c>.LockY()</c>, <c>.LockZ()</c> then call <c>.Do()</c>.
		/// </summary>
		public TransformLookAtBuilder<TransformBuilderReady> LookAt(ILunyObject target)
		{
			var options = new TransformBuilderOptions { Target = target, WorldUp = LunyVector3.Up, AxisLock = LunyVector3.One };
			var token = _script.CreateBuilderToken(nameof(LookAt), "Transform.LookAt()");
			return new TransformLookAtBuilder<TransformBuilderReady>(_script, token, options);
		}

		// --- Move Towards ---

		/// <summary>
		/// Move toward the target position each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformPositionBuilder<TransformBuilderReady> MoveTowards(ILunyObject target)
		{
			var options = new TransformBuilderOptions
				{ Target = target, Speed = 3.0, DeadZone = 0.1, Responsiveness = 1.0, AxisLock = LunyVector3.One };
			var token = _script.CreateBuilderToken(nameof(MoveTowards), "Transform.Move()");
			return new TransformPositionBuilder<TransformBuilderReady>(_script, token, options);
		}

		// --- Rotate Towards ---

		/// <summary>
		/// Rotate toward the target orientation each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (degrees/sec), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformRotationBuilder<TransformBuilderReady> RotateTowards(ILunyObject target)
		{
			var options = new TransformBuilderOptions
				{ Target = target, Speed = 90.0, DeadZone = 0.1, Responsiveness = 1.0, AxisLock = LunyVector3.One };
			var token = _script.CreateBuilderToken(nameof(RotateTowards), "Transform.Rotate()");
			return new TransformRotationBuilder<TransformBuilderReady>(_script, token, options);
		}

		// --- Scale Towards ---

		/// <summary>
		/// Scale toward the target scale each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformScaleBuilder<TransformBuilderReady> ScaleTowards(VariableBlock targetScale)
		{
			var options = new TransformBuilderOptions
				{ TargetScale = targetScale, Speed = 1.0, DeadZone = 0.1, Responsiveness = 1.0, AxisLock = LunyVector3.One };
			var token = _script.CreateBuilderToken(nameof(ScaleTowards), "Transform.Scale()");
			return new TransformScaleBuilder<TransformBuilderReady>(_script, token, options);
		}

		/*
		Category		Relative (Delta)	Absolute (Target)
		-----------------------------------------------------
		Vector			MoveBy(Vector2)		MoveTo(Vector2)
		Directional		MoveForward(5)		MoveForwardTo(10)
		Grid-based		ShiftRight(2)		ShiftRightTo(5)
		*/

		// --- Local Scalar Movement (Relative to "Nose") ---

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformPositionMoveByBlock MoveBy(VariableBlock direction, VariableBlock speed = null) =>
			TransformPositionMoveByBlock.Create(direction, speed, LunyTransformSpace.Self);

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformPositionMoveRelativeBlock MoveForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Forward, speed, LunyTransformSpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformPositionMoveRelativeBlock MoveRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Right, speed, LunyTransformSpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformPositionMoveRelativeBlock MoveUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Up, speed, LunyTransformSpace.Self);

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformPositionMoveRelativeBlock MoveBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Back, speed, LunyTransformSpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformPositionMoveRelativeBlock MoveLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Left, speed, LunyTransformSpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformPositionMoveRelativeBlock MoveDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Down, speed, LunyTransformSpace.Self);

		// --- World Scalar Movement (Relative to "Map") ---

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformPositionMoveByBlock ShiftBy(VariableBlock direction, VariableBlock speed = null) =>
			TransformPositionMoveByBlock.Create(direction, speed, LunyTransformSpace.World);

		/// <summary> Forward/backward on the World forward axis. </summary>
		public TransformPositionMoveRelativeBlock ShiftForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Forward, speed, LunyTransformSpace.World);

		/// <summary> Left/Right on the World right axis. </summary>
		public TransformPositionMoveRelativeBlock ShiftRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Right, speed, LunyTransformSpace.World);

		/// <summary> Up/Down on the World up axis. </summary>
		public TransformPositionMoveRelativeBlock ShiftUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Up, speed, LunyTransformSpace.World);

		/// <summary> Forward/backward on the World forward axis. </summary>
		public TransformPositionMoveRelativeBlock ShiftBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Back, speed, LunyTransformSpace.World);

		/// <summary> Left/Right on the World right axis. </summary>
		public TransformPositionMoveRelativeBlock ShiftLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Left, speed, LunyTransformSpace.World);

		/// <summary> Up/Down on the World up axis. </summary>
		public TransformPositionMoveRelativeBlock ShiftDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformPositionMoveRelativeBlock.Create(amount, LunyVector3.Down, speed, LunyTransformSpace.World);

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
		public ILunyObject Target;
		public VariableBlock TargetScale;
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
