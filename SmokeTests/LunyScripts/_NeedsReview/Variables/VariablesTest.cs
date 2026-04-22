using Luny;

namespace LunyScript.SmokeTests
{
	public sealed class VariablesTest : Script
	{
		public override void Build()
		{
			// Object-bound (local) variables use 'Var'
			var variable = Var.Define("variable"); // will have a value of 0
			var integer = Var.Define("integer", 123);
			var floating = Var.Define("floating point", 1.234);
			var boolean = Var.Define("boolean", true);
			var text = Var.Define("text", "Hello, Goodbye");

			// Global variables use 'GVar' (G = Global)
			var constant = GVar.Constant("constant", 299792458);

			LunyLogger.LogInfo(integer, this);
			LunyLogger.LogInfo(floating, this);
			LunyLogger.LogInfo(boolean, this);
			LunyLogger.LogInfo(text, this);
			LunyLogger.LogInfo(constant, this);

			LunyLogger.LogInfo(Var["undefined variable"], this);

			// CAUTION: Build()-time vs run-time block execution
			var newVar = Var["previously undefined"];
			newVar.SetImmediate(95.96); // set value at build time "immediately"
			newVar.Set(111.222); // this returns a block, it won't change the value until it executes at runtime
			LunyLogger.LogInfo(newVar); // this will log '95.96' !!

			var sameVar = Var["previously undefined"]; // gets the now-defined variable eg the same
			LunyLogger.LogInfo($"{newVar} and {sameVar} are equal: {newVar.Value == sameVar.Value}");

			On.Ready(
				Debug.Log("------------------------------------------------------------------------------"),

				// arithmetics work with both variables (here: integer) and literal values (here: 2)
				Debug.Log(integer), integer.Mul(2), Debug.Log(integer), integer.Sub(integer * 2), Debug.Log(integer),

				// Inc() and Dec() are shorthand for increment (+1) and decrement (-1), alternatives to Add(1) and Sub(1)
				Debug.Log(floating), floating.Sub(2), Debug.Log(floating), floating.Inc(), Debug.Log(floating),

				// Booleans can be set or toggled
				Debug.Log(boolean), boolean.Toggle(), Debug.Log(boolean), boolean.Toggle(), Debug.Log(boolean),

				// Strings can be set
				Debug.Log(text), text.Set("I don't know why you say, 'Goodbye', I say, 'Hello'"), Debug.Log(text),

				// Constants cannot be modified => this would be an ERROR!
				//Debug.Log(constant), constant.Add(100), Debug.Log(constant),
				Debug.Log("------------------------------------------------------------------------------"),

				// COMPARISON
				// Variables can be used as conditions and compared with comparison and logical operators
				// Comparison against literal value (type mismatch doesn't matter)
				If(integer >= 0.123456789)
					.Then(Debug.Log(integer), Debug.Log("The integer is >= 0"))
					.Else(Debug.Log(integer), Debug.Log("The integer is < 0")),

				// Equality test against other variable
				If(floating != integer)
					.Then(Debug.Log(floating), Debug.Log("The floating point variable is not equal to integer"))
					.Else(Debug.Log(floating), Debug.Log("The floating point variable is equal to integer")),

				// Using a logical operator, here: && (AND)
				If(boolean && boolean != false) // better to write: If(boolean)
					.Then(Debug.Log(boolean), Debug.Log("The boolean variable is TRUE"))
					.Else(Debug.Log(boolean), Debug.Log("The boolean variable is FALSE")),

				// Comparing string against boolean or number will result in 'false'
				If(text || text == true || text == 0)
					.Then(Debug.Log(text), Debug.Log("The text variable is true or 0"))
					.Else(Debug.Log(text), Debug.Log("The text variable is neither true nor 0")),
				Debug.Log("------------------------------------------------------------------------------"),

				// DUCK TYPING: variable types can change
				// Changes integer to floating point
				integer.Div(-3.456789), Debug.Log("integer is now a floating point value..."), Debug.Log(integer),
				// Changes string to integer
				text.Set(-123456789), Debug.Log("text is now an integer value..."), Debug.Log(text),
				// Changes boolean to string
				boolean.Set("Once upon a Boolean, there was .."), Debug.Log("boolean is now a string..."), Debug.Log(boolean),

				// CAUTION!
				// This string concatenation uses the integer's value when Build() runs, it does not run as a block:
				Debug.Log($"integer value at Build() time: {integer}"), // prints value during Build()
				Debug.Log(integer), // prints the runtime value
				Debug.Log("------------------------------------------------------------------------------"),

				// Division by Zero is "allowed" => it will return 0, not Infinity! (unexpected by programmers and mathematicians)
				Debug.Log(floating), floating.Div(0), Debug.Log(floating)
			);

			// grow the object over time
			var scale = Var.Define("scale", 0.01);
			On.FrameUpdate(
				Transform.SetScale(scale),
				scale.Add(0.0004)
			);
			//On.Ready(Transform.SetLocalScale(scale / scale + scale / scale));

			// ARITHMETICS
			var num1 = Var.Define("num1", 1);
			var num2 = Var.Define("num2", 2);
			On.Ready(
				Debug.Log(num1), // prints '1'
				Debug.Log(num2), // prints '2'
				num1.Add(num2), // num1: (1 + 2) = 3
				Debug.Log(num1), // prints '3'
				num2.Sub(num1), // num2: (2 - 3) = -1
				Debug.Log(num2), // prints '-1'
				num1.Mul(num2), // num1: (3 * -1) = -3
				Debug.Log(num1), // prints '-3'
				num2.Div(num1), // num2: (-1 / -3) = 0.333333333333333
				Debug.Log(num2) // prints '0.333333333333333'
			);

			// ARITHMETICS WITH OPERATORS
			var num3 = Var.Define("num3", 1);
			var num4 = Var.Define("num4", 2);
			var result = Var.Define("result");
			// Note: operators return VariableBlock instances which must be passed to a Set() method for the final result
			On.Ready(
				result.Set((num4 - (num3 + num4)) / ((num3 + num4) * (num4 - (num3 + num4)))),
				Debug.Log(result) // prints '0.333333333333333' (same as above)
			);

			// ARITHMETICS WITH OPERATORS AND INTERMEDIATE VALUE
			var num5 = Var.Define("num5", 1);
			var num6 = Var.Define("num6", 2);
			var three = Var.Define("intermediate");
			var result2 = Var.Define("result2");
			// Note: operators return VariableBlock instances which must be passed to a Set() method for the final result
			On.Ready(
				three.Set(num5 + num6),
				result2.Set((num6 - three) / (three * (num6 - three))),
				Debug.Log(result2) // prints '0.333333333333333' (same as above)
			);

			// STORED ARITHMETIC OPERATION
			var v1 = Var.Define("v1", 2);
			var v2 = Var.Define("v2", 3);
			var sumOf1And2 = v1 + v2;
			On.Ready(v1.Set(sumOf1And2), Debug.Log(v1)); // (2 + 3) = 5
			On.Ready(v1.Set(sumOf1And2), Debug.Log(v1)); // (5 + 3) = 8
			On.Ready(v1.Set(sumOf1And2), Debug.Log(v1)); // (8 + 3) = 11

			// INCREMENT/DECREMENT
			var increasing = Var.Define("inc");
			var decreasing = Var.Define("dec");
			On.Heartbeat(If(increasing < 3)
				.Then(increasing.Inc(), Debug.Log(increasing),
					decreasing.Dec(), Debug.Log(decreasing))
			);

			// FLIP A BOOLEAN
			var fact = Var.Define("fact", false);
			On.Ready(
				fact.Toggle(), Debug.Log(fact),
				fact.Toggle(), Debug.Log(fact)
			);

			// COMPARISONS
			var compare1 = Var.Define("compare1", 0.123456789);
			var compare2 = Var.Define("compare2", 0.1111111111);
			var truth = Var.Define("another fact", true);
			var message = Var.Define("peace", "We come in peace!");
			On.Ready(
				If(compare1 >= 0.123456789) // compare with literal
					.Then(Debug.Log($"{compare1.Value} is >= 0.123456789"))
					.Else(Debug.Log($"{compare1.Value} is < 0.123456789")),
				If(compare1 >= compare2) // compare with variable
					.Then(Debug.Log($"{compare1.Value} is >= 0.123456789"))
					.Else(Debug.Log($"{compare1.Value} is < 0.123456789")),
				If(compare1) // compare as boolean (non-zero => true)
					.Then(Debug.Log($"{compare1.Value} is true."))
					.Else(Debug.Log($"{compare1.Value} is false.")),
				If(!truth) // compare boolean, negated
					.Then(Debug.Log($"It's a fact. ({truth.Value})"))
					.Else(Debug.Log($"It's an alternative fact!! ({truth.Value})")),
				If(message == "We come in peace!")
					.Then(Debug.Log("They come in peace!"))
					.Else(Debug.Log("They'll leave in pieces .."))
			);

			// STRING CONCATENATION
			var str1 = Var.Define("str1", "A long, long time ago ...");
			var str2 = Var.Define("str2", " in a galaxy far, far away.");
			var drei = Var.Define("three", 3);
			var antisocial = Var.Define("truth", true);
			On.Ready(
				str1.Add(str2),
				Debug.Log(str1),
				str1.Add(" There were "),
				str1.Add(drei),
				str1.Add(" "),
				str1.Add(antisocial),
				str1.Add(" little piggies."),
				Debug.Log(str1)
			);
		}
	}
}
