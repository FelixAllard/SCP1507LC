using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

namespace SCP1507.SCP1507;

public partial class Scp1507
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
    public void DoQuackSound()
    {
        try
        {
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
}