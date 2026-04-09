using LunyScript;

public class Object_Enable_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var blinkName = "will blink";
		var alsoBlinkName = "will also blink";
		On.Heartbeat(Object.Disable(blinkName), Object.SetEnabled(alsoBlinkName, false));
		On.FrameUpdate(Object.Enable(blinkName), Object.SetEnabled(alsoBlinkName, true));
	}
}
