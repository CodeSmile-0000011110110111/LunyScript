using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;

namespace LunyScript
{
	public readonly struct TransformSetRotationTerminalBuilder
	{
		private readonly Script _script;
		private readonly BuilderToken _token;
		private readonly VariableBlock<LunyQuaternion> _rotation;
		private readonly LunyTransformSpace _space;
		private readonly StackTrace _trace;

		internal static TransformSetRotationTerminalBuilder Create(Script script, VariableBlock<LunyQuaternion> rotation,
			LunyTransformSpace space, StackTrace trace)
		{
			var token = script.CreateBuilderToken(nameof(TransformSetRotationTerminalBuilder), "Transform.SetRotation()");
			return new TransformSetRotationTerminalBuilder(script, token, rotation, space, trace);
		}

		private TransformSetRotationTerminalBuilder(Script script, BuilderToken token, VariableBlock<LunyQuaternion> rotation,
			LunyTransformSpace space, StackTrace trace)
		{
			_script = script;
			_token = token;
			_rotation = rotation;
			_space = space;
			_trace = trace;

			var self = this;
			token.AutoFinish = () => self.Finish();
		}

		/// <summary> Apply rotation set in world space instead of local space. </summary>
		public TransformRotationSetBlock InWorldSpace() => Finish(LunyTransformSpace.World);

		internal TransformRotationSetBlock Finish() => Finish(_space);

		private TransformRotationSetBlock Finish(LunyTransformSpace space)
		{
			_script.MarkBuilderTokenFinished(_token);
			return TransformRotationSetBlock.Create(_rotation, space, _trace);
		}
	}
}
