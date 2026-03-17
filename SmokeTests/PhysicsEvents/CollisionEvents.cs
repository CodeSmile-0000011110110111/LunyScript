namespace LunyScript.SmokeTests
{
	public class CollisionEvents : Script
	{
		public override void Build(ScriptBuildContext context) => On.Collision()
			.Begins(Debug.Log("Collision BEGINS"))
			.Continues(Debug.Log("Collision continues ..."))
			.Ends(Debug.Log("Collision ENDS"));
	}
}
