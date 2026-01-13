using Dalamud.Interface;

using Pathfinder.Config;
using Pathfinder.Interop;
using Pathfinder.Objects.Data;
using Pathfinder.Services.Core.Attributes;

namespace Pathfinder.Interface.Shared;

[LocalService]
public class ObjectUiCtx {
	private readonly IUiBuilder _ui;
	private readonly ConfigService _cfg;
	
	public ObjectUiCtx(IUiBuilder ui,  ConfigService cfg) {
		this._ui = ui;
		this._cfg = cfg;
	}
	
	// Hover ctx
    
	public int SetterId;
	private ulong LastUpdateFrame;
	
	private ObjectInfo? _hover;
	public ObjectInfo? Hovered {
		get {
			var result = this._hover != null && this.LastUpdateFrame >= this._ui.FrameCount - 1 ? this._hover : null;
			if (result == null) this.UpdateHoveredHighlight(false);
			return result;
		}
		private set {
			this.LastUpdateFrame = this._ui.FrameCount;
			this._hover = value;
		}
	}

	public void SetHovered(ObjectInfo? value, int id = -1) {
		if (this.Hovered != null || value != this.Hovered)
			this.UpdateHoveredHighlight(false);

		this.Hovered = value;
		this.SetterId = id;

		this.UpdateHoveredHighlight(true);
	}

	private unsafe void UpdateHoveredHighlight(bool state) {
		var drawObject = (DrawObject*)this._hover?.Address;
		if (drawObject == null) return;

		if (state)
			drawObject->OutlineFlags = this._cfg.Get().Table.HighlightOnHover;
		else
			drawObject->OutlineFlags = OutlineChoice.None;
	}

	public bool HoveredThisFrame => this.LastUpdateFrame == this._ui.FrameCount;
}
