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
		/// Reference to proxy for engine object.
		/// </summary>
		[MaybeNull] protected ILunyObject Owner => _runtimeContext.LunyObject;

		public String Name => GetType().Name;

		/// <summary>
		/// True if the script currently runs within the engine's editor (play mode). False in builds.
		/// </summary>
		public Boolean IsEditor => LunyEngine.Instance.Application.IsEditor;

		/// <summary>
		/// Global variables
		/// </summary>
		public VarAccessor GVar => _globalVariables;
		/// <summary>
		/// Instance variables (unique per script/object)
		/// </summary>
		public VarAccessor Var => _instanceVariables;

		public ComponentApi Component => new(this);
		public DebugApi Debug => new(this);
		public EditorApi Editor => new(this);
		public InputBuilder Input => new(this);
		public ObjectBuilder Object => new(this);
		public OnObjectEventBuilder On => new(this);
		public PrefabBuilder Prefab => new(this);
		public SceneApi Scene => new(this);
		public TimeApi Time => new(this);
		public TransformBuilder Transform => new(this);
		public WhenGlobalEventBuilder When => new(this);

		/// <summary>
		/// Creates a named coroutine.
		/// Usage: Coroutine("name").Duration(3).Seconds().OnUpdate(blocks).Elapsed(blocks);
		/// </summary>
		public CoroutineBuilder Coroutine(String name) => new(this, name);

		/// <summary>
		/// Conditional execution: If(conditions).Then(blocks).ElseIf(conditions).Then(blocks).Else(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		public IfBlock If(params ScriptConditionBlock[] conditions) => new(this, conditions);

		/// <summary>
		/// Loop execution: While(conditions).Do(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		public WhileBlockBuilder While(params ScriptConditionBlock[] conditions) => new(conditions);

		/// <summary>
		/// For loop (1-based index): For(numberOfTimes).Do(blocks);
		/// Starts at 1 and increments by 1 until limit is reached (inclusive).
		/// </summary>
		public ForBlockBuilder For(Int32 numberOfTimes) => new(numberOfTimes);

		/// <summary>
		/// For loop (1-based index): For(limit, step).Do(blocks);
		/// If step is positive: starts at 1 and increments by step until limit is reached.
		/// If step is negative: starts at limit and decrements by step until 1 is reached.
		/// </summary>
		public ForBlockBuilder For(Int32 numberOfTimes, Int32 step) => new(numberOfTimes, step);

		/// <summary>
		/// Executes a <see cref="System.Func{bool}"/> (lambda) or parameterless method returning bool.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Check" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="func"></param>
		/// <returns></returns>
		public ScriptConditionBlock Check(Func<Boolean> func) => EvaluateBlock.Create(_ => func());

		/// <summary>
		/// Executes a <see cref="System.Func{IScriptRuntimeContext, bool}" /> (lambda) or method taking a IScriptRuntimeContext parameter and returns bool.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Check" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="func"></param>
		/// <returns></returns>
		public ScriptConditionBlock Check(Func<IScriptRuntimeContext, Boolean> func) => EvaluateBlock.Create(func);

		/// <summary>
		/// Executes a System.Action (lambda) or parameterless method returning void.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <remarks>
		///		// Even a single-line lambda adds notable 'syntax noise':
		/// 	On.Update(Run(() => LunyLogger.LogInfo("custom log inline")));
		///
		///		// Multi-line lambdas are even worse:
		///		On.Update(Run(() =>	{
		///			LunyLogger.LogInfo("custom log inline");
		///		}));
		///
		///		// A named method is much cleaner, and re-usable in the same script:
		///		OnUpdate(Run(MyCustomLog));
		///		private void MyCustomLog() => LunyLogger.LogInfo("custom log");
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		public ScriptActionBlock Run(Action action) => ExecuteBlock.Create(_ => action());

		/// <summary>
		/// Executes a System.Action (lambda) or a method that takes a IScriptRuntimeContext parameter and returns void.
		/// </summary>
		/// <remarks>
		/// - Intended for quick prototyping and testing.
		/// - Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// - Prefer named methods over lambdas to ensure the block-based code continues to read like intent.
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		public ScriptActionBlock Run(Action<IScriptRuntimeContext> action) => ExecuteBlock.Create(action);

		/// <summary>
		/// Logical AND: Returns true if all conditions are true.
		/// </summary>
		public ScriptConditionBlock AND(params ScriptConditionBlock[] conditions) => AndBlock.Create(conditions);

		/// <summary>
		/// Logical OR: Returns true if at least one condition is true.
		/// </summary>
		public ScriptConditionBlock OR(params ScriptConditionBlock[] conditions) => OrBlock.Create(conditions);

		/// <summary>
		/// Logical NOT: Returns the inverse of the condition.
		/// </summary>
		public ScriptConditionBlock NOT(ScriptConditionBlock condition) => NotBlock.Create(condition);
	}
}
