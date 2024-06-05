using System;
using GameNetcodeStuff;
using UnityEngine.UIElements;

namespace SCP1507.SCP1507Alpha;

public class PlayerRecord
{
    

    public PlayerRecord(ulong clientId, int angerMeter)
    {
        this.clientId = clientId;
        this.angerMeter = angerMeter;
        foreach (var player in RoundManager.Instance.playersManager.allPlayerScripts)
        {
            if (player.playerClientId == clientId)
            {
                playerControllerB = player;
            }
        }
    }
    public PlayerControllerB PlayerControllerB => playerControllerB;

    public ulong ClientId => clientId;

    public bool Angered
    {
        get
        {
            if (!playerControllerB.isPlayerDead && playerControllerB.isInsideFactory)
            {
                return angered;
            }
            else
            {
                return false;
            }
            
        }
    } 
    
    public int AngerMeter
    {
        get => angerMeter;
        set
        {
            if (angerMeter - value <= 0)
            {
                angerMeter = 0;
                
            }
            else
            {
                if(angerMeter >= 10)
                {
                    angered = true;
                }
                angerMeter = value; // Set the value directly if within range
            }
        }
    }
    private PlayerControllerB playerControllerB;
    private ulong clientId;
    private int angerMeter;
    private bool angered;
}