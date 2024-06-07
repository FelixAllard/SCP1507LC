using System.Security.Cryptography;
using UnityEngine;

namespace SCP1507.SCP1507;

public partial class Scp1507
{
    private void DoAttackSound()
    {
        creatureSFX.PlayOneShot(attacks[RandomNumberGenerator.GetInt32(attacks.Length)]);
    }
    private void DoQuackSound()
    {
        creatureVoice.PlayOneShot(honks[RandomNumberGenerator.GetInt32(honks.Length)]);
    }
}