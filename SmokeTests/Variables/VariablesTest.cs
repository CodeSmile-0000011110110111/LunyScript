using Luny;

namespace LunyScript.SmokeTests
{
	public sealed class VariablesTest : Script
	{
		public override void Build(ScriptBuildContext context)
		{
			// Object-bound (local) variables use 'Var'
			var variable = Var.Define("variable");
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

			var newVar = Var["not defined"];
			newVar.SetImmediate(95.96); // set value at build time "immediately"
			newVar.Set(111.222); // this returns a block, it won't change the value until it executes at runtime
			var sameVar = Var["not defined"];
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

				// Constants cannot be modified => ERROR: trying to modify constant variable!
				Debug.Log(constant), constant.Add(100), Debug.Log(constant),
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

			var scale = Var.Define("scale", 0.01);
			On.FrameUpdate(Transform.SetLocalScale(scale), scale.Add(0.001));

		}
	}
}
