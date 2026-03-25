using Luny;
using Luny.Engine.Bridge;
using Luny.Engine.Services;
using LunyScript.Api;
using System;
using System.Text;

namespace LunyScript.Blocks
{
	internal enum ObjectCreationMode
	{
		Empty,
		Primitive,
		Prefab,
		Clone,
	}

	internal abstract class ObjectCreateBlock : ActionBlock
	{
		protected readonly String Name;
		protected readonly LunyObjectRef Parent;
		protected readonly LunyVector3? LocalPosition;
		protected readonly LunyQuaternion? LocalRotation;
		protected readonly LunyVector3? LocalScale;

		protected static ILunyObjectService Object => LunyEngine.Instance.Object;

		protected ObjectCreateBlock(ObjectCreateOptions options)
		{
			Name = String.IsNullOrEmpty(options.Name) ? $"(unnamed) created by {options.Script.Name}" : options.Name;
			Parent = options.Parent;
			LocalPosition = options.LocalPosition;
			LocalRotation = options.LocalRotation;
			LocalScale = options.LocalScale;
		}

		public override String ToString()
		{
			var sb = new StringBuilder("(");
			sb.Append('\"');
			sb.Append(Name);
			sb.Append('\"');
			if (Parent != null && Parent.Value != null)
			{
				sb.Append(", parent:");
				sb.Append(Parent.Value);
			}
			if (LocalPosition.HasValue && LocalPosition.Value != LunyVector3.Zero)
			{
				sb.Append(", position:");
				sb.Append(LocalPosition.Value);
			}
			if (LocalRotation.HasValue)
			{
				var eulerAngles = LocalRotation.Value.EulerAngles;
				if (eulerAngles != LunyVector3.Zero)
				{
					sb.Append(", rotation:");
					sb.Append(eulerAngles);
				}
			}
			if (LocalScale.HasValue && LocalScale.Value != LunyVector3.One)
			{
				sb.Append(", scale:");
				sb.Append(LocalScale.Value);
			}
			sb.Append(")");
			return sb.ToString();
		}
	}

	internal sealed class ObjectCreateEmptyBlock : ObjectCreateBlock
	{
		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreateEmptyBlock(options);

		private ObjectCreateEmptyBlock(ObjectCreateOptions options)
			: base(options) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreateEmpty(Name, Parent?.Value, LocalPosition,
			LocalRotation, LocalScale.HasValue ? LocalScale.Value : LunyVector3.One);

		public override String ToString() => $"Object.CreateEmpty{base.ToString()}";
	}

	internal sealed class ObjectCreateCubeBlock : ObjectCreateBlock
	{
		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreateCubeBlock(options);

		private ObjectCreateCubeBlock(ObjectCreateOptions options)
			: base(options) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name, LunyPrimitiveType.Cube,
			Parent?.Value, LocalPosition, LocalRotation, LocalScale.HasValue ? LocalScale.Value : LunyVector3.One);
	}

	internal sealed class ObjectCreateSphereBlock : ObjectCreateBlock
	{
		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreateSphereBlock(options);

		private ObjectCreateSphereBlock(ObjectCreateOptions options)
			: base(options) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name, LunyPrimitiveType.Sphere,
			Parent?.Value, LocalPosition, LocalRotation, LocalScale);
	}

	internal sealed class ObjectCreateCapsuleBlock : ObjectCreateBlock
	{
		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreateCapsuleBlock(options);

		private ObjectCreateCapsuleBlock(ObjectCreateOptions options)
			: base(options) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name,
			LunyPrimitiveType.Capsule, Parent?.Value, LocalPosition, LocalRotation, LocalScale.HasValue ? LocalScale.Value : LunyVector3.One);
	}

	internal sealed class ObjectCreateCylinderBlock : ObjectCreateBlock
	{
		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreateCylinderBlock(options);

		private ObjectCreateCylinderBlock(ObjectCreateOptions options)
			: base(options) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name,
			LunyPrimitiveType.Cylinder, Parent?.Value, LocalPosition, LocalRotation, LocalScale.HasValue ? LocalScale.Value : LunyVector3.One);
	}

	internal sealed class ObjectCreatePlaneBlock : ObjectCreateBlock
	{
		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreatePlaneBlock(options);

		private ObjectCreatePlaneBlock(ObjectCreateOptions options)
			: base(options) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name, LunyPrimitiveType.Plane,
			Parent?.Value, LocalPosition, LocalRotation, LocalScale.HasValue ? LocalScale.Value : LunyVector3.One);
	}

	internal sealed class ObjectCreateQuadBlock : ObjectCreateBlock
	{
		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreateQuadBlock(options);

		private ObjectCreateQuadBlock(ObjectCreateOptions options)
			: base(options) {}

		protected internal override void Execute(IScriptRuntimeContext runtimeContext) => Object.CreatePrimitive(Name, LunyPrimitiveType.Quad,
			Parent?.Value, LocalPosition, LocalRotation, LocalScale.HasValue ? LocalScale.Value : LunyVector3.One);
	}

	internal sealed class ObjectCreatePrefabBlock : ObjectCreateBlock
	{
		private readonly String _prefabAssetName;

		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreatePrefabBlock(options);

		private ObjectCreatePrefabBlock(ObjectCreateOptions options)
			: base(options) => _prefabAssetName = String.IsNullOrEmpty(options.AssetName)
			? $"(Missing Prefab Name) created by {options.Script.Name}"
			: options.AssetName;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var prefab = LunyEngine.Instance.Asset.Load<ILunyPrefab>(_prefabAssetName);
			var instance = Object.CreateFromPrefab(prefab, Parent?.Value, LocalPosition, LocalRotation, LocalScale);
			if (instance == null)
				return;

			instance.Name = Name;
		}
	}

	internal sealed class ObjectCreateCloneBlock : ObjectCreateBlock
	{
		private readonly String _templateName;

		public static ActionBlock Create(ObjectCreateOptions options) => new ObjectCreateCloneBlock(options);

		private ObjectCreateCloneBlock(ObjectCreateOptions options)
			: base(options) => _templateName = options.TemplateName;

		protected internal override void Execute(IScriptRuntimeContext runtimeContext)
		{
			var original = LunyEngine.Instance.TryGetObject(_templateName);
			if (original == null)
				return;

			var instance = Object.Clone(original, Parent?.Value, LocalPosition, LocalRotation, LocalScale);
			if (instance == null)
				return;

			instance.Name = Name;
		}
	}
}
