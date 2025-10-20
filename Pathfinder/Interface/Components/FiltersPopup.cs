using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

using Pathfinder.Config;
using Pathfinder.Config.Data;
using Pathfinder.Services.Core.Attributes;

namespace Pathfinder.Interface.Components; 

[LocalService(ServiceFlags.Transient)]
public class FiltersPopup {
	public void Draw(string id, ConfigFile config) {
		using var popup = ImRaii.Popup(id);
		if (!popup) return;

		var filter = config.Filters;
		var showChara = filter.Flags.HasFlag(WorldObjectType.Chara);

		using var table = ImRaii.Table("##ObjectTypeTable", 2, ImGuiTableFlags.NoSavedSettings);
		if (!table.Success) return;

		ImGui.TableSetupColumn("Object Types");
		ImGui.TableSetupColumn("Character Models", showChara ? ImGuiTableColumnFlags.None : ImGuiTableColumnFlags.Disabled);
		ImGui.TableNextRow();

		ImGui.TableSetColumnIndex(0);
		this.DrawFilterFlag(config, WorldObjectType.Terrain);
		this.DrawFilterFlag(config, WorldObjectType.BgObject);
		this.DrawFilterFlag(config, WorldObjectType.Vfx);
		this.DrawFilterFlag(config, WorldObjectType.Chara, "Characters");

		if (showChara) {
			ImGui.TableSetColumnIndex(1);
			this.DrawFilterFlag(config, WorldObjectType.Human);
			this.DrawFilterFlag(config, WorldObjectType.DemiHuman);
			this.DrawFilterFlag(config, WorldObjectType.Monster);
			this.DrawFilterFlag(config, WorldObjectType.Weapon);
		}
	}

	private void DrawFilterFlag(ConfigFile config, WorldObjectType flag, string? label = null) {
		label ??= flag.ToString();
		using var _col = ImRaii.PushColor(ImGuiCol.Text, config.GetColor(flag));
		var value = config.Filters.Flags.HasFlag(flag);
		if (ImGui.Checkbox(label, ref value))
			config.Filters.Flags ^= flag;
	}
}
