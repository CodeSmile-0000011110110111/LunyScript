using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript.Api.Object
{
	public interface IObjectBuilderState {}
	public interface IObjectBuilderStart : IObjectBuilderState {}
	public interface IObjectBuilderNameSet : IObjectBuilderState, IObjectBuilderCanFinalize {}
	public interface IObjectBuilderCanFinalize {}

	public struct ObjectBuilderStart : IObjectBuilderStart {}
	public struct ObjectBuilderNameSet : IObjectBuilderNameSet {}

	/// <summary>
	/// Provides operations for objects.
	/// </summary>
	public readonly struct ObjectApi
	{
		private readonly Script _script;
		internal ObjectApi(Script script) => _script = script;

		public ScriptActionBlock Enable(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectEnableSelfBlock.Create() : ObjectEnableTargetBlock.Create(name);

		public ScriptActionBlock Disable(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDisableSelfBlock.Create() : ObjectDisableTargetBlock.Create(name);

		public ObjectCreateBuilder<ObjectBuilderNameSet> Create(String name)
		{
			var options = new ObjectCreateOptions { Name = name, Mode = ObjectCreationMode.Empty, LocalScale = LunyVector3.One };
			var token = _script.CreateToken(name, "ObjectCreateBuilder");
			return new ObjectCreateBuilder<ObjectBuilderNameSet>(_script, options, token);
		}

		public ScriptActionBlock Destroy(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDestroySelfBlock.Create() : ObjectDestroyTargetBlock.Create(name);
	}

	public readonly struct PrefabApi
	{
		private readonly Script _script;
		internal PrefabApi(Script script) => _script = script;

		public ObjectCreateBuilder<ObjectBuilderNameSet> Instantiate(String prefabName) => new ObjectApi().Create(prefabName).From(prefabName);
	}

	public readonly struct ObjectCreateBuilder<T> where T : struct, IObjectBuilderState
	{
		internal readonly Script Script;
		internal readonly ObjectCreateOptions Options;
		internal readonly BuilderToken Token;

		internal ObjectCreateBuilder(Script script, ObjectCreateOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;
			var capturedScript = script;
			var capturedOptions = options;
			token?.SetAutoFinalizer(() => FinalizeBuilder(capturedScript, capturedOptions, token));
		}

		public static implicit operator ScriptActionBlock(ObjectCreateBuilder<T> builder) =>
			FinalizeBuilder(builder.Script, builder.Options, builder.Token);

		public ObjectCreateBuilder<T> Parent(ILunyObject parent)
		{
			var options = Options;
			options.Parent = parent;
			return new ObjectCreateBuilder<T>(Script, options, Token);
		}

		public ObjectCreateBuilder<T> Position(LunyVector3 localPosition)
		{
			var options = Options;
			options.LocalPosition = localPosition;
			return new ObjectCreateBuilder<T>(Script, options, Token);
		}

		public ObjectCreateBuilder<T> Rotation(LunyQuaternion localRotation)
		{
			var options = Options;
			options.LocalRotation = localRotation;
			return new ObjectCreateBuilder<T>(Script, options, Token);
		}

		public ObjectCreateBuilder<T> Scale(LunyVector3 localScale)
		{
			var options = Options;
			options.LocalScale = localScale;
			return new ObjectCreateBuilder<T>(Script, options, Token);
		}

		public ObjectCreateBuilder<T> Scale(Double uniformLocalScale)
		{
			var options = Options;
			options.LocalScale = new LunyVector3(uniformLocalScale, uniformLocalScale, uniformLocalScale);
			return new ObjectCreateBuilder<T>(Script, options, Token);
		}

		internal static ScriptActionBlock FinalizeBuilder(Script script, in ObjectCreateOptions options, BuilderToken token)
		{
			var block = options.Mode switch
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
					$"{nameof(ObjectCreateBuilder<ObjectBuilderNameSet>)}: Mode {options.Mode} is not implemented."),
			};

			script.FinalizeToken(token);
			return block;
		}
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
			where T : struct, IObjectBuilderNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Prefab;
			options.AssetName = prefabName;
			return new ObjectCreateBuilder<ObjectBuilderNameSet>(b.Script, options, b.Token);
		}

		public static ObjectCreateBuilder<ObjectBuilderNameSet> Clone<T>(this ObjectCreateBuilder<T> b, String existingName)
			where T : struct, IObjectBuilderNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Clone;
			options.TemplateName = existingName;
			return new ObjectCreateBuilder<ObjectBuilderNameSet>(b.Script, options, b.Token);
		}

		private static ObjectCreateBuilder<ObjectBuilderNameSet> WithPrimitive<T>(this ObjectCreateBuilder<T> b, LunyPrimitiveType type)
			where T : struct, IObjectBuilderNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Primitive;
			options.PrimitiveType = type;
			return new ObjectCreateBuilder<ObjectBuilderNameSet>(b.Script, options, b.Token);
		}
	}
}
