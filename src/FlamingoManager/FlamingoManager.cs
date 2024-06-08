using System;
using System.Linq;
using System.Security.Cryptography;
using GameNetcodeStuff;
using SCP1507.SCP1507;
using SCP1507.SCP1507Alpha;

namespace SCP1507.FlamingoManager;

using System.Collections.Generic;
using UnityEngine;

public class FlamingoManager : MonoBehaviour
{
    private static FlamingoManager _instance;
    public static FlamingoManager Instance => _instance;
    private List<SCP1507.Scp1507> allFlamingo;
    private List<EmoteTime> playersDoingEmotes;

    private void Awake()
    {
        allFlamingo = new List<Scp1507>();
        playersDoingEmotes = new List<EmoteTime>();
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        InvokeRepeating("CheckIfSeen", 2f, 7f);
        InvokeRepeating("Emotehandles", 5f, 1f);
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
                Debug.Log("We are sending!");
                flamingo.alpha.LocalAnger += 1;
                flamingo.alpha.GiveServerAngerServerRpc(playerHeldBy.actualClientId,flamingo.alpha.LocalAnger);
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
                flamingo.alpha.LocalAnger -= 1;
                flamingo.alpha.GiveServerAngerServerRpc(playerHeldBy.actualClientId,flamingo.alpha.LocalAnger);
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
                flamingo.alpha.LocalAnger += 2;
                flamingo.alpha.GiveServerAngerServerRpc(playerHeldBy.actualClientId,flamingo.alpha.LocalAnger);
                return;
            }
        }
    }

    public void CheckIfSeen()
    {
        var filteredPlayerScripts = RoundManager.Instance.playersManager.allPlayerScripts
            .Where(player => !player.isPlayerDead && player.isPlayerControlled && player.isInsideFactory)
            .ToArray();
        PlayerControllerB playerChosen;
        if (filteredPlayerScripts.Length == 0)
        {
             return;
        }
        playerChosen = filteredPlayerScripts[RandomNumberGenerator.GetInt32(filteredPlayerScripts.Length)];
            
        foreach (var flamingo in allFlamingo)
        {
            if (playerChosen.HasLineOfSightToPosition(flamingo.transform.position))
            {
                Debug.LogWarning("Gave 2 anger to server");
                if (flamingo.alpha != null)
                {
                    flamingo.alpha.LocalAnger += 2;
                    flamingo.alpha.GiveServerAngerServerRpc(playerChosen.actualClientId,flamingo.alpha.LocalAnger);
                    return; 
                }
                
            }
        }
    }

    public void Emotehandles()
    {
        Debug.LogWarning("Currently handling :" + playersDoingEmotes.Count);
        
        foreach (var playerEmoting in playersDoingEmotes)
        {
            
            playerEmoting.TimeEmoting += 1;
            if (allFlamingo != null)
            {
                switch (playerEmoting.TimeEmoting)
                {
                    case 10:
                        allFlamingo[0].alpha.LocalAnger -= 1;
                        allFlamingo[0].alpha.GiveServerAngerServerRpc(playerEmoting.ClientId, allFlamingo[0].alpha.LocalAnger);
                        break;
                    case 20:
                        allFlamingo[0].alpha.LocalAnger -= 2;
                        allFlamingo[0].alpha.GiveServerAngerServerRpc(playerEmoting.ClientId, allFlamingo[0].alpha.LocalAnger);
                        break;
                    case 30:
                        allFlamingo[0].alpha.LocalAnger -= 3;
                        allFlamingo[0].alpha.GiveServerAngerServerRpc(playerEmoting.ClientId, allFlamingo[0].alpha.LocalAnger);
                        break;
                    case 40:
                        allFlamingo[0].alpha.LocalAnger -= 4;
                        allFlamingo[0].alpha.GiveServerAngerServerRpc(playerEmoting.ClientId, allFlamingo[0].alpha.LocalAnger);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void StartEmoting(PlayerControllerB actualPlayer)
    {
        bool makeDance = false;
        foreach (var flamingo in allFlamingo)
        {
            if (Vector3.Distance(flamingo.transform.position, actualPlayer.transform.position) < 6)
            {
                flamingo.StartDance();
                makeDance = true;
                   
            }
        }
        if (makeDance == true)
        {
            if (playersDoingEmotes.FirstOrDefault(e => e.ClientId == actualPlayer.actualClientId) == null)
            {
                playersDoingEmotes.Add(new EmoteTime(actualPlayer.actualClientId));
            }
        }
    }
    /// <summary>
    /// When player Stops dancing, need to get called!
    /// </summary>
    /// <param name="actualPlayer"></param>
    public void StopEmoting(PlayerControllerB actualPlayer)
    {
        bool makeDance = false;
        foreach (var flamingo in allFlamingo)
        {
            if (Vector3.Distance(flamingo.transform.position, actualPlayer.transform.position) < 6)
            {
                flamingo.StopDance();
                makeDance = true;
                   
            }
        }
        if (makeDance == true)
        {
            playersDoingEmotes.Remove(playersDoingEmotes.FirstOrDefault(e => e.ClientId == actualPlayer.actualClientId));
        }
    }
}