using LunyScript;

public class Object_Enable_SmokeTest : Script
{
	public override void Build(ScriptBuildContext context)
	{
		var blinkName = "will blink";
		var alsoBlinkName = "will also blink";

		Coroutine("disable").Every(267).Milliseconds().WhenElapsed(Object.Disable(blinkName), Object.SetEnabled(alsoBlinkName, false));
		Coroutine("enable").Every(200).Milliseconds().WhenElapsed(Object.Enable(blinkName), Object.SetEnabled(alsoBlinkName, true));
	}
}
