using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

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
    private GameObject _activeTutorialKeys;
    [SerializeField] private GameObject _wasdParent;
    [SerializeField] private GameObject _arrowsParent;
    [SerializeField] private GameObject _spaceParent;

    private void Start()
    {
        gameObject.SetActive(false);

        EventManager.RaisedHowToPlay.AddListener(RestartTutorial);

        if (GameManager.Instance.IsTutorialDone)
            return;

        _buttonLeft.SetActive(false);
        _buttonRight.SetActive(false);
        _buttonDrop.SetActive(false);
        _wasdParent.SetActive(false);
        _arrowsParent.SetActive(false);
        _spaceParent.SetActive(false);

        EventManager.EndedTutorial.AddListener(() => gameObject.SetActive(false));

        if (GameManager.Instance.userDeviceType == DeviceType.Handheld)
        {
            tutorials = new List<IEnumerator>() { MoveTutorial(), LeftRightButtonsTutorial(), DropDownTutorial() };
        }
        else
        {
            _cursorImage.SetActive(false);
            tutorials = new List<IEnumerator>() { MoveTutorialDesktop(), LeftRightTutorialDesktop(), DropDownTutorialDesktop() };
        }

        tutorialIdx = 0;

        EventManager.StartedGame.AddListener(() => gameObject.SetActive(true));
        EventManager.StartedGame.AddListener(() => StartCoroutine(tutorials[tutorialIdx]));
    }

    private IEnumerator DyeKey(string name, float time)
    {
        List<GameObject> objectsToCheck = new List<GameObject>() { _wasdParent, _arrowsParent, _spaceParent };

        foreach (GameObject obj in objectsToCheck)
            foreach (Transform item in obj.transform)
            {
                if (item.name == name)
                {
                    Image image = item.GetComponent<Image>();

                    image.color = Color.red;

                    yield return new WaitForSeconds(time);

                    image.color = Color.white;

                    yield break;
                }
            }
    }

    private IEnumerator DropDownTutorial()
    {
        _buttonDrop.SetActive(true);

        float delay = 0.5f;
        float afterDropDelay = 4f;

        while (true)
        {
            _cursorImage.transform.localPosition = Vector3.zero;
            _cursorImage.gameObject.SetActive(true);

            _cursorImage.transform.DOMoveX(_buttonDrop.transform.position.x, 2 * delay);
            _cursorImage.transform.DOMoveY(_buttonDrop.transform.position.y, 2 * delay);
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
        _buttonLeft.SetActive(true);
        _buttonRight.SetActive(true);

        float delay = 0.5f;

        while (true)
        {
            _cursorImage.transform.DOMoveX(_buttonRight.transform.position.x, 2 * delay);
            _cursorImage.transform.DOMoveY(_buttonRight.transform.position.y, 2 * delay);
            yield return new WaitForSeconds(2 * delay);
            _cursorImage.transform.DOScale(0.7f, delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedRotate?.Invoke(false, true);
            _cursorImage.transform.DOScale(1f, delay);
            yield return new WaitForSeconds(delay);

            _cursorImage.transform.DOMoveX(_buttonLeft.transform.position.x, 2 * delay);
            _cursorImage.transform.DOMoveY(_buttonLeft.transform.position.y, 2 * delay);
            yield return new WaitForSeconds(2 * delay);
            _cursorImage.transform.DOScale(0.7f, delay);
            yield return new WaitForSeconds(delay);
            EventManager.RaisedRotate?.Invoke(true, false);
            _cursorImage.transform.DOScale(1f, delay);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator LeftRightTutorialDesktop()
    {
        _activeTutorialKeys = _arrowsParent;
        _activeTutorialKeys.SetActive(true);

        float delay = 0.5f;

        while (true)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(DyeKey("right", delay));
            EventManager.RaisedRotate?.Invoke(false, true);
            yield return new WaitForSeconds(2 * delay);
            StartCoroutine(DyeKey("left", delay));
            EventManager.RaisedRotate?.Invoke(true, false);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator DropDownTutorialDesktop()
    {
        _activeTutorialKeys = _spaceParent;
        _activeTutorialKeys.SetActive(true);

        float delay = 0.5f;
        float afterDropDelay = 3f;

        while (true)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(DyeKey("space", delay));
            EventManager.RaisedDropDown?.Invoke();
            yield return new WaitForSeconds(afterDropDelay);
        }
    }

    private IEnumerator MoveTutorialDesktop()
    {
        _activeTutorialKeys = _wasdParent;
        _activeTutorialKeys.SetActive(true);

        float delay = 0.5f;

        while (true)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(DyeKey("w", delay));
            EventManager.RaisedMove?.Invoke(_upArrowX, _upArrowZ);
            yield return new WaitForSeconds(2 * delay);
            StartCoroutine(DyeKey("s", delay));
            EventManager.RaisedMove?.Invoke(-_upArrowX, -_upArrowZ);
            yield return new WaitForSeconds(delay);

            yield return new WaitForSeconds(delay);
            StartCoroutine(DyeKey("a", delay));
            EventManager.RaisedMove?.Invoke(_leftArrowX, _leftArrowZ);
            yield return new WaitForSeconds(2 * delay);
            StartCoroutine(DyeKey("d", delay));
            EventManager.RaisedMove?.Invoke(-_leftArrowX, -_leftArrowZ);
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
        if (_activeTutorialKeys != null)
            _activeTutorialKeys.SetActive(false);
        StopCoroutine(tutorials[tutorialIdx]);

        tutorialIdx++;

        if (tutorialIdx < tutorials.Count)
        {
            StartCoroutine(tutorials[tutorialIdx]);

            return;
        }

        //PlayerPrefs.SetInt("IsTutorialDone", 1);
        //PlayerPrefs.Save();

        YandexGame.savesData.IsTutorialDone = true;
        YandexGame.SaveProgress();

        GameManager.Instance.IsTutorialDone = true;        

        EventManager.EndedTutorial?.Invoke();

        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();

        cubeBlock.StartMoveDown();
    }

    public void RestartTutorial()
    {
        //PlayerPrefs.SetInt("IsTutorialDone", 0);
        //PlayerPrefs.Save();

        YandexGame.savesData.IsTutorialDone = false;
        YandexGame.SaveProgress();

        GameManager.Instance.IsTutorialDone = false;


        //YandexGame.NewLBScoreTimeConvert("TimeLeaderBoardNew", 8.124142141f * 1000f);

        DOTween.KillAll();

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
