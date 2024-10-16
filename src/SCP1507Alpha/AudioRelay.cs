using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace SCP1507.SCP1507Alpha;
public partial class Scp1507Alpha
{
    private void DoAttackSound()
    {
        //I hate doing things this way, but i have no choice since this is for compatibility with another mod
        try{
            creatureSFX.PlayOneShot(attacks[RandomNumberGenerator.GetInt32(attacks.Length)]);
        }
        catch (ArgumentNullException)
        {
            //DoNothing
        }
        catch (NullReferenceException)
        {
            //DoNothing
        }
    }
    private void DoQuackSound()
    {
        try{
            creatureVoice.PlayOneShot(honks[RandomNumberGenerator.GetInt32(honks.Length)]);
        }
        catch (ArgumentNullException)
        {
            //DoNothing
        }
        catch (NullReferenceException)
        {
            //DoNothing
        }
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