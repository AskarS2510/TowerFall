using System.Collections.Generic;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    private GameObject _lastNext;
    [SerializeField] private List<GameObject> _previewObjects = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        _lastNext = null;
        EventManager.SpawnedPlayerBlock.AddListener(ChangePreview);
    }

    private void ChangePreview()
    {
        GameObject next = ObjectPool.Instance.NextObject;

        foreach (var obj in _previewObjects)
        {
            if (obj.name == next.name)
            {
                if (_lastNext != null)
                    _lastNext.transform.position += Vector3.up * 10;

                obj.transform.position -= Vector3.up * 10;

                //transform.position = new Vector3(obj.transform.position.x + 2.5f, transform.position.y, transform.position.z);

                _lastNext = obj;

                break;
            }
        }
    }
}
