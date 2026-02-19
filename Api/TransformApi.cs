using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Blocks.Transform;
using System;

namespace LunyScript.Api
{
	public readonly struct TransformApi
	{
		private readonly Script _script;

		internal TransformApi(Script script) => _script = script;

		public TransformTranslateBlock Shift(VariableBlock direction, Double speed = 1f) =>
			TransformTranslateBlock.Create(direction, speed, LunySpace.World);

		public TransformTranslateBlock Move(VariableBlock direction, Double speed = 1f) =>
			TransformTranslateBlock.Create(direction, speed, LunySpace.Self);
	}
}
