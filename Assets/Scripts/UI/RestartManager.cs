using UnityEngine;

public class RestartManager : MonoBehaviour
{
    [SerializeField] private GameObject _winImage;
    [SerializeField] private GameObject _winLight;
    [SerializeField] private GameObject _loseImage;

    private void Start()
    {
        gameObject.SetActive(false);
        _winImage.SetActive(false);
        _winLight.SetActive(false);
        _loseImage.SetActive(false);

        EventManager.GameOver.AddListener(() => gameObject.SetActive(true));
        EventManager.GameIsWon.AddListener(GameOutcomeImage);
    }

    public void RestartGame()
    {
        gameObject.SetActive(false);
        _winImage.SetActive(false);
        _winLight.SetActive(false);
        _loseImage.SetActive(false);

        EventManager.RestartedGame?.Invoke();
    }

    private void GameOutcomeImage(bool isWin)
    {
        if (isWin)
        {
            _winImage.SetActive(true);
            _winLight.SetActive(true);
        }
        else
            _loseImage.SetActive(true);
    }
}
