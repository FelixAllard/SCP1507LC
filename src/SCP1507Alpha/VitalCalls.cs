using GameNetcodeStuff;

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
            }
        }
    }
}