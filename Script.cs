using Luny;
using Luny.Engine.Bridge;
using LunyScript.ApiBuilders;
using LunyScript.ApiBuilders.Blocks;
using LunyScript.ApiBuilders.Coroutine;
using LunyScript.ApiBuilders.Coroutine.Counter;
using LunyScript.ApiBuilders.Coroutine.Every;
using LunyScript.ApiBuilders.Coroutine.Timer;
using LunyScript.ApiBuilders.Event;
using LunyScript.ApiBuilders.Input;
using LunyScript.ApiBuilders.Object;
using LunyScript.ApiBuilders.Transform;
using LunyScript.Blocks;
using LunyScript.Events;
using LunyScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace LunyScript
{
	/// <summary>
	/// Abstract base class for all LunyScripts.
	/// Provides the API interface for beginner-friendly visual scripting in C#.
	/// Users inherit from this class and implement Build() to construct their script logic.
	/// </summary>
	/// <remarks>
	/// Example script template (duplicate LunyScript.LunyScript is correct):
	///
	///		public class ExampleLunyScript : LunyScript.LunyScript
	///		{
	///			public override void Build()
	///			{
	///				// define behaviour using LunyScript API here ...
	///				OnUpdate(Debug.Log("Hello, LunyScript!"));
	///			}
	///		}
	/// </remarks>
	public abstract class Script
	{
		private IScriptRuntimeContext _runtimeContext;
		private VarAccessor _gVar;
		private VarAccessor _var;
		private List<BuilderToken> _pendingBuilders;
		private HashSet<BuilderToken> _finalizedBuilders;

		/// <summary>
		/// ScriptID of the script for identification.
		/// </summary>
		protected ScriptDefID ScriptDefId => _runtimeContext.ScriptDefId;
		/// <summary>
		/// Reference to proxy for engine object.
		/// Caution: native engine reference could be null.
		/// Check EngineObject.IsValid before accessing.
		/// </summary>
		[MaybeNull] protected ILunyObject LunyObject => _runtimeContext.LunyObject;
		/// <summary>
		/// True if the script runs within the engine's editor (play mode). False in builds.
		/// </summary>
		protected Boolean IsEditor => LunyEngine.Instance.Application.IsEditor;

		internal ScriptEventScheduler Scheduler => _runtimeContext is ScriptRuntimeContext context ? context.Scheduler : null;
		internal ScriptRuntimeContext RuntimeContext => _runtimeContext as ScriptRuntimeContext;

		// implemented APIs
		protected ComponentApi Component => new(this);
		protected DebugApi Debug => new(this);
		protected EditorApi Editor => new(this);
		//protected EngineApi Engine => new(this);
		protected InputBuilder Input => new(this);
		//public LoopApi Loop => new(this);
		protected MethodApi Method => new(this);
		protected ObjectBuilder Object => new(this);
		protected OnEventBuilder On => new(this);
		protected PrefabBuilder Prefab => new(this);
		protected SceneApi Scene => new(this);
		protected TimeApi Time => new(this);
		protected TransformBuilder Transform => new(this);
		protected WhenEventBuilder When => new(this);

		// these API outlines exist to get a feel for the intellisense/autocompletion behaviour ...
		// planned API outline (consider this the TODO list)
		protected ApiPlaceholders.AnimationApi Animation => new(this);
		protected ApiPlaceholders.ApplicationApi Application => new(this);
		protected ApiPlaceholders.AssetApi Asset => new(this);
		protected ApiPlaceholders.AudioApi Audio => new(this);
		protected ApiPlaceholders.CameraApi Camera => new(this);
		protected ApiPlaceholders.DiagnosticsApi Diagnostics => new(this);
		protected ApiPlaceholders.HUDApi HUD => new(this);
		protected ApiPlaceholders.MenuApi Menu => new(this);
		protected ApiPlaceholders.PhysicsApi Physics => new(this);
		protected ApiPlaceholders.PlayerApi Player => new(this);
		protected ApiPlaceholders.StorageApi Storage => new(this);

		// possible future expansions
		protected ApiPlaceholders.AccessibilityApi Accessibility => new(this);
		protected ApiPlaceholders.AccountApi Account => new(this);
		protected ApiPlaceholders.AIApi AI => new(this);
		protected ApiPlaceholders.AsyncApi Async => new(this);
		protected ApiPlaceholders.AvatarApi Avatar => new(this);
		protected ApiPlaceholders.CloudApi Cloud => new(this);
		protected ApiPlaceholders.CutsceneApi Cutscene => new(this);
		protected ApiPlaceholders.EnvironmentApi Environment => new(this);
		protected ApiPlaceholders.GraphicsApi Graphics => new(this);
		protected ApiPlaceholders.L18nApi L18n => new(this);
		protected ApiPlaceholders.LocaleApi Locale => new(this);
		protected ApiPlaceholders.LocalizationApi Localization => new(this);
		protected ApiPlaceholders.NavigationApi Navigation => new(this);
		protected ApiPlaceholders.NetworkApi Network => new(this);
		protected ApiPlaceholders.NPCApi NPC => new(this);
		protected ApiPlaceholders.ParticlesApi Particles => new(this);
		protected ApiPlaceholders.PlatformApi Platform => new(this);
		protected ApiPlaceholders.PoolApi Pool => new(this);
		protected ApiPlaceholders.PostFxApi PostFx => new(this);
		protected ApiPlaceholders.ProgressApi Progress => new(this);
		protected ApiPlaceholders.QualityApi Quality => new(this);
		protected ApiPlaceholders.SessionApi Session => new(this);
		protected ApiPlaceholders.SettingsApi Settings => new(this);
		protected ApiPlaceholders.SpawnApi Spawn => new(this);
		protected ApiPlaceholders.SpriteApi Sprite => new(this);
		protected ApiPlaceholders.StageApi Stage => new(this);
		protected ApiPlaceholders.StoreApi Store => new(this);
		protected ApiPlaceholders.TerrainApi Terrain => new(this);
		protected ApiPlaceholders.TilemapApi Tilemap => new(this);
		protected ApiPlaceholders.TutorialApi Tutorial => new(this);
		protected ApiPlaceholders.UIApi UI => new(this);
		protected ApiPlaceholders.VFXApi VFX => new(this);
		protected ApiPlaceholders.VideoApi Video => new(this);

		protected VarAccessor GVar => _gVar;
		protected VarAccessor Var => _var;

		internal void Initialize(IScriptRuntimeContext runtimeContext)
		{
			_runtimeContext = runtimeContext ?? throw new ArgumentNullException(nameof(runtimeContext));
			_gVar = new VarAccessor(_runtimeContext.GlobalVariables);
			_var = new VarAccessor(_runtimeContext.LocalVariables);
		}

		internal BuilderToken CreateBuilderToken(String name, String type)
		{
			var frame = new StackFrame(3, true);
			var token = new BuilderToken(name, type, frame.GetFileName(), frame.GetFileLineNumber());

			_pendingBuilders ??= new List<BuilderToken>();
			_pendingBuilders.Add(token);
			return token;
		}

		internal void FinalizeBuilderToken(BuilderToken token)
		{
			token?.MarkFinished();

			_finalizedBuilders ??= new HashSet<BuilderToken>();
			_finalizedBuilders.Add(token);
		}

		// Variables and Constants
		protected VariableBlock Define(String name, Variable value) =>
			TableVariableBlock.Create(_runtimeContext.GlobalVariables.DefineConstant(name, value));

		// Logic Flow API

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
		/// For loop (1-based index): For(limit).Do(blocks);
		/// Starts at 1 and increments by 1 until limit is reached (inclusive).
		/// </summary>
		protected ForBlockBuilder For(Int32 limit) => new(limit);

		/// <summary>
		/// For loop (1-based index): For(limit, step).Do(blocks);
		/// If step > 0: starts at 1 and increments by step until limit is reached.
		/// If step < 0: starts at limit and decrements by step until 1 is reached.
		/// </summary>
		protected ForBlockBuilder For(Int32 limit, Int32 step) => new(limit, step);

		// Boolean Modifiers for Conditions

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

		// Coroutines & Timers

		/// <summary>
		/// Creates a named timer.
		/// Usage: Timer("name").In(3).Seconds().Do(blocks);
		/// </summary>
		protected TimerBuilder Timer(String name) => new(this, name);

		/// <summary>
		/// Creates a named counter.
		/// Usage: Counter("name").In(5).Frames().Do(blocks);
		/// </summary>
		protected CounterBuilder Counter(String name) => new(this, name);

		/// <summary>
		/// Creates a named coroutine.
		/// Usage: Coroutine("name").Duration(3).Seconds().OnUpdate(blocks).Elapsed(blocks);
		/// </summary>
		protected CoroutineBuilder Coroutine(String name) => new(this, name);

		/// <summary>
		/// Time-sliced execution: Every(n).Frames().Do(blocks) or Every(n).Heartbeats().Do(blocks).
		/// Supports optional phase offset: Every(n).Frames().DelayBy(offset).Do(blocks).
		/// Use Even or Odd constants for alternating execution.
		/// </summary>
		protected EveryBuilder Every(Int32 interval = 0) => new(this, interval);

		~Script() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Shutdown()
		{
			FinalizePending();
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Called once when the script is initialized.
		/// Users construct their blocks (sequences, statemachines, behaviors) for execution here.
		/// Users can use regular C# syntax (ie call methods, use loops) to construct complex and/or reusable blocks.
		/// </summary>
		/// <param name="context"></param>
		public abstract void Build(ScriptContext context);

		public override String ToString() => _runtimeContext != null ? _runtimeContext.ToString() : GetType().FullName;

		private void FinalizePending()
		{
			if (_pendingBuilders == null || _pendingBuilders.Count == 0)
				return;

			var unfinishedBuilders = new List<BuilderToken>();
			for (var i = 0; i < _pendingBuilders.Count; i++)
			{
				var token = _pendingBuilders[i];
				if (_finalizedBuilders != null && _finalizedBuilders.Contains(token))
					continue;

				if (!token.FinalizeBuilder())
					unfinishedBuilders.Add(token);
			}

			_pendingBuilders.Clear();
			_finalizedBuilders.Clear();

			foreach (var token in unfinishedBuilders)
			{
				BuilderToken.LogUnfinishedBuilder(token);
				token.MarkFinished();
			}

			if (unfinishedBuilders.Count > 0)
				throw new LunyScriptException($"{GetType().Name} script has unfinished Block Builder(s): see warning message(s) above.");
		}
	}
}
