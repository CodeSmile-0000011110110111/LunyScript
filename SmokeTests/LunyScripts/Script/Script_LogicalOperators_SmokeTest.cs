using LunyScript.Blocks;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Script_LogicalOperators_SmokeTest : Script
	{
		public override void Build()
		{
			var fact = Var.Define("fact", true);
			var altFact = Var.Define("alt. fact", false);
			var isDisabled = Var.Define("disabled", true);

			Coroutine("negated variable / condition")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!altFact).Then(NoOp()),
					If(!AlwaysFalse()).Then(NoOp())
				);

			// Double negations => avoid at all costs! Those are awful mindbenders.
			// Including semantically: "is not disabled". The positive form is ALWAYS easier: "is enabled". Oh, right! :)
			Coroutine("🤦 variable double negation 🫨 mindbenders 😧")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!!fact).Then(NoOp()), // double negation
					If(!!!altFact).Then(NoOp()), // triple negation
					If(!!!!fact).Then(NoOp()), // quadruple negation
					Note("Following is a triple-negation too! Same as If(isEnabled).. !!"),
					If(!isDisabled == false).Then(NoOp())
				);

			Coroutine("🤦 condition double negation 🫨 mindbenders 😧")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!!AlwaysTrue()).Then(NoOp()), // double negation
					If(!!!AlwaysFalse()).Then(NoOp()), // triple negation
					If(!!!!AlwaysTrue()).Then(NoOp()) // quadruple negation
				);

			// Logical AND()/OR() blocks and C# logical && and || operators
			Coroutine("logical operators: AND OR && ||")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!fact || altFact || fact && !altFact).Then(NoOp()),
					If(fact || !altFact || !(!fact && altFact)).Then(NoOp()),
					If(!AlwaysTrue() || AlwaysFalse() || AlwaysTrue() && !AlwaysFalse()).Then(NoOp()),
					If(AlwaysTrue() || !AlwaysFalse() || !(!AlwaysTrue() && AlwaysFalse())).Then(NoOp())
				);

			// Double negations => avoid at all costs! Those are awful mindbenders.
			// Including semantically: "is not disabled". The positive form is ALWAYS easier: "is enabled". Oh, right! :)
			Coroutine("double negation: mindbenders")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(!!fact == (!!altFact == false)).Then(NoOp()).ElseIf(!fact != !altFact).Then(NoOp())
				);

			// Logical AND()/OR() blocks and C# logical && and || operators
			Coroutine("equality combined with && ||")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					If(fact == !altFact && fact != altFact && (fact == false || altFact == false)).Then(NoOp())
				);
		}

		private ActionBlock NoOp() => Run(nameof(NoOp), () => {});
		private ConditionBlock AlwaysTrue() => Check(nameof(AlwaysTrue), () => true);
		private ConditionBlock AlwaysFalse() => Check(nameof(AlwaysFalse), () => false);
	}
}
