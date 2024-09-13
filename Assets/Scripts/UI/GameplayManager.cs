using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance.IsTutorialDone)
        {
            ChangeActiveAll(true);

            return;
        }

        ChangeActiveAll(false);

        EventManager.EndedTutorial.AddListener(() => ChangeActiveAll(true));
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
