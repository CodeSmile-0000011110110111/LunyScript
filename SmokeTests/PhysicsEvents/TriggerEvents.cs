using LunyScript.Api;

namespace LunyScript.SmokeTests
{
	public class TriggerEvents : Script
	{
		public override void Build(ScriptBuildContext context) => On.Trigger()
			.Entered(Debug.Log("Trigger ENTERED"))
			.Overlapping(Debug.Log("Trigger staying ..."))
			.Exited(Debug.Log("Trigger EXITED"));
	}
}
