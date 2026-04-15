namespace LunyScript.SmokeTests
{
	public class CollisionEvents : Script
	{
		public override void Build(ScriptBuildContext context) => On.Collision()
			.Started(Debug.Log("Collision STARTED"))
			.Touching(Debug.Log("Collision continuing ..."))
			.Ended(Debug.Log("Collision ENDED"));
	}
}
