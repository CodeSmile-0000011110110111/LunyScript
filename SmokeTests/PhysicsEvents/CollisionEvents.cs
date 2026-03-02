namespace LunyScript.SmokeTests.PhysicsEvents
{
	public class CollisionEvents : Script
	{
		public override void Build(ScriptContext context) => On.Collision()
			.Begins(Debug.Log("Collision BEGINS"))
			.Continues(Debug.Log("Collision continues ..."))
			.Ends(Debug.Log("Collision ENDS"));
	}
}
