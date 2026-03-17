namespace LunyScript.SmokeTests
{
	public class TriggerEvents : Script
	{
		public override void Build(ScriptBuildContext context) => On.Trigger()
			.Begins(Debug.Log("Trigger BEGINS"))
			.Continues(Debug.Log("Trigger continues ..."))
			.Ends(Debug.Log("Trigger ENDS"));
	}
}
