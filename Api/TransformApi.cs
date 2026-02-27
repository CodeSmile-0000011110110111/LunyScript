using Luny.Engine.Bridge;
using LunyScript.Api.Transform;
using LunyScript.Blocks;
using LunyScript.Blocks.Transform;

namespace LunyScript.Api
{
	public readonly struct TransformApi
	{
		private readonly Script _script;

		internal TransformApi(Script script) => _script = script;

		// --- Set (Absolute Snap) ---

		/// <summary> Instantly set the World position. </summary>
		public TransformSetPositionBlock SetPosition(VariableBlock position) => TransformSetPositionBlock.Create(position);

		/// <summary> Instantly set the Local position. </summary>
		public TransformSetLocalPositionBlock SetLocalPosition(VariableBlock position) => TransformSetLocalPositionBlock.Create(position);

		/// <summary> Instantly set the World rotation. </summary>
		public TransformSetRotationBlock SetRotation(VariableBlock rotation) => TransformSetRotationBlock.Create(rotation);

		/// <summary> Instantly set the Local rotation. </summary>
		public TransformSetLocalRotationBlock SetLocalRotation(VariableBlock rotation) => TransformSetLocalRotationBlock.Create(rotation);

		/// <summary> Instantly set the Local scale. </summary>
		public TransformSetLocalScaleBlock SetLocalScale(VariableBlock scale) => TransformSetLocalScaleBlock.Create(scale);

		// --- Look At ---

		/// <summary>
		/// Instantly orient to face the target.
		/// Chain <c>.WorldUp(v)</c>, <c>.LockX()</c>, <c>.LockY()</c>, <c>.LockZ()</c> then call <c>.Do()</c>.
		/// </summary>
		public TransformLookAtBuilder<TransformBuilderReady> LookAt(ILunyObject target)
		{
			var options = new TransformLookAtOptions { Target = target, WorldUp = LunyVector3.Up, AxisLock = LunyVector3.One };
			var token = _script.CreateToken(nameof(LookAt), "TransformLookAtBuilder");
			return new TransformLookAtBuilder<TransformBuilderReady>(_script, options, token);
		}

		// --- Move Towards ---

		/// <summary>
		/// Move toward the target position each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformMoveBuilder<TransformBuilderReady> MoveTowards(ILunyObject target)
		{
			var options = new TransformTowardsObjectOptions { Target = target, Speed = 3.0, DeadZone = 0.1, Responsiveness = 1.0 };
			var token = _script.CreateToken(nameof(MoveTowards), "TransformMoveBuilder");
			return new TransformMoveBuilder<TransformBuilderReady>(_script, options, token);
		}

		// --- Rotate Towards ---

		/// <summary>
		/// Rotate toward the target orientation each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (degrees/sec), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformRotateBuilder<TransformBuilderReady> RotateTowards(ILunyObject target)
		{
			var options = new TransformTowardsObjectOptions { Target = target, Speed = 90.0, DeadZone = 0.1, Responsiveness = 1.0 };
			var token = _script.CreateToken(nameof(RotateTowards), "TransformRotateBuilder");
			return new TransformRotateBuilder<TransformBuilderReady>(_script, options, token);
		}

		// --- Scale Towards ---

		/// <summary>
		/// Scale toward the target scale each frame.
		/// Chain <c>.Speed(n)</c>, <c>.Responsiveness(n)</c>, <c>.DeadZone(n)</c>, <c>.LockX/Y/Z()</c>
		/// then call <c>.Do()</c> (linear), <c>.Lerp()</c> or <c>.Slerp()</c>.
		/// </summary>
		public TransformScaleBuilder<TransformBuilderReady> ScaleTowards(VariableBlock targetScale)
		{
			var options = new TransformTowardsVariableOptions { TargetScale = targetScale, Speed = 1.0, DeadZone = 0.1, Responsiveness = 1.0 };
			var token = _script.CreateToken(nameof(ScaleTowards), "TransformScaleBuilder");
			return new TransformScaleBuilder<TransformBuilderReady>(_script, options, token);
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
		public TransformMoveForwardBlock MoveBy(VariableBlock direction, VariableBlock speed = null) =>
			TransformMoveForwardBlock.Create(direction, speed, LunySpace.Self);

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformMoveAbsoluteBlock MoveForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Forward, speed, LunySpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformMoveAbsoluteBlock MoveRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Right, speed, LunySpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformMoveAbsoluteBlock MoveUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Up, speed, LunySpace.Self);

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformMoveAbsoluteBlock MoveBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Back, speed, LunySpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformMoveAbsoluteBlock MoveLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Left, speed, LunySpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformMoveAbsoluteBlock MoveDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Down, speed, LunySpace.Self);

		// --- World Scalar Movement (Relative to "Map") ---

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformMoveForwardBlock ShiftBy(VariableBlock direction, VariableBlock speed = null) =>
			TransformMoveForwardBlock.Create(direction, speed, LunySpace.World);

		/// <summary> Forward/backward on the World forward axis. </summary>
		public TransformMoveAbsoluteBlock ShiftForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Forward, speed, LunySpace.World);

		/// <summary> Left/Right on the World right axis. </summary>
		public TransformMoveAbsoluteBlock ShiftRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Right, speed, LunySpace.World);

		/// <summary> Up/Down on the World up axis. </summary>
		public TransformMoveAbsoluteBlock ShiftUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Up, speed, LunySpace.World);

		/// <summary> Forward/backward on the World forward axis. </summary>
		public TransformMoveAbsoluteBlock ShiftBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Back, speed, LunySpace.World);

		/// <summary> Left/Right on the World right axis. </summary>
		public TransformMoveAbsoluteBlock ShiftLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Left, speed, LunySpace.World);

		/// <summary> Up/Down on the World up axis. </summary>
		public TransformMoveAbsoluteBlock ShiftDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveAbsoluteBlock.Create(amount, LunyVector3.Down, speed, LunySpace.World);

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
}
