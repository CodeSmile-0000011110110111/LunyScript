using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Coroutines;
using System;

namespace LunyScript.BlockBuilders
{
	/// <summary>
	/// Shared utilities for LunyScript builder pattern.
	/// </summary>
	internal static class BuilderUtility
	{
		public static ScriptActionBlock[] Append(ScriptActionBlock[] existing, ScriptActionBlock[] additional)
		{
			if (existing == null || existing.Length == 0)
				return additional;
			if (additional == null || additional.Length == 0)
				return existing;

			LunyLogger.LogWarning("Appending multiple Coroutine blocks due to use of two or more same-behaviour block methods. " +
			                      "Please review the Coroutine builder statements to avoid the array copy operations.");

			var result = new ScriptActionBlock[existing.Length + additional.Length];
			Array.Copy(existing, 0, result, 0, existing.Length);
			Array.Copy(additional, 0, result, existing.Length, additional.Length);
			return result;
		}

		public static ICoroutineBlock Finalize(IScript script, in Coroutine.Options options, BuilderToken token)
		{
			if (options.OnFrameUpdate == null && options.OnHeartbeat == null && options.OnElapsed == null &&
			    options.OnStarted == null && options.OnStopped == null && options.OnPaused == null && options.OnResumed == null)
			{
				LunyLogger.LogWarning($"{nameof(Coroutine)} '{options.Name}' was finalized without any action blocks. " +
				                      "It will run but perform no actions.", script);
			}

			var scriptInternal = (ILunyScriptInternal)script;
			var block = scriptInternal.RuntimeContext.Coroutines.Register(script, in options);
			scriptInternal.FinalizeToken(token);
			return block;
		}

		public static ScriptActionBlock Finalize(IScript script, in ObjectCreateOptions options, BuilderToken token)
		{
			var block = options.Mode switch
			{
				ObjectCreationMode.Empty => ObjectCreateEmptyBlock.Create(options.Name),
				ObjectCreationMode.Primitive => options.PrimitiveType switch
				{
					LunyPrimitiveType.Cube => ObjectCreateCubeBlock.Create(options.Name),
					LunyPrimitiveType.Sphere => ObjectCreateSphereBlock.Create(options.Name),
					LunyPrimitiveType.Capsule => ObjectCreateCapsuleBlock.Create(options.Name),
					LunyPrimitiveType.Cylinder => ObjectCreateCylinderBlock.Create(options.Name),
					LunyPrimitiveType.Plane => ObjectCreatePlaneBlock.Create(options.Name),
					LunyPrimitiveType.Quad => ObjectCreateQuadBlock.Create(options.Name),
					var _ => ObjectCreateEmptyBlock.Create(options.Name),
				},
				ObjectCreationMode.Prefab => ObjectCreatePrefabBlock.Create(options.Name, options.AssetName),
				ObjectCreationMode.Clone => ObjectCreateCloneBlock.Create(options.Name, options.AssetName),
				var _ => throw new NotImplementedException(
					$"{nameof(ObjectBuilder<ObjectBuilderNameSet>)}: Mode {options.Mode} is not implemented."),
			};

			((ILunyScriptInternal)script).FinalizeToken(token);
			return block;
		}
	}
}
