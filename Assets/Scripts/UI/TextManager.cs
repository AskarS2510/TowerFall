using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        EventManager.UpdatedScore.AddListener(UpdateText);
        EventManager.UpdatedSkip.AddListener(UpdateText);

        UpdateText();
    }

    private void UpdateText()
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
