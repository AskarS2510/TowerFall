using System.Collections.Generic;
using UnityEngine;

public class MarkerPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefabMarker;

    private int _amountToPool = 8;
    private List<GameObject> _pool;
    private float _positionOffset = 1.2f;

    private void Start()
    {
        CreatePool();

        EventManager.ChangedPosition.AddListener(ChangeMarkerPosition);
        EventManager.StoppedMovement.AddListener(ClearPool);
        EventManager.SpawnedPlayerBlock.AddListener(SpawnMarker);
    }

    private void CreatePool()
    {
        _pool = new List<GameObject>();

        for (int i = 0; i < _amountToPool; i++)
        {
            GameObject newPrefab = Instantiate(_prefabMarker, transform);

            newPrefab.SetActive(false);

            _pool.Add(newPrefab);
        }
    }

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            if (!_pool[i].activeInHierarchy)
            {
                _pool[i].SetActive(true);

                return _pool[i];
            }
        }

        return null;
    }

    private void ClearPool()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            _pool[i].SetActive(false);
        }
    }

    private void ChangeMarkerPosition(int x, int z)
    {
        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();
        List<Vector3Int> blockScheme = cubeBlock.RotatedBlockScheme;

        for (int i = 0; i < blockScheme.Count; i++)
        {
            _pool[i].transform.position = new Vector3((x + blockScheme[i].x) * _positionOffset,
                _pool[i].transform.position.y, (z + blockScheme[i].z) * _positionOffset);
        }
    }

    private void SpawnMarker()
    {
        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();
        List<Vector3Int> blockScheme = cubeBlock.RotatedBlockScheme;

        for (int i = 0; i < blockScheme.Count; i++)
        {
            GameObject marker = GetPooledObject();

            marker.transform.position = new Vector3(cubeBlock.transform.position.x + blockScheme[i].x * _positionOffset,
                marker.transform.position.y, cubeBlock.transform.position.z + blockScheme[i].z * _positionOffset);
        }
    }
}
