﻿using GameNetcodeStuff;
using HarmonyLib;

namespace SCP1507.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
    [HarmonyPatch("StartPerformingEmoteServerRpc")]
    [HarmonyPostfix]
    private static void PostFixStart(PlayerControllerB __instance)
    {
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            FlamingoManager.FlamingoManager.Instance.StartEmoting(__instance);
        }
    }
    //TODO Fix this for they just won't stop dancing!
    [HarmonyPatch("StopPerformingEmoteServerRpc")]
    [HarmonyPostfix]
    private static void PostFixStop(PlayerControllerB __instance)
    {
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            FlamingoManager.FlamingoManager.Instance.StartEmoting(__instance);
        }
    }
}