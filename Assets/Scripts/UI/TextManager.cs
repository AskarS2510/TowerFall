using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TextManager : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        EventManager.UpdatedScore.AddListener(UpdateText);
        EventManager.UpdatedSkip.AddListener(UpdateText);

        UpdateText();
    }

    void UpdateText()
    {
        if (gameObject.name == "Score Text")
        {
            textMesh.text = GameManager.Score.ToString();
        }

        if (gameObject.name == "Remain Text")
        {
            textMesh.text = GameManager.SkipLeft.ToString();
        }
    }
}
