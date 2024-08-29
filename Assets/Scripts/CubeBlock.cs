using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBlock : MonoBehaviour
{
    public List<Vector3Int> DefaultBlockScheme = new List<Vector3Int>(); // Копирует позицию кубов в UnityEditor без домножения на _positionOffset
    public List<Vector3Int> RotatedBlockScheme; // Копирует позицию кубов в UnityEditor без домножения на _positionOffset
    public GameObject BlockStructure; // Основной цветной блок
    public Vector3Int StuckPosition;
    public Quaternion StuckRotation;
    public Vector3Int PositionInt;

    private static float s_speededMoveDownTime = 0.1f;
    private static float s_defaultMoveDownTime = 0.5f;
    private static float s_moveDownTime = s_defaultMoveDownTime;
    private float _positionOffset = 1.2f;
    private MapManager _mapManager;
    private IEnumerator _moveDownCoroutine;
    [SerializeField] private GameObject _mapManagerObj;

    private void OnEnable()
    {
        _moveDownCoroutine = MoveDown();

        EventManager.RaisedMove.AddListener(MoveHorizontal);
    }

    private void Awake()
    {
        RotatedBlockScheme = new List<Vector3Int>(DefaultBlockScheme);
        _mapManager = _mapManagerObj.GetComponent<MapManager>();

        EventManager.TurnedOnSpeed.AddListener(SpeedUpMoveDown);

        EventManager.TurnedOffSpeed.AddListener(SlowMoveDown);
    }

    private IEnumerator MoveDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(s_moveDownTime);

            if (PositionInt.y == 0)
            {
                DestroyOrStick();

                yield break;
            }

            if (isDownAvailable())
                ChangePosition(PositionInt + Vector3Int.down);
            else
            {
                DestroyOrStick();

                yield break;
            }

            if (isStuckHorizontal())
            {
                DestroyOrStick();

                yield break;
            }
        }
    }

    private void DestroyOrStick()
    {
        EventManager.StoppedMovement?.Invoke();
        EventManager.RaisedMove.RemoveListener(MoveHorizontal);

        if (DestroyNearbySameColors())
        {
            SpawnDestructionEffect();

            // Убираем объект из видимости
            ChangePosition(PositionInt + 120 * Vector3Int.down);

            foreach (var item in DefaultBlockScheme)
            {
                GameManager.UpdateScore("Inner");
            }

            //yield return new WaitForSeconds(1f);

            EventManager.AllowedDestroyFlying?.Invoke();

            //yield return new WaitForSeconds(1f);
        }
        else
        {
            //yield return new WaitForSeconds(1f);

            StuckPosition = PositionInt;
            StuckRotation = transform.rotation;

            // Убираем объект из видимости
            ChangePosition(PositionInt + 120 * Vector3Int.down);

            EventManager.Stuck?.Invoke();

            //yield return new WaitForSeconds(1f);
        }

        gameObject.SetActive(false);

        EventManager.ReadyForNextBlock?.Invoke();
    }

    private bool isStuckHorizontal()
    {
        List<Vector3Int> shifts = new List<Vector3Int> { Vector3Int.left, Vector3Int.right,
            Vector3Int.forward, Vector3Int.back };

        foreach (var shift in shifts)
        {
            Vector3Int newBlockPos = PositionInt + shift;

            foreach (Vector3Int pos in RotatedBlockScheme)
            {
                Vector3Int newCubePos = newBlockPos + pos;

                if (_mapManager.CubeMap.ContainsKey(newCubePos))
                {
                    string name = _mapManager.CubeMap[newCubePos].name;
                    string nameWithoutClone = name.Substring(0, name.Length - 7);

                    if (nameWithoutClone == BlockStructure.name)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool DestroyNearbySameColors()
    {
        bool isFoundToDestroy = false;

        List<Vector3Int> shifts = new List<Vector3Int> { Vector3Int.left, Vector3Int.right,
            Vector3Int.forward, Vector3Int.back,
            Vector3Int.up, Vector3Int.down };

        foreach (var shift in shifts)
        {
            Vector3Int newBlockPos = PositionInt + shift;

            foreach (Vector3Int pos in RotatedBlockScheme)
            {
                Vector3Int newCubePos = newBlockPos + pos;

                if (_mapManager.CubeMap.ContainsKey(newCubePos))
                {
                    string name = _mapManager.CubeMap[newCubePos].name;
                    string nameWithoutClone = name.Substring(0, name.Length - 7);

                    if (nameWithoutClone == BlockStructure.name)
                    {
                        isFoundToDestroy = true;

                        EventManager.Destroyed?.Invoke(_mapManager.CubeMap[newCubePos].CubeID);
                    }
                }
            }
        }

        return isFoundToDestroy;
    }

    public void StartMoveDown()
    {
        StartCoroutine(_moveDownCoroutine);
    }

    private bool isDownAvailable()
    {
        Vector3Int shift = Vector3Int.down;

        Vector3Int newBlockPos = PositionInt + shift;

        foreach (Vector3Int pos in RotatedBlockScheme)
        {
            Vector3Int newCubePos = newBlockPos + pos;

            if (_mapManager.CubeMap.ContainsKey(newCubePos))
                return false;
        }

        return true;
    }

    private void MoveHorizontal(int xIndex, int zIndex)
    {
        Vector3Int shift = new Vector3Int(xIndex, 0, zIndex);
        Vector3Int newPos = PositionInt + shift;

        foreach (Vector3Int pos in RotatedBlockScheme)
        {
            Vector3Int newCubePos = newPos + pos;

            // Если место уже занято каким-либо кубом, то не перемещаем объект
            if (_mapManager.CubeMap.ContainsKey(newCubePos))
                return;

            if (!IsOnMap(newCubePos))
                return;
        }

        // Если все свободно, то перемещаем
        ChangePosition(PositionInt + shift);

        EventManager.ChangedPosition?.Invoke(PositionInt.x, PositionInt.z);

        if (isStuckHorizontal())
        {
            StopCoroutine(_moveDownCoroutine);

            DestroyOrStick();
        }
    }

    private bool IsOnMap(Vector3Int newCubePos)
    {
        if (newCubePos.x > _mapManager.CenterMaxIdxX)
        {
            return false;
        }

        if (newCubePos.x < -_mapManager.CenterMaxIdxX)
        {
            return false;
        }

        if (newCubePos.z > _mapManager.CenterMaxIdxZ)
        {
            return false;
        }

        if (newCubePos.z < -_mapManager.CenterMaxIdxZ)
        {
            return false;
        }

        return true;
    }

    private void SpawnDestructionEffect()
    {
        foreach (Vector3Int pos in RotatedBlockScheme)
        {
            Vector3Int newPos = PositionInt + pos;

            ParticlesPool.Instance.SpawnParticles(newPos);
        }
    }

    public void ChangePosition(Vector3Int position)
    {
        PositionInt = position;
        transform.position = (Vector3)position * _positionOffset;
    }

    public void RotateFromCount(int leftCount, int rightCount)
    {
        int rotationDegree = rightCount - leftCount;

        if (rotationDegree % 4 == 0)
        {
            RotateSchemeFromDefault(0);

            return;
        }

        if (rotationDegree % 4 == 1 || rotationDegree % 4 == -3)
        {
            RotateSchemeFromDefault(1);

            return;
        }

        if (rotationDegree % 4 == 2 || rotationDegree % 4 == -2)
        {
            RotateSchemeFromDefault(2);

            return;
        }

        if (rotationDegree % 4 == 3 || rotationDegree % 4 == -1)
        {
            RotateSchemeFromDefault(3);

            return;
        }
    }

    private void RotateSchemeFromDefault(int degree)
    {
        RotatedBlockScheme = new List<Vector3Int>(DefaultBlockScheme);

        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Сколько раз умножить на матрицу поворота
        for (int i = 0; i < degree; i++)
        {
            transform.Rotate(Vector3.up, -90f);

            // Поворачиваем каждый куб в блоке
            for (int j = 0; j < RotatedBlockScheme.Count; j++)
            {
                int x = -RotatedBlockScheme[j].z;

                int z = RotatedBlockScheme[j].x;

                RotatedBlockScheme[j] = new Vector3Int(x, RotatedBlockScheme[j].y, z);
            }
        }
    }

    private void SpeedUpMoveDown()
    {
        s_moveDownTime = s_speededMoveDownTime;
    }
    private void SlowMoveDown()
    {
        s_moveDownTime = s_defaultMoveDownTime;
    }
}
