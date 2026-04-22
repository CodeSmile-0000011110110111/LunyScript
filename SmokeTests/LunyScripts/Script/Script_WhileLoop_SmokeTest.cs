using LunyScript.Blocks;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Script_WhileLoop_SmokeTest : Script
	{
		public override void Build()
		{
			// Loops are safeguarded against infinite loops, they exit with an error after ScriptEngine.MaxLoopIterations (default: 32k)
			var counterWhile = Var.Define("counter While()");
			var counterWhileNested = Var.Define("counter While() nested");
			var iterationCountWhile = Var.Define("iterations While()");
			var iterationCountWhileNested = Var.Define("iterations While() nested");

			// increment 5 times with while loop
			Coroutine("While()")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					iterationCountWhile.Set(5),
					While(iterationCountWhile > 0)
						.Do(iterationCountWhile.Dec(), counterWhile.Inc())
				);

			// increment 10 times with nested while loop
			Coroutine("While(), nested")
				.Every(.5)
				.Seconds()
				.WhenElapsed(
					iterationCountWhile.Set(5),
					While(iterationCountWhile > 0)
						.Do(iterationCountWhile.Dec(),
							iterationCountWhileNested.Set(2),
							While(iterationCountWhileNested > 0)
								.Do(iterationCountWhileNested.Dec(), counterWhileNested.Inc()))
				);
		}

		private ActionBlock NoOp() => Run(nameof(NoOp), () => {});
	}
}
