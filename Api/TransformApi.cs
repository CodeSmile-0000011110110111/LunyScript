using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Blocks.Transform;

namespace LunyScript.Api
{
	public readonly struct TransformApi
	{
		private readonly Script _script;

		internal TransformApi(Script script) => _script = script;

		/*
		Category		Relative (Delta)	Absolute (Target)
		-----------------------------------------------------
		Vector			MoveBy(Vector2)		MoveTo(Vector2)
		Directional		MoveForward(5)		MoveForwardTo(10)
		Grid-based		ShiftRight(2)		ShiftRightTo(5)
		*/

		// --- Local Scalar Movement (Relative to "Nose") ---

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformTranslateBlock MoveBy(VariableBlock direction, VariableBlock speed = null) =>
			TransformTranslateBlock.Create(direction, speed, LunySpace.Self);

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformTranslateAxisBlock MoveForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Forward, speed, LunySpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformTranslateAxisBlock MoveRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Right, speed, LunySpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformTranslateAxisBlock MoveUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Up, speed, LunySpace.Self);

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformTranslateAxisBlock MoveBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Back, speed, LunySpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformTranslateAxisBlock MoveLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Left, speed, LunySpace.Self);

		/// <summary> Sideways relative to orientation. </summary>
		public TransformTranslateAxisBlock MoveDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Down, speed, LunySpace.Self);

		// --- World Scalar Movement (Relative to "Map") ---

		/// <summary> Forward/Backward based on orientation. </summary>
		public TransformTranslateBlock ShiftBy(VariableBlock direction, VariableBlock speed = null) =>
			TransformTranslateBlock.Create(direction, speed, LunySpace.World);

		/// <summary> Forward/backward on the World forward axis. </summary>
		public TransformTranslateAxisBlock ShiftForward(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Forward, speed, LunySpace.World);

		/// <summary> Left/Right on the World right axis. </summary>
		public TransformTranslateAxisBlock ShiftRight(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Right, speed, LunySpace.World);

		/// <summary> Up/Down on the World up axis. </summary>
		public TransformTranslateAxisBlock ShiftUp(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Up, speed, LunySpace.World);

		/// <summary> Forward/backward on the World forward axis. </summary>
		public TransformTranslateAxisBlock ShiftBack(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Back, speed, LunySpace.World);

		/// <summary> Left/Right on the World right axis. </summary>
		public TransformTranslateAxisBlock ShiftLeft(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Left, speed, LunySpace.World);

		/// <summary> Up/Down on the World up axis. </summary>
		public TransformTranslateAxisBlock ShiftDown(VariableBlock amount, VariableBlock speed = null) =>
			TransformTranslateAxisBlock.Create(amount, LunyVector3.Down, speed, LunySpace.World);

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
