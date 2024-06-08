using System;
using System.Collections.Generic;
using SCP1507.DataClass;
using Unity.Netcode;

namespace SCP1507.SCP1507Alpha;

public partial class Scp1507Alpha
{
    /// <summary>
    /// Must be call by clients to give the anger they have to the server
    /// </summary>
    /// <param name="clientId">Client ID of the current player</param>
    /// <param name="anger">The newly set Anger meter of the player</param>
    [ServerRpc(RequireOwnership = false)]
    public void GiveServerAngerServerRpc(ulong clientId, int anger)
    {
        try
        {
            PlayerRecord? currentPlayer = GetPlayerByClientId(clientId);
            //Won't run if caught by try catch :)
            currentPlayer.AngerMeter = anger;
            MonsterLogger("Player is " + currentPlayer.AngerMeter);
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine(e);
            listOfAnger.Add(new PlayerRecord(
                clientId,
                anger)
            );
        }
    }
    /// <summary>
    /// Start The lookAt coroutine
    /// </summary>
    [ClientRpc]
    public void StartLookAtCoroutineClientRpc()
    {
        if (beingLookedAtCoroutine == null)
        {
            beingLookedAtCoroutine = StartCoroutine(CheckIfLookedCoroutine());
        }
    }
    //ANIMATION EVENT
    /// <summary>
    /// Synchronised animation for clients [TRIGGERS]
    /// </summary>
    /// <param name="animationName">Name of the inspector animation</param>
    [ClientRpc]
    public void PlayAnimationClientRpc(string animationName)
    {
        creatureAnimator.SetTrigger(animationName);
    }
    /// <summary>
    /// Synchronised animation for clients [BOOLEANS]
    /// </summary>
    /// <param name="animationName">Name of the inspector animation</param>
    /// <param name="value">New Value [Boolean]</param>
    [ClientRpc]
    public void PlayAnimationClientRpc(string animationName, bool value)
    {
        creatureAnimator.SetBool(animationName, value);
    }
    
    

}