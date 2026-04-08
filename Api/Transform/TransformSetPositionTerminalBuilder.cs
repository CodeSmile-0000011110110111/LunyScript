using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public readonly struct TransformSetPositionTerminalBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly VariableBlock<LunyVector3> _position;
		private readonly LunyTransformSpace _space;
		private readonly StackTrace _trace;

		internal static TransformSetPositionTerminalBuilder Create(Script script, VariableBlock<LunyVector3> position,
			LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetPositionTerminalBuilder), "Transform.SetPosition()");
			return new TransformSetPositionTerminalBuilder(script, token, position, space, trace);
		}

		private TransformSetPositionTerminalBuilder(Script script, BuilderToken token, VariableBlock<LunyVector3> position,
			LunyTransformSpace space, StackTrace trace)
		{
			_script = script;
			_token = token;
			_position = position;
			_space = space;
			_trace = trace;

			var self = this;
			token.AutoFinish = () => self.Finish();
		}

		/// <summary> Apply position set in world space instead of local space. </summary>
		public TransformPositionSetBlock InWorldSpace() => Finish(LunyTransformSpace.World);

		internal TransformPositionSetBlock Finish() => Finish(_space);

		private TransformPositionSetBlock Finish(LunyTransformSpace space)
		{
			_script.MarkBuilderTokenFinished(_token);
			return TransformPositionSetBlock.Create(_position, space, _trace);
		}
	}
}
