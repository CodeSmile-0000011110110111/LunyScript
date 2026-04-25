using Luny;
using System;
using UnityEngine;

namespace LunyScript.Blocks.Material
{
	public sealed class MaterialSetFromArrayBlock : ActionBlock
	{
		private GameObjectArray _targets;
		private MaterialArray _materials;
		private VariableBlock _materialIndex;
		private Boolean _useSharedMaterial;

		public static ActionBlock Create(MaterialArray materials, VariableBlock materialIndex, GameObjectArray targets = null,
			Boolean useSharedMaterial = true, LunyStackTrace trace = null) =>
			new MaterialSetFromArrayBlock(materials, materialIndex, targets, useSharedMaterial, trace);

		private MaterialSetFromArrayBlock(MaterialArray materials, VariableBlock materialIndex, GameObjectArray targets,
			Boolean useSharedMaterial, LunyStackTrace trace)
			: base(trace)
		{
			_targets = targets;
			_materials = materials;
			_materialIndex = materialIndex;
			_useSharedMaterial = useSharedMaterial;
		}

		protected internal override void Execute(IScriptRuntimeContext context)
		{
			var index = Mathf.Abs(_materialIndex.Variable.AsInt32());
			if (index >= _materials.Length)
				index %= _materials.Length;

			var material = _materials[index];
			if (material == null)
				return;

			if (_targets == null)
			{
				var go = context.LunyGameObject.NativeObject as GameObject;
				AssignRendererMaterial(go, material);
			}
			else
			{
				foreach (var target in _targets.Array)
				{
					if (target != null)
						AssignRendererMaterial(target, material);
				}
			}
		}

		private void AssignRendererMaterial(GameObject go, UnityEngine.Material material)
		{
			var renderers = go.GetComponents<Renderer>();
			foreach (var renderer in renderers)
			{
				if (_useSharedMaterial)
					renderer.sharedMaterial = material;
				else
					renderer.material = material;
			}
		}

		public override String ToString() => $"{_materials}[{_materialIndex}], targets:{_targets}, useShared:{_useSharedMaterial}";
	}
}
