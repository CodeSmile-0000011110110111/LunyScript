using Luny;
using Luny.Engine.Bridge;
using System;

namespace LunyScript
{
	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public interface IScriptEngine
	{
		ITable GlobalVariables { get; }
		IScriptRuntimeContext GetScriptContext(LunyNativeObjectId lunyNativeObjectID);
	}

	internal interface IScriptEngineInternal
	{
		event Action<ScriptRuntimeContext> OnScriptBuilt;
	}

	/// <summary>
	/// Public interface for LunyScript
	/// </summary>
	public sealed class ScriptEngine : IScriptEngine, IScriptEngineInternal
	{
		public static event Action<IScriptEngine> OnScriptEngineInitialized;
		private LunyScriptRunner _runner;

		public static IScriptEngine Instance { get; private set; }
		public ITable GlobalVariables => ScriptRuntimeContext.GetGlobalVariables();

		/// <summary>
		/// Maximum allowed iterations for While/For loops to prevent engine hangs.
		/// Only active in DEBUG or UNITY_EDITOR builds.
		/// </summary>
		public static Int32 MaxLoopIterations => Int16.MaxValue;

		internal static void ForceReset_UnitTestsOnly()
		{
			Instance = null;
			ScriptRuntimeContext.ClearGlobalVariables();
		}

		private ScriptEngine() {} // hide default ctor

		internal ScriptEngine(LunyScriptRunner scriptRunner)
		{
			LunyTraceLogger.LogInfoCreateSingletonInstance(typeof(ScriptEngine));

			if (Instance != null)
				throw new InvalidOperationException($"{nameof(IScriptEngine)} singleton duplication!");
			if (scriptRunner == null)
				throw new ArgumentNullException(nameof(scriptRunner));

			_runner = scriptRunner;
			Instance = this;

			OnScriptEngineInitialized?.Invoke(Instance);
			OnScriptEngineInitialized = null;
		}

		public IScriptRuntimeContext GetScriptContext(LunyNativeObjectId lunyNativeObjectID) =>
			_runner.Contexts.GetByNativeObjectID(lunyNativeObjectID);

		event Action<ScriptRuntimeContext> IScriptEngineInternal.OnScriptBuilt
		{
			add => OnScriptBuilt += value;
			remove => OnScriptBuilt -= value;
		}
		private event Action<ScriptRuntimeContext> OnScriptBuilt;

		~ScriptEngine() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Shutdown()
		{
			LunyTraceLogger.LogInfoShuttingDown(this);
			Instance = null;
			_runner = null;
			GC.SuppressFinalize(this);
			LunyTraceLogger.LogInfoShutdownComplete(this);
		}

		internal void InvokeOnScriptBuilt(ScriptRuntimeContext runtimeContext) => OnScriptBuilt?.Invoke(runtimeContext);
	}
}
