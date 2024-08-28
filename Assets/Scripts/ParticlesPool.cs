using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefabPartice;
    public static ParticlesPool Instance;

    private int _amountToPool = 20;
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
            GameObject newPrefab = Instantiate(_prefabPartice, transform);

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

    public void SpawnParticles(Vector3Int posInt)
    {
        GameObject particle = GetPooledObject();

        if (particle == null)
            return;

        particle.transform.position = (Vector3)posInt * _positionOffset;
        particle.GetComponent<ParticleSystem>().Play();
        ClearWithDelay(particle);
    }

    private void ClearWithDelay(GameObject obj)
    {
        StartCoroutine(ClearParticle(obj));
    }

    private IEnumerator ClearParticle(GameObject obj)
    {
        yield return new WaitForSeconds(1f);

        obj.SetActive(false);
    }
}
