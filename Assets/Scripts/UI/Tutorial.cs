using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private int _upArrowX = 0;
    private int _upArrowZ = 1;
    private int _leftArrowX = -1;
    private int _leftArrowZ = 0;

    void Start()
    {
        gameObject.SetActive(false);

        EventManager.StartedGame.AddListener(() => gameObject.SetActive(true));
        EventManager.EndedTutorial.AddListener(() => gameObject.SetActive(false));

        EventManager.StartedGame.AddListener(() => StartCoroutine(StartTouchTutorial()));
    }

    private IEnumerator StartTouchTutorial()
    {
        //float delay = 0.5f;
        float delay = 0.001f;

        yield return new WaitForSeconds(delay);
        EventManager.RaisedMove?.Invoke(-_upArrowX, -_upArrowZ);
        yield return new WaitForSeconds(delay);
        EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);
        yield return new WaitForSeconds(delay);
        EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);
        yield return new WaitForSeconds(delay);
        EventManager.RaisedMove?.Invoke(-_upArrowX, -_upArrowZ);

        yield return new WaitForSeconds(delay);
        EventManager.RaisedMove?.Invoke(_leftArrowX, _leftArrowZ);
        yield return new WaitForSeconds(delay);
        EventManager.RaisedMove?.Invoke(-_leftArrowX, -_leftArrowZ);
        yield return new WaitForSeconds(delay);
        EventManager.RaisedMove?.Invoke(-_leftArrowX, -_leftArrowZ);
        yield return new WaitForSeconds(delay);
        EventManager.RaisedMove?.Invoke(_leftArrowX, _leftArrowZ);

        GameManager.IsTutorialDone = true;

        EventManager.EndedTutorial?.Invoke();

        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();

        cubeBlock.StartMoveDown();
    }
}
