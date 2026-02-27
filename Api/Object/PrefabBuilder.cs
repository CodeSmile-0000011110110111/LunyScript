using System;

namespace LunyScript.Api.Object
{
	public readonly struct PrefabBuilder
	{
		private readonly Script _script;
		internal PrefabBuilder(Script script) => _script = script;

		public ObjectCreateBuilder<ObjectBuilderNameSet> Instantiate(String prefabName) => new ObjectBuilder(_script).Create(prefabName).From(prefabName);
	}
}
