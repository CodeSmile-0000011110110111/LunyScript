using Luny;
using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace LunyScript
{
	/// <summary>
	/// Contains design-time data for a script, mainly Inspector-assigned values and references.
	/// </summary>
	[Serializable]
	public sealed class ScriptInspectorData
	{
		public Variable _variable;
		public Variable<LunyVector3> _vectorVariable;

		public List<String> _references;
		public List<Double> _numbers;
		public List<Boolean> _flags;
		public List<String> _texts;

		public InspectorScript _script;
	}

	[Serializable]
	public sealed class InspectorScript : Script
	{
		public Double _inspectorScriptValue = 1.234;
		public override void Build(ScriptBuildContext context) => throw new NotImplementedException();
	}
}
