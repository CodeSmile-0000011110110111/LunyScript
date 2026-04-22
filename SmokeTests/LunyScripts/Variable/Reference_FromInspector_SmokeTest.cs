using LunyScript;

public class Reference_FromInspector_SmokeTest : Script
{
	public override void Build()
	{
		var hello = Var.Define("hello", "Hello, ");
		var world = Var.Define("world", nameof(Reference_FromInspector_SmokeTest));

		On.Ready(Debug.Log(hello + world + "!"));

		var speed = Var["cube rotation speed"];
		var cube = Ref["cube reference"];
		On.FrameUpdate(Transform.RotateBy(speed).Target(cube).InWorldSpace());
	}
}
