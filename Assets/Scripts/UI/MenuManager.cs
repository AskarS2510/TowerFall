using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private int _waitBeforeEnable = 2;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_waitBeforeEnable);

        transform.DOLocalMoveY(0f, 1f);
    }

    public void StartGame()
    {
        gameObject.SetActive(false);

        EventManager.StartedGame?.Invoke();
    }
}
