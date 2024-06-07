using System;
using System.Linq;
using System.Security.Cryptography;
using GameNetcodeStuff;
using SCP1507.SCP1507;

namespace SCP1507.FlamingoManager;

using System.Collections.Generic;
using UnityEngine;

public class FlamingoManager : MonoBehaviour
{
    private static FlamingoManager _instance;
    public static FlamingoManager Instance => _instance;
    private List<SCP1507.Scp1507> allFlamingo;

    private void Awake()
    {
        allFlamingo = new List<Scp1507>();
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        InvokeRepeating("CheckIfSeen", 2f, 7f);
    }
    
    public void RegisterScp1507Instance(SCP1507.Scp1507 instance)
    {
        allFlamingo.Add(instance);
    }

    public void UnregisterScp1507Instance(SCP1507.Scp1507 instance)
    {
        allFlamingo.Remove(instance);
    }

    public void AirHornCheck(PlayerControllerB playerHeldBy)
    {
        foreach (var flamingo in allFlamingo)
        {
            float distance = Vector3.Distance(playerHeldBy.transform.position, flamingo.transform.position);
            if (distance <= 5f)
            {
                flamingo.alpha.GiveServerAngerServerRpc(playerHeldBy.actualClientId,flamingo.alpha.localAnger+1);
                return;
            }
        }
    }
    public void ClownHornCheck(PlayerControllerB playerHeldBy)
    {
        foreach (var flamingo in allFlamingo)
        {
            float distance = Vector3.Distance(playerHeldBy.transform.position, flamingo.transform.position);
            if (distance <= 5f)
            {
                flamingo.alpha.GiveServerAngerServerRpc(playerHeldBy.actualClientId,flamingo.alpha.localAnger-1);
                return;
            }
        }
    }
    public void PickupItemTrigger(PlayerControllerB playerHeldBy, Vector3 positionOfPickup)
    {
        foreach (var flamingo in allFlamingo)
        {
            float distance = Vector3.Distance(positionOfPickup, flamingo.transform.position);
            if (distance <= 5f)
            {
                flamingo.alpha.GiveServerAngerServerRpc(playerHeldBy.actualClientId,flamingo.alpha.localAnger+2);
                return;
            }
        }
    }

    public void CheckIfSeen()
    {
        var filteredPlayerScripts = RoundManager.Instance.playersManager.allPlayerScripts
            .Where(player => !player.isPlayerDead && player.isPlayerControlled && player.isInsideFactory)
            .ToArray();
        PlayerControllerB playerChosen =
            filteredPlayerScripts[RandomNumberGenerator.GetInt32(filteredPlayerScripts.Length)];
        foreach (var flamingo in allFlamingo)
        {
            if (playerChosen.HasLineOfSightToPosition(flamingo.transform.position))
            {
                Debug.LogWarning("Gave 2 anger to server");
                if (flamingo.alpha != null)
                {
                    flamingo.alpha.GiveServerAngerServerRpc(playerChosen.actualClientId,flamingo.alpha.localAnger+2);
                    return; 
                }
                
            }
        }
    }
}