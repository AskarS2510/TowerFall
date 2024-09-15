using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefabParticleAudio;
    public static ParticlesPool Instance;

    private int _amountToPool = 25;
    private List<GameObject> _particlesPool;
    private List<ParticleSystem> _particlesComponents;
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
        _particlesPool = new List<GameObject>();
        _particlesComponents = new List<ParticleSystem>();

        for (int i = 0; i < _amountToPool; i++)
        {
            GameObject newPrefab = Instantiate(_prefabParticleAudio, transform);

            newPrefab.SetActive(false);

            _particlesPool.Add(newPrefab);
            _particlesComponents.Add(newPrefab.GetComponent<ParticleSystem>());
        }
    }

    private int GetPooledObjectIdx()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            if (!_particlesPool[i].activeInHierarchy)
            {
                _particlesPool[i].SetActive(true);

                return i;
            }
        }

        return -1;
    }

    public void SpawnParticles(Vector3Int posInt, Color color)
    {
        int idx = GetPooledObjectIdx();

        if (idx == -1)
            return;

        GameObject pooledObject = _particlesPool[idx];

        ParticleSystem.MainModule main = _particlesComponents[idx].main;
        main.startColor = color;

        pooledObject.transform.position = (Vector3)posInt * _positionOffset;
        _particlesComponents[idx].Play();

        ClearWithDelay(pooledObject);
    }

    private void ClearWithDelay(GameObject obj)
    {
        StartCoroutine(ClearParticle(obj));
    }

    private IEnumerator ClearParticle(GameObject obj)
    {
        yield return new WaitForSeconds(GameManager.Instance.EffectsDuration);

        obj.SetActive(false);
    }
}
