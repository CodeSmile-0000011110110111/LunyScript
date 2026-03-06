using Luny.Engine.Bridge;
using System;

namespace LunyScript.SmokeTests.Object
{
	public sealed class ObjectCreateTestScript : Script
	{
		public const String DestroyedObjectName = "will be destroyed";

		public const String ParentName = "CreatedObjects";

		public override void Build(ScriptContext context)
		{
			var isDestroyed = Var.Define("isDestroyed", false);

			On.Created(Object.Create(DestroyedObjectName));
			On.AfterFrameUpdate(If(isDestroyed == false).Then(isDestroyed.Set(true), Object.Destroy(DestroyedObjectName)));

			On.Ready(Object.Create("Void").Parent(ParentName));
			On.Ready(Object.Create("CorneryThingy").AsCube().LocalPosition(new LunyVector3(1, 0, 0)).Parent(ParentName));
			On.Ready(Object.Create("Planetesimal").AsSphere().Parent(ParentName).LocalPosition(new LunyVector3(2, 0, 0)));
			On.Ready(Object.Create("FeaturelessBody").AsCapsule().Parent(ParentName).LocalPosition(new LunyVector3(3, 0, 0)));
			On.Ready(Object.Create("PipeDream").AsCylinder().Parent(ParentName).LocalPosition(new LunyVector3(4, 0, 0)));

			On.Ready(Object.Create("QuadQuadInTheWhat?")
				.AsQuad()
				.Parent(ParentName)
				.LocalPosition(new LunyVector3(5, 0, -1))
				.LocalRotation(new LunyVector3(30, 60, -30)));

			On.Ready(Object.Create("AsFlatAsCanBe")
				.AsPlane()
				.Parent(ParentName)
				.LocalPosition(new LunyVector3(2.5, 0, 0.5))
				.LocalRotation(new LunyVector3(-111, 0, 0))
				.LocalScale(0.5));

			var axisPrefabPath = "Packages/de.codesmile.lunyscript/LunyScript.Unity/SmokeTests/Prefabs/Axis";
			On.Ready(Prefab.Instantiate(axisPrefabPath) // extension (.prefab) is optional
				.Parent(ParentName)
				.LocalPosition(new LunyVector3(2, 0, -1))
				.LocalScale(0.5)
				.LocalRotation(new LunyVector3(15, 65, -35)));

			// alias for Prefab.Instantiate()
			On.Ready(Object.Create("Another Axis")
				.With(axisPrefabPath)
				.Parent(ParentName)
				.LocalPosition(new LunyVector3(2.2, 0, -1.1))
				.LocalScale(0.4)
				.LocalRotation(new LunyVector3(16, 66, -36)));

			// cloning works on all objects - not just sheep
			On.Ready(Object.Create("Clone of 'Another Axis'")
				.Clone("Another Axis")
				.Parent(ParentName)
				.LocalPosition(new LunyVector3(2.4, 0, -1.2))
				.LocalScale(0.3)
				.LocalRotation(new LunyVector3(26, 56, -46)));

			var prefabDoesNotExist = "does not exist, spawns placeholder";
			On.Ready(Prefab.Instantiate(prefabDoesNotExist)
				.Parent(ParentName)
				.LocalPosition(new LunyVector3(3, 2, -1))
				.LocalScale(new LunyVector3(0.3333, 0.65, 1.2))
				.LocalRotation(new LunyVector3(15, 65, -35)));

			var toBeDeleted = new[]
			{
				"Axis",
				"Void",
				prefabDoesNotExist,
				"Clone of 'Another Axis'",
				"Another Axis",
				axisPrefabPath,
				"CorneryThingy",
				"Planetesimal",
				"FeaturelessBody",
				"PipeDream",
				"QuadQuadInTheWhat?",
				"AsFlatAsCanBe",
				"Directional Light",
				ParentName,
				nameof(ObjectCreateTestScript), // kills the script
			};
			var timeOffset = 3;
			var frequency = 0.222f;
			for (var i = 0; i < toBeDeleted.Length; i++)
			{
				var name = toBeDeleted[i];
				Coroutine($"destroy timer for {name}").In(i * frequency + timeOffset).Seconds().WhenElapsed(Object.Destroy(name));
			}

			// one final spawn
			On.Destroyed(
				Prefab.Instantiate(null).LocalPosition(new LunyVector3(3, 7, 0)).LocalRotation(new LunyVector3(15, 65, -35)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 7.5, 0)).LocalRotation(new LunyVector3(125, 65, -35)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 9, 0)).LocalRotation(new LunyVector3(15, 165, -135)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 12, 0)).LocalRotation(new LunyVector3(135, 15, -135)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 25, 0)).LocalRotation(new LunyVector3(150, 165, -235)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 57, 0)).LocalRotation(new LunyVector3(-25, -65, 135)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 117, 0)).LocalRotation(new LunyVector3(-99, -165, 200)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 366, 0)).LocalRotation(new LunyVector3(-225, 250, -90)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 633, 0)).LocalRotation(new LunyVector3(325, 295, 50)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 1177, 0)).LocalRotation(new LunyVector3(25, 95, -50)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 1977, 0)).LocalRotation(new LunyVector3(-32, -29, 300)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 2777, 0)).LocalRotation(new LunyVector3(-32, -25, 30)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3.5, 2778, 0)).LocalRotation(new LunyVector3(-32, -5, 30)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(2, 2779, 0)).LocalRotation(new LunyVector3(-32, -29, 30)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(4, 2780, 0)).LocalRotation(new LunyVector3(-32, -95, 30)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 2781, 1)).LocalRotation(new LunyVector3(-3, -295, 30)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(3, 2782, -1)).LocalRotation(new LunyVector3(-32, -25, 30)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(2, 2783, 0)).LocalRotation(new LunyVector3(-2, -295, 30)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(2, 2784, 1)).LocalRotation(new LunyVector3(-32, -95, 30)),
				Prefab.Instantiate("").LocalPosition(new LunyVector3(4, 2785, -1)).LocalRotation(new LunyVector3(-3, -55, 30))
			);
		}
	}
}
