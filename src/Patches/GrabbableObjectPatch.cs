using HarmonyLib;
using UnityEngine;

namespace SCP1507.Patches;

[HarmonyPatch(typeof(GrabbableObject))]
internal class GrabbableObjectPatch
{
    [HarmonyPatch("GrabServerRpc")]
    [HarmonyPostfix]
    private static void PostFix(GrabbableObject __instance)
    {
        Debug.Log("WE DID GRAB ITEM!");
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            FlamingoManager.FlamingoManager.Instance.PickupItemTrigger(__instance.playerHeldBy, __instance.transform.position);
        }
    }
}