using Luny;

namespace LunyScript.Blocks
{
	internal sealed class SceneReloadBlock : ScriptActionBlock
	{
		public static ScriptActionBlock Create() => new SceneReloadBlock();

		private SceneReloadBlock() {}

		protected internal  override void Execute(IScriptRuntimeContext runtimeContext) => LunyEngine.Instance.Scene.ReloadScene();
	}
}
