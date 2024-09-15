using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaders : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);

        EventManager.RaisedShowLeaders.AddListener(() => gameObject.SetActive(true));
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
