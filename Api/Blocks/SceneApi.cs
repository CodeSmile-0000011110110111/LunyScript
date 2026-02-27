using LunyScript.Blocks;

namespace LunyScript.Api.Blocks
{
	/// <summary>
	/// Provides operations for managing Scenes and accessing the scene hierarchy.
	/// </summary>
	public readonly struct SceneApi
	{
		private readonly Script _script;
		internal SceneApi(Script script) => _script = script;

		public ScriptActionBlock Reload() => SceneReloadBlock.Create();
	}
}
