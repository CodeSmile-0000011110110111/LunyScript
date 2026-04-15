using LunyScript.Blocks;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Variable_Comparisons_SmokeTest : Script
	{
		public override void Build(ScriptBuildContext context)
		{
			var fact = Var.Define("fact", true);
			var altFact = Var.Define("alt. fact", false);

			On.FrameUpdate(
				If(fact).Then(NoOp()).ElseIf(altFact).Then(NoOp()),
				If(!fact).Then(NoOp()).ElseIf(!altFact).Then(NoOp()),
				If(fact == altFact).Then(NoOp()).ElseIf(fact != altFact).Then(NoOp()),
				If(!fact == !altFact).Then(NoOp()).ElseIf(!fact != !altFact).Then(NoOp())
			);

			On.AfterFrameUpdate(If(fact == altFact || fact != !altFact || fact && altFact).Then(NoOp()));

			var counter = Var.Define("counter");
			var other = Var.Define("other");
			Coroutine("counter routine")
				.Every(500)
				.Milliseconds()
				.WhenElapsed(
					If(counter <= 1 && counter == other)
						.Then(NoOp())
						.ElseIf(counter <= 2 && !(counter != other))
						.Then(NoOp())
						.ElseIf(counter <= 3 || counter != other && !counter == !other)
						.Then(NoOp())
						.Else(counter.Set(0)),
					counter.Inc()
				);
		}

		private ActionBlock NoOp() => Run(nameof(NoOp), () => {});
	}
}
