using Luny.Engine.Services;
using LunyScript.ApiBuilders.Event;
using System;

namespace LunyScript.Blocks.Guards
{
	internal sealed class CooldownGuard<T> : EventGuard where T : struct, ICollisionBuilderState
	{
		private ILunyTimeService _time;
		private Double _lastExecutionTime;
		private Double _cooldownInSeconds;

		public CooldownGuard(Double cooldownInSeconds, ILunyTimeService time)
		{
			_cooldownInSeconds = cooldownInSeconds;
			_time = time;
		}

		public override Boolean CanExecute()
		{
			var now = _time.ElapsedSeconds;
			var cooldownElapsedSeconds = now - _lastExecutionTime;
			var canRunAgain = cooldownElapsedSeconds >= _cooldownInSeconds;
			return canRunAgain;
		}

		public override void WillExecute() => _lastExecutionTime = _time.ElapsedSeconds;
	}
}
