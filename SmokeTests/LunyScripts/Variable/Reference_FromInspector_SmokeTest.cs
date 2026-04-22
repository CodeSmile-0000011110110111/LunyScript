using Luny;
using Luny.Unity.Bridge;
using LunyScript;
using UnityEngine;

public class Reference_FromInspector_SmokeTest : Script
{
	public override void Build()
	{
		var hello = Var.Define("hello", "Hello, ");
		var world = Var.Define("world", nameof(Reference_FromInspector_SmokeTest));

		On.Ready(Debug.Log(hello + world + "!"));

		var speed = Var["cube rotation speed"];
		var cube = Ref.GetGameObject("cube reference"); // get the reference, properly type-cast
		On.FrameUpdate(Transform.RotateBy(speed).Target(cube).InWorldSpace());

		// The Ref indexer returns System.Object types that require casting ...
		var cubeUncast = Ref["cube reference"] as GameObject;
		// ... and for use with blocks they need to be converted to a Luny instance.
		var lunyCube = UnityGameObject.ToLuny(cubeUncast);
		On.FrameUpdate(Transform.RotateBy(speed * 2).AroundZ().Target(lunyCube));

		var cubeTransform = Ref.GetTransform("Rotating Cube Transform");
		LunyLogger.LogWarning(cubeTransform);
		// On.FrameUpdate(Transform.RotateBy(speed).Target(cubeTransform).InWorldSpace());

		var dss = Ref["Default Style Sheet"];
		LunyLogger.LogInfo(dss);
	}
}
