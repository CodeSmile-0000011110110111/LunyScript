using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Object
{
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

			script.FinalizeBuilderToken(token);
			return block;
		}
	}
}
