using LunyScript.Api;
using System;

namespace LunyScript.SmokeTests
{
	public sealed class ObjectCreateTestScript : Script
	{
		public const String AutoDestroyObjectName = "will be destroyed";
		public const String ParentName = "Created Objects";

		public override void Build(ScriptContext context)
		{
			var isDestroyed = Var.Define("isDestroyed", false);

			On.Created(Object.Create(AutoDestroyObjectName));
			On.AfterFrameUpdate(If(isDestroyed == false).Then(isDestroyed.Set(true), Object.Destroy(AutoDestroyObjectName)));

			On.Ready(Object.Create(ParentName).LocalPosition(0, 1, 1));
			On.Ready(Object.Create("CorneryThingy").AsCube().LocalPosition(1, 0, 0).Parent(ParentName));
			On.Ready(Object.Create("Planetesimal").AsSphere().Parent(ParentName).LocalPosition(2, 0, 0));
			On.Ready(Object.Create("FeaturelessBody").AsCapsule().Parent(ParentName).LocalPosition(3.0, 0.0, 0.0));
			On.Ready(Object.Create("PipeDream").AsCylinder().Parent(ParentName).LocalPosition(4, 0, 0));

			On.Ready(Object.Create("WhatWhatInTheQuad?")
				.AsQuad()
				.Parent(ParentName)
				.LocalPosition(5, 0, -1)
				.LocalRotation(30, 60, -30));

			On.Ready(Object.Create("AsFlatAsCanBe")
				.AsPlane()
				.Parent(ParentName)
				.LocalPosition(2.5, 0, 0.5)
				.LocalRotation(-111, 0, 0)
				.LocalScale(0.5));

			var axisPrefabPath = "Packages/de.codesmile.lunyscript/LunyScript.Unity/SmokeTests/Prefabs/Axis";
			On.Ready(Prefab.Instantiate(axisPrefabPath) // extension (.prefab) is optional
				.Parent(ParentName)
				.LocalPosition(2, 0, -1)
				.LocalScale(0.5)
				.LocalRotation(15, 65, -35));

			// alias for Prefab.Instantiate()
			On.Ready(Object.Create("Another Axis")
				.With(axisPrefabPath)
				.Parent(ParentName)
				.LocalPosition(2.2, 0, -1.1)
				.LocalScale(0.4)
				.LocalRotation(16, 66, -36));

			// cloning works on all objects - not just sheep
			On.Ready(Object.Create("Clone of 'Another Axis'")
				.Clone("Another Axis")
				.Parent(ParentName)
				.LocalPosition(2.4, 0, -1.2)
				.LocalScale(0.3)
				.LocalRotation(26, 56, -46));

			var prefabDoesNotExist = "does not exist, spawns placeholder";
			On.Ready(Prefab.Instantiate(prefabDoesNotExist)
				.Parent(ParentName)
				.LocalPosition(3, 2, -1)
				.LocalRotation(15, 65, -35)
				.LocalScale(0.3333, 0.65, 1.2));

			IncrementallyDeleteSceneObjects(prefabDoesNotExist, axisPrefabPath);
			CreateFinalObjectsWhenDestroyed();
		}

		private void IncrementallyDeleteSceneObjects(String prefabDoesNotExist, String axisPrefabPath)
		{
			var toBeDeleted = new[]
			{
				"Axis",
				"WhatWhatInTheQuad?",
				"AsFlatAsCanBe",
				"?", // on purpose
				prefabDoesNotExist,
				"Clone of 'Another Axis'",
				"Another Axis",
				axisPrefabPath,
				"CorneryThingy",
				"Planetesimal",
				"FeaturelessBody",
				"PipeDream",
				"Directional Light",
				ParentName,
				nameof(ObjectCreateTestScript), // kills the script
			};
			var timeOffset = 3;
			var frequency = 0.222f;
			for (var i = 0; i < toBeDeleted.Length; i++)
			{
				var name = toBeDeleted[i];
				Coroutine($"destroy timer for {name}")
					.In(i * frequency + timeOffset)
					.Seconds()
					.WhenElapsed(Debug.Log($"Destroy({name})"),
						Object.Destroy(name));
			}
		}

		private void CreateFinalObjectsWhenDestroyed() => On.Destroyed(
			// purposefully adding some "mistakes" (null, empty string) to test placeholder creation
			Prefab.Instantiate(null).LocalPosition(3, 7, 0).LocalRotation(15, 65, -35),
			Prefab.Instantiate("").LocalPosition(3, 7.5, 0).LocalRotation(125, 65, -35),
			Prefab.Instantiate("?").LocalPosition(3, 9, 0).LocalRotation(15, 165, -135),
			Prefab.Instantiate("").LocalPosition(3, 12, 0).LocalRotation(135, 15, -135),
			Prefab.Instantiate("").LocalPosition(3, 25, 0).LocalRotation(150, 165, -235),
			Prefab.Instantiate("").LocalPosition(3, 55, 0).LocalRotation(-25, -65, 335),
			Prefab.Instantiate("").LocalPosition(3, 115, 0).LocalRotation(-99, -165, 200),
			Prefab.Instantiate("").LocalPosition(3, 365, 0).LocalRotation(-225, 250, -90),
			Prefab.Instantiate("").LocalPosition(3, 633, 0).LocalRotation(325, 295, 50),
			Prefab.Instantiate("").LocalPosition(3, 1177, 0).LocalRotation(25, 95, -50),
			Prefab.Instantiate("").LocalPosition(3, 1977, 0).LocalRotation(-32, -29, 300),
			Prefab.Instantiate("").LocalPosition(3, 2777, 0).LocalRotation(-32, -25, 30),
			Prefab.Instantiate("").LocalPosition(3.5, 2778, 0).LocalRotation(-32, -5, 30),
			Prefab.Instantiate("").LocalPosition(2, 2779, 0).LocalRotation(-32, -29, 30),
			Prefab.Instantiate("").LocalPosition(4, 2780, 0).LocalRotation(-32, -95, 30),
			Prefab.Instantiate("").LocalPosition(3, 2781, 1).LocalRotation(-3, -295, 30),
			Prefab.Instantiate("").LocalPosition(3, 2782, -1).LocalRotation(-32, -25, 30),
			Prefab.Instantiate("").LocalPosition(2, 2783, 0).LocalRotation(-2, -295, 30),
			Prefab.Instantiate("").LocalPosition(2, 2784, 1).LocalRotation(-32, -95, 30),
			Prefab.Instantiate("").LocalPosition(4, 2785, -1).LocalRotation(-3, -55, 30)
		);
	}
}
