namespace LunyScript
{
	public sealed class ApiPlaceholders
	{
		public readonly struct AnimationApi
		{
			private readonly Script _script;
			internal AnimationApi(Script script) => _script = script;
		}

		public readonly struct ApplicationApi
		{
			private readonly Script _script;
			internal ApplicationApi(Script script) => _script = script;
		}

		public readonly struct AssetApi
		{
			private readonly Script _script;
			internal AssetApi(Script script) => _script = script;
		}

		public readonly struct AudioApi
		{
			private readonly Script _script;
			public AudioApi(Script script) => _script = script;
		}

		public readonly struct CameraApi
		{
			private readonly Script _script;
			internal CameraApi(Script script) => _script = script;
		}

		public readonly struct DiagnosticsApi
		{
			private readonly Script _script;
			internal DiagnosticsApi(Script script) => _script = script;
		}

		public readonly struct EngineApi
		{
			private readonly Script _script;
			internal EngineApi(Script script) => _script = script;

			/// <summary>
			/// Logs a message that appears in both debug and release builds.
			/// Posts to both Luny internal log (if enabled) and engine logging.
			/// </summary>
			// public ScriptActionBlock Log(String message) => EngineLogBlock.Create(message);
		}

		public readonly struct HUDApi
		{
			private readonly Script _script;
			internal HUDApi(Script script) => _script = script;
		}

		/*
		public readonly struct LoopApi
		{
			private readonly Script _script;
			internal LoopApi(Script script) => _script = script;

			/// <summary>
			/// Returns the current iteration count of the innermost surrounding loop.
			/// Resolves at runtime via ILunyScriptContext.
			/// </summary>
			public VariableBlock Counter => LoopCounterVariableBlock.Instance;
		}
	*/
		public readonly struct MenuApi
		{
			private readonly Script _script;
			internal MenuApi(Script script) => _script = script;
		}

		public readonly struct PhysicsApi
		{
			private readonly Script _script;
			internal PhysicsApi(Script script) => _script = script;
		}

		public readonly struct PlayerApi
		{
			private readonly Script _script;
			internal PlayerApi(Script script) => _script = script;
		}

		public readonly struct StorageApi
		{
			private readonly Script _script;
			internal StorageApi(Script script) => _script = script;
		}

		public readonly struct AccessibilityApi
		{
			private readonly Script _script;
			internal AccessibilityApi(Script script) => _script = script;
		}

		public readonly struct AccountApi
		{
			private readonly Script _script;
			internal AccountApi(Script script) => _script = script;
		}

		public readonly struct AIApi
		{
			private readonly Script _script;
			internal AIApi(Script script) => _script = script;
		}

		public readonly struct AsyncApi
		{
			private readonly Script _script;
			internal AsyncApi(Script script) => _script = script;
		}

		public readonly struct AvatarApi
		{
			private readonly Script _script;
			internal AvatarApi(Script script) => _script = script;
		}

		public readonly struct CloudApi
		{
			private readonly Script _script;
			internal CloudApi(Script script) => _script = script;
		}

		public readonly struct CutsceneApi
		{
			private readonly Script _script;
			internal CutsceneApi(Script script) => _script = script;
		}

		public readonly struct EnvironmentApi
		{
			private readonly Script _script;
			internal EnvironmentApi(Script script) => _script = script;
		}

		public readonly struct GraphicsApi
		{
			private readonly Script _script;
			internal GraphicsApi(Script script) => _script = script;
		}

		public readonly struct L18nApi
		{
			private readonly Script _script;
			internal L18nApi(Script script) => _script = script;
		}

		public readonly struct LocaleApi
		{
			private readonly Script _script;
			internal LocaleApi(Script script) => _script = script;
		}

		public readonly struct LocalizationApi
		{
			private readonly Script _script;
			internal LocalizationApi(Script script) => _script = script;
		}

		public readonly struct NavigationApi
		{
			private readonly Script _script;
			internal NavigationApi(Script script) => _script = script;
		}

		public readonly struct NetworkApi
		{
			private readonly Script _script;
			internal NetworkApi(Script script) => _script = script;
		}

		public readonly struct NPCApi
		{
			private readonly Script _script;
			internal NPCApi(Script script) => _script = script;
		}

