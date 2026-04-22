using LunyScript.Blocks;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Script_If_ElseIf_Else_SmokeTest : Script
	{
		public override void Build()
		{
			var fact = Var.Define("fact", true);
			var altFact = Var.Define("alt. fact", false);

			Coroutine("simple equality test")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(fact).Then(NoOp()).ElseIf(altFact).Then(NoOp())
				);

			Coroutine("simple equality test, nested")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(fact)
						.Then(If(!altFact).Then(NoOp()))
						.ElseIf(altFact)
						.Then(If(!fact).Then(NoOp()))
				);

			Coroutine("simple equality test, multiple conditions")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(fact, !altFact).Then(NoOp()).ElseIf(!fact, altFact).Then(NoOp())
				);

			Coroutine("negated equality tests")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!fact).Then(NoOp()).ElseIf(!altFact).Then(NoOp()),
					If(!fact).Then(NoOp()).ElseIf(!altFact).Then(NoOp())
				);

			Coroutine("using equality operators")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(fact == altFact).Then(NoOp()).ElseIf(fact != altFact).Then(NoOp()),
					If(!fact == !altFact).Then(NoOp()).ElseIf(!fact != !altFact).Then(NoOp())
				);
		}

		private ActionBlock NoOp() => Run(nameof(NoOp), () => {});
	}
}
