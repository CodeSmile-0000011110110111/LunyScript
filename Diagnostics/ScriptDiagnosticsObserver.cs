using Luny;
using System;

namespace LunyScript.Diagnostics
{
	/// <summary>
	/// Engine lifecycle observer for diagnostics tooling. Autoloaded by reflection via ILunyEngineObserver discovery.
	/// Exposes static startup/shutdown events so editor windows and runtime diagnostics tools can track engine lifetime
	/// without coupling to engine-specific lifecycle methods.
	/// </summary>
	public sealed class ScriptDiagnosticsObserver : ILunyEngineObserver
	{
		/// <summary>
		/// Fired when the engine starts up and the singleton Instance becomes valid.
		/// Subscribe to this event when Instance is null to be notified when diagnostics become available.
		/// </summary>
		public static event Action<ScriptDiagnosticsObserver> OnDiagnosticsStartup;

		/// <summary>
		/// Fired just before the engine shuts down. Instance is still valid when this fires.
		/// </summary>
		public static event Action<ScriptDiagnosticsObserver> OnDiagnosticsShutdown;
		public static ScriptDiagnosticsObserver Instance { get; private set; }

		internal static void ResetStaticFields()
		{
			Instance = null;
			OnDiagnosticsStartup = null;
			OnDiagnosticsShutdown = null;
		}

		void ILunyEngineObserver.OnEngineStartup()
		{
			Instance = this;
			OnDiagnosticsStartup?.Invoke(this);
		}

		void ILunyEngineObserver.OnEngineHeartbeat() {}

		void ILunyEngineObserver.OnEngineFrameUpdate() {}

		void ILunyEngineObserver.OnEngineShutdown()
		{
			OnDiagnosticsShutdown?.Invoke(this);
			ResetStaticFields();
		}
	}
}
