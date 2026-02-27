using Luny.Engine.Bridge;
using LunyScript.Blocks;
using System;

namespace LunyScript.ApiBuilders.Object
{
	public interface IObjectBuilderState {}
	public interface IObjectBuilderStart : IObjectBuilderState {}
	public interface IObjectBuilderNameSet : IObjectBuilderState, IObjectBuilderCanFinalize {}
	public interface IObjectBuilderCanFinalize {}

	public struct ObjectBuilderStart : IObjectBuilderStart {}
	public struct ObjectBuilderNameSet : IObjectBuilderNameSet {}

	/// <summary>
	/// Provides operations for objects.
	/// </summary>
	public readonly struct ObjectBuilder
	{
		private readonly Script _script;
		internal ObjectBuilder(Script script) => _script = script;

		public ScriptActionBlock Enable(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectEnableSelfBlock.Create() : ObjectEnableTargetBlock.Create(name);

		public ScriptActionBlock Disable(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDisableSelfBlock.Create() : ObjectDisableTargetBlock.Create(name);

		public ObjectCreateBuilder<ObjectBuilderNameSet> Create(String name)
		{
			var options = new ObjectCreateOptions { Name = name, Mode = ObjectCreationMode.Empty, LocalScale = LunyVector3.One };
			var token = _script.CreateBuilderToken(name, "ObjectCreateBuilder");
			return new ObjectCreateBuilder<ObjectBuilderNameSet>(_script, options, token);
		}

		public ScriptActionBlock Destroy(String name = null) =>
			String.IsNullOrEmpty(name) ? ObjectDestroySelfBlock.Create() : ObjectDestroyTargetBlock.Create(name);
	}
}
