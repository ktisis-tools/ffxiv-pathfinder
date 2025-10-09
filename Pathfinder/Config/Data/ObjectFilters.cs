using System;

namespace Pathfinder.Config.Data; 

[Flags]
public enum WorldObjectType {
	None = 0,
	
	BgObject = 0x1,
	Terrain = 0x2,
	Chara = 0x4,
	Vfx = 0x8,
	
	Human = 0x10,
	DemiHuman = 0x20,
	Monster = 0x40,
	Weapon = 0x80,
	
	// __Sound = 0x100, TODO
	
	All = 0xFF
}

public class ObjectFilters {
	public WorldObjectType Flags = WorldObjectType.All;

	public string SearchString = string.Empty;

	public FilterConstraint MinRadius = new() { Value = 0.5f };
	public FilterConstraint MaxRadius = new() { Value = 5.0f };
}

public class FilterConstraint {
	public bool Enabled = true;
	public float Value = 1.0f;
}
