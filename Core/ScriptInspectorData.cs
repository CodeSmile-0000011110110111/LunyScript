using Luny;
using System;
using System.Collections.Generic;

namespace LunyScript
{
	/// <summary>
	/// Serializable representation of a single local variable for Inspector editing.
	/// </summary>
	[Serializable]
	public sealed class InspectorVariable
	{
		public String Name;
		public Variable.ValueType Type;
		public Boolean BoolValue;
		public Double NumberValue;
		public String TextValue;

		private WeakReference<Table.VarHandleBase> _varHandle;

		public Variable ToVariable()
		{
			if (_varHandle != null && _varHandle.TryGetTarget(out var h) && h is Table.VarHandle handle)
				return handle.Variable;

			return Type switch
			{
				Variable.ValueType.Boolean => Variable.Named(BoolValue, Name),
				Variable.ValueType.Number => Variable.Named(NumberValue, Name),
				var _ => Variable.Named(TextValue, Name),
			};
		}

		public void FromVariable(String name, Variable v)
		{
			Name = name;
			Type = v.Type;
			BoolValue = v.AsBoolean();
			NumberValue = v.AsDouble();
			TextValue = v.AsString();
		}

		internal void SetVarHandle(Table.VarHandleBase varHandle) => _varHandle = new WeakReference<Table.VarHandleBase>(varHandle);
	}

	/// <summary>
	/// Contains design-time data for a script, mainly Inspector-assigned variable values.
	/// </summary>
	[Serializable]
	public sealed class ScriptInspectorData
	{
		public List<InspectorVariable> Variables = new();
	}
}
