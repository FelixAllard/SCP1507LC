using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace SCP1507.SCP1507Alpha;

public partial class Scp1507Alpha :EnemyAI
{
    [Header("Make anger")] 
    public int localAnger;

    public int alphaId;
    public List<PlayerRecord> listOfAnger;
    [Header("NetStats")] 
    private ulong localPlayerId;
    private PlayerControllerB localPlayer;
    [Header("Audio")]
    public AudioClip[] honks;
    public AudioClip[] attacks;
    public AudioSource walkingSource;

    [Header("FlamingoBasics")] 
    private bool isSeen = false;

    [Header("Attack")]
    public int damage;
    public Transform AttackArea;
    public float attackCooldown;
    private float attackCooldownBeheader;
    [Header("Look")]
    public Transform lookAt;
    public Transform defaultLookAt;
    private Coroutine beingLookedAtCoroutine;
    private Coroutine KillCoroutine;
    
    
    
    [NonSerialized]
    private NetworkVariable<NetworkBehaviourReference> _playerNetVar = new();
    public PlayerControllerB Scp1507AlphaTargetPlayer
    {
        get
        {
            return (PlayerControllerB)_playerNetVar.Value;
        }
        set 
        {
            if (value == null)
            {
                _playerNetVar.Value = null;
            }
            else
            {
                _playerNetVar.Value = new NetworkBehaviourReference(value);
            }
        }
    }
    private void Awake()
    {
        alphaId = RandomNumberGenerator.GetInt32(1000);
        localAnger = 0;
        listOfAnger = new List<PlayerRecord>();

    }

    public override void Start()
    {
        base.Start();
        localPlayer = RoundManager.Instance.playersManager.localPlayerController;
        localPlayerId = localPlayer.playerClientId;
        StartAlphaSearch();
        attackCooldownBeheader = attackCooldown;
        attackCooldown = 0;
        PlayAnimationClientRpc("IsAlpha", true);
        if (FlamingoManager.FlamingoManager.Instance == null)
        {
            GameObject managerObject = new GameObject("Scp1507Manager");
            managerObject.AddComponent<FlamingoManager.FlamingoManager>();
        }
    }

    private void LateUpdate()
    {
        if (Scp1507AlphaTargetPlayer != null)
        {
            lookAt.position = Scp1507AlphaTargetPlayer.playerEye.position;
        }
        else
        {
            lookAt.position = defaultLookAt.position;
        }
        attackCooldown += Time.deltaTime;
    }

    public override void DoAIInterval()
    {
        base.DoAIInterval();
        switch (currentBehaviourStateIndex)
        {
            case (int)StateA.Seen:
                
                agent.isStopped=true;
                //StartLookAtCoroutineClientRpc();
                PlayAnimationClientRpc("Walking", false);
                if (!CheckIfAPlayerHasVisionToCurrentPosition())
                {
                    
                    SwitchToBehaviourClientRpc((int)StateA.Walking);
                }
                if (FindAlphaTarget())
                {
                    StartCrusade();
                    SwitchToBehaviourClientRpc((int)StateA.Targeting);
                }


                
                break;
            case (int)StateA.Walking:
                agent.isStopped=false;
                
                PlayAnimationClientRpc("Walking", true);
                if (creatureAnimator.GetBool("Dancing"))
                {
                    PlayAnimationClientRpc("Dancing", false);
                }
                if (CheckIfAPlayerHasVisionToCurrentPosition())
                {
                    SwitchToBehaviourClientRpc((int)StateA.Seen);
                }
                if (FindAlphaTarget())
                {
                    StartCrusade();
                    SwitchToBehaviourClientRpc((int)StateA.Targeting);
                }

                if (RandomNumberGenerator.GetInt32(100) <= 3)
                {
                    SpawnNewFlamingo();
                }
                break;
            case (int)StateA.Targeting:
                agent.isStopped = false;
                StopCoroutine(searchCoroutine);
                PlayAnimationClientRpc("Walking", true);
                agent.SetDestination(Scp1507AlphaTargetPlayer.transform.position);
                if (creatureAnimator.GetBool("Dancing"))
                {
                    PlayAnimationClientRpc("Dancing", false);
                }
                if (Vector3.Distance(
                        transform.position, 
                        Scp1507AlphaTargetPlayer.transform.position
                    ) < 1.2f && 
                    attackCooldown>=attackCooldownBeheader
                )
                {
                    PlayAnimationClientRpc("Attack");
                }
                if (!Scp1507AlphaTargetPlayer.isInsideFactory || Scp1507AlphaTargetPlayer.isPlayerDead)
                {
                    agent.ResetPath();
                    StartAlphaSearch();
                    StopMoving();
                    
                    SwitchToBehaviourClientRpc((int)StateA.Seen);
                }
                
                break;
            default:
                MonsterLogger("WRONG BEHAVIOUR STATE INDEX!", true);
                break;
            
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
            if (!(Vector3.Distance(playerScript.transform.position, transform.position) < 6))
                break;
            if(!playerScript.performingEmote)
                break;
            return true;
        }

        return false;
    }
    /// <summary>
    /// Start the search coroutine and all it's necessity!
    /// </summary>
    private void StartAlphaSearch()
    {
        if (searchCoroutine == null)
        {
            StartSearch(transform.position);
        }
    }
    /// <summary>
    /// Will find the Alpha target and put it in the TargetPlayer. 
    /// </summary>
    /// <returns>Will return true if the Alpha can target and false if it can't</returns>
    private bool FindAlphaTarget()
    {
        if (listOfAnger == null)
        {
            MonsterLogger("List of Anger is null",true);
            return false;
        }

        int highestAnger = 0;
        PlayerRecord highestPlayerRecord = null;

        foreach (var anger in listOfAnger)
        {
            if (anger == null)
            {
                MonsterLogger("Encounteed a null value Entry", true);
                continue;
            }

            if (anger.Angered && highestAnger < anger.AngerMeter)
            {
                highestAnger = anger.AngerMeter;
                highestPlayerRecord = anger;
            }
        }

        if (highestPlayerRecord == null)
        {
            MonsterLogger("No Valid player to target!");
            
            return false;
        }

        Scp1507AlphaTargetPlayer = highestPlayerRecord.PlayerControllerB;
        MonsterLogger("Player" + highestPlayerRecord.PlayerControllerB.playerUsername + " Is not the designated target");
        return true;
    }

