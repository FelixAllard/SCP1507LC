﻿using System.Collections.Generic;
using SCP1507.SCP1507;
using UnityEngine;
using Random = System.Random;

namespace SCP1507.SCP1507Alpha;

public partial class Scp1507Alpha
{
    /// <summary>
    /// Spawns a new flamingo in a raidius around the Alpha
    /// </summary>
    public void SpawnNewFlamingo()
    {
        var allEnemiesList = new List<SpawnableEnemyWithRarity>();
        allEnemiesList.AddRange(RoundManager.Instance.currentLevel.Enemies);
        var enemyToSpawn = allEnemiesList.Find(x => x.enemyType.enemyName.Equals("scp1507"));
        GameObject flamingoObject = RoundManager.Instance.SpawnEnemyGameObject(
            RoundManager.Instance.GetRandomNavMeshPositionInRadius(
                transform.position,
                5f
            ),
            UnityEngine.Random.RandomRangeInt(0,360),
            RoundManager.Instance.currentLevel.Enemies.IndexOf(enemyToSpawn),
            enemyToSpawn.enemyType
        );
        flamingoObject.GetComponent<SCP1507.Scp1507>().StartCallAlphaClientRpc(alphaId);
    }
    /// <summary>
    /// Communication Bridge Between alpha and other flamingo's : ACTIVATE RAMPAGE. To be called when alpha dies
    /// </summary>
    public void ActivateRampage()
    {
        foreach (var flamingo in FindObjectsOfType<SCP1507.Scp1507>())
        {
            flamingo.TriggerRampage(alphaId);
        }
    }
    /// <summary>
    /// Communication Bridge Between alpha and scp
    /// </summary>
    public void StartCrusade()
    {
        foreach (var flamingo in FindObjectsOfType<SCP1507.Scp1507>())
        {
            flamingo.TriggerAlphaCharge(alphaId);
        }
    }
    /// <summary>
    /// Make the flamingo stop moving and become inanimate once again
    /// </summary>
    public void StopMoving()
    {
        foreach (var flamingo in FindObjectsOfType<SCP1507.Scp1507>())
        {
            flamingo.StopMoving(alphaId);
        }
    }
}