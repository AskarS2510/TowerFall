using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _previewObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.SpawnedPlayerBlock.AddListener(ChangePreview);
    }

    void ChangePreview()
    {
        GameObject next = ObjectPool.Instance.NextObject;

        foreach (var obj in _previewObjects)
        {
            if (obj.name == next.name)
            {
                transform.position = new Vector3(obj.transform.position.x + 2.5f, transform.position.y, transform.position.z);

                break;
            }
        }
    }
}
