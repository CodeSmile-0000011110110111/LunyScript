using LunyScript.Blocks;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Script_Comments_SmokeTest : Script
	{
		public override void Build(ScriptBuildContext context) => Coroutine("comments")
			.Every(2)
			.Seconds()
			.WhenElapsed(
				Note("This is a comment or any other kind of message ..."),
				Note("More kind of notes with custom formatting/styling could be added."),
				Note("----------------------------------------------------------"),
				Note("A 'NoOp' is a no-operation which does nothing:"),
				NoOp()
			);

		private ActionBlock NoOp() => Run(nameof(NoOp), () => {});
		private ConditionBlock AlwaysTrue() => Check(nameof(AlwaysTrue), () => true);
		private ConditionBlock AlwaysFalse() => Check(nameof(AlwaysFalse), () => false);
	}
}
