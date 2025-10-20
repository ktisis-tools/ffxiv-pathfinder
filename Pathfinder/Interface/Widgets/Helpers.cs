using Dalamud.Interface;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

namespace Pathfinder.Interface.Widgets; 

public static class Helpers {
	public static void Hint(string text, FontAwesomeIcon icon = FontAwesomeIcon.QuestionCircle) {
		using (ImRaii.PushFont(UiBuilder.IconFont))
			using (ImRaii.PushColor(ImGuiCol.Text, 0x80FFFFFF))
				ImGui.Text(icon.ToIconString());

		HoverTooltip(text);
	}
	
	public static bool HoverTooltip(string text) {
		var hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
		if (hover) ImGui.SetTooltip(text);
		return hover;
	}
	
	public unsafe static bool DimColor(ImGuiCol col, float factor) {
		var ptr = ImGui.GetStyleColorVec4(col);
		if (ptr == null) return false;
		var value = *ptr;
		value.X *= factor;
		value.Y *= factor;
		value.Z *= factor;
		ImGui.PushStyleColor(col, value);
		return true;
	}

	public static uint ColorAlpha(uint color, byte alpha)
		=> color & 0x00FFFFFF | (uint)(alpha << 24);
}
