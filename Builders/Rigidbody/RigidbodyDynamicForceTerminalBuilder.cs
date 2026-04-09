using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	internal record RigidbodyForceOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;
		public VariableBlock Amount;
		public LunyAxis Axis;
		public LunyVector3 Vector;
		public Boolean UseVector;
		public Boolean IsImpulse;
		public Boolean IgnoreMass;
		public Boolean HasAtPositionOffset;
		public LunyVector3 AtPositionOffset;
		public LunyObjectRef AtPositionChildRef;
		public LunyTransformSpace Space;
	}

	public readonly struct RigidbodyDynamicForceTerminalBuilder
	{
		internal readonly RigidbodyForceOptions Options;

		internal static RigidbodyDynamicForceTerminalBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyAxis axis,
			Boolean isImpulse, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyDynamicForceTerminalBuilder), "Rigidbody.Dynamic.AddForce(axis)");
			var options = new RigidbodyForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false,
				IsImpulse = isImpulse, IgnoreMass = false,
				HasAtPositionOffset = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyDynamicForceTerminalBuilder(options);
		}

		internal static RigidbodyDynamicForceTerminalBuilder CreateVector(Script script, LunyVector3 force, Boolean isImpulse, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyDynamicForceTerminalBuilder), "Rigidbody.Dynamic.AddForce(vector)");
			var options = new RigidbodyForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Vector = force, UseVector = true,
				IsImpulse = isImpulse, IgnoreMass = false,
				HasAtPositionOffset = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyDynamicForceTerminalBuilder(options);
		}

		internal RigidbodyDynamicForceTerminalBuilder(in RigidbodyForceOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyDynamicForceTerminalBuilder b) => Finish(b.Options);

		/// <summary> Ignore mass when applying the force (pure acceleration). </summary>
		public RigidbodyDynamicForceTerminalBuilder IgnoreMass() => new(Options with { IgnoreMass = true });

		/// <summary> Apply force at a local-space offset position, generating torque. </summary>
		public RigidbodyDynamicForceTerminalBuilder AtPosition(LunyVector3 localOffset) => new(Options with
		{
			HasAtPositionOffset = true, AtPositionOffset = localOffset, AtPositionChildRef = null,
		});

		/// <summary>
		/// Apply force at the world position of a child object.
		/// The child must exist at build time — its world position is captured immediately and stored in the block.
		/// </summary>
		public RigidbodyDynamicForceTerminalBuilder AtPosition(LunyObjectRef childRef) =>
			new(Options with { HasAtPositionOffset = false, AtPositionChildRef = childRef });

		/// <summary> Apply force in world space instead of local space. </summary>
		public ActionBlock InWorldSpace() => Finish(Options with { Space = LunyTransformSpace.World });

		internal ActionBlock Finish() => Finish(Options);

		private static ActionBlock Finish(in RigidbodyForceOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			var forceMode = ToForceMode(options.IsImpulse, options.IgnoreMass);
			if (options.AtPositionChildRef != null)
			{
				var child = options.AtPositionChildRef.Value;
				if (child == null || !child.IsValid)
				{
					LunyLogger.LogWarning(
						$"{nameof(RigidbodyDynamicForceTerminalBuilder)}: {nameof(AtPosition)} child '{options.AtPositionChildRef}' not found or invalid — block will not be created");
					return null;
				}
				var worldPosition = child.Transform.Position;
				return options.UseVector
					? RigidbodyDynamicAddForceAtPositionBlock.CreateVectorWithWorldPosition(options.Vector, forceMode, worldPosition,
						options.Trace)
					: RigidbodyDynamicAddForceAtPositionBlock.CreateAxisWithWorldPosition(options.Amount, options.Axis, forceMode,
						worldPosition, options.Trace);
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

		private static LunyForceMode ToForceMode(Boolean isImpulse, Boolean ignoreMass)
		{
			if (!isImpulse && !ignoreMass)
				return LunyForceMode.Force;
			if (!isImpulse && ignoreMass)
				return LunyForceMode.Acceleration;
			if (isImpulse && !ignoreMass)
				return LunyForceMode.Impulse;

			return LunyForceMode.VelocityChange;
		}
	}
}
