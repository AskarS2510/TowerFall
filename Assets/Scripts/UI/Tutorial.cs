using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    private int _upArrowX = 0;
    private int _upArrowZ = 1;
    private int _leftArrowX = -1;
    private int _leftArrowZ = 0;

    [SerializeField] private GameObject _cursorImage;

    void Start()
    {
        gameObject.SetActive(false);

        if (GameManager.Instance.IsTutorialDone)
            return;

        EventManager.EndedTutorial.AddListener(() => gameObject.SetActive(false));

        EventManager.RaisedHowToPlay.AddListener(RestartTutorial);

        EventManager.StartedGame.AddListener(() => gameObject.SetActive(true));
        EventManager.StartedGame.AddListener(() => StartCoroutine(StartTouchTutorial()));
    }

    private IEnumerator StartTouchTutorial()
    {
        _cursorImage.transform.localPosition = Vector3.zero;

        float delay = 0.5f;
        //float delay = 0.001f;
        float cursorPath = 40;

        while (true)
        {
            _cursorImage.transform.DOLocalMoveX(-cursorPath, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(-cursorPath, 2 * delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedMove?.Invoke(-_upArrowX, -_upArrowZ);
            yield return new WaitForSeconds(delay);
            _cursorImage.transform.DOLocalMoveX(0, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(0, 2 * delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);
            yield return new WaitForSeconds(delay);

            _cursorImage.transform.DOLocalMoveX(cursorPath, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(cursorPath, 2 * delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);
            yield return new WaitForSeconds(delay);
            _cursorImage.transform.DOLocalMoveX(0, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(0, 2 * delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedMove?.Invoke(-_upArrowX, -_upArrowZ);
            yield return new WaitForSeconds(delay);


            _cursorImage.transform.DOLocalMoveX(-cursorPath, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(cursorPath, 2 * delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedMove?.Invoke(_leftArrowX, _leftArrowZ);
            yield return new WaitForSeconds(delay);
            _cursorImage.transform.DOLocalMoveX(0, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(0, 2 * delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedMove?.Invoke(-_leftArrowX, -_leftArrowZ);
            yield return new WaitForSeconds(delay);


            _cursorImage.transform.DOLocalMoveX(cursorPath, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(-cursorPath, 2 * delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedMove?.Invoke(-_leftArrowX, -_leftArrowZ);
            yield return new WaitForSeconds(delay);
            _cursorImage.transform.DOLocalMoveX(0, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(0, 2 * delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedMove?.Invoke(_leftArrowX, _leftArrowZ);
            yield return new WaitForSeconds(delay);
        }
    }

    public void OnOK()
    {
        GameManager.Instance.IsTutorialDone = true;

        StopCoroutine(StartTouchTutorial());

        EventManager.EndedTutorial?.Invoke();

        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();

        cubeBlock.StartMoveDown();
    }

    public void RestartTutorial()
    {
        GameManager.Instance.IsTutorialDone = false;

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
