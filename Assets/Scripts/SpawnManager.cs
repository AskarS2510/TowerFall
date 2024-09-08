using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Vector3Int PositionInt;

    private int _leftRotateCount, _rightRotateCount;
    private float _positionOffset = 1.2f;
    [SerializeField] private MapManager _mapManager;

    private void Start()
    {
        _leftRotateCount = 0;
        _rightRotateCount = 0;

        EventManager.StartedGame.AddListener(SpawnPlayerBlock);

        EventManager.RestartedGame.AddListener(PrepareAndSpawn);

        EventManager.ReadyForNextBlock.AddListener(SpawnPlayerBlock);
        EventManager.Stuck.AddListener(SpawnStuckBlock);
        EventManager.DoneRotation.AddListener(RotateSpawnPostiton);
    }

    private void SpawnPlayerBlock()
    {
        if (GameManager.IsGameOver)
            return;

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

        if (GameManager.IsTutorialDone)
            cubeBlock.StartMoveDown();

        EventManager.SpawnedPlayerBlock?.Invoke();
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