		public readonly struct ParticlesApi
		{
			private readonly Script _script;
			internal ParticlesApi(Script script) => _script = script;
		}

		public readonly struct PoolApi
		{
			private readonly Script _script;
			internal PoolApi(Script script) => _script = script;
		}

		public readonly struct PostFxApi
		{
			private readonly Script _script;
			internal PostFxApi(Script script) => _script = script;
		}

		public readonly struct ProgressApi
		{
			private readonly Script _script;
			internal ProgressApi(Script script) => _script = script;
		}

		public readonly struct QualityApi
		{
			private readonly Script _script;
			internal QualityApi(Script script) => _script = script;
		}

		public readonly struct ScriptApi
		{
			private readonly Script _script;
			internal ScriptApi(Script script) => _script = script;
		}

		public readonly struct SessionApi
		{
			private readonly Script _script;
			internal SessionApi(Script script) => _script = script;
		}

		public readonly struct SettingsApi
		{
			private readonly Script _script;
			internal SettingsApi(Script script) => _script = script;
		}

		public readonly struct SpawnApi
		{
			private readonly Script _script;
			internal SpawnApi(Script script) => _script = script;
		}

		public readonly struct SpriteApi
		{
			private readonly Script _script;
			internal SpriteApi(Script script) => _script = script;
		}

		public readonly struct StageApi
		{
			private readonly Script _script;
			internal StageApi(Script script) => _script = script;
		}

		public readonly struct TerrainApi
		{
			private readonly Script _script;
			internal TerrainApi(Script script) => _script = script;
		}

		public readonly struct TilemapApi
		{
			private readonly Script _script;
			internal TilemapApi(Script script) => _script = script;
		}

		public readonly struct TutorialApi
		{
			private readonly Script _script;
			internal TutorialApi(Script script) => _script = script;
		}

		public readonly struct UIApi
		{
			private readonly Script _script;
			internal UIApi(Script script) => _script = script;
		}

		public readonly struct VFXApi
		{
			private readonly Script _script;
			internal VFXApi(Script script) => _script = script;
		}

		public readonly struct VideoApi
		{
			private readonly Script _script;
			internal VideoApi(Script script) => _script = script;
		}

		public readonly struct PlatformApi
		{
			private readonly Script _script;
			internal PlatformApi(Script script) => _script = script;

			public DesktopApi Desktop => new(_script);

			public readonly struct DesktopApi
			{
				private readonly Script _script;
				internal DesktopApi(Script script) => _script = script;
			}

			public LinuxApi Linux => new(_script);

			public readonly struct LinuxApi
			{
				private readonly Script _script;
				internal LinuxApi(Script script) => _script = script;
			}

			public MobileApi Mobile => new(_script);

			public readonly struct MobileApi
			{
				private readonly Script _script;
				internal MobileApi(Script script) => _script = script;
			}

			public OSXApi OSX => new(_script);

			public readonly struct OSXApi
			{
				private readonly Script _script;
				internal OSXApi(Script script) => _script = script;
			}

			public WebApi Web => new(_script);

			public readonly struct WebApi
			{
				private readonly Script _script;
				internal WebApi(Script script) => _script = script;
			}

			public WindowsApi Windows => new(_script);

			public readonly struct WindowsApi
			{
				private readonly Script _script;
				internal WindowsApi(Script script) => _script = script;
			}

			public XRApi XR => new(_script);

			public readonly struct XRApi
			{
				private readonly Script _script;
				internal XRApi(Script script) => _script = script;
			}
		}

		public readonly struct StoreApi
		{
			private readonly Script _script;
			internal StoreApi(Script script) => _script = script;

			public AppleApi Apple => new(_script);

			public readonly struct AppleApi
			{
				private readonly Script _script;
				internal AppleApi(Script script) => _script = script;
			}

			public EpicApi Epic => new(_script);

			public readonly struct EpicApi
			{
				private readonly Script _script;
				internal EpicApi(Script script) => _script = script;
			}

			public GoogleApi Google => new(_script);

			public readonly struct GoogleApi
			{
				private readonly Script _script;
				internal GoogleApi(Script script) => _script = script;
			}

			public SteamApi Steam => new(_script);

			public readonly struct SteamApi
			{
				private readonly Script _script;
				internal SteamApi(Script script) => _script = script;
			}
		}
	}
}
