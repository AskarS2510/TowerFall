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

        textMesh.text = fromTimeString;
    }
}
