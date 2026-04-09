using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly struct RigidbodyDynamicAngularForceTerminalBuilder
	{
		internal readonly RigidbodyForceOptions Options;

		internal static RigidbodyDynamicAngularForceTerminalBuilder CreateAxisRelative(Script script, VariableBlock amount, LunyAxis axis,
			Boolean isImpulse, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyDynamicAngularForceTerminalBuilder),
				"Rigidbody.Dynamic.AddAngularForce(axis)");
			var options = new RigidbodyForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false,
				IsImpulse = isImpulse, IgnoreMass = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyDynamicAngularForceTerminalBuilder(options);
		}

		internal static RigidbodyDynamicAngularForceTerminalBuilder CreateVector(Script script, LunyVector3 torque, Boolean isImpulse,
			StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyDynamicAngularForceTerminalBuilder),
				"Rigidbody.Dynamic.AddAngularForce(vector)");
			var options = new RigidbodyForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Vector = torque, UseVector = true,
				IsImpulse = isImpulse, IgnoreMass = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyDynamicAngularForceTerminalBuilder(options);
		}

		internal RigidbodyDynamicAngularForceTerminalBuilder(in RigidbodyForceOptions options)
		{
			Options = options;

			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyDynamicAngularForceTerminalBuilder b) => Finish(b.Options);

		/// <summary> Ignore moment of inertia when applying the torque. </summary>
		public RigidbodyDynamicAngularForceTerminalBuilder IgnoreMass() => new(Options with { IgnoreMass = true });

		/// <summary> Apply torque in world space instead of local space. </summary>
		public ActionBlock InWorldSpace() => Finish(Options with { Space = LunyTransformSpace.World });

		internal ActionBlock Finish() => Finish(Options);

		private static ActionBlock Finish(in RigidbodyForceOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			var forceMode = ToForceMode(options.IsImpulse, options.IgnoreMass);
			return options.UseVector
				? RigidbodyDynamicAddAngularForceBlock.CreateVector(options.Vector, forceMode, options.Space, options.Trace)
				: RigidbodyDynamicAddAngularForceBlock.CreateAxisRelative(options.Amount, options.Axis, forceMode, options.Space,
					options.Trace);
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
