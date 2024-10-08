﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using GameNetcodeStuff;
using SCP1507.SCP1507Alpha;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

namespace SCP1507.SCP1507;

public partial class Scp1507 : EnemyAI
{
    [Header("Alpha")] 
    [NonSerialized] 
    public Scp1507Alpha alpha;
    [NonSerialized]
    public int alphaId;
    private bool isAlphaAlive = false;
    [Header("Attack")]
    public float attackCooldown;
    private float attackCooldownBeheader;
    public int damage;
    public Transform AttackArea;
    private Coroutine KillCoroutine;
    [Header("Audio")] 
    public AudioClip destroyDoor;
    public AudioClip[] honks;
    public AudioClip[] attacks;
    public AudioClip attackDoor;
    public AudioSource walkingSource;
    private FlamingoManager.FlamingoManager flamingoManager;
    
    
    [Header("LookAt")] 
    public Transform lookAt;
    public Transform defaultLookAt;


    private Coroutine startCo;

    [NonSerialized]
    private NetworkVariable<NetworkBehaviourReference> _playerNetVar = new();
    public PlayerControllerB Scp1507TargetPlayer
    {
        get
        {
            return (PlayerControllerB)_playerNetVar.Value;
        }
        set 
        {
            if (value == null)
            {
                _playerNetVar.Value = default(NetworkBehaviourReference);
            }
            else
            {
                _playerNetVar.Value = new NetworkBehaviourReference(value);
            }
        }
    }

    private void Awake()
    {
        attackCooldownBeheader = attackCooldown;
        attackCooldown = 0;
        startCo = StartCoroutine(StartCoroutineFLAMINGO());
        damage = Plugin.FlamingoConfig.DAMAGE_NORMAL.Value;
        if (FlamingoManager.FlamingoManager.Instance != null)
        {
            flamingoManager = FlamingoManager.FlamingoManager.Instance;
            flamingoManager.RegisterScp1507Instance(this); 
        }
        //Makes a random bool value VVV
        Random random = new Random();
        bool randomBool = random.Next(2) == 0;
        PlayAnimationClientRpc("DanceDirection", randomBool);
        //Makes a random bool value AAA
        
    }

    IEnumerator StartCoroutineFLAMINGO()
    {
        yield return new WaitForSeconds(3f);
        TriggerRampage(2000);
    }
    [ClientRpc]
    public void StartCallAlphaClientRpc(int x)
    {
        StopCoroutine(startCo);
        foreach (var alphaFla in FindObjectsOfType<SCP1507Alpha.Scp1507Alpha>())
        {
            if (alphaFla.alphaId == x)
            {
                alpha = alphaFla;
                alphaId = x;
                isAlphaAlive = true;
                return;
            }
        }
    }
    //TODO Fix the error message when player dies or  exit facility
    private void LateUpdate()
    {
        if (isAlphaAlive)
        {
            if (alpha.LocalAnger >= 7)
            {
                if (Scp1507TargetPlayer != default)
                {
                    lookAt.position = Scp1507TargetPlayer.playerEye.position;
                }
                else
                {
                    lookAt.position = defaultLookAt.position;
                }
            }
            else
            {
                lookAt.position = defaultLookAt.position;
            }
        }
        else
        {
            if (GetClosestPlayer() != null)
            {
                lookAt.position = GetClosestPlayer().transform.position;
            }
        
        }
        attackCooldown += Time.deltaTime;
    }

