using System;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    private void Awake()
    {
        if (gameObject.name != "Passed Time Text")
            return;

        EventManager.GameIsWon.AddListener((value) => UpdatePassedTime());
    }

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        EventManager.UpdatedSkip.AddListener(UpdateSkip);
        EventManager.UpdatedTime.AddListener(UpdateLeftTime);

        UpdateSkip();
        UpdateLeftTime();
    }

    private void UpdateSkip()
    {
        if (gameObject.name != "Skip Text")
            return;

        textMesh.text = GameManager.Instance.SkipLeft.ToString();
    }

    private void UpdatePassedTime()
    {
        if (gameObject.name != "Passed Time Text")
            return;

        float time = GameManager.Instance.PassedTime;

        TimeSpan result = TimeSpan.FromSeconds(time);

        string fromTimeString;

        fromTimeString = result.ToString("m':'ss");

        textMesh.text = fromTimeString;
    }

    private void UpdateLeftTime()
    {
        if (gameObject.name != "Left Time Text")
            return;

        float time = GameManager.Instance.LeftTime;

        TimeSpan result = TimeSpan.FromSeconds(time);

        string fromTimeString;
        if (time > 60)
            fromTimeString = result.ToString("m':'ss");
        else
            fromTimeString = result.ToString("ss");

        if (time < 10)
            fromTimeString = result.ToString("%s");

        textMesh.text = fromTimeString;
    }
}
