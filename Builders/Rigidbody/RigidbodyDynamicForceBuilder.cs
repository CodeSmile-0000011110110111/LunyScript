using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public static class RigidbodyDynamicForceBuilderExtensions
	{
		/// <summary> Ignore mass when applying the force (pure acceleration). </summary>
		public static RigidbodyDynamicForceBuilder IgnoreMass(this RigidbodyDynamicForceBuilder b) => new(b.Options with { IgnoreMass = true });

		/// <summary> Apply force at a local-space offset position, generating torque. </summary>
		public static RigidbodyDynamicForceBuilder AtPosition(this RigidbodyDynamicForceBuilder b, LunyVector3 localOffset) => new(
			b.Options with
			{
				HasAtPositionOffset = true, AtPositionOffset = localOffset, AtPositionChildRef = null,
			});

		/// <summary>
		/// Apply force at the world position of a child object.
		/// The child must exist at build time — its world position is captured immediately and stored in the block.
		/// </summary>
		public static RigidbodyDynamicForceBuilder AtPosition(this RigidbodyDynamicForceBuilder b, LunyObjectRef childRef) =>
			new(b.Options with { HasAtPositionOffset = false, AtPositionChildRef = childRef });

		/// <summary> Apply force in world space instead of local space. </summary>
		public static RigidbodyDynamicForceBuilder InWorldSpace(this RigidbodyDynamicForceBuilder b) =>
			new(b.Options with { Space = LunyTransformSpace.World });
	}

	public readonly struct RigidbodyDynamicForceBuilder
	{
		internal readonly RigidbodyAddForceOptions Options;

		internal static RigidbodyDynamicForceBuilder CreateLocalForce(Script script, VariableBlock amount, LunyAxis axis,
			Boolean isImpulse, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyDynamicForceBuilder), "Rigidbody.Dynamic.AddForce(axis)");
			var options = new RigidbodyAddForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false,
				IsImpulse = isImpulse, IgnoreMass = false,
				HasAtPositionOffset = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyDynamicForceBuilder(options);
		}

		internal static RigidbodyDynamicForceBuilder CreateWorldForce(Script script, LunyVector3 force, Boolean isImpulse, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyDynamicForceBuilder), "Rigidbody.Dynamic.AddForce(vector)");
			var options = new RigidbodyAddForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Vector = force, UseVector = true,
				IsImpulse = isImpulse, IgnoreMass = false,
				HasAtPositionOffset = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyDynamicForceBuilder(options);
		}

		internal RigidbodyDynamicForceBuilder(in RigidbodyAddForceOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyDynamicForceBuilder b) => Finish(b.Options);

		private static ActionBlock Finish(in RigidbodyAddForceOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			var forceMode = RigidbodyBuilder.ToForceMode(options.IsImpulse, options.IgnoreMass);
			if (options.AtPositionChildRef != null)
			{
				return options.UseVector
					? RigidbodyDynamicAddForceAtPositionBlock.CreateVectorWithWorldPosition(options.Vector, forceMode,
						options.AtPositionChildRef,
						options.Trace)
					: RigidbodyDynamicAddForceAtPositionBlock.CreateAxisWithWorldPosition(options.Amount, options.Axis, forceMode,
						options.AtPositionChildRef, options.Trace);
			}
			if (options.HasAtPositionOffset)
			{
				return options.UseVector
					? RigidbodyDynamicAddForceAtPositionBlock.CreateVectorWithLocalOffset(options.Vector, forceMode, options.AtPositionOffset,
						options.Trace)
					: RigidbodyDynamicAddForceAtPositionBlock.CreateAxisWithLocalOffset(options.Amount, options.Axis, forceMode,
						options.AtPositionOffset, options.Trace);
			}
			return options.UseVector
				? RigidbodyDynamicAddForceBlock.CreateVector(options.Vector, forceMode, options.Space, options.Trace)
				: RigidbodyDynamicAddForceBlock.CreateAxisRelative(options.Amount, options.Axis, forceMode, options.Space, options.Trace);
		}
	}
}
