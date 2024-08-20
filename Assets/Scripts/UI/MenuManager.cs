using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        gameObject.SetActive(false);

        EventManager.StartedGame?.Invoke();
    }
}
