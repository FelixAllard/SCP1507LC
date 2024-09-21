using GameNetcodeStuff;
using HarmonyLib;

namespace SCP1507.Configurations;

public static class ConfigurationsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
    public static void InitializeLocalPlayer() {
        if (RoundManager.Instance.IsHost) {
            Config.MessageManager.RegisterNamedMessageHandler("SCP1507_OnRequestConfigSync", Config.OnRequestSync);
            Config.Synced = true;

            return;
        }

        Config.Synced = false;
        Config.MessageManager.RegisterNamedMessageHandler("SCP1507_OnReceiveConfigSync", Config.OnReceiveSync);
        Config.RequestSync();
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
    public static void PlayerLeave() {
        Config.RevertSync();
    }
}