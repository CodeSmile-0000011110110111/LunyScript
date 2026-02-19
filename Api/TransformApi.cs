using LunyScript.Blocks;
using LunyScript.Blocks.Transform;
using System;

namespace LunyScript.Api
{
	public readonly struct TransformApi
	{
		private readonly Script _script;

		internal TransformApi(Script script) => _script = script;

		public TransformMoveBlock Move(VariableBlock direction, Double speed = 1f) => TransformMoveBlock.Create(direction, speed);
	}
}
