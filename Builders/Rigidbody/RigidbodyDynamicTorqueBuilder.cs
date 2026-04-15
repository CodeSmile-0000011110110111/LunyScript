using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	public static class RigidbodyDynamicTorqueBuilderExtensions
	{
		/// <summary> Ignore moment of inertia when applying the torque. </summary>
		public static RigidbodyDynamicTorqueBuilder IgnoreMass(this RigidbodyDynamicTorqueBuilder b) =>
			new(b.Options with { IgnoreMass = true });

		/// <summary> Apply torque in world space instead of local space. </summary>
		public static RigidbodyDynamicTorqueBuilder InWorldSpace(this RigidbodyDynamicTorqueBuilder b) =>
			new(b.Options with { Space = LunyTransformSpace.World });
	}

	public readonly struct RigidbodyDynamicTorqueBuilder
	{
		internal readonly RigidbodyAddForceOptions Options;

		internal static RigidbodyDynamicTorqueBuilder CreateLocalForce(Script script, VariableBlock amount, LunyAxis axis,
			Boolean isImpulse, LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyDynamicTorqueBuilder), "Rigidbody.Dynamic.AddAngularForce(axis)");
			var options = new RigidbodyAddForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Amount = amount, Axis = axis, UseVector = false,
				IsImpulse = isImpulse, IgnoreMass = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyDynamicTorqueBuilder(options);
		}

		internal static RigidbodyDynamicTorqueBuilder CreateWorldForce(Script script, LunyVector3 torque, Boolean isImpulse,
			LunyStackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(RigidbodyDynamicTorqueBuilder), "Rigidbody.Dynamic.AddAngularForce(vector)");
			var options = new RigidbodyAddForceOptions
			{
				Script = script, Token = token, Trace = trace,
				Vector = torque, UseVector = true,
				IsImpulse = isImpulse, IgnoreMass = false, Space = LunyTransformSpace.Local,
			};
			return new RigidbodyDynamicTorqueBuilder(options);
		}

		internal RigidbodyDynamicTorqueBuilder(in RigidbodyAddForceOptions options)
		{
			Options = options;
			var capturedOptions = options;
			options.Token.AutoFinish = () => Finish(capturedOptions);
		}

		public static implicit operator ActionBlock(RigidbodyDynamicTorqueBuilder b) => Finish(b.Options);

		private static ActionBlock Finish(in RigidbodyAddForceOptions options)
		{
			options.Script.MarkBuilderTokenFinished(options.Token);
			var forceMode = RigidbodyBuilder.ToForceMode(options.IsImpulse, options.IgnoreMass);
			return options.UseVector
				? RigidbodyDynamicAddAngularForceBlock.CreateVector(options.Vector, forceMode, options.Space, options.Trace)
				: RigidbodyDynamicAddAngularForceBlock.CreateAxisRelative(options.Amount, options.Axis, forceMode, options.Space,
					options.Trace);
		}
	}
}
