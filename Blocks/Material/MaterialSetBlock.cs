using Luny;
using System;
using UnityEngine;

namespace LunyScript.Blocks.Material
{
	public sealed class MaterialSetBlock : ActionBlock
	{
		private UnityEngine.Material _material;
		private Boolean _inChildren;
		private Boolean _useSharedMaterial;

		public static ActionBlock Create(UnityEngine.Material material, Boolean inChildren, Boolean useSharedMaterial, LunyStackTrace trace) =>
			new MaterialSetBlock(material, inChildren, useSharedMaterial, trace);

		private MaterialSetBlock(UnityEngine.Material material, Boolean inChildren, Boolean useSharedMaterial, LunyStackTrace trace)
			: base(trace)
		{
			_material = material;
			_inChildren = inChildren;
			_useSharedMaterial = useSharedMaterial;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var go = context.LunyGameObject.NativeObject as GameObject;
			var renderers = _inChildren ? go.GetComponentsInChildren<Renderer>() : go.GetComponents<Renderer>();

			foreach (var renderer in renderers)
			{
				if (_useSharedMaterial)
					renderer.sharedMaterial = _material;
				else
					renderer.material = _material;
			}
		}

		public override String ToString() => _material != null ? _material.name : nameof(MaterialSetBlock);
	}
}
