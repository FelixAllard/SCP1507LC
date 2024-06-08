using HarmonyLib;
using UnityEngine;

namespace SCP1507.Patches;

[HarmonyPatch(typeof(NoisemakerProp))]
internal class NoiseItemPatch
{
    [HarmonyPatch("ItemActivate")]
    [HarmonyPrefix]
    private static bool PreFix(NoisemakerProp __instance, bool used, bool buttonDown = true)
    {
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            Debug.Log("Activated!");
            if (__instance.name == "Airhorn(Clone)")
            {
                Debug.Log("It's an Airhorn!");
                FlamingoManager.FlamingoManager.Instance.AirHornCheck(__instance.playerHeldBy);
            }

            if (__instance.name == "Clownhorn(Clone)")
            {
                Debug.Log("It's a clown Horn");
                FlamingoManager.FlamingoManager.Instance.ClownHornCheck(__instance.playerHeldBy);
                
            }
        }
        return true;
    }
}