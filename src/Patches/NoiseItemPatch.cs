using HarmonyLib;

namespace SCP1507.Patches;

[HarmonyPatch(typeof(NoisemakerProp))]
internal class NoiseItemPatch
{
    [HarmonyPatch("ItemActivate")]
    [HarmonyPostfix]
    private static void PostFix(NoisemakerProp __instance, bool used, bool buttonDown = true)
    {
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            if (__instance.name == "Airhorn(Clone)")
            {
                FlamingoManager.FlamingoManager.Instance.AirHornCheck(__instance.playerHeldBy);
            }

            if (__instance.name == "Clownhorn(Clone)")
            {
                FlamingoManager.FlamingoManager.Instance.ClownHornCheck(__instance.playerHeldBy);
                
            }
        }
        
    }
}