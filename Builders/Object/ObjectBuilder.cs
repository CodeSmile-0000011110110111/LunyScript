using Luny;
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
		private readonly LunyStackTrace _trace;

		internal ObjectBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		[NeedsReview] [NeedsSmokeTest]
		public ActionBlock Enable(LunyObjectRef target = null) => target == null
			? ObjectEnableSelfBlock.Create(_trace.Add(nameof(Enable)))
			: ObjectEnableTargetBlock.Create(target, _trace.Add(nameof(Enable)));

		[NeedsReview] [NeedsSmokeTest]
		public ActionBlock Disable(LunyObjectRef target = null) => target == null
			? ObjectDisableSelfBlock.Create(_trace.Add(nameof(Disable)))
			: ObjectDisableTargetBlock.Create(target, _trace.Add(nameof(Disable)));

		[NeedsReview] [NeedsSmokeTest]
		public ActionBlock SetEnabled(LunyObjectRef target, VariableBlock enabled) =>
			ObjectSetEnabledBlock.Create(target, enabled, _trace.Add(nameof(SetEnabled)));

		[NeedsReview] [NeedsSmokeTest]
		public ObjectCreateBuilder<ObjectBuilderNameSet> Create(String name)
		{
			var token = _script.CreateBuilderToken(name, "Object." + nameof(Create));
			var options = new ObjectCreateOptions
				{ Script = _script, Token = token, Trace = _trace.Add(nameof(Create)), Name = name, CreateMode = ObjectCreationMode.Empty };
			return new ObjectCreateBuilder<ObjectBuilderNameSet>(options);
		}

		[NeedsReview] [NeedsSmokeTest]
		public ActionBlock Destroy(LunyObjectRef target = null) => target == null
			? ObjectDestroySelfBlock.Create(_trace.Add(nameof(Destroy)))
			: ObjectDestroyTargetBlock.Create(target, _trace.Add(nameof(Destroy)));
	}

	public static class ObjectBuilderInstantiateExtensions
	{
		public static ObjectCreateBuilder<ObjectBuilderNameSet> From<T>(this ObjectCreateBuilder<T> b, String prefabName)
			where T : struct, IObjectBuilderNameSet => new(b.Options with { CreateMode = ObjectCreationMode.Prefab, AssetName = prefabName });

		public static ObjectCreateBuilder<ObjectBuilderNameSet> Clone<T>(this ObjectCreateBuilder<T> b, String existingName)
			where T : struct, IObjectBuilderNameSet =>
			new(b.Options with { CreateMode = ObjectCreationMode.Clone, TemplateName = existingName });
	}

	public static class ObjectBuilderPrimitiveExtensions
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

		private static ObjectCreateBuilder<ObjectBuilderNameSet> WithPrimitive<T>(this ObjectCreateBuilder<T> b, LunyPrimitiveType type)
			where T : struct, IObjectBuilderNameSet => new(b.Options with { CreateMode = ObjectCreationMode.Primitive, PrimitiveType = type });
	}

	public readonly struct ObjectCreateBuilder<T> where T : struct, IObjectBuilderState
	{
		internal readonly ObjectCreateOptions Options;

		internal ObjectCreateBuilder(in ObjectCreateOptions options)
		{
			Options = options;
			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions.Script, capturedOptions.Token, capturedOptions);
		}

		public static implicit operator ActionBlock(ObjectCreateBuilder<T> builder) =>
			Finish(builder.Options.Script, builder.Options.Token, builder.Options);

		public ObjectCreateBuilder<T> Parent(LunyObjectRef parent) => new(Options with { Parent = parent });
		public ObjectCreateBuilder<T> Position(Double x, Double y, Double z) => new(Options with { LocalPosition = new LunyVector3(x, y, z) });
		public ObjectCreateBuilder<T> Position(LunyVector3 localPosition) => new(Options with { LocalPosition = localPosition });

		public ObjectCreateBuilder<T> Rotation(Double x, Double y, Double z) =>
			new(Options with { LocalRotation = LunyQuaternion.Euler(x, y, z) });

		public ObjectCreateBuilder<T> Rotation(LunyVector3 localEulerAngles) =>
			new(Options with { LocalRotation = LunyQuaternion.Euler(localEulerAngles) });

		public ObjectCreateBuilder<T> Rotation(LunyQuaternion localRotation) => new(Options with { LocalRotation = localRotation });

		public ObjectCreateBuilder<T> Scale(Double uniformScale) =>
			new(Options with { Scale = new LunyVector3(uniformScale, uniformScale, uniformScale) });

		public ObjectCreateBuilder<T> Scale(Double x, Double y, Double z) => new(Options with { Scale = new LunyVector3(x, y, z) });
		public ObjectCreateBuilder<T> Scale(LunyVector3 scale) => new(Options with { Scale = scale });

		internal static ActionBlock Finish(Script script, BuilderToken token, in ObjectCreateOptions options)
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
		internal Script Script;
		internal BuilderToken Token;
		internal LunyStackTrace Trace;

		public String Name;
		public ObjectCreationMode CreateMode;
		public LunyPrimitiveType PrimitiveType;
		public String AssetName;
		public LunyObjectRef Parent;
		public String TemplateName;
		public LunyVector3 LocalPosition;
		public LunyQuaternion LocalRotation;
		public LunyVector3? Scale;
	}
}
