using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Vector3Int PositionInt;

    private int _leftRotateCount, _rightRotateCount;
    private float _positionOffset = 1.2f;
    private MapManager _mapManager;
    [SerializeField] private GameObject _mapManagerObj;

    // Start is called before the first frame update
    private void Start()
    {
        _mapManager = _mapManagerObj.GetComponent<MapManager>();
        _leftRotateCount = 0;
        _rightRotateCount = 0;

        EventManager.StartedGame.AddListener(SpawnPlayerBlock);

        EventManager.RestartedGame.AddListener(PrepareAndSpawn);

        EventManager.ReadyForNextBlock.AddListener(SpawnPlayerBlock);
        EventManager.Stuck.AddListener(SpawnStuckBlock);
        EventManager.RaisedRotate.AddListener(RotateSpawnPostiton);
    }

    private void SpawnPlayerBlock()
    {
        if (_mapManager.CubeMap.Count == 0)
        {
            EventManager.GameOver?.Invoke();

            return;
        }

        if (_mapManager.MaxHeight() >= PositionInt.y)
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
    }

    private void SpawnStuckBlock()
    {
        CubeBlock cubeBlock = ObjectPool.Instance.ActiveObject.GetComponent<CubeBlock>();
        List<Vector3Int> blockScheme = cubeBlock.RotatedBlockScheme;
        GameObject blockStructure = cubeBlock.BlockStructure;
        Vector3Int stuckObjectPos = cubeBlock.StuckPosition;
        Quaternion stuckObjectRotation = cubeBlock.StuckRotation;

        foreach (Vector3Int pos in blockScheme)
        {
            Vector3Int newPos = stuckObjectPos + pos;

            GameObject newCubeObject = Instantiate(blockStructure, (Vector3)newPos * _positionOffset, stuckObjectRotation, transform);

            Cube newCube = newCubeObject.GetComponent<Cube>();

            _mapManager.AddCube(newPos, newCube);
        }

        GameManager.AddSkip();

        _mapManager.MatchIDs();
    }

    private void SpawnMarker()
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

    private void RotateSpawnPostiton(bool left, bool right)
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

    private void PrepareAndSpawn()
    {
        _mapManager.PrepareMap();

        SpawnPlayerBlock();
    }
}
