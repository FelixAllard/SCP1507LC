﻿using System.Collections;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace SCP1507.SCP1507Alpha;

public partial class Scp1507Alpha : EnemyAI
{
    /// <summary>
    /// When he gets hit, will be called
    /// </summary>
    /// <param name="force"></param>
    /// <param name="playerWhoHit"></param>
    /// <param name="playHitSFX"></param>
    /// <param name="hitID"></param>
    public override void HitEnemy(int force = 1, PlayerControllerB? playerWhoHit = null, bool playHitSFX = false, int hitID = -1) {
        base.HitEnemy(force, playerWhoHit, playHitSFX, hitID);
        if (playerWhoHit != null)
        {
            GiveServerAngerServerRpc(playerWhoHit.actualClientId, 20);
        }
        if(isEnemyDead){
            return;
        }
        enemyHP -= force;
        if (IsOwner) {
            if (enemyHP <= 0 && !isEnemyDead) {
                // Our death sound will be played through creatureVoice when KillEnemy() is called.
                // KillEnemy() will also attempt to call creatureAnimator.SetTrigger("KillEnemy"),
                // so we don't need to call a death animation ourselves.
                
                //StopCoroutine(SwingAttack());
                // We need to stop our search coroutine, because the game does not do that by default.
                if (searchCoroutine != null)
                {
                    StopCoroutine(searchCoroutine);
                }
                KillEnemyOnOwnerClient();
                ActivateRampage();
            }
        }
    }
    /// <summary>
    /// Called by animation and resolve damage on player.
    /// </summary>
    [ClientRpc]
    public void SwingAttackHitClientRpc() {
        int playerLayer = 1 << 3; // This can be found from the game's Asset Ripper output in Unity
        Collider[] hitColliders = Physics.OverlapBox(AttackArea.position, AttackArea.localScale, Quaternion.identity, playerLayer);
        if(hitColliders.Length > 0){
            foreach (var player in hitColliders){
                PlayerControllerB playerControllerB = MeetsStandardPlayerCollisionConditions(player);
                if (playerControllerB != null)
                {
                    DoAttackSound();
                    KillCoroutine = StartCoroutine(DamagePlayerCoroutine(playerControllerB));
                }
            }
        }
        attackCooldown = 0;
    }
    /// <summary>
    /// Called by attack in VitalCalls
    /// </summary>
    /// <param name="playerControllerB">The player which will loose hp</param>
    /// <returns></returns>
    IEnumerator DamagePlayerCoroutine(PlayerControllerB playerControllerB)
    {
        yield return new WaitForSeconds(0.1f);
        playerControllerB.DamagePlayer(damage);
        StopCoroutine(KillCoroutine);
    }
    
    
}