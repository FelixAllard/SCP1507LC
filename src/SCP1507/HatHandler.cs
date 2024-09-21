using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace SCP1507.SCP1507;


public class HatHandler :MonoBehaviour
{
    public List<GameObject> allHats;

    private void Awake()
    {
        if (Plugin.FlamingoConfig.CUSTOM_HAT.Value)
        {
            int chosenHat = RandomNumberGenerator.GetInt32(allHats.Count);
            allHats[chosenHat].SetActive(true);
            allHats.RemoveAt(chosenHat);
        }
        foreach (var hat in allHats)
        {
            Destroy(hat);
        }
    }
}