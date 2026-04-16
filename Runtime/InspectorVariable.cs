using Luny;
using System;

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

		public Variable ToVariable() => Type switch
		{
			Variable.ValueType.Boolean => Variable.Named(BoolValue, Name),
			Variable.ValueType.Number => Variable.Named(NumberValue, Name),
			var _ => Variable.Named(TextValue, Name),
		};

		public void FromVariable(String name, Variable v)
		{
			Name = name;
			Type = v.Type;
			BoolValue = v.AsBoolean();
			NumberValue = v.AsDouble();
			TextValue = v.AsString();
		}
	}
}
