using LunyScript;

public class Object_Create_SmokeTest : Script
{
	public override void Build()
	{
		var parentName = "empty";
		On.Ready(Object.Create(parentName));
		On.Ready(Object.Create("child of empty").Parent(parentName));

		parentName = "primitives";
		On.Ready(Object.Create(parentName).Position(0.1, 0.1, 0.1).Rotation(3, 4, 5).Scale(0.97, 0.95, 0.92));
		On.Ready(Object.Create("capsule").Parent(parentName).AsCapsule().Position(0, 1, 1));
		On.Ready(Object.Create("cube").Parent(parentName).AsCube().Position(1, 1, 2));
		On.Ready(Object.Create("cylinder").Parent(parentName).AsCylinder().Position(2, 1, 3));
		On.Ready(Object.Create("plane").Parent(parentName).AsPlane().Position(3, 1, 4).Scale(0.2));
		On.Ready(Object.Create("quad").Parent(parentName).AsQuad().Position(4, 1, 5));
		On.Ready(Object.Create("sphere").Parent(parentName).AsSphere().Position(5, 1, 6));

		parentName = "primitives (rotated)";
		On.Ready(Object.Create(parentName).Position(0, 2.1, 0));
		On.Ready(Object.Create("capsule").Parent(parentName).AsCapsule().Position(0, 1, 1).Rotation(10, 20, 30));
		On.Ready(Object.Create("cube").Parent(parentName).AsCube().Position(1, 1, 2).Rotation(10, 20, 30));
		On.Ready(Object.Create("cylinder").Parent(parentName).AsCylinder().Position(2, 1, 3).Rotation(10, 20, 30));
		On.Ready(Object.Create("plane").Parent(parentName).AsPlane().Position(3, 1, 4).Rotation(100, 200, 300).Scale(0.2));
		On.Ready(Object.Create("quad").Parent(parentName).AsQuad().Position(4, 1, 5).Rotation(10, 20, 30));
		On.Ready(Object.Create("sphere").Parent(parentName).AsSphere().Position(5, 1, 6).Rotation(10, 20, 30));

		parentName = "primitives (scaled)";
		On.Ready(Object.Create(parentName).Position(0, 3.2, 0));
		On.Ready(Object.Create("capsule").Parent(parentName).AsCapsule().Position(0, 1, 1).Rotation(10, 20, 30).Scale(0.2));
		On.Ready(Object.Create("cube").Parent(parentName).AsCube().Position(1, 1, 2).Rotation(10, 20, 30).Scale(0.2));
		On.Ready(Object.Create("cylinder").Parent(parentName).AsCylinder().Position(2, 1, 3).Rotation(10, 20, 30).Scale(0.2));
		On.Ready(Object.Create("plane").Parent(parentName).AsPlane().Position(3, 1, 4).Rotation(100, 200, 300).Scale(0.1));
		On.Ready(Object.Create("quad").Parent(parentName).AsQuad().Position(4, 1, 5).Rotation(10, 20, 30).Scale(0.2));
		On.Ready(Object.Create("sphere").Parent(parentName).AsSphere().Position(5, 1, 6).Rotation(10, 20, 30).Scale(0.2));

		parentName = "prefabs";
		On.Ready(Object.Create(parentName).Position(-1, 2, -1).Rotation(-3, -4, -5).Scale(0.5));
		var prefabPath = "Packages/de.codesmile.lunyscript/LunyScript.Unity/SmokeTests/Prefabs/Cublet";
		On.Ready(Object.Create("instance via Object.Create")
			.From(prefabPath)
			.Position(1, 3, -1)
			.Rotation(45, 45, 45)
			.Scale(.25, 4, 2)
			.Parent(parentName));
		// alias:
		On.Ready(Prefab.Instantiate("instance via Prefab.Instantiate")
			.From(prefabPath)
			.Position(1, 3, -1)
			.Rotation(-45, -45, -45)
			.Scale(.5, 6, 2)
			.Parent(parentName));
	}
}
