using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private void Start()
    {
        ChangeActiveAll(false);

        EventManager.GameOver.AddListener(() => ChangeActiveAll(false));

        if (GameManager.Instance.IsTutorialDone)
        {
            EventManager.StartedGame.AddListener(() => ChangeActiveAll(true));
        }
        else
        {
            EventManager.EndedTutorial.AddListener(() => ChangeActiveAll(true));
        }
    }

    private void ChangeActiveAll(bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;

            child.SetActive(active);
        }
    }
}
