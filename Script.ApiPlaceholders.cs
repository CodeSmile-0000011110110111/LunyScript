namespace LunyScript
{
	public abstract partial class Script
	{
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
	}
}
