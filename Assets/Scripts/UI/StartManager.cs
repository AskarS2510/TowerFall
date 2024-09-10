using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    private float _waitBeforeEnable = 5f;
    private float _flyTime = 0.5f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_waitBeforeEnable);

        transform.DOLocalMoveY(0f, _flyTime);
    }

    public void StartGame()
    {
        gameObject.SetActive(false);

        EventManager.StartedGame?.Invoke();
    }
}
