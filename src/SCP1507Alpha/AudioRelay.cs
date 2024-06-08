using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace SCP1507.SCP1507Alpha;
public partial class Scp1507Alpha
{
    private void DoAttackSound()
    {
        creatureSFX.PlayOneShot(attacks[RandomNumberGenerator.GetInt32(attacks.Length)]);
    }
    private void DoQuackSound()
    {
        creatureVoice.PlayOneShot(honks[RandomNumberGenerator.GetInt32(honks.Length)]);
    }

    IEnumerator QuackAlpha()
    {
        while (!isEnemyDead)
        {
            yield return new WaitForSeconds(FlamingoManager.FlamingoManager.RandomFloatBetween(5, 10));
            DoQuackSound();
        }
    }
}