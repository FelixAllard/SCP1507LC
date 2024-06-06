using System;
using UnityEngine.Experimental.GlobalIllumination;

namespace SCP1507.SCP1507Alpha;

public class DoorHold
{
    public DoorHold(DoorLock door)
    {
        this.door = door;
        numberOfHits = 1;
    }
    public DoorLock Door => door;

    public bool CanBreak => canBreak;
    
    public int NumberOfHits
    {
        get => numberOfHits;
        set
        {
            numberOfHits = value;
            if (numberOfHits >= 10)
            {
                canBreak = true;
            }
        }
    }
    

    

    private DoorLock door;
    private int numberOfHits;
    private bool canBreak = false;
}