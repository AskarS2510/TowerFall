using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public GameObject ActiveObject;
    public GameObject NextObject;

    private int _amountToPool;
    private GameObject _lastNext;
    [SerializeField] private List<GameObject> _pooledObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _amountToPool = _pooledObjects.Count;

        int randomPrefabIdx = Random.Range(0, _amountToPool);

        NextObject = _pooledObjects[randomPrefabIdx];

        _lastNext = NextObject;
    }

    public GameObject GetPooledObject()
    {
        ActiveObject = NextObject;

        if (MapManager.s_leftColorCubes.Count == 0)
            return null;

        int randomPrefabIdx = NextIdx();

        NextObject = _pooledObjects[randomPrefabIdx];

        _lastNext = NextObject;

        return ActiveObject;
    }

    int NextIdx()
    {
        Shuffle(_pooledObjects);

        string name;
        string nameWithoutBlock;

        int randomPrefabIdx = 0;

        for (int i = 0; i < _amountToPool; i++)
        {
            // Делаем следующий объект без повторений только если на поле больше 1 цеета
            if (MapManager.s_leftColorCubes.Count != 1 && _lastNext == _pooledObjects[i])
                continue;

            name = _pooledObjects[i].name;
            nameWithoutBlock = name.Substring(0, name.Length - 6);

            if (MapManager.s_leftColorCubes.ContainsKey(nameWithoutBlock))
            {
                randomPrefabIdx = i;

                break;
            }
        }

        return randomPrefabIdx;
    }

    void Shuffle(List<GameObject> list)
    {
        int n = list.Count;

        for (int i = 0; i < n; i++)
        {
            // Pick a new index higher than current for each item in the array
            int r = i + Random.Range(0, n - i);

            // Swap item into new spot
            GameObject temp = list[r];
            list[r] = list[i];
            list[i] = temp;
        }
    }
}
