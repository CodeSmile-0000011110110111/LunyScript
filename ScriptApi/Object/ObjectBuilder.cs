using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public interface IObjectBuilderState {}

	public interface IObjectBuilderNameSet : IObjectBuilderState, IObjectBuilderCanFinish {}
	public struct ObjectBuilderNameSet : IObjectBuilderNameSet {}
	public interface IObjectBuilderCanFinish {}

	/// <summary>
	/// Provides operations for objects.
	/// </summary>
	public readonly struct ObjectBuilder
	{
		private readonly Script _script;
		internal ObjectBuilder(Script script) => _script = script;

		public ScriptActionBlock Enable(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectEnableSelfBlock.Create() : ObjectEnableTargetBlock.Create(name);

		public ScriptActionBlock Disable(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDisableSelfBlock.Create() : ObjectDisableTargetBlock.Create(name);

		public ObjectCreateBuilder<ObjectBuilderNameSet> Create(String name)
		{
			var options = new ObjectCreateOptions { Name = name, CreateMode = ObjectCreationMode.Empty, LocalScale = LunyVector3.One };
			var token = _script.CreateBuilderToken(name, "Object.Create");
			return new ObjectCreateBuilder<ObjectBuilderNameSet>(_script, token, options);
		}

		public ScriptActionBlock Destroy(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDestroySelfBlock.Create() : ObjectDestroyTargetBlock.Create(name);
	}

	public readonly struct PrefabBuilder
	{
		private readonly Script _script;
		internal PrefabBuilder(Script script) => _script = script;

		public ObjectCreateBuilder<ObjectBuilderNameSet> Instantiate(String prefabName) =>
			new ObjectBuilder(_script).Create(prefabName).From(prefabName);
	}

	public static class ObjectBuilderExtensions
	{
		public static ObjectCreateBuilder<ObjectBuilderNameSet> AsCube<T>(this ObjectCreateBuilder<T> b)
			where T : struct, IObjectBuilderNameSet => b.WithPrimitive(LunyPrimitiveType.Cube);

		public static ObjectCreateBuilder<ObjectBuilderNameSet> AsSphere<T>(this ObjectCreateBuilder<T> b)
			where T : struct, IObjectBuilderNameSet => b.WithPrimitive(LunyPrimitiveType.Sphere);

		public static ObjectCreateBuilder<ObjectBuilderNameSet> AsCapsule<T>(this ObjectCreateBuilder<T> b)
			where T : struct, IObjectBuilderNameSet => b.WithPrimitive(LunyPrimitiveType.Capsule);

		public static ObjectCreateBuilder<ObjectBuilderNameSet> AsCylinder<T>(this ObjectCreateBuilder<T> b)
			where T : struct, IObjectBuilderNameSet => b.WithPrimitive(LunyPrimitiveType.Cylinder);

		public static ObjectCreateBuilder<ObjectBuilderNameSet> AsPlane<T>(this ObjectCreateBuilder<T> b)
			where T : struct, IObjectBuilderNameSet => b.WithPrimitive(LunyPrimitiveType.Plane);

		public static ObjectCreateBuilder<ObjectBuilderNameSet> AsQuad<T>(this ObjectCreateBuilder<T> b)
			where T : struct, IObjectBuilderNameSet => b.WithPrimitive(LunyPrimitiveType.Quad);

		public static ObjectCreateBuilder<ObjectBuilderNameSet> From<T>(this ObjectCreateBuilder<T> b, String prefabName)
			where T : struct, IObjectBuilderNameSet => new(b.Script, b.Token,
			b.Options with { CreateMode = ObjectCreationMode.Prefab, AssetName = prefabName });

		public static ObjectCreateBuilder<ObjectBuilderNameSet> Clone<T>(this ObjectCreateBuilder<T> b, String existingName)
			where T : struct, IObjectBuilderNameSet => new(b.Script, b.Token,
			b.Options with { CreateMode = ObjectCreationMode.Clone, TemplateName = existingName });

		private static ObjectCreateBuilder<ObjectBuilderNameSet> WithPrimitive<T>(this ObjectCreateBuilder<T> b, LunyPrimitiveType type)
			where T : struct, IObjectBuilderNameSet => new(b.Script, b.Token,
			b.Options with { CreateMode = ObjectCreationMode.Primitive, PrimitiveType = type });
	}

	public readonly struct ObjectCreateBuilder<T> where T : struct, IObjectBuilderState
	{
		internal readonly Script Script;
		internal readonly BuilderToken Token;
		internal readonly ObjectCreateOptions Options;

		internal ObjectCreateBuilder(Script script, BuilderToken token, in ObjectCreateOptions options)
		{
			Script = script;
			Options = options;
			Token = token;

			var capturedOptions = options;
			token.AutoFinish = () => Finish(script, token, capturedOptions);
		}

		public static implicit operator ScriptActionBlock(ObjectCreateBuilder<T> builder) =>
			Finish(builder.Script, builder.Token, builder.Options);

		public ObjectCreateBuilder<T> Parent(ILunyObject parent) => new(Script, Token, Options with { Parent = parent });

		public ObjectCreateBuilder<T> Position(LunyVector3 localPosition) => new(Script, Token, Options with { LocalPosition = localPosition });

		public ObjectCreateBuilder<T> Rotation(LunyQuaternion localRotation) =>
			new(Script, Token, Options with { LocalRotation = localRotation });

		public ObjectCreateBuilder<T> Scale(LunyVector3 localScale) => new(Script, Token, Options with { LocalScale = localScale });

		public ObjectCreateBuilder<T> Scale(Double uniformLocalScale) => new(Script, Token,
			Options with { LocalScale = new LunyVector3(uniformLocalScale, uniformLocalScale, uniformLocalScale) });

		internal static ScriptActionBlock Finish(Script script, BuilderToken token, in ObjectCreateOptions options)
		{
			var block = options.CreateMode switch
			{
				ObjectCreationMode.Empty => ObjectCreateEmptyBlock.Create(options),
				ObjectCreationMode.Primitive => options.PrimitiveType switch
				{
					LunyPrimitiveType.Cube => ObjectCreateCubeBlock.Create(options),
					LunyPrimitiveType.Sphere => ObjectCreateSphereBlock.Create(options),
					LunyPrimitiveType.Capsule => ObjectCreateCapsuleBlock.Create(options),
					LunyPrimitiveType.Cylinder => ObjectCreateCylinderBlock.Create(options),
					LunyPrimitiveType.Plane => ObjectCreatePlaneBlock.Create(options),
					LunyPrimitiveType.Quad => ObjectCreateQuadBlock.Create(options),
					var _ => ObjectCreateEmptyBlock.Create(options),
				},
				ObjectCreationMode.Prefab => ObjectCreatePrefabBlock.Create(options),
				ObjectCreationMode.Clone => ObjectCreateCloneBlock.Create(options),
				var _ => throw new NotImplementedException(
					$"{nameof(ObjectCreateBuilder<ObjectBuilderNameSet>)}: Mode {options.CreateMode} is not implemented."),
			};

			script.MarkBuilderTokenFinished(token);
			return block;
		}
	}

	internal record ObjectCreateOptions
	{
		public String Name;
		public ObjectCreationMode CreateMode;
		public LunyPrimitiveType PrimitiveType;
		public String AssetName;
		public ILunyObject Parent;
		public String TemplateName;
		public LunyVector3 LocalPosition;
		public LunyQuaternion LocalRotation;
		public LunyVector3 LocalScale;
	}
}
