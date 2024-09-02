using System.Collections.Generic;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    private GameObject _lastNext;
    [SerializeField] private List<GameObject> _previewObjects = new List<GameObject>();

    private void Start()
    {
        CreatePoolFromChildren();

        OffShadowsCasting();

        _lastNext = null;

        EventManager.SpawnedPlayerBlock.AddListener(ChangePreview);

        ChangePreview();
    }

    private void CreatePoolFromChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _previewObjects.Add(transform.GetChild(i).gameObject);
        }
    }

    private void OffShadowsCasting()
    {
        foreach (var item in _previewObjects)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                GameObject cube = item.transform.GetChild(i).gameObject;

                Renderer renderer = cube.GetComponent<Renderer>();

                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
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

                _lastNext = obj;

                break;
            }
        }
    }
}
