using System;

namespace LunyScript.ApiBuilders.Blocks
{
	public readonly struct TimeApi
	{
		private readonly Script _script;

		internal TimeApi(Script script)
		{
			_script = script;
			ElapsedSeconds = Double.NaN;
		}

		public readonly Double ElapsedSeconds; // TODO: implementation
	}
}
