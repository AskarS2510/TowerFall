using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    private int _upArrowX = 0;
    private int _upArrowZ = 1;
    private int _leftArrowX = -1;
    private int _leftArrowZ = 0;
    private int tutorialIdx;
    private List<IEnumerator> tutorials;

    [SerializeField] private GameObject _cursorImage;
    [SerializeField] private GameObject _buttonLeft;
    [SerializeField] private GameObject _buttonRight;
    [SerializeField] private GameObject _buttonDrop;

    void Start()
    {
        gameObject.SetActive(false);

        if (GameManager.Instance.IsTutorialDone)
            return;

        _buttonLeft.SetActive(false);
        _buttonRight.SetActive(false);
        _buttonDrop.SetActive(false);

        EventManager.EndedTutorial.AddListener(() => gameObject.SetActive(false));

        EventManager.RaisedHowToPlay.AddListener(RestartTutorial);

        tutorials = new List<IEnumerator>() { MoveTutorial(), LeftRightButtonsTutorial(), DropDownTutorial() };
        tutorialIdx = 0;

        EventManager.StartedGame.AddListener(() => gameObject.SetActive(true));
        EventManager.StartedGame.AddListener(() => StartCoroutine(tutorials[tutorialIdx]));
    }
    private IEnumerator DropDownTutorial()
    {
        if (GameManager.Instance.userDeviceType != DeviceType.Desktop)
            _buttonDrop.SetActive(true);

        float xRight = 190f;
        float y = -165f;
        float delay = 0.5f;
        float afterDropDelay = 4f;


        while (true)
        {
            _cursorImage.transform.localPosition = Vector3.zero;
            _cursorImage.gameObject.SetActive(true);

            _cursorImage.transform.DOLocalMoveX(xRight, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(y, 2 * delay);
            yield return new WaitForSeconds(2 * delay);
            _cursorImage.transform.DOScale(0.7f, delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedDropDown?.Invoke();
            _cursorImage.transform.DOScale(1f, delay);

            _cursorImage.gameObject.SetActive(false);

            yield return new WaitForSeconds(afterDropDelay);
        }
    }

    private IEnumerator LeftRightButtonsTutorial()
    {
        _cursorImage.transform.localPosition = Vector3.zero;
        if (GameManager.Instance.userDeviceType != DeviceType.Desktop)
        {
            _buttonLeft.SetActive(true);
            _buttonRight.SetActive(true);
        }

        float xRight = 190f;
        float xLeft = -175f;
        float y = -40f;
        float delay = 0.5f;

        while (true)
        {
            _cursorImage.transform.DOLocalMoveX(xRight, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(y, 2 * delay);
            yield return new WaitForSeconds(2 * delay);
            _cursorImage.transform.DOScale(0.7f, delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedRotate?.Invoke(false, true);
            _cursorImage.transform.DOScale(1f, delay);
            yield return new WaitForSeconds(delay);

            _cursorImage.transform.DOLocalMoveX(xLeft, 2 * delay);
            _cursorImage.transform.DOLocalMoveY(y, 2 * delay);
            yield return new WaitForSeconds(2 * delay);
            _cursorImage.transform.DOScale(0.7f, delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedRotate?.Invoke(true, false);
            _cursorImage.transform.DOScale(1f, delay);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator MoveTutorial()
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
        StopCoroutine(tutorials[tutorialIdx]);

        tutorialIdx++;

        if (tutorialIdx < tutorials.Count)
        {
            StartCoroutine(tutorials[tutorialIdx]);

            return;
        }

        GameManager.Instance.IsTutorialDone = true;        

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
