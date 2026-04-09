using Luny;

namespace LunyScript
{
	public struct ComponentApi
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal ComponentApi(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}
	}
}
