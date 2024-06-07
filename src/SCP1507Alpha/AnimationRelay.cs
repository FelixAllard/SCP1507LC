using System;
using SCP1507.SCP1507Alpha;
using UnityEngine;

namespace SCP1507.SCP1507;

public class AnimationRelaySCP1507Alpha : MonoBehaviour
{
    public Scp1507Alpha mainScript;
    /// <summary>
    /// The alpha won't break doors! NEVER CALLED
    /// </summary>
    public void AttackDoorAnimationHandle()
    {
        MonsterLogger("Started attack sequence against door! FROM ALPHA. This is not supposed to happen and may cause issue!", true);
    }

    public void AttackPlayerAnimationHandle()
    {
        mainScript.SwingAttackHitClientRpc();
    }

    public void FootStepAnimationHandle()
    {
        mainScript.walkingSource.Play();
    }
    /// <summary>
    /// Monster Logger
    /// </summary>
    /// <param name="message"></param>
    /// <param name="reportable"></param>
    private void MonsterLogger(String message, bool reportable = false)
    {
        if(!reportable)
            Debug.Log($"[{PluginInfo.PLUGIN_GUID}][SCP1507Alpha.Animations][{(reportable ? "PLEASE REPORT TO US IN THE DISCORD CHANNEL" : "Don't Report")}] ~ {message}");
        else
            Debug.LogError($"[{PluginInfo.PLUGIN_GUID}][SCP1507Alpha.Animations][{(reportable ? "PLEASE REPORT TO US IN THE DISCORD CHANNEL" : "Don't Report")}] ~ {message}");
        
    }
}