    /// <summary>
    /// Check if any player has LOS towards the current flamingo
    /// </summary>
    /// <returns></returns>
    private bool CheckIfAPlayerHasVisionToCurrentPosition()
    {
        foreach (var player in RoundManager.Instance.playersManager.allPlayerScripts)
        {
            if (player.HasLineOfSightToPosition(transform.position))
            {
                isSeen = true;
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Check if a specific player has a line of sight on a certain position
    /// </summary>
    /// <returns></returns>
    private bool CheckIfLocalPlayerHasVisionToCurrentPosition_ONEPLAYER()
    {
        
        if (localPlayer.HasLineOfSightToPosition(transform.position))
        {
            isSeen = true;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Check if the localPlayer is continuously looking at the alpha
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckIfLookedCoroutine()
    {
        int numberOfSecondsSeen = 0;
        while (true)
        {
            if (CheckIfLocalPlayerHasVisionToCurrentPosition_ONEPLAYER())
            {
                numberOfSecondsSeen += 1;
                switch (numberOfSecondsSeen)
                {
                    case 10:
                        AddToAnger(1);
                        break;
                    case 20:
                        AddToAnger(2);
                        break;
                    case 30:
                        AddToAnger(3);
                        break;
                    case 40:
                        AddToAnger(4);
                        break;
                    default:
                        break;
                }

                yield return new WaitForSeconds(1f);
            }
            else
            {
                numberOfSecondsSeen = 0;
            }
                
        }
    }

        
    

    /// <summary>
    /// Will be called from the client to add to the current anger on the server
    /// </summary>
    /// <param name="x"></param>
    public void AddToAnger(int x)
    {
        localAnger += x;
        GiveServerAngerServerRpc(localPlayerId, localAnger );
    }
    /// <summary>
    /// Will be called from the client to set to the current anger on the server
    /// </summary>
    /// <param name="x"></param>
    public void SetToAnger(int x)
    {
        localAnger = x;
        GiveServerAngerServerRpc(localPlayerId, localAnger);
    }
    /// <summary>
    /// Will be called from the client to remove to the current anger on the server
    /// </summary>
    /// <param name="x"></param>
    public void RemoveToAnger(int x)
    {
        localAnger -= x;
        GiveServerAngerServerRpc(localPlayerId, localAnger);
    }
    
    /// <summary>
    /// Will return the record from the clientId
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    private PlayerRecord? GetPlayerByClientId(ulong clientId)
    {
        for (int i = 0; i < listOfAnger.Count; i++)
        {
        
            if (listOfAnger[i].ClientId == clientId)
            {
                return listOfAnger[i];
            }
        }
        throw new KeyNotFoundException("Player with the given ClientId was not found. CREATING NEW PROFILE");
    }
    
    
    /// <summary>
    /// Monster Logger
    /// </summary>
    /// <param name="message"></param>
    /// <param name="reportable"></param>
    private void MonsterLogger(String message, bool reportable = false)
    {
        if(!reportable)
            Debug.Log($"[{PluginInfo.PLUGIN_GUID}][SCP1507Alpha][{(reportable ? "PLEASE REPORT TO US IN THE DISCORD CHANNEL" : "Don't Report")}] ~ {message}");
        else
            Debug.LogError($"[{PluginInfo.PLUGIN_GUID}][SCP1507Alpha][{(reportable ? "PLEASE REPORT TO US IN THE DISCORD CHANNEL" : "Don't Report")}] ~ {message}");
        
    }

}