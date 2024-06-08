using HarmonyLib;

namespace SCP1507.Patches;

[HarmonyPatch(typeof(GrabbableObject))]
internal class PhysicProp
{
    [HarmonyPatch("GrabItem")]
    [HarmonyPostfix]
    private static void PostFix(GrabbableObject __instance)
    {
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            FlamingoManager.FlamingoManager.Instance.PickupItemTrigger(__instance.playerHeldBy, __instance.transform.position);
        }
    }
}