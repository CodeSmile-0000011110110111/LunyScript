using LunyScript;

public class Object_Enable_SmokeTest : Script
{
	public override void Build()
	{
		var blinkName = "will blink";
		var alsoBlinkName = "will also blink";

		On.Ready(For(10).Do(Debug.Log("ten")));

		var disableRoutine = Coroutine("on/off")
			.In(500)
			.Milliseconds()
			.WhenStarted(Object.Enable(blinkName), Object.SetEnabled(alsoBlinkName, false))
			.WhenElapsed(Object.Disable(blinkName), Object.SetEnabled(alsoBlinkName, true));

		Coroutine("restart on/off").Every(1000).Milliseconds().WhenElapsed(disableRoutine.Start());
	}
}
