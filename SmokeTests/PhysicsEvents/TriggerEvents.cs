namespace LunyScript.SmokeTests
{
	public class TriggerEvents : Script
	{
		public override void Build(ScriptBuildContext context) => On.Trigger()
			.Entered(Debug.Log("Trigger ENTERED"))
			.Staying(Debug.Log("Trigger staying ..."))
			.Exited(Debug.Log("Trigger EXITED"));
	}
}
