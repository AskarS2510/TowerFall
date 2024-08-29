using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent ReadyForNextBlock;
    public static UnityEvent<int> Destroyed;
    public static UnityEvent Stuck;
    public static UnityEvent StoppedMovement;
    public static UnityEvent AllowedDestroyFlying;
    public static UnityEvent MovementEnded;
    public static UnityEvent<int, int> ChangedPosition;
    public static UnityEvent SpawnedPlayerBlock;
    public static UnityEvent<bool, bool> RaisedRotate;
    public static UnityEvent<int, int> RaisedMove;
    public static UnityEvent TurnedOnSpeed;
    public static UnityEvent TurnedOffSpeed;
    public static UnityEvent UpdatedScore;
    public static UnityEvent UpdatedSkip;
    public static UnityEvent ExceededSkip;
    public static UnityEvent StartedGame;
    public static UnityEvent GameOver;
    public static UnityEvent RestartedGame;

    static EventManager()
    {
        ReadyForNextBlock = new UnityEvent();
        Destroyed = new UnityEvent<int>();
        Stuck = new UnityEvent();
        StoppedMovement = new UnityEvent();
        AllowedDestroyFlying = new UnityEvent();
        MovementEnded = new UnityEvent();
        ChangedPosition = new UnityEvent<int, int>();
        SpawnedPlayerBlock = new UnityEvent();
        RaisedRotate = new UnityEvent<bool, bool>();
        RaisedMove = new UnityEvent<int, int>();
        UpdatedScore = new UnityEvent();
        UpdatedSkip = new UnityEvent();
        ExceededSkip = new UnityEvent();
        StartedGame = new UnityEvent();
        GameOver = new UnityEvent();
        RestartedGame = new UnityEvent();
        TurnedOnSpeed = new UnityEvent();
        TurnedOffSpeed = new UnityEvent();
    }
}
