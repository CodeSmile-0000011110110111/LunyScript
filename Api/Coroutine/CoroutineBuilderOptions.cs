namespace LunyScript.Api.Coroutine
{
	public interface ICoroutineBuilderState {}
	/// <summary>Frame-update unit — <c>WhenStarted/Stopped/Paused/Resumed</c> and <c>Do()</c> available.</summary>
	public interface ICoroutineFrameUnit : ICoroutineBuilderState {}
	/// <summary>Heartbeat unit — <c>WhenStarted/Stopped/Paused/Resumed</c> and <c>Do()</c> available.</summary>
	public interface ICoroutineHeartbeatUnit : ICoroutineBuilderState {}
	/// <summary>Shared base for both coroutine unit states — enables shared lifecycle extension methods.</summary>
	public interface ICoroutineReadyUnit : ICoroutineBuilderState {}

	public struct CoroutineFrameUnit : ICoroutineFrameUnit, ICoroutineReadyUnit {}
	public struct CoroutineHeartbeatUnit : ICoroutineHeartbeatUnit, ICoroutineReadyUnit {}
}
