using LunyScript.BlockBuilders;
using System;

namespace LunyScript.SmokeTests.Objects
{
	public sealed class ObjectTestScript : Script
	{
		public const String DestroyedObjectName = "destroyed";
		public const String EmptyObjectName = "empty";
		public const String CubeObjectName = "cube";
		public const String SphereObjectName = "sphere";

		public override void Build(ScriptContext context)
		{
			On.Created(Object.Create(DestroyedObjectName));
			On.AfterFrameUpdate(Object.Destroy(DestroyedObjectName));

			On.Ready(Object.Create(EmptyObjectName));
			On.Ready(Object.Create(CubeObjectName).AsCube());
			On.Ready(Object.Create(SphereObjectName).AsSphere());
			On.Ready(Prefab.Instantiate("TestPrefab"));
		}
	}
}
