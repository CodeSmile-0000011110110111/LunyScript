using Luny;
using System;
using UnityEngine;

namespace LunyScript.Blocks.Material
{
	public sealed class MaterialSetBlock : ActionBlock
	{
		private GameObject[] _targets;
		private UnityEngine.Material _material;
		private Boolean _useSharedMaterial;

		public static ActionBlock Create(GameObject[] targets, UnityEngine.Material material, Boolean useSharedMaterial,
			LunyStackTrace trace = null) => new MaterialSetBlock(targets, material, useSharedMaterial, trace);

		public static ActionBlock Create(GameObject[] targets, UnityEngine.Material material,
			LunyStackTrace trace = null) => new MaterialSetBlock(targets, material, true, trace);

		public static ActionBlock Create(UnityEngine.Material material, Boolean useSharedMaterial, LunyStackTrace trace = null) =>
			new MaterialSetBlock(null, material, useSharedMaterial, trace);

		public static ActionBlock Create(UnityEngine.Material material, LunyStackTrace trace = null) =>
			new MaterialSetBlock(null, material, true, trace);

		private MaterialSetBlock(GameObject[] targets, UnityEngine.Material material, Boolean useSharedMaterial, LunyStackTrace trace)
			: base(trace)
		{
			_targets = targets;
			_material = material;
			_useSharedMaterial = useSharedMaterial;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			if (_targets == null)
			{
				var go = context.LunyGameObject.NativeObject as GameObject;
				AssignRendererMaterial(go);
			}
			else
			{
				foreach (var target in _targets)
				{
					if (target != null)
						AssignRendererMaterial(target);
				}
			}
		}

		private void AssignRendererMaterial(GameObject go)
		{
			var renderers = go.GetComponents<Renderer>();
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
