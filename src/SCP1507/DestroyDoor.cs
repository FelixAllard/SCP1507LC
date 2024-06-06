using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace SCP1507.SCP1507;

public partial class Scp1507
{
    /// <summary>
    /// This will make sure we only call the destroy door when we are touching a door
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<DoorLock>())
        {
            PlayAnimationClientRpc("AttackDoor", true);
        }
    }
    /// <summary>
    /// When the door breaks, we want the flamingo to stop being stupid
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<DoorLock>())
        {
            PlayAnimationClientRpc("AttackDoor", false);
        }
    }

    /// <summary>
    /// Check if the door is actually breakable or not!
    /// </summary>
    /// <param name="Door"></param>
    private void CheckIfCanBreakDoor(DoorLock Door)
    {
        if (alpha.HitDoor(Door))
        {
            return;
        }
        try
        {
            var ThisDoor = Door.transform.parent.transform.parent.transform.parent.gameObject;
            if (!ThisDoor.GetComponent<Rigidbody>())
            {
                if (Vector3.Distance(transform.position, ThisDoor.transform.position) <= 4f)
                {
                    BashDoorClientRpc(ThisDoor, (targetPlayer.transform.position - transform.position).normalized * 20);
                }
            }
        }
        catch (NullReferenceException e)
        {
            try
            {
                Door.OpenDoorAsEnemyClientRpc();
            }
            catch (Exception y)
            {
                Debug.Log("The doors are not formatted the right way and as such Flamingo's may seems really stupid hitting doors " + y);
            }
        }
        
    }
    

    /// <summary>
    /// Will destroy the door!
    /// </summary>
    /// <param name="netObjRef"></param>
    /// <param name="Position"></param>
    [ClientRpc]
    public void BashDoorClientRpc(NetworkObjectReference netObjRef, Vector3 Position)
    {
        if (netObjRef.TryGet(out NetworkObject netObj))
        {
            var ThisDoor = netObj.gameObject;
            var rig = ThisDoor.AddComponent<Rigidbody>();
            var newAS = ThisDoor.AddComponent<AudioSource>();
            newAS.spatialBlend = 1;
            newAS.maxDistance = 60;
            newAS.rolloffMode = AudioRolloffMode.Linear;
            newAS.volume = 1;
            StartCoroutine(TurnOffC(rig, .12f));
            rig.AddForce(Position, ForceMode.Impulse);
            newAS.PlayOneShot(destroyDoor);
        }
    }
    /// <summary>
    /// Will delete the foor after an amount of time
    /// </summary>
    /// <param name="rigidbody"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator TurnOffC(Rigidbody rigidbody,float time)
    {
        rigidbody.detectCollisions = false;
        yield return new WaitForSeconds(time);
        rigidbody.detectCollisions = true;
        Destroy(rigidbody.gameObject, 5);
    }
    
    
}