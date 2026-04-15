using LunyScript.Blocks;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Script_CheckBlock_SmokeTest : Script
	{
		public override void Build(ScriptBuildContext context)
		{
			Coroutine("literal test")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(AlwaysTrue()).Then(NoOp()),
					If(AlwaysFalse()).Then(NoOp())
				);
			Coroutine("literal test, negated")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!AlwaysTrue()).Then(NoOp()),
					If(!AlwaysFalse()).Then(NoOp())
				);

			Coroutine("check and run blocks, unnamed")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(Check(() => true)).Then(Run(() => {}))
				);
		}

		private ActionBlock NoOp() => Run(nameof(NoOp), () => {});
		private ConditionBlock AlwaysTrue() => Check(nameof(AlwaysTrue), () => true);
		private ConditionBlock AlwaysFalse() => Check(nameof(AlwaysFalse), () => false);
	}
}
