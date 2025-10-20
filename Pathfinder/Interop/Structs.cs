using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Pathfinder.Interop;

// VfxObject :: GameObject -> 2A0 (VfxResourceInstance)
// VfxResourceInstance -> 08 (Unk)
// Unk -> 18 (ApricotResourceHandle::ResourceHandle)
// ApricotResourceHandle -> 48 (FilePath)

[StructLayout(LayoutKind.Explicit, Size = 0x380)]
public struct VfxObject {
	[FieldOffset(0)] public Object Object;

	// [FieldOffset(0x260)] public Vector4 Color; ? see KT

	[FieldOffset(0x2A0)] public unsafe VfxResourceInstance* ResourceInstance;
}

[StructLayout(LayoutKind.Explicit, Size = 0xD0)]
public struct VfxResourceInstance {
	[FieldOffset(0)] public unsafe nint* __vfTable;

	[FieldOffset(0x08)] public unsafe VfxResourceUnk* ResourceUnk;

	// [FieldOffset(0x60)] public unsafe ResourceHandle* Handle; ? see KT
}

[StructLayout(LayoutKind.Explicit, Size = 0x20)]
public struct VfxResourceUnk {
	[FieldOffset(0)] public unsafe nint* __vfTable;

	[FieldOffset(0x18)] public unsafe ApricotResourceHandle* ApricotResourceHandle;
}

[StructLayout(LayoutKind.Explicit, Size = 0xC8)]
public struct ApricotResourceHandle {
	[FieldOffset(0)] public unsafe nint* __vfTable;
	[FieldOffset(0)] public ResourceHandle Handle;
	// todo: what else is in here?
}