    public override void DoAIInterval()
    {
        base.DoAIInterval();
        switch (currentBehaviourStateIndex)
        {
            case (int)State.Unmoving:
                agent.isStopped = true;
                agent.speed = 5f;
                agent.ResetPath();
                break;
            case (int)State.AlphaCharge:
                agent.isStopped = false;
                Scp1507TargetPlayer = GetClosestPlayer();
                agent.speed = 5f;
                if (creatureAnimator.GetBool("Dancing"))
                {
                    PlayAnimationClientRpc("Dancing", false);
                }

                if (Scp1507TargetPlayer != default)
                {
                    if (Vector3.Distance(transform.position, Scp1507TargetPlayer.transform.position) > 10f)
                    {
                        agent.SetDestination(RoundManager.Instance.GetRandomNavMeshPositionInRadius(alpha.transform.position));
                        agent.speed = 10f;
                    }
                    else
                    {
                        if (Scp1507TargetPlayer != default)
                        {
                            agent.SetDestination(Scp1507TargetPlayer.transform.position);
                            if (Vector3.Distance(
                                    transform.position, 
                                    Scp1507TargetPlayer.transform.position
                                ) < 1.2f && attackCooldown>=1f)
                            {
                                PlayAnimationClientRpc("Attack");
                            }
                        }
                    }
                }
                
                break;
            case (int)State.Rampage:
                agent.speed = 5f;
                Scp1507TargetPlayer = GetClosestPlayer();
                if (Scp1507TargetPlayer != default)
                {
                    agent.SetDestination(Scp1507TargetPlayer.transform.position);
                    if (Vector3.Distance(
                            transform.position, 
                            Scp1507TargetPlayer.transform.position
                        ) < 1.2f && attackCooldown>=1f)
                    {
                        PlayAnimationClientRpc("Attack");
                    }
                }
                else
                {
                    agent.ResetPath();
                }
                break;
            case (int)State.None:
                
                break;
            default:
                MonsterLogger("WRONG BEHAVIOUR STATE INDEX!", true);
                break;
            
        }
    }

    
    /// <summary>
    /// Change State of flamingo to Unmoving
    /// </summary>
    public void StopMoving(int flamingoId)
    {
        
        if (flamingoId == alphaId)
        {
            PlayAnimationClientRpc("Walking", false);
            SwitchToBehaviourClientRpc((int)State.Unmoving);
        }
    }
    /// <summary>
    /// Change state of flamingo to Alpha Charge
    /// </summary>

    public void TriggerAlphaCharge(int flamingoId)
    {
        if (flamingoId == alphaId)
        {
            PlayAnimationClientRpc("Walking", true);
            SwitchToBehaviourClientRpc((int)State.AlphaCharge);
        }
    }
    /// <summary>
    /// Change the state of the flamingo to rampaging
    /// </summary>
    public void TriggerRampage(int flamingoId)
    { 
        
        if (flamingoId == alphaId || flamingoId == 2000)
        {
            PlayAnimationClientRpc("Walking", true);
            SwitchToBehaviourClientRpc((int)State.Rampage);
        }
    }

    /// <summary>
    /// Checks if a nearby player dances close to the flamingo
    /// </summary>
    /// <returns>True if player dances, false if not</returns>
    private bool CheckIfPlayerDances()
    {
        foreach(PlayerControllerB playerScript in RoundManager.Instance.playersManager.allPlayerScripts)
        {
            if ((Vector3.Distance(playerScript.transform.position, transform.position) < 10f))
                break;
            if(!playerScript.performingEmote)
                break;
            return true;
        }

        return false;
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
    /// <summary>
    /// Called by Flamingo Manager! Makes the flamingo start dancing!
    /// </summary>
    public void StartDance()
    {
        PlayAnimationClientRpc("Dancing", true);
    }
    /// <summary>
    /// Called by Flamingo Manager! Makes the flamingo stop dancing!
    /// </summary>
    public void StopDance()
    {
        PlayAnimationClientRpc("Dancing", false);
    }
    private void MonsterLogger(String message, bool reportable = false)
    {
        if(!reportable)
            Plugin.Logger.LogInfo($"[{PluginInfo.PLUGIN_GUID}][SCP1507Instance][{(reportable ? "PLEASE REPORT TO US IN THE DISCORD CHANNEL" : "Don't Report")}] ~ {message}");
        else
        {
            Plugin.Logger.LogError($"[{PluginInfo.PLUGIN_GUID}][SCP1507Instance][{(reportable ? "PLEASE REPORT TO US IN THE DISCORD CHANNEL" : "Don't Report")}] ~ {message}");
        }
    }

    
}