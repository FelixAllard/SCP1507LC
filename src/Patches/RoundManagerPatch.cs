using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace SCP1507.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal class RoundManagerPatch
{
    [HarmonyPatch("UnloadSceneObjectsEarly")]
    [HarmonyPostfix]
    private static void PostFixStart(RoundManager __instance)
    {
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            Object.Destroy(FlamingoManager.FlamingoManager.Instance.gameObject);
        }
    }
}