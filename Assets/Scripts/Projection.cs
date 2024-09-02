using System.Collections.Generic;
using UnityEngine;

public class Projection : MonoBehaviour
{
    private float _positionOffset = 1.2f;
    private GameObject _currentProjection;
    private float _alphaProjection = 0.5f;
    [SerializeField] private List<GameObject> _projectionObjects = new List<GameObject>();

    void Start()
    {
        CreatePoolFromChildren();

        ChangeProjectionAlpha();
        OffShadowsCasting();

        EventManager.StoppedMovement.AddListener(ClearProjection);
        EventManager.SpawnedPlayerBlock.AddListener(ChangeProjection);
        EventManager.ChangedPosition.AddListener((int x, int z) => ChangeProjection());
    }

    private void CreatePoolFromChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _projectionObjects.Add(transform.GetChild(i).gameObject);
        }
    }

    private void ChangeProjection()
    {
        GameObject activeObject = ObjectPool.Instance.ActiveObject;

        foreach (var obj in _projectionObjects)
        {
            if (obj.name == activeObject.name)
            {
                CubeBlock cubeBlock = activeObject.GetComponent<CubeBlock>();

                obj.transform.position = (Vector3)cubeBlock.FinalPosition * _positionOffset;

                obj.transform.rotation = activeObject.transform.rotation;

                _currentProjection = obj;

                break;
            }
        }
    }

    private void ChangeProjectionAlpha()
    {
        foreach (var item in _projectionObjects)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                Material material = item.transform.GetChild(i).GetComponent<Renderer>().material;

                Color color = material.color;

                color.a = _alphaProjection;

                material.color = color;
            }
        }
    }

    private void OffShadowsCasting()
    {
        foreach (var item in _projectionObjects)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                GameObject cube = item.transform.GetChild(i).gameObject;

                Renderer renderer = cube.GetComponent<Renderer>();

                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }

    private void ClearProjection()
    {
        _currentProjection.transform.position = transform.position;
    }
}
