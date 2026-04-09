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
		/// <summary> Rotate around <paramref name="axis"/> by <paramref name="amount"/> degrees per second. Chain <c>.Clamp(min, max)</c> and/or <c>.InWorldSpace()</c>. </summary>
		public TransformRotateBuilder<TransformBuilderReady> Rotate(VariableBlock amount, LunyAxis axis) =>
			TransformRotateBuilder<TransformBuilderReady>.Create(_script, amount, axis, _trace.Add(nameof(Rotate)));

		// --- Set (Absolute Snap) ---

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the local position. Append <c>.InWorldSpace()</c> to set world position. </summary>
		public TransformSetPositionTerminalBuilder SetPosition(VariableBlock<LunyVector3> position) =>
			TransformSetPositionTerminalBuilder.Create(_script, position, LunyTransformSpace.Local, _trace.Add(nameof(SetPosition)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the local rotation. Append <c>.InWorldSpace()</c> to set world rotation. </summary>
		public TransformSetRotationTerminalBuilder SetRotation(LunyVector3 eulerAngles) =>
			TransformSetRotationTerminalBuilder.Create(_script, LunyQuaternion.Euler(eulerAngles), LunyTransformSpace.Local, _trace.Add(nameof(SetRotation)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the local rotation. Append <c>.InWorldSpace()</c> to set world rotation. </summary>
		public TransformSetRotationTerminalBuilder SetRotation(VariableBlock<LunyQuaternion> rotation) =>
			TransformSetRotationTerminalBuilder.Create(_script, rotation, LunyTransformSpace.Local, _trace.Add(nameof(SetRotation)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the local scale. </summary>
		public TransformScaleSetLocalBlock SetScale(Double uniformScale) =>
			TransformScaleSetLocalBlock.Create(LunyVector3.Uniform(uniformScale));

		[NeedsReview, NeedsSmokeTest]
		public TransformScaleSetLocalUniformBlock SetScale(VariableBlock uniformScale) =>
			TransformScaleSetLocalUniformBlock.Create(uniformScale);

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Instantly set the local scale. </summary>
		public TransformScaleSetLocalBlock SetScale(VariableBlock<LunyVector3> scale) => TransformScaleSetLocalBlock.Create(scale);

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
		*/

		// --- Local Movement (Relative to orientation) ---

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Floor-plane movement based on a 2D direction vector. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveTerminalBuilder MoveBy(VariableBlock<LunyVector2> direction, VariableBlock speed = null) =>
			TransformMoveTerminalBuilder.CreateDirectional(_script, direction, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveBy)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Forward based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveTerminalBuilder MoveForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveTerminalBuilder.CreateAxisRelative(_script, amount, LunyVector3.Forward, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveForward)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Backward based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveTerminalBuilder MoveBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveTerminalBuilder.CreateAxisRelative(_script, amount, LunyVector3.Back, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveBack)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Right based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveTerminalBuilder MoveRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveTerminalBuilder.CreateAxisRelative(_script, amount, LunyVector3.Right, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveRight)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Left based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveTerminalBuilder MoveLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveTerminalBuilder.CreateAxisRelative(_script, amount, LunyVector3.Left, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveLeft)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Up based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveTerminalBuilder MoveUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveTerminalBuilder.CreateAxisRelative(_script, amount, LunyVector3.Up, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveUp)));

		[NeedsReview, NeedsSmokeTest]
		/// <summary> Down based on orientation. Append <c>.InWorldSpace()</c> for world-axis movement. </summary>
		public TransformMoveTerminalBuilder MoveDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformMoveTerminalBuilder.CreateAxisRelative(_script, amount, LunyVector3.Down, speed, LunyTransformSpace.Local, _trace.Add(nameof(MoveDown)));

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
		public StackTrace Trace;
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
