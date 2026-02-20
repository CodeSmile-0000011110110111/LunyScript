namespace LunyScript.SmokeTests.Input
{
	public sealed class InputToTransformMove : Script
	{
		public override void Build(ScriptContext context)
		{
			// On.FrameUpdate(
			// 	If(Input.Direction("Move")).Then(Var["move count"].Inc()),
			// 	If(Input.Button("Jump").IsJustPressed).Then(Var["jump count"].Inc()),
			// 	If(Input.Button("Crouch").IsJustPressed).Then(Var["crouch count"].Inc()),
			// 	If(AND(Var["move count"] > 0), Var["jump count"] > 0, Var["crouch count"] > 0).Then(Debug.LogInfo("yay"))
			// );

			On.FrameUpdate(
				Transform.MoveBy(Input.Direction("Move"), 4),
				Transform.MoveUp(Input.Button("Jump").Strength, 4),
				Transform.MoveDown(Input.Button("Crouch").Strength, 4)
			);
		}
	}


	public sealed class InputToTransformShift : Script
	{
		public override void Build(ScriptContext context)
		{
			On.FrameUpdate(
				Transform.ShiftBy(Input.Direction("Move"), 4),
				Transform.ShiftUp(Input.Button("Jump").Strength, 4),
				Transform.ShiftDown(Input.Button("Crouch").Strength, 4)
			);
		}
	}


	// Typical Unity tutorial script doing the same as Transform.Move*
	/*
	using UnityEngine;

	public class InputToTransformMove : MonoBehaviour
	{
		public float moveSpeed = 4f;

		void Update()
		{
			// Horizontal and vertical input
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			// Move in local space (relative to object's forward)
			transform.position = transform.position + transform.forward * vertical * moveSpeed * Time.deltaTime;
			transform.position = transform.position + transform.right * horizontal * moveSpeed * Time.deltaTime;

			// Jump (local up)
			if (Input.GetKey(KeyCode.Space))
			{
				transform.position = transform.position + transform.up * moveSpeed * Time.deltaTime;
			}

			// Crouch (local down)
			if (Input.GetKey(KeyCode.C))
			{
				transform.position = transform.position + -transform.up * moveSpeed * Time.deltaTime;
			}
		}
	}
	*/

	// Typical Unity tutorial script doing the same as Transform.Shift*
	/*
	using UnityEngine;

	public class InputToTransformShift : MonoBehaviour
	{
		public float shiftSpeed = 4f;

		void Update()
		{
			// Horizontal and vertical input
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			// Shift in world space (always along world axes)
			transform.position = new Vector3(
				transform.position.x + horizontal * shiftSpeed * Time.deltaTime,
				transform.position.y,
				transform.position.z + vertical * shiftSpeed * Time.deltaTime
			);

			// Jump (world up)
			if (Input.GetKey(KeyCode.Space))
			{
				transform.position = new Vector3(
					transform.position.x,
					transform.position.y + shiftSpeed * Time.deltaTime,
					transform.position.z
				);
			}

			// Crouch (world down)
			if (Input.GetKey(KeyCode.C))
			{
				transform.position = new Vector3(
					transform.position.x,
					transform.position.y + -shiftSpeed * Time.deltaTime,
					transform.position.z
				);
			}
		}
	}
	*/


}
