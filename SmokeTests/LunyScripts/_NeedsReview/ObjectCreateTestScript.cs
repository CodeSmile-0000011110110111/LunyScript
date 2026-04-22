using System;

namespace LunyScript.SmokeTests
{
	public sealed class ObjectCreateTestScript : Script
	{
		public const String AutoDestroyObjectName = "will be destroyed";
		public const String ParentName = "Created Objects";

		public override void Build()
		{
			var isDestroyed = Var.Define("isDestroyed", false);

			On.Created(Object.Create(AutoDestroyObjectName));
			On.AfterFrameUpdate(If(isDestroyed == false).Then(isDestroyed.Set(true), Object.Destroy(AutoDestroyObjectName)));

			On.Ready(Object.Create(ParentName).Position(0, 1, 1));
			On.Ready(Object.Create("CorneryThingy").AsCube().Position(1, 0, 0).Parent(ParentName));
			On.Ready(Object.Create("Planetesimal").AsSphere().Parent(ParentName).Position(2, 0, 0));
			On.Ready(Object.Create("FeaturelessBody").AsCapsule().Parent(ParentName).Position(3.0, 0.0, 0.0));
			On.Ready(Object.Create("PipeDream").AsCylinder().Parent(ParentName).Position(4, 0, 0));

			On.Ready(Object.Create("WhatWhatInTheQuad?")
				.AsQuad()
				.Parent(ParentName)
				.Position(5, 0, -1)
				.Rotation(30, 60, -30));

			On.Ready(Object.Create("AsFlatAsCanBe")
				.AsPlane()
				.Parent(ParentName)
				.Position(2.5, 0, 0.5)
				.Rotation(-111, 0, 0)
				.Scale(0.5));

			var axisPrefabPath = "Packages/de.codesmile.lunyscript/LunyScript.Unity/SmokeTests/Prefabs/Axis";
			On.Ready(Prefab.Instantiate(axisPrefabPath) // extension (.prefab) is optional
				.Parent(ParentName)
				.Position(2, 0, -1)
				.Scale(0.5)
				.Rotation(15, 65, -35));

			// alias for Prefab.Instantiate()
			On.Ready(Object.Create("Another Axis")
				.From(axisPrefabPath)
				.Parent(ParentName)
				.Position(2.2, 0, -1.1)
				.Scale(0.4)
				.Rotation(16, 66, -36));

			// cloning works on all objects - not just sheep
			On.Ready(Object.Create("Clone of 'Another Axis'")
				.Clone("Another Axis")
				.Parent(ParentName)
				.Position(2.4, 0, -1.2)
				.Scale(0.3)
				.Rotation(26, 56, -46));

			var prefabDoesNotExist = "does not exist, spawns placeholder";
			On.Ready(Prefab.Instantiate(prefabDoesNotExist)
				.Parent(ParentName)
				.Position(3, 2, -1)
				.Rotation(15, 65, -35)
				.Scale(0.3333, 0.65, 1.2));

			IncrementallyDeleteSceneObjects(prefabDoesNotExist, axisPrefabPath);
			CreateFinalObjectsWhenDestroyed();

			var createCount = Var.Define("create count", 300);
			On.Ready(For(createCount).Do(Object.Create("Placeholder").From("???").Position(1, 2.5, .5).Scale(.1)));
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
			Prefab.Instantiate(null).Position(3, 7, 0).Rotation(15, 65, -35),
			Prefab.Instantiate("").Position(3, 7.5, 0).Rotation(125, 65, -35),
			Prefab.Instantiate("?").Position(3, 9, 0).Rotation(15, 165, -135),
			Prefab.Instantiate("").Position(3, 12, 0).Rotation(135, 15, -135),
			Prefab.Instantiate("").Position(3, 25, 0).Rotation(150, 165, -235),
			Prefab.Instantiate("").Position(3, 55, 0).Rotation(-25, -65, 335),
			Prefab.Instantiate("").Position(3, 115, 0).Rotation(-99, -165, 200),
			Prefab.Instantiate("").Position(3, 365, 0).Rotation(-225, 250, -90),
			Prefab.Instantiate("").Position(3, 633, 0).Rotation(325, 295, 50),
			Prefab.Instantiate("").Position(3, 1177, 0).Rotation(25, 95, -50),
			Prefab.Instantiate("").Position(3, 1977, 0).Rotation(-32, -29, 300),
			Prefab.Instantiate("").Position(3, 2777, 0).Rotation(-32, -25, 30),
			Prefab.Instantiate("").Position(3.5, 2778, 0).Rotation(-32, -5, 30),
			Prefab.Instantiate("").Position(2, 2779, 0).Rotation(-32, -29, 30),
			Prefab.Instantiate("").Position(4, 2780, 0).Rotation(-32, -95, 30),
			Prefab.Instantiate("").Position(3, 2781, 1).Rotation(-3, -295, 30),
			Prefab.Instantiate("").Position(3, 2782, -1).Rotation(-32, -25, 30),
			Prefab.Instantiate("").Position(2, 2783, 0).Rotation(-2, -295, 30),
			Prefab.Instantiate("").Position(2, 2784, 1).Rotation(-32, -95, 30),
			Prefab.Instantiate("").Position(4, 2785, -1).Rotation(-3, -55, 30)
		);
	}
}
