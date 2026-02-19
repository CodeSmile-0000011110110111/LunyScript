using LunyScript.BlockBuilders;
using LunyScript.Blocks;
using System;

namespace LunyScript.Api
{
	public readonly struct PrefabApi
	{
		private readonly IScript _script;
		internal PrefabApi(IScript script) => _script = script;

		public ScriptActionBlock Instantiate(String prefabName) => _script.Object.Create(prefabName).From(prefabName).Do();
	}
}
