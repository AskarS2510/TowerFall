using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class MapManager : MonoBehaviour
{
    public static Dictionary<string, bool> s_leftColorCubes = new Dictionary<string, bool>();
    public static int s_lastUsedID = -1;
    public int CenterMaxIdxX;
    public int CenterMaxIdxZ;
    public Dictionary<Vector3Int, Cube> CubeMap = new Dictionary<Vector3Int, Cube>();

    private Dictionary<Vector3Int, bool> _visited = new Dictionary<Vector3Int, bool>();
    private int _mapCenterMaxIdxX = 1;
    private int _mapCenterMaxIdxZ = 1;
    private int _mapCenterStartIdxY = 0;
    private int[,] _posToCheck = { { -1, 0, 0 }, { 1, 0, 0 },
        { 0, -1, 0 }, { 0, 1, 0 },
        { 0, 0, -1 }, { 0, 0, 1 } };
    private float _positionOffset = 1.2f;
    [SerializeField] private int _mapCenterMaxIdxY;
    [SerializeField] private List<GameObject> _prefabCubes;

    private void Awake()
    {
        PrepareMap();
    }

    public void PrepareMap()
    {
        CreateCubeMap();

        MatchIDs();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Random.seed = 0;

        EventManager.Destroyed.AddListener(DestroyWithID);
        EventManager.AllowedDestroyFlying.AddListener(DestroyFlyingCubes);
        EventManager.ExceededSkip.AddListener(AddBottomLayer);
        //CubeBlock.ChangedPosition.AddListener(ChangeTransparency);

        //SpawnManager.Instantiated.AddListener(AddCube);
        //SpawnManager.BlockInstantiated.AddListener(MatchIDs);
    }

    void CreateCubeMap()
    {
        ClearMap();

        for (int i = -_mapCenterMaxIdxX; i <= _mapCenterMaxIdxX; i++)
            for (int j = _mapCenterStartIdxY; j <= _mapCenterMaxIdxY; j++)
                for (int k = -_mapCenterMaxIdxZ; k <= _mapCenterMaxIdxZ; k++)
                {
                    Vector3Int cubePosition = new Vector3Int(i, j, k);

                    GameObject randomCube = RandomCube();

                    GameObject cubeObject = Instantiate(randomCube, (Vector3)cubePosition * _positionOffset, randomCube.transform.rotation);

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

        s_lastUsedID = -1;

        foreach (KeyValuePair<Vector3Int, Cube> item in CubeMap)
        {
            if (item.Value.CubeID == -1)
            {
                s_lastUsedID++;
                item.Value.CubeID = s_lastUsedID;
            }

            FindNeighbours(item);
        }
    }

    void ChangeIDtoCubes(int prevID, int newID)
    {
        foreach (KeyValuePair<Vector3Int, Cube> item in CubeMap)
        {
            if (item.Value.CubeID == prevID)
            {
                item.Value.CubeID = newID;
            }
        }
    }

    void FindNeighbours(KeyValuePair<Vector3Int, Cube> item)
    {
        for (int i = 0; i < _posToCheck.GetLength(0); i++)
        {
            Vector3Int posToCheck = new Vector3Int(item.Key.x + _posToCheck[i, 0],
                item.Key.y + _posToCheck[i, 1], item.Key.z + _posToCheck[i, 2]);

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

    void DestroyWithID(int ID)
    {
        var itemsToRemove = CubeMap.Where(item => item.Value.CubeID == ID).ToArray();
        foreach (var item in itemsToRemove)
        {
            CubeMap.Remove(item.Key);
            Destroy(item.Value.gameObject);

            GameManager.UpdateScore("Inner");
        }
    }

    public void AddCube(Vector3Int position, Cube cube)
    {
        CubeMap.Add(position, cube);
    }

    void DFS(Vector3Int initialPos)
    {
        // ���� ���� ��� �������, �� ������ ��������, ����� ���������� �������� �������
        if (!_visited.ContainsKey(initialPos))
        {
            _visited.Add(initialPos, true);
        }
        else
        {
            return;
        }

        for (int i = 0; i < _posToCheck.GetLength(0); i++)
        {
            Vector3Int posToCheck = new Vector3Int(initialPos.x + _posToCheck[i, 0],
                initialPos.y + _posToCheck[i, 1], initialPos.z + _posToCheck[i, 2]);

            if (CubeMap.ContainsKey(posToCheck))
            {
                DFS(posToCheck);
            }
        }
    }

    void DestroyFlyingCubes()
    {
        _visited.Clear();

        // ���� ��� �� ����� (������� ����������� � ��������� ������)
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
            CubeMap.Remove(item.Key);
            Destroy(item.Value.gameObject);

            GameManager.UpdateScore("Flying");
        }
    }

    void AddBottomLayer()
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

                    GameObject cubeObject = Instantiate(randomCube, (Vector3)cubePosition * _positionOffset, randomCube.transform.rotation);

                    extendedMap.Add(cubePosition, cubeObject.GetComponent<Cube>());
                }
            }

        CubeMap = extendedMap;
    }

    void ClearMap()
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
            string name = item.Value.name;
            string nameWithoutCloneAndCube = name.Substring(0, name.Length - 12);

            if (!s_leftColorCubes.ContainsKey(nameWithoutCloneAndCube))
                s_leftColorCubes.Add(nameWithoutCloneAndCube, true);
        }
    }
}
