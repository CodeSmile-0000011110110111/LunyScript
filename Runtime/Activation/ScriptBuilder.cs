using Luny;
using Luny.Engine.Bridge;
using LunyScript.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace LunyScript.Activation
{
	/// <summary>
	/// Scans scenes at runtime to discover objects that should run LunyScripts.
	/// Binds scripts to objects based on name matching (exact, case-sensitive).
	/// </summary>
	internal static class ScriptBuilder
	{
		public static void BuildAndActivateLunyScripts(LunyScriptRunner runner, IEnumerable<ILunyObject> lunyObjects)
		{
			var sw = Stopwatch.StartNew();

			var activatedCount = 0;
			var buildContext = new ScriptContext();
			var runtimeContexts = CreateRuntimeContexts(lunyObjects, runner.Scripts, runner.Contexts);
			foreach (var runtimeContext in runtimeContexts)
			{
				BuildAndRegisterLunyScript(buildContext, runtimeContext, runner);
				activatedCount++;
			}

			ActivateScripts(runtimeContexts);

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Built {activatedCount} script(s) in {ms} ms", nameof(ScriptBuilder));
		}

		public static void BuildAndActivateLunyScript(LunyScriptRunner runner, ILunyObject lunyObject)
		{
			//LunyLogger.LogInfo($"{lunyObject} Activating Script ...", nameof(ScriptBuilder));

			var buildContext = new ScriptContext();
			var runtimeContext = TryCreateRuntimeContext(runner.Scripts, runner.Contexts, lunyObject);
			if (runtimeContext != null)
			{
				BuildAndRegisterLunyScript(buildContext, runtimeContext, runner);
				runtimeContext.Activate();
			}
		}

		private static void BuildAndRegisterLunyScript(ScriptContext scriptContext, ScriptRuntimeContext runtimeContext, LunyScriptRunner runner)
		{
			try
			{
				LunyLogger.LogInfo($"Building {runtimeContext} ...", nameof(ScriptBuilder));

				// Create script instance, initialize with context, and call Build()
				var scriptInstance = (Script)Activator.CreateInstance(runtimeContext.ScriptType);
				scriptInstance.Initialize(runtimeContext);
				scriptInstance.Build(scriptContext);
				scriptInstance.Shutdown();

				runner.InvokeOnScriptBuilt(runtimeContext);

				// hook up events
				runner.ObjectEventHandler.Register(runtimeContext);
				runner.SceneEventHandler.Register(runtimeContext);
			}
			catch (Exception ex)
			{
				LunyLogger.LogException(ex, nameof(ScriptBuilder));
				throw;
			}
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
		private static IReadOnlyList<ScriptRuntimeContext> CreateRuntimeContexts(IEnumerable<ILunyObject> lunyObjects,
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
			                   $"{lunyObjects.Count()} {nameof(LunyObject)}s.", nameof(ScriptBuilder));

			return createdContexts;
		}

		private static ScriptRuntimeContext TryCreateRuntimeContext(ScriptDefinitionRegistry scripts, ScriptRuntimeContextRegistry contexts,
			ILunyObject lunyObject)
		{
			if (lunyObject == null || !lunyObject.IsValid)
				return null;

			// Check if we have a script matching this object's name
			var scriptDef = scripts.GetByName(lunyObject.Name);
			if (scriptDef == null)
				return null;

			// Create ScriptContext for this object-script pair
			return contexts.CreateContext(scriptDef, lunyObject);
		}
	}
}
