using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPool : MonoBehaviour
{
    [SerializeField] private ParticleSystem _prefabParticle;
    public static ParticlesPool Instance;

    private int _amountToPool = 20;
    private List<ParticleSystem> _particlesPool;
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
        _particlesPool = new List<ParticleSystem>();

        for (int i = 0; i < _amountToPool; i++)
        {
            ParticleSystem newPrefab = Instantiate(_prefabParticle, transform);

            newPrefab.gameObject.SetActive(false);

            _particlesPool.Add(newPrefab);
        }
    }

    private ParticleSystem GetPooledObject()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            if (!_particlesPool[i].gameObject.activeInHierarchy)
            {
                _particlesPool[i].gameObject.SetActive(true);

                return _particlesPool[i];
            }
        }

        return null;
    }

    public void SpawnParticles(Vector3Int posInt, Color color)
    {
        ParticleSystem particle = GetPooledObject();

        if (particle == null)
            return;

        ParticleSystem.MainModule main = particle.main;
        main.startColor = color;

        particle.transform.position = (Vector3)posInt * _positionOffset;
        particle.Play();
        ClearWithDelay(particle.gameObject);
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
