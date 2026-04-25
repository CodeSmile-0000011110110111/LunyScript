using Luny;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LunyScript
{
	[Serializable]
	public abstract class UnityReferences<T> where T : Object
	{
		private static T _placeholder;
		[SerializeField] private T[] _array = System.Array.Empty<T>();

		private Boolean _didAcquirePlaceholder;

		public T this[Int32 index]
		{
			get => IsValidIndex(index) ? _array[index] : Placeholder;
			set
			{
				if (index < 0)
					return;

				if (index >= _array.Length)
					System.Array.Resize(ref _array, index + 1);

				_array[index] = value;
			}
		}
		public T Placeholder
		{
			get
			{
				if (!_didAcquirePlaceholder)
				{
					_didAcquirePlaceholder = true;
					_placeholder = CreatePlaceholder();
					LunyLogger.LogWarning($"Using {typeof(T).Name} placeholder: {_placeholder}", this);
				}

				return _placeholder;
			}
		}
		public T[] Array { get => _array; set => _array = value; }

		public T FirstOrNull => this[0];
		public Int32 Length => _array?.Length ?? 0;

		public static implicit operator T[](UnityReferences<T> refs) => refs.Array;

		protected abstract T CreatePlaceholder();

		private Boolean IsValidIndex(Int32 index) => _array != null && index >= 0 && index < _array.Length;

		public override String ToString() => $"{GetType().Name}[{Length}]";
	}

	[Serializable] public sealed class GameObjectArray : UnityReferences<GameObject>
	{
		protected override GameObject CreatePlaceholder()
		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.name = "[PLACEHOLDER]";
			go.transform.localRotation = Quaternion.Euler(45, 45, 45);
			go.AddComponent<BoxCollider>();
			go.AddComponent<Rigidbody>();
			return go;
		}
	}

	[Serializable] public sealed class ScriptableObjectArray : UnityReferences<ScriptableObject>
	{
		protected override ScriptableObject CreatePlaceholder() => null;
	}

	[Serializable] public sealed class MaterialArray : UnityReferences<Material>
	{
		protected override Material CreatePlaceholder() => new(Shader.Find("Universal Render Pipeline/Simple Lit"));
	}
}
