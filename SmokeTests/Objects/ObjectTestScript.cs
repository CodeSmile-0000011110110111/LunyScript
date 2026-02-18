using LunyScript.Api;
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
			On.Created(Object.Create(DestroyedObjectName).Do());
			On.AfterFrameUpdate(Object.Destroy(DestroyedObjectName));

			On.Ready(Object.Create(EmptyObjectName).Do());
			On.Ready(Object.Create(CubeObjectName).AsCube().Do());
			On.Ready(Object.Create(SphereObjectName).AsSphere().Do());
			On.Ready(Prefab.Instantiate("TestPrefab"));
		}
	}
}
