using System.Collections.Generic;

namespace SCP1507.SCP1507Alpha;

public partial class Scp1507Alpha
{
    private List<DoorHold> currentBreakingDoors;
    
    public bool HitDoor(DoorLock door)
    {
        foreach (var doorBreaking in currentBreakingDoors)
        {
            if (door == doorBreaking.Door)
            {
                doorBreaking.NumberOfHits += 1;
                if (doorBreaking.CanBreak)
                {
                    return true;
                }

                return false;
            }
        }
        currentBreakingDoors.Add(new DoorHold(door));
        return false;
    }

    public void DeleteDoor(DoorLock door)
    {
        foreach (var doors in currentBreakingDoors)
        {
            if (door == doors.Door)
            {
                currentBreakingDoors.Remove(doors);
            }
        }
    }
}