using HarmonyLib;
namespace BossDirections.Patches {

	[HarmonyPatch(typeof(Player), "UseHotbarItem")]

	public class Player_UseHotbarItem_Patch {
		[HarmonyPrefix]
		static bool Prefix(Player __instance, int index) {
			ItemDrop.ItemData item = __instance.GetInventory().GetItemAt(index - 1, 0);
			var offered = BossDirections.TryOffer(item);
			return !offered;
		}
	}

}