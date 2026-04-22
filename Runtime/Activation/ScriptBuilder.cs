using Luny;
using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace LunyScript
{
	/// <summary>
	/// Scans scenes at runtime to discover objects that should run LunyScripts.
	/// Binds scripts to objects based on name matching (exact, case-sensitive).
	/// </summary>
	internal static class ScriptBuilder
	{
		public static void BuildAndActivateLunyScripts(LunyScriptRunner runner, IEnumerable<ILunyGameObject> lunyObjects)
		{
			var sw = Stopwatch.StartNew();

			var activatedCount = 0;
			var buildContext = new ScriptBuildContext();
			var runtimeContexts = CreateRuntimeContexts(lunyObjects, runner.Scripts, runner.Contexts);
			foreach (var runtimeContext in runtimeContexts.OrderBy(context => context.ScriptId))
			{
				BuildAndRegisterLunyScript(buildContext, runtimeContext, runner);
				activatedCount++;
			}

			ActivateScripts(runtimeContexts);

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Built {activatedCount} script(s) in {ms} ms", nameof(ScriptBuilder));
		}

		public static void BuildAndActivateLunyScript(LunyScriptRunner runner, ILunyGameObject lunyGameObject)
		{
			//LunyLogger.LogInfo($"{lunyObject} Activating Script ...", nameof(ScriptBuilder));

			var buildContext = new ScriptBuildContext();
			var runtimeContext = TryCreateRuntimeContext(runner.Scripts, runner.Contexts, lunyGameObject);
			if (runtimeContext != null)
			{
				BuildAndRegisterLunyScript(buildContext, runtimeContext, runner);
				runtimeContext.Activate();
			}
		}

		private static void BuildAndRegisterLunyScript(ScriptBuildContext buildContext, ScriptRuntimeContext runtimeContext,
			LunyScriptRunner runner)
		{
			LunyLogger.LogInfo($"Building {runtimeContext.ScriptType.Name} for {runtimeContext}", nameof(ScriptBuilder));

			// Create script instance, initialize with context, and call Build()
			var scriptInstance = (Script)Activator.CreateInstance(runtimeContext.ScriptType);

			runner.InvokeOnScriptInstantiated(buildContext, runtimeContext);

			scriptInstance.Initialize(buildContext, runtimeContext);
			scriptInstance.Build(buildContext);
			scriptInstance.Shutdown();

			runner.InvokeOnScriptBuilt(runtimeContext);

			// hook up events
			runner.ObjectEventHandler.Register(runtimeContext);
			runner.SceneEventHandler.Register(runtimeContext);
			runner.InputEventHandler.Register(runtimeContext);
		}

		private static void ActivateScripts(IEnumerable<ScriptRuntimeContext> contexts)
		{
			// sends initial OnCreate and (if enabled) OnEnable events
			foreach (var context in contexts)
				context.Activate();
		}

		/// <summary>
		/// Processes the current scene, finding objects and binding them to scripts.
		/// Creates run contexts for matching object-script pairs.
		/// </summary>
		private static IReadOnlyList<ScriptRuntimeContext> CreateRuntimeContexts(IEnumerable<ILunyGameObject> lunyObjects,
			ScriptDefinitionRegistry scripts, ScriptRuntimeContextRegistry contexts)
		{
			var createdContexts = new List<ScriptRuntimeContext>();
			foreach (var lunyObject in lunyObjects)
			{
				var context = TryCreateRuntimeContext(scripts, contexts, lunyObject);
				if (context != null)
					createdContexts.Add(context);
			}

			LunyLogger.LogInfo($"{createdContexts.Count} {nameof(ScriptRuntimeContext)}s created from " +
			                   $"{lunyObjects.Count()} {nameof(LunyGameObject)}s.", nameof(ScriptBuilder));

			return createdContexts;
		}

		private static ScriptRuntimeContext TryCreateRuntimeContext(ScriptDefinitionRegistry scripts, ScriptRuntimeContextRegistry contexts,
			ILunyGameObject lunyGameObject)
		{
			if (lunyGameObject == null || !lunyGameObject.IsValid)
				return null;

			// Check if we have a script matching this object's name
			var scriptDef = scripts.GetByName(lunyGameObject.Name);
			if (scriptDef == null)
				return null;

			// Create ScriptContext for this object-script pair
			return contexts.CreateContext(scriptDef, lunyGameObject);
		}
	}
}
