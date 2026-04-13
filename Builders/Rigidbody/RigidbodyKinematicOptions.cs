using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	internal record RigidbodyKinematicOptions
	{
		public Script Script;
		public BuilderToken Token;
		public LunyStackTrace Trace;

		public VariableBlock Amount;
		public LunyAxis Axis;
		public Boolean UseVector;
		public LunyVector3 Vector;
		public LunyVector3 EulerDelta;
		public LunyTransformSpace Space;
	}
}
