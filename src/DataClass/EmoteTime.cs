using GameNetcodeStuff;

namespace SCP1507.SCP1507Alpha;

public class EmoteTime
{
    public EmoteTime(ulong clientId)
    {
        this.clientId = clientId;
        timeEmoting = 0;
    }
    public int TimeEmoting
    {
        get => timeEmoting;
        set
        {
            timeEmoting = value;
        } 
    }

    private ulong clientId;

    public ulong ClientId => clientId;

    private int timeEmoting;
}