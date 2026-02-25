using Luny;
using Luny.Engine.Bridge;
using LunyScript.Diagnostics;
using LunyScript.Events;
using System;
using System.Collections.Generic;

namespace LunyScript
{
	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered sequences.
	/// </summary>
	public interface IScriptRuntimeContext
	{
		ScriptDefID ScriptDefId { get; }
		Type ScriptType { get; }
		ILunyObject LunyObject { get; }
		ITable GlobalVariables { get; }
		ITable LocalVariables { get; }
		//Stack<Int32> LoopStack { get; }
		//Int32 LoopCount { get; }
	}

	internal interface IScriptContextInternal {}

	/// <summary>
	/// Runtime context for a LunyScript instance operating on a specific object.
	/// Contains the script metadata, object reference, variables, and registered sequences.
	/// </summary>
	public sealed class ScriptRuntimeContext : IScriptRuntimeContext, IScriptContextInternal
	{
		private static readonly ITable s_GlobalVariables = new Table();

		private readonly IScriptDefinition _scriptDef;
		private readonly ILunyObject _lunyObject;

		private ScriptEventScheduler _scheduler;
		private ScriptObjectCoroutineRunner _coroutines;
		private ScriptDebugHooks _debugHooks;
		private ScriptBlockProfiler _blockProfiler;
		private ITable _localVariables;
		//private Stack<Int32> _loopStack;

		/// <summary>
		/// The ID of the script definition this context executes.
		/// </summary>
		public ScriptDefID ScriptDefId => _scriptDef.ScriptDefId;
		/// <summary>
		/// The C# Type of the script (for hot reload matching).
		/// </summary>
		public Type ScriptType => _scriptDef.Type;
		/// <summary>
		/// The engine object/node this script operates on.
		/// </summary>
		public ILunyObject LunyObject => _lunyObject;
		/// <summary>
		/// Global variables shared across all scripts.
		/// </summary>
		public ITable GlobalVariables => s_GlobalVariables;
		/// <summary>
		/// Per-object variables for this script instance.
		/// </summary>
		public ITable LocalVariables => _localVariables ??= new Table();
		/// <summary>
		/// Stack for loop iteration counters.
		/// </summary>
		//public Stack<Int32> LoopStack => _loopStack ??= new Stack<Int32>();
		/// <summary>
		/// Current loop iteration count. Returns 0 outside of loops.
		/// </summary>
		//public Int32 LoopCount => LoopStack?.Count > 0 ? LoopStack.Peek() : 0;
		/// <summary>
		/// Debugging hooks for execution tracing and breakpoints.
		/// </summary>
		internal ScriptDebugHooks DebugHooks => _debugHooks ??= new ScriptDebugHooks();
		/// <summary>
		/// Block-level profiler for tracking blocks performance.
		/// </summary>
		internal ScriptBlockProfiler BlockProfiler => _blockProfiler ??= new ScriptBlockProfiler();
		/// <summary>
		/// Event scheduler for managing sequences across all event types.
		/// </summary>
		internal ScriptEventScheduler Scheduler => _scheduler ??= new ScriptEventScheduler();

		/// <summary>
		/// Coroutine runner for managing timers and coroutines.
		/// </summary>
		internal ScriptObjectCoroutineRunner Coroutines => _coroutines ??= new ScriptObjectCoroutineRunner(this);

		internal static void ClearGlobalVariables() => s_GlobalVariables?.RemoveAll();
		internal static ITable GetGlobalVariables() => s_GlobalVariables;

		public ScriptRuntimeContext(IScriptDefinition definition, ILunyObject lunyObject)
		{
			_scriptDef = definition ?? throw new ArgumentNullException(nameof(definition));
			_lunyObject = lunyObject ?? throw new ArgumentNullException(nameof(lunyObject));
			//LunyLogger.LogInfo($"new {this} ({GetHashCode()})", this);
		}

		internal void Activate() => _lunyObject.Initialize();

		internal void Shutdown()
		{
			_coroutines?.Shutdown();
			_scheduler?.Shutdown();
			GC.SuppressFinalize(this);
		}

		~ScriptRuntimeContext() => LunyTraceLogger.LogInfoFinalized(this);
		public override String ToString() => $"{ScriptDefId} -> {LunyObject}";
	}
}
