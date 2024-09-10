using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static Dictionary<Color, bool> s_leftColorCubes = new Dictionary<Color, bool>();
    public int lastUsedID = -1;
    public int CenterMaxIdxX;
    public int CenterMaxIdxZ;
    public Dictionary<Vector3Int, Cube> CubeMap = new Dictionary<Vector3Int, Cube>();

    private Dictionary<Vector3Int, bool> _visited = new Dictionary<Vector3Int, bool>();
    private int _mapCenterMaxIdxX = 1;
    private int _mapCenterMaxIdxZ = 1;
    private int _mapCenterStartIdxY = 0;
    private List<Vector3Int> _shifts = new List<Vector3Int> { Vector3Int.left, Vector3Int.right,
            Vector3Int.up, Vector3Int.down,
            Vector3Int.forward, Vector3Int.back };
    private float _positionOffset = 1.2f;
    [SerializeField] private int _mapCenterMaxIdxY;
    [SerializeField] private List<GameObject> _prefabCubes;
    [SerializeField] private AudioSource _animationAudio;
    [SerializeField] private AudioSource _newLayerAudio;

    private void Awake()
    {
        PrepareMap(false);
    }

    public void PrepareMap(bool doEvent)
    {
        CreateCubeMap();

        MatchIDs();

        StartCoroutine(AnimateBuilding(doEvent));
    }

    private void Start()
    {
        EventManager.Destroyed.AddListener(DestroyWithID);
        EventManager.AllowedDestroyFlying.AddListener(DestroyFlyingCubes);
        EventManager.ExceededSkip.AddListener(AddBottomLayer);

        EventManager.RestartedGame.AddListener(() => PrepareMap(true));
    }

    private void CreateCubeMap()
    {
        ClearMap();

        for (int j = _mapCenterStartIdxY; j <= _mapCenterMaxIdxY; j++)
            for (int i = -_mapCenterMaxIdxX; i <= _mapCenterMaxIdxX; i++)
                for (int k = -_mapCenterMaxIdxZ; k <= _mapCenterMaxIdxZ; k++)
                {
                    Vector3Int cubePosition = new Vector3Int(i, j, k);

                    GameObject randomCube = RandomCube();

                    GameObject cubeObject = Instantiate(randomCube, (Vector3)cubePosition * _positionOffset, randomCube.transform.rotation, transform);

                    CubeMap.Add(cubePosition, cubeObject.GetComponent<Cube>());
                }
    }

    GameObject RandomCube()
    {
        return _prefabCubes[Random.Range(0, _prefabCubes.Count)];
    }

    public void MatchIDs()
    {
        foreach (KeyValuePair<Vector3Int, Cube> item in CubeMap)
        {
            item.Value.CubeID = -1;
        }

        lastUsedID = -1;

        foreach (KeyValuePair<Vector3Int, Cube> item in CubeMap)
        {
            if (item.Value.CubeID == -1)
            {
                lastUsedID++;
                item.Value.CubeID = lastUsedID;
            }

            FindNeighbours(item);
        }
    }

    private void ChangeIDtoCubes(int prevID, int newID)
    {
        foreach (KeyValuePair<Vector3Int, Cube> item in CubeMap)
        {
            if (item.Value.CubeID == prevID)
            {
                item.Value.CubeID = newID;
            }
        }
    }

    private void FindNeighbours(KeyValuePair<Vector3Int, Cube> item)
    {
        foreach (var shift in _shifts)
        {
            Vector3Int posToCheck = item.Key + shift;

            if (CubeMap.ContainsKey(posToCheck) && CubeMap[posToCheck].name == item.Value.name)
            {
                if (CubeMap[posToCheck].CubeID != -1)
                {
                    ChangeIDtoCubes(item.Value.CubeID, CubeMap[posToCheck].CubeID);
                }
                else
                {
                    CubeMap[posToCheck].CubeID = item.Value.CubeID;
                }
            }
        }
    }

    private void DestroyWithID(int ID)
    {
        var itemsToRemove = CubeMap.Where(item => item.Value.CubeID == ID).ToArray();
        foreach (var item in itemsToRemove)
        {
            ParticlesPool.Instance.SpawnParticles(item.Key, item.Value.material.color);

            CubeMap.Remove(item.Key);
            Destroy(item.Value.gameObject);

            GameManager.Instance.UpdateScore();
        }
    }

    public void AddCube(Vector3Int position, Cube cube)
    {
        CubeMap.Add(position, cube);
    }

    private void DFS(Vector3Int initialPos)
    {
        // Если узел уже посещен, то работа окончена, иначе продолжаем смотреть соседей
        if (!_visited.ContainsKey(initialPos))
        {
            _visited.Add(initialPos, true);
        }
        else
        {
            return;
        }

        foreach (var shift in _shifts)
        {
            Vector3Int posToCheck = initialPos + shift;

            if (CubeMap.ContainsKey(posToCheck))
            {
                DFS(posToCheck);
            }
        }
    }

    private void DestroyFlyingCubes()
    {
        _visited.Clear();

        // Ищем куб на земле (правило согласовано с начальной картой)
        for (int i = -CenterMaxIdxX; i <= CenterMaxIdxX; i++)
        {
            for (int k = -CenterMaxIdxZ; k <= CenterMaxIdxZ; k++)
            {
                Vector3Int checkPosition = new Vector3Int(i, _mapCenterStartIdxY, k);

                if (CubeMap.ContainsKey(checkPosition))
                {
                    DFS(checkPosition);
                }
            }
        }

        var unvisited = new Dictionary<Vector3Int, Cube>();

        foreach (var item in CubeMap)
        {
            if (!_visited.ContainsKey(item.Key))
            {
                unvisited.Add(item.Key, item.Value);
            }
        }

        foreach (var item in unvisited)
        {
            ParticlesPool.Instance.SpawnParticles(item.Key, item.Value.material.color);

            CubeMap.Remove(item.Key);
            Destroy(item.Value.gameObject);

            GameManager.Instance.UpdateScore();
        }
    }

    private void AddBottomLayer()
    {
        var extendedMap = new Dictionary<Vector3Int, Cube>();

        foreach (var item in CubeMap)
        {
            Vector3Int uppedKey = new Vector3Int(item.Key.x, item.Key.y + 1, item.Key.z);

            extendedMap.Add(uppedKey, item.Value);

            Vector3 prevPos = item.Value.transform.position;

            item.Value.transform.position = new Vector3(prevPos.x, prevPos.y + _positionOffset, prevPos.z);
        }

        for (int i = -CenterMaxIdxX; i <= CenterMaxIdxX; i++)
            for (int k = -CenterMaxIdxZ; k <= CenterMaxIdxZ; k++)
            {
                Vector3Int cubePosition = new Vector3Int(i, _mapCenterStartIdxY, k);

                if (CubeMap.ContainsKey(cubePosition))
                {
                    GameObject randomCube = RandomCube();

                    GameObject cubeObject = Instantiate(randomCube, (Vector3)cubePosition * _positionOffset, randomCube.transform.rotation, transform);

                    extendedMap.Add(cubePosition, cubeObject.GetComponent<Cube>());
                }
            }

        CubeMap = extendedMap;

        _newLayerAudio.Play();
    }

    private void ClearMap()
    {
        foreach (var item in CubeMap)
        {
            Destroy(item.Value.gameObject);
        }

        CubeMap.Clear();
    }

    public int MaxHeight()
    {
        int maxHeight = 0;

        foreach (var item in CubeMap)
        {
            if (item.Key.y > maxHeight)
                maxHeight = item.Key.y;
        }

        return maxHeight;
    }

    public void DefineLeftColors()
    {
        s_leftColorCubes.Clear();

        foreach (var item in CubeMap)
        {
            var color = item.Value.material.color;

            if (!s_leftColorCubes.ContainsKey(color))
                s_leftColorCubes.Add(color, true);
        }
    }

    private IEnumerator AnimateBuilding(bool doEvent)
    {
        float flyTime = 0.1f;
        float waitTime = 0.05f;

        foreach (var item in CubeMap)
        {
            Vector3 pos = (Vector3)item.Key * _positionOffset;

            item.Value.gameObject.transform.position = pos + Vector3.up * 40;
        }

        foreach (var item in CubeMap)
        {
            Vector3 pos = (Vector3)item.Key * _positionOffset;

            item.Value.gameObject.transform.DOMoveY(pos.y, flyTime).SetEase(Ease.OutExpo);

            _animationAudio.Play();

            yield return new WaitForSeconds(waitTime);
        }

        yield return null;

        if (doEvent)
        {
            yield return new WaitForSeconds(flyTime);
            EventManager.PreparedMap?.Invoke();
        }
    }

    [SerializeField] private float _forceY;
    [SerializeField] private float _maxForceUp;
    [SerializeField] private Vector3 shakeStrength;

    public enum BlockOutcome
    {
        Destruction,
        Stuck
    }

    public void StartAnimation(Vector3 explosionPosFromInt, BlockOutcome outcome)
    {
        if (outcome == BlockOutcome.Destruction)
            StartCoroutine(AnimateExplosion(explosionPosFromInt));

        if (outcome == BlockOutcome.Stuck)
            StartCoroutine(AnimateStick(explosionPosFromInt));
    }

    private IEnumerator AnimateStick(Vector3 stickFromPos)
    {
        float flyTime = 0.2f;
        float flyTime2 = 1f;
        float waitTime = flyTime;

        _forceY = stickFromPos.y;

        foreach (var item in CubeMap)
        {
            Vector3Int startPos = item.Key;

            if (_forceY != 0)
            {
                float scale = (_forceY - startPos.y) / _forceY + 1f;

                item.Value.transform.DOMoveY(startPos.y * _positionOffset - startPos.y / 2f, flyTime);
                item.Value.transform.DOMoveX(startPos.x * _positionOffset * scale, flyTime);
                item.Value.transform.DOMoveZ(startPos.z * _positionOffset * scale, flyTime);

                item.Value.transform.DOScaleX(scale, flyTime);
                item.Value.transform.DOScaleZ(scale, flyTime);
            }
            else
            {
                item.Value.transform.DOMoveY(startPos.y * _positionOffset + startPos.y, flyTime);
            }
        }

        yield return new WaitForSeconds(waitTime);

        foreach (var item in CubeMap)
        {
            Vector3Int startPos = item.Key;

            item.Value.transform.DOMove((Vector3)startPos * _positionOffset, flyTime2).SetEase(Ease.OutBounce);

            if (_forceY != 0)
            {
                item.Value.transform.DOScaleX(1f, flyTime2).SetEase(Ease.OutBounce);
                item.Value.transform.DOScaleZ(1f, flyTime2).SetEase(Ease.OutBounce);
            }

        }
    }

    private IEnumerator AnimateExplosion(Vector3 explosionPosFromInt)
    {
        float flyTime = 0.2f;
        float flyTime2 = 1f;
        float waitTime = flyTime;
        float waitTime2 = flyTime2;

        _forceY = explosionPosFromInt.y;

        Vector3 massCenter = Vector3.zero;

        foreach (var item in CubeMap)
        {
            massCenter += item.Key;
        }

        massCenter /= CubeMap.Count;

        Vector3 direction = massCenter.normalized - explosionPosFromInt.normalized;

        float forcePartX;
        float forcePartZ;

        if (direction.x == 0 && direction.z == 0)
        {
            forcePartX = 0;
            forcePartZ = 0;
        }
        else
        {
            forcePartX = direction.x / Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
            forcePartZ = direction.z / Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
        }


        float maxHeight = MaxHeight();

        if (maxHeight == 0)
            yield break;

        foreach (var item in CubeMap)
        {
            Vector3Int startPos = item.Key;

            float force;

            if (_forceY < maxHeight / 2 + 1 || (Mathf.Abs(forcePartX) <= 0.1f && Mathf.Abs(forcePartZ) <= 0.1f))
            {
                item.Value.transform.DOMoveY(startPos.y * _positionOffset + startPos.y, flyTime);
            }
            else
            {
                force = startPos.y;

                item.Value.transform.DOMoveX((startPos.x + force * forcePartX) * _positionOffset, flyTime);
                item.Value.transform.DOMoveZ((startPos.z + force * forcePartZ) * _positionOffset, flyTime);
            }
        }

        yield return new WaitForSeconds(waitTime);

        foreach (var item in CubeMap)
        {
            Vector3Int startPos = item.Key;

            item.Value.transform.DOMove((Vector3)startPos * _positionOffset, flyTime2).SetEase(Ease.OutBounce);
        }
    }
}
