using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance;
    [HideInInspector] public bool IsTutorialDone;
    [HideInInspector] public int DestroyedOnWave;
    [HideInInspector] public float EffectsDuration;
    [HideInInspector] public float LeftTime;
    [HideInInspector] public float PassedTime;
    [HideInInspector] public bool IsGameOver;
    [HideInInspector] public int SkipCount;
    [HideInInspector] public int MaxAllowedSkipCount;
    [HideInInspector] public int SkipLeft;
    //[HideInInspector] public float DefaultAudioValue = 0.1f;
    //[HideInInspector] public float DefaultSense = 0.5f;
    [HideInInspector] public IEnumerator _timer;
    [HideInInspector] public bool IsTopExplosion;
    public DeviceType userDeviceType;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();

        IsTutorialDone = YandexGame.savesData.IsTutorialDone;

        //if (PlayerPrefs.GetInt("IsTutorialDone", 0) == 0)
        //    IsTutorialDone = false;
        //else
        //    IsTutorialDone = true;

        if (Instance == null)
        {
            Instance = this;
        }

        if (YandexGame.EnvironmentData.isDesktop)
            userDeviceType = DeviceType.Desktop;
        else
            userDeviceType = DeviceType.Handheld;

        //userDeviceType = DeviceType.Handheld;
    }

    private void Start()
    {
        YandexGame.StickyAdActivity(true);

        //IsTutorialDone = false;
        EffectsDuration = 1.2f;

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
        Time.timeScale = 0f;

        if (_timer != null)
            StopCoroutine(_timer);
        _timer = null;
    }

    private void StartGame()
    {
        ResetStats();

        _timer = Timer();
        StartCoroutine(_timer);
    }

    private void RestartGame()
    {
        //PlayerPrefs.Save();
        YandexGame.SaveProgress();

        DOTween.KillAll();

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        }

        EventManager.UpdatedSkip?.Invoke();
    }

    private void ResetStats()
    {
        PassedTime = 0f;
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

    private void Update()
    {
        PassedTime += Time.deltaTime;
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
                EventManager.GameIsWon?.Invoke(false);

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
