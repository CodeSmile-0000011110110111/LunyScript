using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript
{
	public abstract partial class Script
	{
		/// <summary>
		/// Reference to script's engine object.
		/// </summary>
		[MaybeNull] protected ILunyObject Self => _runtimeContext?.LunyObject;

		[NotNull] public String Name => GetType().Name;

		/// <summary>
		/// True if the script currently runs within the engine's editor (play mode). False in builds.
		/// </summary>
		public Boolean IsEditor => LunyEngine.Instance.Application.IsEditor;

		/// <summary>
		/// Global variables
		/// </summary>
		[NotNull] public ScriptVariables GVar => _globalVariables;
		/// <summary>
		/// Instance variables (unique per script/object)
		/// </summary>
		[NotNull] public ScriptVariables Var => _instanceVariables;

		public ComponentApi Component => new(this, ScriptTrace.TryCreateStackTrace(nameof(Component)));
		public DebugApi Debug => new(this, ScriptTrace.TryCreateStackTrace(nameof(Debug)));
		public EditorApi Editor => new(this, ScriptTrace.TryCreateStackTrace(nameof(Editor)));
		public InputBuilder Input => new(this, ScriptTrace.TryCreateStackTrace(nameof(Input)));
		public ObjectBuilder Object => new(this, ScriptTrace.TryCreateStackTrace(nameof(Object)));
		public OnObjectEventBuilder On => new(this, ScriptTrace.TryCreateStackTrace(nameof(On)));
		public PrefabBuilder Prefab => new(this, ScriptTrace.TryCreateStackTrace(nameof(Prefab)));
		public SceneApi Scene => new(this, ScriptTrace.TryCreateStackTrace(nameof(Scene)));
		public TimeApi Time => new(this, ScriptTrace.TryCreateStackTrace(nameof(Time)));
		public RigidbodyBuilder Rigidbody => new(this, ScriptTrace.TryCreateStackTrace(nameof(Rigidbody)));
		public TransformBuilder Transform => new(this, ScriptTrace.TryCreateStackTrace(nameof(Transform)));
		public WhenGlobalEventBuilder When => new(this, ScriptTrace.TryCreateStackTrace(nameof(When)));

		/// <summary>
		/// Creates a named coroutine.
		/// Usage: Coroutine("name").Duration(3).Seconds().OnUpdate(blocks).Elapsed(blocks);
		/// </summary>
		public CoroutineBuilder Coroutine(String name) => new(this, name, ScriptTrace.TryCreateStackTrace(nameof(Coroutine)));

		/// <summary>
		/// Conditional execution: If(conditions).Then(blocks).ElseIf(conditions).Then(blocks).Else(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		public IfBlockBuilder If(params ConditionBlock[] conditions) => new(this, conditions, ScriptTrace.TryCreateStackTrace(nameof(If)));

		/// <summary>
		/// Loop execution: While(conditions).Do(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		public WhileBlockBuilder While(params ConditionBlock[] conditions) => new(conditions, ScriptTrace.TryCreateStackTrace(nameof(While)));

		/// <summary>
		/// For loop (1-based index): For(numberOfTimes).Do(blocks);
		/// Starts at 1 and increments by 1 until limit is reached (inclusive).
		/// </summary>
		public ForBlockBuilder For(VariableBlock numberOfTimes) => new(numberOfTimes, ScriptTrace.TryCreateStackTrace(nameof(For)));

		/// <summary>
		/// For loop (1-based index): For(limit, step).Do(blocks);
		/// If step is positive: starts at 1 and increments by step until limit is reached.
		/// If step is negative: starts at limit and decrements by step until 1 is reached.
		/// </summary>
		public ForBlockBuilder For(VariableBlock numberOfTimes, VariableBlock step) =>
			new(numberOfTimes, step, ScriptTrace.TryCreateStackTrace(nameof(For)));

		/// <summary>
		/// Executes a `System.Func&lt;IScriptRuntimeContext, bool&gt;` (lambda) or method taking a IScriptRuntimeContext parameter and returns bool.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Check" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="func"></param>
		/// <returns></returns>
		public ConditionBlock Check(Func<IScriptRuntimeContext, Boolean> func) =>
			CheckBlock.Create(null, func, ScriptTrace.TryCreateStackTrace(nameof(Check)));

		/// <summary>
		/// Executes a `System.Func&lt;IScriptRuntimeContext, bool&gt;` (lambda) or method taking a IScriptRuntimeContext parameter and returns bool.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Check" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="func"></param>
		/// <returns></returns>
		public ConditionBlock Check(String blockName, Func<IScriptRuntimeContext, Boolean> func) =>
			CheckBlock.Create(blockName, func, ScriptTrace.TryCreateStackTrace(nameof(Check)));

		/// <summary>
		/// Executes a `System.Func&lt;bool&gt;` (lambda) or parameterless method returning bool.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Check" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="func"></param>
		/// <returns></returns>
		public ConditionBlock Check(Func<Boolean> func) => CheckBlock.Create(null, _ => func(), ScriptTrace.TryCreateStackTrace(nameof(Check)));

		/// <summary>
		/// Executes a `System.Func&lt;bool&gt;` (lambda) or parameterless method returning bool.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Check" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="func"></param>
		/// <returns></returns>
		public ConditionBlock Check(String blockName, Func<Boolean> func) =>
			CheckBlock.Create(blockName, _ => func(), ScriptTrace.TryCreateStackTrace(nameof(Check)));

		/// <summary>
		/// Executes a `System.Action` (lambda) or parameterless method returning void.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		///
		/// ```
		///	// A lambda adds notable 'syntax noise':
		/// On.Ready(Run(() => LunyLogger.LogInfo("custom log inline")));
		///
		///	// Multi-line lambdas are even worse:
		///	On.Ready(Run(() => {
		///	    LunyLogger.LogInfo("custom log inline");
		///	}));
		///
		///	// A named method is much cleaner, and re-usable:
		///	On.Ready(Run(MyCustomLog));
		///
		///	internal static void MyCustomLog() {
		///	    LunyLogger.LogInfo("custom log");
		///	}
		/// ```
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		public ActionBlock Run(Action action) => RunBlock.Create(null, _ => action(), ScriptTrace.TryCreateStackTrace(nameof(Run)));

		/// <summary>
		/// Executes a `System.Action` (lambda) or parameterless method returning void.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		///
		/// ```
		///	// A lambda adds notable 'syntax noise':
		/// On.Ready(Run(() => LunyLogger.LogInfo("custom log inline")));
		///
		///	// Multi-line lambdas are even worse:
		///	On.Ready(Run(() => {
		///	    LunyLogger.LogInfo("custom log inline");
		///	}));
		///
		///	// A named method is much cleaner, and re-usable:
		///	On.Ready(Run(MyCustomLog));
		///
		///	internal static void MyCustomLog() {
		///	    LunyLogger.LogInfo("custom log");
		///	}
		/// ```
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		public ActionBlock Run(String blockName, Action action) =>
			RunBlock.Create(blockName, _ => action(), ScriptTrace.TryCreateStackTrace(nameof(Run)));

		/// <summary>
		/// Executes a `System.Action` (lambda) or a method that takes a IScriptRuntimeContext parameter and returns void.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		public ActionBlock Run(Action<IScriptRuntimeContext> action) =>
			RunBlock.Create(null, action, ScriptTrace.TryCreateStackTrace(nameof(Run)));

		/// <summary>
		/// Executes a `System.Action` (lambda) or a method that takes a IScriptRuntimeContext parameter and returns void.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		public ActionBlock Run(String blockName, Action<IScriptRuntimeContext> action) =>
			RunBlock.Create(blockName, action, ScriptTrace.TryCreateStackTrace(nameof(Run)));

		/*
		/// <summary>
		/// Logical AND: Returns true if all conditions are true. Requires at least two conditions.
		/// </summary>
		public ConditionBlock AND(params ConditionBlock[] conditions) =>
			AndOperatorBlock.Create(conditions, ScriptTrace.TryCreateStackTrace(nameof(AND)));

		/// <summary>
		/// Logical OR: Returns true if at least one condition is true. Requires at least two conditions.
		/// </summary>
		public ConditionBlock OR(params ConditionBlock[] conditions) =>
			OrOperatorBlock.Create(conditions, ScriptTrace.TryCreateStackTrace(nameof(OR)));
			*/

		/// <summary>
		/// Does nothing, just prints a 'note' (comment, hint, etc) in the Block Inspector.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public ActionBlock Note(String message) => RunBlock.Create(message, _ => {}, ScriptTrace.TryCreateStackTrace("// "));
	}
}
