using HarmonyLib;
namespace BossDirections.Patches {

	[HarmonyPatch(typeof(Fireplace), nameof(Fireplace.UseItem))]
    static class Fireplace_UseItem_Patch
    {
        static bool Prefix(Fireplace __instance, ref bool __result, Humanoid user, ItemDrop.ItemData item)
        {
            if (__instance.TryOffer(user, item))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Vegvisir), nameof(Vegvisir.UseItem))]
    static class Vegvisir_UseItem_Patch
    {
        static bool Prefix(Vegvisir __instance, ref bool __result, Humanoid user, ItemDrop.ItemData item)
        {
            if (__instance.TryOffer(user, item))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.UseItem))]
    static class OfferingBowl_UseItem_Patch
    {
        static bool Prefix(OfferingBowl __instance, ref bool __result, Humanoid user, ItemDrop.ItemData item)
        {
            if (__instance.TryOffer(user, item))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}