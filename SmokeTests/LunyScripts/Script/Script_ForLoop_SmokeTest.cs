using LunyScript.Blocks;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Script_ForLoop_SmokeTest : Script
	{
		public override void Build()
		{
			// Loops are safeguarded against infinite loops, they exit with an error after ScriptEngine.MaxLoopIterations (default: 32k)
			var counterFor = Var.Define("counter For()");
			var counterForNested = Var.Define("counter For() nested");
			var counterForStep = Var.Define("counter For() with step size");
			var counterForStepBack = Var.Define("counter For() with step size, backwards");

			// increment 5 times with for loop
			Coroutine("For()")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					For(5)
						.Do(counterFor.Inc())
				);

			// increment 10 times with nested for loop
			Coroutine("For(), nested")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					For(5)
						.Do(For(2)
							.Do(counterForNested.Inc()))
				);

			// increment 5 times (count to 25 in steps of five) with for loop
			Coroutine("For() with step size")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					For(25, 5)
						.Do(counterForStep.Inc())
				);

			// negative step size => backwards iteration
			// increment 5 times backwards (count from 25 back to 0 in steps of five) with for loop
			Coroutine("For(), iterating backwards")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					For(25, -5)
						.Do(counterForStepBack.Inc())
				);
		}

		private ActionBlock NoOp() => Run(nameof(NoOp), () => {});
	}
}
