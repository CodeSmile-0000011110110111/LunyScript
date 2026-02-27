using System;

namespace LunyScript.Api.Blocks
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
