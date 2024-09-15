using System;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        EventManager.UpdatedSkip.AddListener(UpdateSkip);
        EventManager.UpdatedTime.AddListener(UpdatePassedTime);
        EventManager.UpdatedTime.AddListener(UpdateLeftTime);

        UpdateSkip();
        UpdateLeftTime();
        UpdatePassedTime();
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
