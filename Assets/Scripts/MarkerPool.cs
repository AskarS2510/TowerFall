using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefabMarker;
    public static MarkerPool Instance;

    private int _amountToPool = 8;
    private List<GameObject> _pool;
    private float _positionOffset = 1.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _pool = new List<GameObject>();

        for (int i = 0; i < _amountToPool; i++)
        {
            GameObject newPrefab = Instantiate(_prefabMarker);

            newPrefab.SetActive(false);

            _pool.Add(newPrefab);
        }

        EventManager.ChangedPosition.AddListener(ChangeMarkerPosition);
        EventManager.StoppedMovement.AddListener(ClearPool);
    }

    public GameObject GetPooledObject()
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

    public void ClearPool()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            _pool[i].SetActive(false);
        }
    }

    void ChangeMarkerPosition(int x, int z)
    {
        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();
        List<Vector3Int> blockScheme = cubeBlock.RotatedBlockScheme;

        for (int i = 0; i < blockScheme.Count; i++)
        {
            _pool[i].transform.position = new Vector3((x + blockScheme[i].x) * _positionOffset,
                _pool[i].transform.position.y, (z + blockScheme[i].z) * _positionOffset);
        }
    }
}
