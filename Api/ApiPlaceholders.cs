namespace LunyScript.Api
{
	public sealed class ApiPlaceholders
	{
		public readonly struct AnimationApi
		{
			private readonly IScript _script;
			internal AnimationApi(IScript script) => _script = script;
		}

		public readonly struct ApplicationApi
		{
			private readonly IScript _script;
			internal ApplicationApi(IScript script) => _script = script;
		}

		public readonly struct AssetApi
		{
			private readonly IScript _script;
			internal AssetApi(IScript script) => _script = script;
		}

		public readonly struct AudioApi
		{
			private readonly IScript _script;
			public AudioApi(IScript script) => _script = script;
		}

		public readonly struct CameraApi
		{
			private readonly IScript _script;
			internal CameraApi(IScript script) => _script = script;
		}

		public readonly struct DiagnosticsApi
		{
			private readonly IScript _script;
			internal DiagnosticsApi(IScript script) => _script = script;
		}

		public readonly struct HUDApi
		{
			private readonly IScript _script;
			internal HUDApi(IScript script) => _script = script;
		}

		public readonly struct MenuApi
		{
			private readonly IScript _script;
			internal MenuApi(IScript script) => _script = script;
		}

		public readonly struct PhysicsApi
		{
			private readonly IScript _script;
			internal PhysicsApi(IScript script) => _script = script;
		}

		public readonly struct PlayerApi
		{
			private readonly IScript _script;
			internal PlayerApi(IScript script) => _script = script;
		}

		public readonly struct StorageApi
		{
			private readonly IScript _script;
			internal StorageApi(IScript script) => _script = script;
		}

		public readonly struct AccessibilityApi
		{
			private readonly IScript _script;
			internal AccessibilityApi(IScript script) => _script = script;
		}

		public readonly struct AccountApi
		{
			private readonly IScript _script;
			internal AccountApi(IScript script) => _script = script;
		}

		public readonly struct AIApi
		{
			private readonly IScript _script;
			internal AIApi(IScript script) => _script = script;
		}

		public readonly struct AsyncApi
		{
			private readonly IScript _script;
			internal AsyncApi(IScript script) => _script = script;
		}

		public readonly struct AvatarApi
		{
			private readonly IScript _script;
			internal AvatarApi(IScript script) => _script = script;
		}

		public readonly struct CloudApi
		{
			private readonly IScript _script;
			internal CloudApi(IScript script) => _script = script;
		}

		public readonly struct CutsceneApi
		{
			private readonly IScript _script;
			internal CutsceneApi(IScript script) => _script = script;
		}

		public readonly struct EnvironmentApi
		{
			private readonly IScript _script;
			internal EnvironmentApi(IScript script) => _script = script;
		}

		public readonly struct GraphicsApi
		{
			private readonly IScript _script;
			internal GraphicsApi(IScript script) => _script = script;
		}

		public readonly struct L18nApi
		{
			private readonly IScript _script;
			internal L18nApi(IScript script) => _script = script;
		}

		public readonly struct LocaleApi
		{
			private readonly IScript _script;
			internal LocaleApi(IScript script) => _script = script;
		}

		public readonly struct LocalizationApi
		{
			private readonly IScript _script;
			internal LocalizationApi(IScript script) => _script = script;
		}

		public readonly struct NavigationApi
		{
			private readonly IScript _script;
			internal NavigationApi(IScript script) => _script = script;
		}

		public readonly struct NetworkApi
		{
			private readonly IScript _script;
			internal NetworkApi(IScript script) => _script = script;
		}

		public readonly struct NPCApi
		{
			private readonly IScript _script;
			internal NPCApi(IScript script) => _script = script;
		}

		public readonly struct ParticlesApi
		{
			private readonly IScript _script;
			internal ParticlesApi(IScript script) => _script = script;
		}

		public readonly struct PoolApi
		{
			private readonly IScript _script;
			internal PoolApi(IScript script) => _script = script;
		}

		public readonly struct PostFxApi
		{
			private readonly IScript _script;
			internal PostFxApi(IScript script) => _script = script;
		}

		public readonly struct ProgressApi
		{
			private readonly IScript _script;
			internal ProgressApi(IScript script) => _script = script;
		}

		public readonly struct QualityApi
		{
			private readonly IScript _script;
			internal QualityApi(IScript script) => _script = script;
		}

		public readonly struct ScriptApi
		{
			private readonly IScript _script;
			internal ScriptApi(IScript script) => _script = script;
		}

		public readonly struct SessionApi
		{
			private readonly IScript _script;
			internal SessionApi(IScript script) => _script = script;
		}

		public readonly struct SettingsApi
		{
			private readonly IScript _script;
			internal SettingsApi(IScript script) => _script = script;
		}

		public readonly struct SpawnApi
		{
			private readonly IScript _script;
			internal SpawnApi(IScript script) => _script = script;
		}

		public readonly struct SpriteApi
		{
			private readonly IScript _script;
			internal SpriteApi(IScript script) => _script = script;
		}

		public readonly struct StageApi
		{
			private readonly IScript _script;
			internal StageApi(IScript script) => _script = script;
		}

		public readonly struct TerrainApi
		{
			private readonly IScript _script;
			internal TerrainApi(IScript script) => _script = script;
		}

		public readonly struct TilemapApi
		{
			private readonly IScript _script;
			internal TilemapApi(IScript script) => _script = script;
		}

		public readonly struct TutorialApi
		{
			private readonly IScript _script;
			internal TutorialApi(IScript script) => _script = script;
		}

		public readonly struct UIApi
		{
			private readonly IScript _script;
			internal UIApi(IScript script) => _script = script;
		}

		public readonly struct VFXApi
		{
			private readonly IScript _script;
			internal VFXApi(IScript script) => _script = script;
		}

		public readonly struct VideoApi
		{
			private readonly IScript _script;
			internal VideoApi(IScript script) => _script = script;
		}

		public readonly struct PlatformApi
		{
			private readonly IScript _script;
			internal PlatformApi(IScript script) => _script = script;

			public DesktopApi Desktop => new(_script);

			public readonly struct DesktopApi
			{
				private readonly IScript _script;
				internal DesktopApi(IScript script) => _script = script;
			}

			public LinuxApi Linux => new(_script);

			public readonly struct LinuxApi
			{
				private readonly IScript _script;
				internal LinuxApi(IScript script) => _script = script;
			}

			public MobileApi Mobile => new(_script);

			public readonly struct MobileApi
			{
				private readonly IScript _script;
				internal MobileApi(IScript script) => _script = script;
			}

			public OSXApi OSX => new(_script);

			public readonly struct OSXApi
			{
				private readonly IScript _script;
				internal OSXApi(IScript script) => _script = script;
			}

			public WebApi Web => new(_script);

			public readonly struct WebApi
			{
				private readonly IScript _script;
				internal WebApi(IScript script) => _script = script;
			}

			public WindowsApi Windows => new(_script);

			public readonly struct WindowsApi
			{
				private readonly IScript _script;
				internal WindowsApi(IScript script) => _script = script;
			}

			public XRApi XR => new(_script);

			public readonly struct XRApi
			{
				private readonly IScript _script;
				internal XRApi(IScript script) => _script = script;
			}
		}

		public readonly struct StoreApi
		{
			private readonly IScript _script;
			internal StoreApi(IScript script) => _script = script;

			public AppleApi Apple => new(_script);

			public readonly struct AppleApi
			{
				private readonly IScript _script;
				internal AppleApi(IScript script) => _script = script;
			}

			public EpicApi Epic => new(_script);

			public readonly struct EpicApi
			{
				private readonly IScript _script;
				internal EpicApi(IScript script) => _script = script;
			}

			public GoogleApi Google => new(_script);

			public readonly struct GoogleApi
			{
				private readonly IScript _script;
				internal GoogleApi(IScript script) => _script = script;
			}

			public SteamApi Steam => new(_script);

			public readonly struct SteamApi
			{
				private readonly IScript _script;
				internal SteamApi(IScript script) => _script = script;
			}
		}
	}
}
