using LunyScript.Blocks;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Script_If_ElseIf_Else_SmokeTest : Script
	{
		public override void Build(ScriptBuildContext context)
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

			Coroutine("simple equality test, multiple conditions (implicit AND combined)")
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
					If(NOT(fact)).Then(NoOp()).ElseIf(NOT(altFact)).Then(NoOp())
				);

			Coroutine("using equality operators")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(fact == altFact).Then(NoOp()).ElseIf(fact != altFact).Then(NoOp()),
					If(!fact == !altFact).Then(NoOp()).ElseIf(!fact != !altFact).Then(NoOp())
				);

			// Double negations => avoid at all costs! Those are awful mindbenders.
			// Including semantically: "is not disabled". The positive form is ALWAYS easier: "is enabled". Oh, right! :)
			Coroutine("double negation: mindbenders")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!!fact == !!altFact).Then(NoOp()).ElseIf(!fact != !altFact).Then(NoOp())
				);

			// Logical AND()/OR() blocks and C# logical && and || operators
			Coroutine("logical operators: AND OR && ||")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(OR(fact == altFact, fact != !altFact, AND(fact, altFact))).Then(NoOp()),
					If(fact == altFact || fact != !altFact || fact && altFact).Then(NoOp())
				);

			Coroutine("literal test")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(AlwaysTrue()).Then(NoOp()).Else(NoOp()),
					If(AlwaysFalse()).Then(NoOp()).Else(NoOp())
				);
			Coroutine("literal test, negated")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!AlwaysTrue()).Then(NoOp()).Else(NoOp()),
					If(!AlwaysFalse()).Then(NoOp()).Else(NoOp())
				);
		}

		private ActionBlock NoOp() => Run(nameof(NoOp), () => {});
		private ConditionBlock AlwaysTrue() => Check(nameof(AlwaysTrue), () => true);
		private ConditionBlock AlwaysFalse() => Check(nameof(AlwaysFalse), () => false);
	}
}
