using System;
using UnityEngine;

namespace LunyScript
{
	public abstract class LunyScriptDataBehaviour : MonoBehaviour
	{
		[SerializeField] private String _key;

		public String Key { get => _key; set => _key = value; }
	}
}
