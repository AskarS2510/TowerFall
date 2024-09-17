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

        EventManager.PreparedMap.AddListener(SpawnPlayerBlock);

        EventManager.ReadyForNextBlock.AddListener(SpawnPlayerBlock);

        EventManager.Stuck.AddListener(SpawnStuckBlock);
        EventManager.DoneRotation.AddListener(RotateSpawnPostiton);
    }

    private void SpawnPlayerBlock()
    {
        if (GameManager.Instance.IsGameOver)
            return;

        if (_mapManager.CubeMap.Count == 0)
        {
            EventManager.GameOver?.Invoke();
            EventManager.GameIsWon?.Invoke(true);

            return;
        }

        if (_mapManager.MaxHeight() >= PositionInt.y)
        {
            EventManager.GameOver?.Invoke();
            EventManager.GameIsWon?.Invoke(false);

            return;
        }

        _mapManager.DefineLeftColors();

        CubeBlock cubeBlock = ObjectPool.Instance.GetPooledObject().GetComponent<CubeBlock>();

        cubeBlock.gameObject.SetActive(true);

        cubeBlock.RotateFromCount(_leftRotateCount, _rightRotateCount);

        cubeBlock.ChangePosition(PositionInt);

        cubeBlock.TurnOnMovingSounds();

        if (GameManager.Instance.IsTutorialDone)
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

            GameObject newCubeObject = Instantiate(blockStructure, (Vector3)newPos * _positionOffset, stuckObjectRotation, _mapManager.transform);

            Cube newCube = newCubeObject.GetComponent<Cube>();

            _mapManager.AddCube(newPos, newCube);
        }

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
}
