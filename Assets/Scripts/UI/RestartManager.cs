using UnityEngine;

public class RestartManager : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);

        EventManager.GameOver.AddListener(() => gameObject.SetActive(true));
    }

    public void RestartGame()
    {
        gameObject.SetActive(false);

        EventManager.RestartedGame?.Invoke();
    }
}
