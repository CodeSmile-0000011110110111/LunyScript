using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public readonly struct RigidbodyAddAngularForceBuilder
	{
		internal readonly RigidbodyAddForceOptions Options;

		internal static RigidbodyAddAngularForceBuilder CreateLocalForce(Script script, VariableBlock amount, LunyAxis axis,
			Boolean isImpulse, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyAddAngularForceBuilder),
				"Rigidbody.Dynamic.AddAngularForce(axis)");
			var options = new RigidbodyAddForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false,
				IsImpulse = isImpulse, IgnoreMass = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyAddAngularForceBuilder(options);
		}

		internal static RigidbodyAddAngularForceBuilder CreateWorldForce(Script script, LunyVector3 torque, Boolean isImpulse,
			LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyAddAngularForceBuilder),
				"Rigidbody.Dynamic.AddAngularForce(vector)");
			var options = new RigidbodyAddForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Vector = torque, UseVector = true,
				IsImpulse = isImpulse, IgnoreMass = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyAddAngularForceBuilder(options);
		}

		internal RigidbodyAddAngularForceBuilder(in RigidbodyAddForceOptions options)
		{
			Options = options;
			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyAddAngularForceBuilder b) => Finish(b.Options);

		/// <summary> Ignore moment of inertia when applying the torque. </summary>
		public RigidbodyAddAngularForceBuilder IgnoreMass() => new(Options with { IgnoreMass = true });

		/// <summary> Apply torque in world space instead of local space. </summary>
		public ActionBlock InWorldSpace() => Finish(Options with { Space = LunyTransformSpace.World });

		private static ActionBlock Finish(in RigidbodyAddForceOptions options)
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
