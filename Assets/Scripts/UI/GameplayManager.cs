using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        EventManager.StartedGame.AddListener(() => gameObject.SetActive(true));
        //EventManager.GameOver.AddListener(() => gameObject.SetActive(false));
        EventManager.RestartedGame.AddListener(() => gameObject.SetActive(true));
    }
}
