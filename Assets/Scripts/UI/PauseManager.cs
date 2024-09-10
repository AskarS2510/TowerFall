using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void UnpauseGame()
    {
        gameObject.SetActive(false);

        Time.timeScale = 1.0f;

        EventManager.UnpausedGame?.Invoke();
    }

    public void PauseGame()
    {
        gameObject.SetActive(true);

        Time.timeScale = 0.0f;

        EventManager.PausedGame?.Invoke();
    }

    public void RaiseHowToPlay()
    {
        EventManager.RaisedHowToPlay?.Invoke();
    }
}
