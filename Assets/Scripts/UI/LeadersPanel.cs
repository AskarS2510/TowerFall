using UnityEngine;
using YG;

public class LeadersPanel : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderboard;

    void Awake()
    {
        Debug.Log("BestTime = " + YandexGame.savesData.BestTime);
        _leaderboard.UpdateLB();

        gameObject.SetActive(false);

        EventManager.RaisedShowLeaders.AddListener(ActivateLeaderboard);

        EventManager.GameIsWon.AddListener(AddTimeToLeaderboard);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void ActivateLeaderboard()
    {
        gameObject.SetActive(true);
    }

    private void AddTimeToLeaderboard(bool isWin)
    {
        Debug.Log("AddTimeToLeaderboard method");

        if (!isWin)
            return;

        Debug.Log("AddTimeToLeaderboard method isWin = " + isWin);
        Debug.Log("passedTime = " + GameManager.Instance.PassedTime);

        float winTime = GameManager.Instance.PassedTime;

        Debug.Log("winTime = " + winTime);

        if (winTime <= YandexGame.savesData.BestTime)
        {
            Debug.Log("enter if");
            Debug.Log("NewLBScoreTimeConvert winTime = " + winTime);
            Debug.Log("BestTime = " + YandexGame.savesData.BestTime);

            YandexGame.NewLBScoreTimeConvert("TimeLeaderBoardNew", winTime * 1000f);
            _leaderboard.UpdateLB();

            YandexGame.savesData.BestTime = winTime;
            YandexGame.SaveProgress();
        }
    }
}
