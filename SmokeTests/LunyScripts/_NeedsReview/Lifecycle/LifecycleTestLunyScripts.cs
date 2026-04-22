namespace LunyScript.SmokeTests
{
	public sealed class Assert_Runs_OnCreated : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			On.Created(Run(AssertDidRun));

			On.AfterFrameUpdate(
				//Debug.LogWarning("Reloading scene now ..."),
				//Scene.Reload()
			);
			//Every.TimeInterval(TimeSpan.FromSeconds(1));
		}
	}

	public sealed class Assert_Runs_OnDestroyed : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			On.Created(Object.Destroy());
			On.Destroyed(Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_OnEnabled : LunyScriptSmokeTestBase
	{
		public override void Build() => On.Enabled(Run(AssertDidRun));
	}

	public sealed class Assert_Runs_OnDisabled : LunyScriptSmokeTestBase
	{
		public override void Build()
		{
			On.Created(Object.Disable());
			On.Disabled(Run(AssertDidRun));
		}
	}

	public sealed class Assert_Runs_OnReady : LunyScriptSmokeTestBase
	{
		public override void Build() => On.Ready(Run(AssertDidRun));
	}

	public sealed class Assert_Runs_OnHeartbeat : LunyScriptSmokeTestBase
	{
		public override void Build() => On.Heartbeat(Run(AssertDidRun),
			Object.Destroy() // prevent log spam
		);
	}

	public sealed class Assert_Runs_OnFrameUpdate : LunyScriptSmokeTestBase
	{
		public override void Build() => On.FrameUpdate(Run(AssertDidRun),
			Object.Destroy() // prevent log spam
		);
	}

	public sealed class Assert_Runs_OnFrameLateUpdate : LunyScriptSmokeTestBase
	{
		public override void Build() => On.AfterFrameUpdate(Run(AssertDidRun),
			Object.Destroy() // prevent log spam
		);
	}
}
