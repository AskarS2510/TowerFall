
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool IsTutorialDone;
    public static int DestroyedOnWave;
    public static float DelayBetweenWaves;
    public static float LeftTime;
    public static bool IsGameOver;
    public static int SkipCount;
    public static int MaxAllowedSkipCount;
    public static int SkipLeft;
    public static IEnumerator _timer;

    private void Start()
    {
        IsTutorialDone = false;
        DelayBetweenWaves = 1.2f;

        ResetStats();

        EventManager.StartedGame.AddListener(StartGame);

        EventManager.RestartedGame.AddListener(RestartGame);

        EventManager.GameOver.AddListener(GameOver);

        EventManager.SpawnedPlayerBlock.AddListener(ResetDestroyedCount);

        EventManager.DoneDestruction.AddListener(AddTime);
    }

    public static void UpdateScore()
    {
        DestroyedOnWave++;
    }

    private void GameOver()
    {
        StopCoroutine(_timer);
        _timer = null;
    }

    private void StartGame()
    {
        _timer = Timer();
        StartCoroutine(_timer);
    }

    private void RestartGame()
    {
        ResetStats();
        StartGame();
    }
    public static void AddSkip()
    {
        if (IsGameOver)
            return;

        SkipCount++;
        SkipLeft--;

        if (SkipCount > MaxAllowedSkipCount)
        {
            SkipCount = 0;
            SkipLeft = MaxAllowedSkipCount;

            EventManager.ExceededSkip?.Invoke();
        }

        EventManager.UpdatedSkip?.Invoke();
    }

    private static void ResetStats()
    {
        DestroyedOnWave = 0;
        LeftTime = 60;
        IsGameOver = false;
        SkipCount = 0;
        MaxAllowedSkipCount = 2;
        SkipLeft = MaxAllowedSkipCount;

        EventManager.UpdatedSkip?.Invoke();
    }

    public static void ResetDestroyedCount()
    {
        DestroyedOnWave = 0;
    }

    private static IEnumerator Timer()
    {
        while (true)
        {
            EventManager.UpdatedTime?.Invoke();

            yield return new WaitForSeconds(1f);

            LeftTime--;

            if (LeftTime < 0)
            {
                IsGameOver = true;

                EventManager.GameOver?.Invoke();

                yield break;
            }
        }
    }

    private void AddTime()
    {
        LeftTime += DestroyedOnWave / 2f;

        EventManager.UpdatedTime?.Invoke();
    }
}
