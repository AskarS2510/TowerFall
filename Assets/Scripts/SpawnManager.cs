using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    public Vector3Int PositionInt;

    private int _maxHeight;
    private int _leftRotateCount, _rightRotateCount;
    private float _positionOffset = 1.2f;
    private MapManager _mapManager;
    [SerializeField] private GameObject _mapManagerObj;

    // Start is called before the first frame update
    void Start()
    {
        _mapManager = _mapManagerObj.GetComponent<MapManager>();
        _leftRotateCount = 0;
        _rightRotateCount = 0;
        _maxHeight = 0;

        EventManager.StartedGame.AddListener(SpawnPlayerBlock);

        EventManager.RestartedGame.AddListener(PrepareAndSpawn);

        EventManager.ReadyForNextBlock.AddListener(SpawnPlayerBlock);
        EventManager.Stuck.AddListener(SpawnStuckBlock);
        EventManager.ClickedRotate.AddListener(RotateSpawnPostiton);
    }

    void SpawnPlayerBlock()
    {
        _maxHeight = _mapManager.MaxHeight();

        if (_mapManager.CubeMap.Count == 0)
        {
            EventManager.GameOver?.Invoke();

            return;
        }

        if (_maxHeight >= PositionInt.y)
        {
            EventManager.GameOver?.Invoke();

            return;
        }

        _mapManager.DefineLeftColors();

        CubeBlock cubeBlock = ObjectPool.Instance.GetPooledObject().GetComponent<CubeBlock>();

        cubeBlock.gameObject.SetActive(true);

        cubeBlock.ChangePosition(PositionInt);

        cubeBlock.RotateFromCount(_leftRotateCount, _rightRotateCount);

        cubeBlock.StartMoveDown();

        EventManager.SpawnedPlayerBlock?.Invoke();

        SpawnMarker();

        //_mapManager.ChangeTransparency(cubeBlock.transform.position.x, cubeBlock.transform.position.z);
    }

    void SpawnStuckBlock()
    {
        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();
        List<Vector3Int> blockScheme = cubeBlock.RotatedBlockScheme;
        GameObject blockStructure = cubeBlock.BlockStructure;
        Vector3Int stuckObjectPos = cubeBlock.StuckPosition;
        Quaternion stuckObjectRotation = cubeBlock.StuckRotation;

        foreach (Vector3Int pos in blockScheme)
        {
            Vector3Int newPos = stuckObjectPos + pos;

            GameObject newCubeObject = Instantiate(blockStructure, (Vector3)newPos * _positionOffset, stuckObjectRotation);

            Cube newCube = newCubeObject.GetComponent<Cube>();

            _mapManager.AddCube(newPos, newCube);
        }

        GameManager.AddSkip();

        _mapManager.MatchIDs();
    }

    void SpawnMarker()
    {
        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();
        List<Vector3Int> blockScheme = cubeBlock.RotatedBlockScheme;

        for (int i = 0; i < blockScheme.Count; i++)
        {
            GameObject marker = MarkerPool.Instance.GetPooledObject();

            marker.transform.position = new Vector3(cubeBlock.transform.position.x + blockScheme[i].x * _positionOffset,
                marker.transform.position.y, cubeBlock.transform.position.z + blockScheme[i].z * _positionOffset);
        }
    }

    void RotateSpawnPostiton(bool left, bool right)
    {
        if (right)
        {
            _rightRotateCount++;
        }

        if (left)
        {
            _leftRotateCount++;
        }
    }

    void PrepareAndSpawn()
    {
        _mapManager.PrepareMap();

        SpawnPlayerBlock();
    }
}
