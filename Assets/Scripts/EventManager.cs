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
    public static UnityEvent<bool, bool> DoneRotation;
    public static UnityEvent<int, int> RaisedMove;
    public static UnityEvent RaisedSwitchSpeed;
    public static UnityEvent RaisedDropDown;
    public static UnityEvent UpdatedSkip;
    public static UnityEvent UpdatedTime;
    public static UnityEvent ExceededSkip;
    public static UnityEvent StartedGame;
    public static UnityEvent GameOver;
    public static UnityEvent RestartedGame;
    public static UnityEvent PausedGame;
    public static UnityEvent UnpausedGame;
    public static UnityEvent EndedTutorial;
    public static UnityEvent DoneDestruction;
    public static UnityEvent PreparedMap;
    public static UnityEvent RaisedHowToPlay;
    public static UnityEvent AskedHowToPlay;
    public static UnityEvent<string, float> RaisedSlider;

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
        DoneRotation = new UnityEvent<bool, bool>();
        RaisedMove = new UnityEvent<int, int>();
        UpdatedSkip = new UnityEvent();
        UpdatedTime = new UnityEvent();
        ExceededSkip = new UnityEvent();
        StartedGame = new UnityEvent();
        GameOver = new UnityEvent();
        RestartedGame = new UnityEvent();
        RaisedSwitchSpeed = new UnityEvent();
        RaisedDropDown = new UnityEvent();
        PausedGame = new UnityEvent();
        UnpausedGame = new UnityEvent();
        EndedTutorial = new UnityEvent();
        DoneDestruction = new UnityEvent();
        PreparedMap = new UnityEvent();
        RaisedHowToPlay = new UnityEvent();
        AskedHowToPlay = new UnityEvent();
        RaisedSlider = new UnityEvent<string, float>();
    }
}
