using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRestart : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);

        EventManager.AskedHowToPlay.AddListener(() => gameObject.SetActive(true));
    }

    public void RaiseHowToPlay()
    {
        EventManager.RaisedHowToPlay?.Invoke();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
