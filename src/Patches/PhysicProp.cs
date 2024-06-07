using HarmonyLib;

namespace SCP1507.Patches;

[HarmonyPatch(typeof(PhysicsProp))]
internal class PhysicProp
{
    [HarmonyPatch("GrabItemOnClient")]
    [HarmonyPostfix]
    private static void PostFix(PhysicsProp __instance)
    {
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            FlamingoManager.FlamingoManager.Instance.PickupItemTrigger(__instance.playerHeldBy, __instance.transform.position);
        }
    }
}