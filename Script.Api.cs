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

		/// <summary>
		/// True if the script currently runs within the engine's editor (play mode). False in builds.
		/// </summary>
		protected Boolean IsEditor => LunyEngine.Instance.Application.IsEditor;

		/// <summary>
		/// Global variables
		/// </summary>
		protected VarAccessor GVar => _globalVariables;
		/// <summary>
		/// Instance variables (unique per script/object)
		/// </summary>
		protected VarAccessor Var => _instanceVariables;

		protected ComponentApi Component => new(this);
		protected DebugApi Debug => new(this);
		protected EditorApi Editor => new(this);
		protected InputBuilder Input => new(this);
		protected ObjectBuilder Object => new(this);
		protected OnObjectEventBuilder On => new(this);
		protected PrefabBuilder Prefab => new(this);
		protected SceneApi Scene => new(this);
		protected TimeApi Time => new(this);
		protected TransformBuilder Transform => new(this);
		protected WhenEventBuilder When => new(this);

		/// <summary>
		/// Creates a named coroutine.
		/// Usage: Coroutine("name").Duration(3).Seconds().OnUpdate(blocks).Elapsed(blocks);
		/// </summary>
		protected CoroutineBuilder Coroutine(String name) => new(this, name);

		/// <summary>
		/// Creates a named counter.
		/// Usage: Counter("name").In(5).Frames().Do(blocks);
		/// </summary>
		protected CoroutineCounterBuilder<CoroutineCounterBuilderStart> Counter(String name) => CoroutineCounterBuilder<CoroutineCounterBuilderStart>.Create(this, name);

		/// <summary>
		/// Time-sliced execution: Every(n).Frames().Do(blocks) or Every(n).Heartbeats().Do(blocks).
		/// Supports optional phase offset: Every(n).Frames().DelayBy(offset).Do(blocks).
		/// Use Even or Odd constants for alternating execution.
		/// </summary>
		protected CoroutineEveryBuilder<CoroutineEveryBuilderStart> Every(Int32 interval) => CoroutineEveryBuilder<CoroutineEveryBuilderStart>.Create(this, interval);

		/// <summary>
		/// Creates a named timer.
		/// Usage: Timer("name").In(3).Seconds().Do(blocks);
		/// </summary>
		protected CoroutineTimerBuilder<CoroutineTimerBuilderStart> Timer(String name) => CoroutineTimerBuilder<CoroutineTimerBuilderStart>.Create(this, name);

		/// <summary>
		/// Defines a named, read-only variable.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected VariableBlock Define(String name, Variable value) =>
			TableVariableBlock.Create(_runtimeContext.GlobalVariables.DefineConstant(name, value));

		/// <summary>
		/// Conditional execution: If(conditions).Then(blocks).ElseIf(conditions).Then(blocks).Else(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		protected IfBlockBuilder If(params ScriptConditionBlock[] conditions) => new(conditions);

		/// <summary>
		/// Loop execution: While(conditions).Do(blocks);
		/// Multiple conditions are implicitly AND combined.
		/// </summary>
		protected WhileBlockBuilder While(params ScriptConditionBlock[] conditions) => new(conditions);

		/// <summary>
		/// For loop (1-based index): For(numberOfTimes).Do(blocks);
		/// Starts at 1 and increments by 1 until limit is reached (inclusive).
		/// </summary>
		protected ForBlockBuilder For(Int32 numberOfTimes) => new(numberOfTimes);

		/// <summary>
		/// For loop (1-based index): For(limit, step).Do(blocks);
		/// If step > 0: starts at 1 and increments by step until limit is reached.
		/// If step < 0: starts at limit and decrements by step until 1 is reached.
		/// </summary>
		protected ForBlockBuilder For(Int32 numberOfTimes, Int32 step) => new(numberOfTimes, step);

		/// <summary>
		/// Executes a System.Func<bool> (lambda) or parameterless method returning bool.
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
		/// Executes a System.Func<IScriptRuntimeContext, bool> (lambda) or method taking a IScriptRuntimeContext parameter and returns bool.
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
		protected ScriptConditionBlock AND(params ScriptConditionBlock[] conditions) => AndBlock.Create(conditions);

		/// <summary>
		/// Logical OR: Returns true if at least one condition is true.
		/// </summary>
		protected ScriptConditionBlock OR(params ScriptConditionBlock[] conditions) => OrBlock.Create(conditions);

		/// <summary>
		/// Logical NOT: Returns the inverse of the condition.
		/// </summary>
		protected ScriptConditionBlock NOT(ScriptConditionBlock condition) => NotBlock.Create(condition);
	}
}
