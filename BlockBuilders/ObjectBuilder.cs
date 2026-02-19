using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript.BlockBuilders
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

		public ObjectBuilder<ObjectBuilderNameSet> Create(String name)
		{
			var options = new ObjectCreateOptions { Name = name, Mode = ObjectCreationMode.Empty };
			var token = _script.CreateToken(name, "ObjectCreate");
			return new ObjectBuilder<ObjectBuilderNameSet>(_script, options, token);
		}

		public ScriptActionBlock Destroy(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDestroySelfBlock.Create() : ObjectDestroyTargetBlock.Create(name);
	}

	public readonly struct ObjectBuilder<T> where T : struct, IObjectBuilderState
	{
		internal readonly Script Script;
		internal readonly ObjectCreateOptions Options;
		internal readonly BuilderToken Token;

		internal ObjectBuilder(Script script, ObjectCreateOptions options, BuilderToken token)
		{
			Script = script;
			Options = options;
			Token = token;
		}

		/// <summary>
		/// Completes the builder and returns the executable block.
		/// </summary>
		public ScriptActionBlock Do() => BuilderUtility.Finalize(Script, Options, Token);
	}

	public static class ObjectBuilderExtensions
	{
		public static ObjectBuilder<ObjectBuilderNameSet> AsCube<T>(this ObjectBuilder<T> b) where T : struct, IObjectBuilderNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Cube);

		public static ObjectBuilder<ObjectBuilderNameSet> AsSphere<T>(this ObjectBuilder<T> b) where T : struct, IObjectBuilderNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Sphere);

		public static ObjectBuilder<ObjectBuilderNameSet> AsCapsule<T>(this ObjectBuilder<T> b) where T : struct, IObjectBuilderNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Capsule);

		public static ObjectBuilder<ObjectBuilderNameSet> AsCylinder<T>(this ObjectBuilder<T> b) where T : struct, IObjectBuilderNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Cylinder);

		public static ObjectBuilder<ObjectBuilderNameSet> AsPlane<T>(this ObjectBuilder<T> b) where T : struct, IObjectBuilderNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Plane);

		public static ObjectBuilder<ObjectBuilderNameSet> AsQuad<T>(this ObjectBuilder<T> b) where T : struct, IObjectBuilderNameSet =>
			b.WithPrimitive(LunyPrimitiveType.Quad);

		public static ObjectBuilder<ObjectBuilderNameSet> From<T>(this ObjectBuilder<T> b, String prefabName)
			where T : struct, IObjectBuilderNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Prefab;
			options.AssetName = prefabName;
			return new ObjectBuilder<ObjectBuilderNameSet>(b.Script, options, b.Token);
		}

		public static ObjectBuilder<ObjectBuilderNameSet> Clone<T>(this ObjectBuilder<T> b, String existingName)
			where T : struct, IObjectBuilderNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Clone;
			options.AssetName = existingName;
			return new ObjectBuilder<ObjectBuilderNameSet>(b.Script, options, b.Token);
		}

		private static ObjectBuilder<ObjectBuilderNameSet> WithPrimitive<T>(this ObjectBuilder<T> b, LunyPrimitiveType type)
			where T : struct, IObjectBuilderNameSet
		{
			var options = b.Options;
			options.Mode = ObjectCreationMode.Primitive;
			options.PrimitiveType = type;
			return new ObjectBuilder<ObjectBuilderNameSet>(b.Script, options, b.Token);
		}
	}
}
