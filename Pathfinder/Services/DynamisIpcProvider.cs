using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Pathfinder.Services.Core.Attributes;

namespace Pathfinder.Services;

[GlobalService]
public class DynamisIpcProvider
{
    private readonly ICallGateSubscriber<(uint, uint)> _getApiVersion;
    private readonly ICallGateSubscriber<nint, object?> _inspectObject;
    private readonly ICallGateSubscriber<nint, object?> _addressTooltip;
    
    public DynamisIpcProvider(
        IDalamudPluginInterface dpi
    )
    {
        this._getApiVersion = dpi.GetIpcSubscriber<(uint, uint)>("Dynamis.GetApiVersion");
        this._inspectObject = dpi.GetIpcSubscriber<nint, object?>("Dynamis.InspectObject.V1");
        this._addressTooltip = dpi.GetIpcSubscriber<nint, object?>("Dynamis.ImGuiDrawPointerTooltipDetails.V1");
    }
    
    public bool IsCompatible((uint MajorVersion, uint MinorVersion, ulong FeatureFlags) actual, (uint MajorVersion, uint MinorVersion, ulong FeatureFlags) required)
        => actual.MajorVersion == required.MajorVersion && actual.MinorVersion >= required.MinorVersion && (actual.FeatureFlags & required.FeatureFlags) == required.FeatureFlags;

    public void InpectObject(nint address) => this._inspectObject.InvokeAction(address);

    public void DrawTooltip(nint address) => this._addressTooltip.InvokeFunc(address);
}