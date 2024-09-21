using System;
using BepInEx.Configuration;
using Unity.Collections;
using Unity.Netcode;

namespace SCP1507.Configurations;

[Serializable]
public class Config : SyncedInstance<Config>
{ 
    public ConfigEntry<int> MAXIMUM_NUMBER_OF_FLAMINGO { get; private set; } //Implemented
    public ConfigEntry<bool> CAN_SPAWN_ALPHA { get; private set; } //Implemented
    public ConfigEntry<int> CHANCE_SPAWN_NEW_FLAMINGO { get; private set; } //Implemented
    public ConfigEntry<int> RARITY_ALPHA { get; private set; } //Implemented
    public ConfigEntry<int> CHANCE_SPAWN_NEW_ALPHA { get; private set; } //Implemented
    public ConfigEntry<int> DAMAGE_ALPHA { get; private set; } //Implemented
    public ConfigEntry<int> DAMAGE_NORMAL { get; private set; } //Implemented
    public ConfigEntry<int> ANGER_FOR_ANGRY_FLAMINGO { get; private set; } //Implemented
    public ConfigEntry<bool> CUSTOM_HAT { get; private set; } //Implemented

    
    public Config(ConfigFile cfg)
    {
        InitInstance(this);
        MAXIMUM_NUMBER_OF_FLAMINGO = cfg.Bind("Flamingo Spawn", "Maximum number of flamingo!", 30,
            "This value defines the number of flamingo there can be on the map. It does not take alpha's into account"
        );
        CAN_SPAWN_ALPHA = cfg.Bind("Flamingo Spawn", "Can alpha spawn alpha?", false,
            "Enabling this option will make alpha able to spawn other alpha's. This can go out of hand quickly"
        );
        CHANCE_SPAWN_NEW_FLAMINGO = cfg.Bind("Flamingo Spawn", "Spawn speed flamingo", 4,
            "Every 0.2 seconds, the alpha will generate a number between 0 and 100. If the number generated is lower than the value specified, he will spawn a new flamingo. The higher this value the more likely a flamingo will spawn. Do not"
        );
        RARITY_ALPHA = cfg.Bind("Flamingo Spawn", "Rarity alpha flamingo", 40,
            "This is the rarity for an alpha flamingo to spawn direcly registered into the game"
            );
        CHANCE_SPAWN_NEW_ALPHA = cfg.Bind("Flamingo Spawn", "Chances to spawn a new alpha by alpha", 10,
            "Whenever a flamingo is summoned this is the chance for it to summon an additional alpha flamingo. See Spawn speed flamingo for how this value works."
        );
        DAMAGE_ALPHA = cfg.Bind("Stats", "Chances to spawn alpha", 20,
            "What are the chance for an alpha to spawn when spawning a new flamingo"
        );
        DAMAGE_NORMAL = cfg.Bind("Stats", "Chances to spawn alpha", 5,
            "This is the amount of raw damage dealt by flamingo"
        );
        ANGER_FOR_ANGRY_FLAMINGO = cfg.Bind("Anger", "Anger before flamingos are angry", 10,
            "This is the amount of anger before the flamingo's are angry"
        );
        CUSTOM_HAT = cfg.Bind("Cosmetics", "Can flamingos have custom hat?", true,
            "True = yes, false = no"
        );
        


    }
    public static void RequestSync() {
        if (!IsClient) return;

        using FastBufferWriter stream = new(IntSize, Allocator.Temp);
        MessageManager.SendNamedMessage("Xilef992SCP1507_OnRequestConfigSync", 0uL, stream);
    }
    public static void OnRequestSync(ulong clientId, FastBufferReader _) {
        if (!IsHost) return;

        Plugin.Logger.LogInfo($"Config sync request received from client: {clientId}");

        byte[] array = SerializeToBytes(Instance);
        int value = array.Length;

        using FastBufferWriter stream = new(value + IntSize, Allocator.Temp);

        try {
            stream.WriteValueSafe(in value, default);
            stream.WriteBytesSafe(array);

            MessageManager.SendNamedMessage("Xilef992SCP1507_OnReceiveConfigSync", clientId, stream);
        } catch(Exception e) {
            Plugin.Logger.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
        }
    }
    public static void OnReceiveSync(ulong _, FastBufferReader reader) {
        if (!reader.TryBeginRead(IntSize)) {
            Plugin.Logger.LogError("Config sync error: Could not begin reading buffer.");
            return;
        }

        reader.ReadValueSafe(out int val, default);
        if (!reader.TryBeginRead(val)) {
            Plugin.Logger.LogError("Config sync error: Host could not sync.");
            return;
        }

        byte[] data = new byte[val];
        reader.ReadBytesSafe(ref data, val);

        SyncInstance(data);

        Plugin.Logger.LogInfo("Successfully synced config with host.");
    }
    
}