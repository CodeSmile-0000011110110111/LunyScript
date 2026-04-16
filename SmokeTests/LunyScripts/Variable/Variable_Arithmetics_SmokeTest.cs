using Luny;
using System;

namespace LunyScript.SmokeTests.Blocks.Variables
{
	public class Variable_Arithmetics_SmokeTest : Script
	{
		public override void Build(ScriptBuildContext context)
		{
			const Double factor = 1.111111111111111;

			var inspectorTestVar = Var["InspectorValue"];
			LunyLogger.LogInfo($"Inspector variable is: {inspectorTestVar}");
			On.Ready(inspectorTestVar.Sub(120000));

			var setVar = Var.Define("set");
			var incVar = Var.Define("inc");
			var decVar = Var.Define("dec");
			var addVar = Var.Define("add");
			var subVar = Var.Define("sub");
			var mulVar = Var.Define("mul", factor);
			var divVar = Var.Define("div", Int32.MinValue);
			var toggleVar = Var.Define("toggle");

			Coroutine("arithmetics shmarithmetics")
				.Every(200)
				.Milliseconds()
				.WhenElapsed(
					setVar.Set(Int16.MinValue),
					incVar.Inc(),
					decVar.Dec(),
					addVar.Add(factor),
					subVar.Sub(factor),
					mulVar.Mul(factor),
					divVar.Div(factor),
					toggleVar.Toggle()
				);

			// var setLiteralVar = Var.Define("set Literal");
			// var incLiteralVar = Var.Define("inc Literal");
			// var decLiteralVar = Var.Define("dec Literal");
			var addLiteralVar = Var.Define("add Literal");
			var subLiteralVar = Var.Define("sub Literal");
			var mulLiteralVar = Var.Define("mul Literal", factor);
			var divLiteralVar = Var.Define("div Literal", Int32.MinValue);
			var complexMathVar = Var.Define("complex math");
			var toggleLiteralVar = Var.Define("toggle Literal");

			Coroutine("literal arithmetics")
				.Every(200)
				.Milliseconds()
				.WhenElapsed(
					addLiteralVar + factor,
					subLiteralVar - factor,
					mulLiteralVar * factor,
					divLiteralVar / factor,
					complexMathVar + 1 * 10 / 5 - 3 - (complexMathVar - 1) * (10 / (5 - 3)) / 1234.56789,
					toggleLiteralVar.Set(!toggleLiteralVar)

					// C# design may prevent supporting these as auto-converting action blocks, but may be supportable by wrapping in Set()
					//
					// unsupported as implicitly converted ActionBlock (works for conditions!):
					// !toggleLiteralVar
					//
					// unsupported:
					// setLiteralVar = Int64.MinValue,
					// ++incLiteralVar,
					// incLiteralVar++,
					// --decLiteralVar,
					// decLiteralVar--,
					// incLiteralVar.Set(++incLiteralVar),
					// incLiteralVar.Set(incLiteralVar++),
					// decLiteralVar.Set(--decLiteralVar),
					// decLiteralVar.Set(decLiteralVar--),
				);

			// duck typing: variables can change type at runtime
			var counter = Var.Define("🦆 type counter");
			var changeType = Var.Define("🦆 changes type");
			Coroutine("🦆🦆🦆")
				.Every(1)
				.Seconds()
				.WhenElapsed(
					If(counter == 1)
						.Then(changeType.Set("A 🦆 is not a 🪿 but both are 🍽️🍗😋"))
						.ElseIf(counter == 2)
						.Then(changeType.Set(8008.15))
						.Else(changeType.Set(true), counter.Set(0)),
					counter.Inc()
				);

			var divByZero = Var.Define("div by zero", 1234567890);
			On.Ready(
				divByZero.Div(0) // not an error, variable is set to 0 (Mathematicians cry in agony)
			);
		}
	}
}
