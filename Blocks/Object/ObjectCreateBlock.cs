using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Services;
using System;

namespace LunyScript.Blocks
{
	internal enum ObjectCreationMode
	{
		Empty,
		Primitive,
		Prefab,
		Clone,
	}

	internal struct ObjectCreateOptions
	{
		public String Name;
		public ObjectCreationMode Mode;
		public LunyPrimitiveType PrimitiveType;
		public String AssetName;
	}

	internal abstract class ObjectCreateBlock : ScriptActionBlock
	{
		protected readonly String Name;
		protected static ILunyObjectService Object => LunyEngine.Instance.Object;

		protected ObjectCreateBlock(String name) => Name = name;
	}

	internal sealed class ObjectCreateEmptyBlock : ObjectCreateBlock
	{
		public static ScriptActionBlock Create(String name) => new ObjectCreateEmptyBlock(name);

		private ObjectCreateEmptyBlock(String name)
			: base(name) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreateEmpty(Name);
	}

	internal sealed class ObjectCreateCubeBlock : ObjectCreateBlock
	{
		public static ScriptActionBlock Create(String name) => new ObjectCreateCubeBlock(name);

		private ObjectCreateCubeBlock(String name)
			: base(name) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name, LunyPrimitiveType.Cube);
	}

	internal sealed class ObjectCreateSphereBlock : ObjectCreateBlock
	{
		public static ScriptActionBlock Create(String name) => new ObjectCreateSphereBlock(name);

		private ObjectCreateSphereBlock(String name)
			: base(name) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			Object.CreatePrimitive(Name, LunyPrimitiveType.Sphere);
	}

	internal sealed class ObjectCreateCapsuleBlock : ObjectCreateBlock
	{
		public static ScriptActionBlock Create(String name) => new ObjectCreateCapsuleBlock(name);

		private ObjectCreateCapsuleBlock(String name)
			: base(name) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			Object.CreatePrimitive(Name, LunyPrimitiveType.Capsule);
	}

	internal sealed class ObjectCreateCylinderBlock : ObjectCreateBlock
	{
		public static ScriptActionBlock Create(String name) => new ObjectCreateCylinderBlock(name);

		private ObjectCreateCylinderBlock(String name)
			: base(name) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			Object.CreatePrimitive(Name, LunyPrimitiveType.Cylinder);
	}

	internal sealed class ObjectCreatePlaneBlock : ObjectCreateBlock
	{
		public static ScriptActionBlock Create(String name) => new ObjectCreatePlaneBlock(name);

		private ObjectCreatePlaneBlock(String name)
			: base(name) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name, LunyPrimitiveType.Plane);
	}

	internal sealed class ObjectCreateQuadBlock : ObjectCreateBlock
	{
		public static ScriptActionBlock Create(String name) => new ObjectCreateQuadBlock(name);

		private ObjectCreateQuadBlock(String name)
			: base(name) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name, LunyPrimitiveType.Quad);
	}

	internal sealed class ObjectCreatePrefabBlock : ObjectCreateBlock
	{
		private readonly String _assetName;

		public static ScriptActionBlock Create(String instanceName, String assetName) => new ObjectCreatePrefabBlock(instanceName, assetName);

		private ObjectCreatePrefabBlock(String instanceName, String assetName)
			: base(instanceName) => _assetName = assetName;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var prefab = LunyEngine.Instance.Asset.Load<ILunyPrefab>(_assetName);
			var instance = Object.CreateFromPrefab(prefab);
			if (instance != null)
				instance.Name = Name;
		}
	}

	internal sealed class ObjectCreateCloneBlock : ObjectCreateBlock
	{
		private readonly String _sourceName;

		public static ScriptActionBlock Create(String instanceName, String sourceName) => new ObjectCreateCloneBlock(instanceName, sourceName);

		private ObjectCreateCloneBlock(String instanceName, String sourceName)
			: base(instanceName) => _sourceName = sourceName;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) =>
			throw new NotImplementedException($"{nameof(ObjectCreateCloneBlock)}.{nameof(Execute)} is not yet implemented.");
	}
}
