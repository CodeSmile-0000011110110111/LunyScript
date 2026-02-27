using System;

namespace LunyScript.Api.Coroutine.Timer
{
	public interface ITimerBuilderState {}
	/// <summary>Initial state — next: call <c>In()</c> or <c>Every()</c>.</summary>
	public interface ITimerBuilderStart : ITimerBuilderState {}
	/// <summary>Amount set — next: call <c>Seconds()</c>, <c>Milliseconds()</c>, or <c>Minutes()</c>.</summary>
	public interface ITimerAmountSet : ITimerBuilderState {}
	/// <summary>Unit chosen — ready to finalize via <c>Do()</c>.</summary>
	public interface ITimerUnitSet : ITimerBuilderState {}

	public struct TimerBuilderStart : ITimerBuilderStart {}
	public struct TimerAmountSet : ITimerAmountSet {}
	public struct TimerUnitSet : ITimerUnitSet {}

	internal struct TimerOptions
	{
		internal String Name;
		internal Double Amount;
		internal Coroutines.Coroutine.Continuation Continuation;
		internal Double DurationInSeconds; // set after unit chosen
	}
}
