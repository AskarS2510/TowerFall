using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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
