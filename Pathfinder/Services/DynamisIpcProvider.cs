using System;

using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Pathfinder.Services.Core.Attributes;

namespace Pathfinder.Services;

// ty @Glorou

[GlobalService]
public class DynamisIpcProvider : IDisposable {
	private readonly IDalamudPluginInterface _dpi;
	private readonly ICallGateSubscriber<uint, uint, ulong, Version, object?>? _initialized;
	private readonly ICallGateSubscriber<object?>? _disposing;
	private ICallGateSubscriber<nint, object?>? _drawTooltip;
	private ICallGateSubscriber<nint, Func<string?>?, object?>? _openMenu;

	private readonly ValueTuple<uint, uint, ulong> RequiredVersion = (1, 6, 0);

	public bool Available;

	public DynamisIpcProvider(IDalamudPluginInterface dpi) {
		this._dpi = dpi;

		try {
			this._initialized = dpi.GetIpcSubscriber<uint, uint, ulong, Version, object?>("Dynamis.ApiInitialized");
			this._initialized.Subscribe(this.Initialize);
			this._disposing = dpi.GetIpcSubscriber<object?>("Dynamis.ApiDisposing");
			this._disposing.Subscribe(this.IpcDispose);
			this.CheckApi();
		} catch (Exception err) {
			Pathfinder.Log.Debug($"[DynamisIpcProvider] - failed to subscribe: {err}");
		}
	}

	public void DrawTooltip(nint address) => this._drawTooltip?.InvokeAction(address);
	public void OpenMenu(nint address, string name) => this._openMenu?.InvokeAction(address, () => name);

	private void CheckApi() {
		if (this._dpi.GetIpcSubscriber<(uint, uint, ulong)>("Dynamis.GetApiVersion") is { } subscriber) {
			var version = subscriber.InvokeFunc();
			this.Initialize(version.Item1, version.Item2, version.Item3, new Version());
		}
	}

	private void Initialize(uint major, uint minor, ulong flags, Version _) {
		this.IpcDispose();
		var actual = (major, minor, flags);
		if (!this.IsCompatible(actual, this.RequiredVersion)) {
			Pathfinder.Log.Debug($"[DynamisIpcProvider] - failed to validate API version {actual} (expected at least {this.RequiredVersion})");
			return;
		}

		try {
			this._drawTooltip = this._dpi.GetIpcSubscriber<nint, object?>("Dynamis.ImGuiDrawPointerTooltipDetails.V1");
			this._openMenu = this._dpi.GetIpcSubscriber<nint, Func<string?>?, object?>("Dynamis.ImGuiOpenPointerContextMenu.V1");
		} catch (Exception err) {
			Pathfinder.Log.Debug($"[DynamisIpcProvider] - failed to initialize: {err}");
			return;
		}

		Pathfinder.Log.Debug($"[DynamisIpcProvider] - attached successfully!");
		this.Available = true;
	}

	private void IpcDispose() {
		if (!this.Available) return;
		Pathfinder.Log.Debug($"[DynamisIpcProvider] - detaching...");

		this._drawTooltip = null;
		this._openMenu = null;
		this.Available = false;
	}

	private bool IsCompatible((uint MajorVersion, uint MinorVersion, ulong FeatureFlags) actual, (uint MajorVersion, uint MinorVersion, ulong FeatureFlags) required)
		=> actual.MajorVersion == required.MajorVersion && actual.MinorVersion >= required.MinorVersion && (actual.FeatureFlags & required.FeatureFlags) == required.FeatureFlags;

	public void Dispose() {
		this.Available = false;
		this.IpcDispose();
		this._initialized?.Unsubscribe(this.Initialize);
		this._disposing?.Unsubscribe(this.IpcDispose);
	}
}
