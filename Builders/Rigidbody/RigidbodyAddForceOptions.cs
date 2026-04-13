using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript
{
	internal record RigidbodyAddForceOptions
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
}
