using LunyScript.Blocks;
using System;

namespace LunyScript.Api.Blocks
{
	public readonly struct MethodApi
	{
		private readonly Script _script;
		internal MethodApi(Script script) => _script = script;

		/// <summary>
		/// Executes Action (or: method).
		/// </summary>
		/// <remarks>
		/// Intended for quick prototyping and testing. Prefer to build your own IScriptBlock based API.
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		public ScriptActionBlock Run(Action<IScriptRuntimeContext> action) => ExecuteBlock.Create(action);

		/// <summary>
		/// Executes Action (or: method).
		/// </summary>
		/// <remarks>
		/// Intended for quick prototyping and testing. Prefer to build your own IScriptBlock based API.
		/// </remarks>
		/// <remarks>
		/// Prefer to convert "Run" code into a custom IBlock class after its initial development and testing,
		/// If not that, use named methods rather than lambdas - this ensures the block-based code continues to read like intent.
		///
		///		// Even a single-line lambda adds notable 'syntax noise' (worse for multi-line lambdas):
		/// 	OnUpdate(Run(() => LunyLogger.LogInfo("custom log inline")));
		///
		///		// A named method is much cleaner, and re-usable in the same script:
		///		OnUpdate(Run(MyLog));
		///		private Action MyLog() => () => LunyLogger.LogInfo("custom log method");
		///
		///		// C# extension methods for LunyScript also work nice but require the 'this' prefix:
		///		OnUpdate(Run(this.MyLog()));
		///		public static MyLunyScriptExtensions
		///		{
		///			public static Action MyLog(this LunyScript ls) =>
		///				() => LunyLogger.LogInfo("custom log ext method");
		///		}
		///
		///		// Best: Create your own static factory class returning IBlock instances.
		///		// The block's Execute method has access to the context (object reference, variables, etc.).
		///		// The static Create() method helps to later adapt the creation code without having to modify callers.
		///		OnUpdate(MyBlocks.MyLog());
		///		internal static MyBlocks
		///		{
		///			public static ILunyScriptBlock MyLog() => MyLogBlock.Create();
		///			internal sealed class MyLogBlock : ILunyScriptBlock
		///			{
		///				public static ILunyScriptBlock Create() => new MyLogBlock();
		///				public void Execute(ILunyScriptContext context) => LunyLogger.LogInfo("custom log block");
		///			}
		///		}
		/// </remarks>
		/// <param name="action"></param>
		/// <returns></returns>
		public ScriptActionBlock Run(Action action) => ExecuteBlock.Create(_ => action());

		/// <summary>
		/// Condition block that runs a Func (or: method) returning bool.
		/// </summary>
		/// <remarks>
		/// Intended for quick prototyping and testing. Prefer to build your own IScriptBlock based API.
		/// </remarks>
		/// <param name="func"></param>
		/// <returns></returns>
		public ScriptConditionBlock IsTrue(Func<IScriptRuntimeContext, Boolean> func) => EvaluateBlock.Create(func);

		/// <summary>
		/// Condition block that runs a Func (or: method) returning bool.
		/// </summary>
		/// <remarks>
		/// Intended for quick prototyping and testing. Prefer to build your own IScriptBlock based API.
		/// </remarks>
		/// <param name="func"></param>
		/// <returns></returns>
		public ScriptConditionBlock IsTrue(Func<Boolean> func) => EvaluateBlock.Create(_ => func());
	}
}
