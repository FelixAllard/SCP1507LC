﻿using System.Collections;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace SCP1507.SCP1507;

public partial class Scp1507
{
    [ClientRpc]
    public void SwingAttackHitClientRpc() {
        int playerLayer = 1 << 3; // This can be found from the game's Asset Ripper output in Unity
        Collider[] hitColliders = Physics.OverlapBox(AttackArea.position, AttackArea.localScale, Quaternion.identity, playerLayer);
        if(hitColliders.Length > 0){
            foreach (var player in hitColliders){
                PlayerControllerB playerControllerB = MeetsStandardPlayerCollisionConditions(player);
                if (playerControllerB != null)
                {
                    creatureSFX.Play();
                    MonsterLogger(playerControllerB.health.ToString());
                    KillCoroutine = StartCoroutine(DamagePlayerCoroutine(playerControllerB));
                }
            }
        }
        attackCooldown = attackCooldownBeheader;
    }
    /// <summary>
    /// Called by attack in VitalCalls
    /// </summary>
    /// <param name="playerControllerB">The player which will loose hp</param>
    /// <returns></returns>
    IEnumerator DamagePlayerCoroutine(PlayerControllerB playerControllerB)
    {
        yield return new WaitForSeconds(0.3f);
        StopCoroutine(KillCoroutine);
        playerControllerB.DamagePlayer(damage);
    }
    
}