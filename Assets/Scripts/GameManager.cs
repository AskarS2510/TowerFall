
public static class GameManager
{
    public static long Score;
    public static int SkipCount;
    public static int maxAllowedSkipCount;
    public static int SkipLeft;

    static GameManager()
    {
        ResetStats();

        EventManager.RestartedGame.AddListener(ResetStats);
    }

    public static void UpdateScore(string sourceName)
    {
        if (sourceName == "Flying")
        {
            Score += 20;
        }

        if (sourceName == "Inner")
        {
            Score += 20;
        }

        EventManager.UpdatedScore?.Invoke();
    }

    public static void AddSkip()
    {
        SkipCount++;
        SkipLeft--;

        if (SkipCount > maxAllowedSkipCount)
        {
            SkipCount = 0;
            SkipLeft = maxAllowedSkipCount;

            EventManager.ExceededSkip?.Invoke();
        }

        EventManager.UpdatedSkip?.Invoke();
    }

    private static void ResetStats()
    {
        Score = 0;
        SkipCount = 0;
        maxAllowedSkipCount = 2;

        SkipLeft = maxAllowedSkipCount;

        EventManager.UpdatedSkip?.Invoke();
    }
}
