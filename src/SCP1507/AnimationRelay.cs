using UnityEngine;

namespace SCP1507.SCP1507;

public class AnimationRelaySCP1507 : MonoBehaviour
{
    public Scp1507 mainScript;
    public void AttackDoorAnimationHandle()
    {
        mainScript.creatureSFX.PlayOneShot(mainScript.destroyDoor);
        mainScript.SwingAttackHitDoor();
    }

    public void AttackPlayerAnimationHandle()
    {
        mainScript.SwingAttackHitClientRpc();
    }

    public void FootStepAnimationHandle()
    {
        mainScript.walkingSource.Play();
    }
}