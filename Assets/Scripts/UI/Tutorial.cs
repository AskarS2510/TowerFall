using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        EventManager.StartedGame.AddListener(() => gameObject.SetActive(true));
        EventManager.EndedTutorial.AddListener(() => gameObject.SetActive(false));
    }
}
