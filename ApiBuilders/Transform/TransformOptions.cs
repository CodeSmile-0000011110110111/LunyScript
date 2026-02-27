using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Transform
{
	public interface ITransformBuilderState {}
	public interface ITransformBuilderReady : ITransformBuilderState {}

	public struct TransformBuilderReady : ITransformBuilderReady {}

	internal struct TransformLookAtOptions
	{
		public ILunyObject Target;
		public LunyVector3 WorldUp;
		public LunyVector3 AxisLock;
	}

	internal struct TransformTowardsObjectOptions
	{
		public ILunyObject Target;
		public Double Speed;
		public Double DeadZone;
		public Boolean LockX;
		public Boolean LockY;
		public Boolean LockZ;
		public Double Responsiveness;
	}

	internal struct TransformTowardsVariableOptions
	{
		public VariableBlock TargetScale;
		public Double Speed;
		public Double DeadZone;
		public Boolean LockX;
		public Boolean LockY;
		public Boolean LockZ;
		public Double Responsiveness;
	}
}
