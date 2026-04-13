using Luny;

namespace LunyScript
{
	public interface ITransformBuilderState {}
	public interface ITransformBuilderReady : ITransformBuilderState {}

	public struct TransformBuilderReady : ITransformBuilderReady {}

	public readonly partial struct TransformBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal TransformBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}
	}
}
