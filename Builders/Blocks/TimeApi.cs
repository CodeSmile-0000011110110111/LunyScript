using Luny;

namespace LunyScript
{
	public readonly struct TimeApi
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal TimeApi(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}
	}
}
