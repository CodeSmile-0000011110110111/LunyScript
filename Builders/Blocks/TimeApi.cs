using Luny;

namespace LunyScript
{
	public readonly struct TimeApi
	{
		private readonly Script _script;
		private readonly StackTrace _trace;

		internal TimeApi(Script script, StackTrace trace)
		{
			_script = script;
			_trace = trace;
		}
	}
}
