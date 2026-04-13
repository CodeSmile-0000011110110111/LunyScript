using Luny;
using System;

namespace LunyScript
{
	public readonly struct PrefabBuilder
	{
		private readonly Script _script;
		private readonly LunyStackTrace _trace;

		internal PrefabBuilder(Script script, LunyStackTrace trace)
		{
			_script = script;
			_trace = trace;
		}

		public ObjectCreateBuilder<ObjectBuilderNameSet> Instantiate(String prefabName) =>
			new ObjectBuilder(_script, _trace).Create(prefabName).From(prefabName);
	}
}
