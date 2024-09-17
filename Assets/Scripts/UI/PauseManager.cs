using UnityEngine;
using YG;

public class PauseManager : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void UnpauseGame()
    {
        YandexGame.SaveProgress();

        gameObject.SetActive(false);

        if (!GameManager.Instance.IsGameOver)
            Time.timeScale = 1.0f;

        EventManager.UnpausedGame?.Invoke();
    }

    public void PauseGame()
    {
        gameObject.SetActive(true);

        Time.timeScale = 0.0f;

        EventManager.PausedGame?.Invoke();
    }

    public void AskHowToPlay()
    {
        EventManager.AskedHowToPlay?.Invoke();
    }

    public void RaiseShowLeaders()
    {
        EventManager.RaisedShowLeaders?.Invoke();
    }
}
