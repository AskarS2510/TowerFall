using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsTutorialDone;
    public int DestroyedOnWave;
    public float DelayBetweenWaves;
    public float LeftTime;
    public bool IsGameOver;
    public int SkipCount;
    public int MaxAllowedSkipCount;
    public int SkipLeft;
    public IEnumerator _timer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //IsTutorialDone = false;
        DelayBetweenWaves = 1.2f;

        ResetStats();

        if (!IsTutorialDone)
            EventManager.EndedTutorial.AddListener(StartGame);
        else
            EventManager.StartedGame.AddListener(StartGame);

        EventManager.RestartedGame.AddListener(RestartGame);
        EventManager.PreparedMap.AddListener(StartGame);

        EventManager.GameOver.AddListener(GameOver);

        EventManager.SpawnedPlayerBlock.AddListener(ResetDestroyedCount);

        EventManager.DoneDestruction.AddListener(AddTime);
    }

    public void UpdateScore()
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
    }
    public void AddSkip()
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

    private void ResetStats()
    {
        DestroyedOnWave = 0;
        LeftTime = 90;
        IsGameOver = false;
        SkipCount = 0;
        MaxAllowedSkipCount = 2;
        SkipLeft = MaxAllowedSkipCount;

        EventManager.UpdatedSkip?.Invoke();
        EventManager.UpdatedTime?.Invoke();
    }

    public void ResetDestroyedCount()
    {
        DestroyedOnWave = 0;
    }

    private IEnumerator Timer()
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
