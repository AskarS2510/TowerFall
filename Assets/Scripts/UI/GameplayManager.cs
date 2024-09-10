using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance.IsTutorialDone)
            return;

        gameObject.SetActive(false);

        EventManager.EndedTutorial.AddListener(() => gameObject.SetActive(true));
    }
}
