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
    public Vector3Int FinalPosition;
    public Color BlockColor;

    private float _speededMoveDownTime = 0.1f;
    private float _defaultMoveDownTime = 0.5f;
    private float _moveDownTime;
    private bool _isSpeeded;
    private float _positionOffset = 1.2f;
    [SerializeField] private MapManager _mapManager;

    private void OnEnable()
    {
        _isSpeeded = false;
        _moveDownTime = _defaultMoveDownTime;

        GiveControls();
    }

    private void Awake()
    {
        RotatedBlockScheme = new List<Vector3Int>(DefaultBlockScheme);

        BlockColor = BlockStructure.GetComponent<Cube>().material.color;

        gameObject.SetActive(false);
    }

    private IEnumerator MoveDown()
    {

        FinalPosition = GetFinalPosition();

        while (true)
        {
            yield return new WaitForSeconds(_moveDownTime);

            if (PositionInt == FinalPosition)
            {
                DestroyOrStick();

                yield break;
            }
            else
                ChangePosition(PositionInt + Vector3Int.down);
        }
    }

    private Vector3Int GetFinalPosition()
    {
        Vector3Int currentPos = PositionInt;

        for (int i = currentPos.y; i >= 0; i--)
        {
            currentPos.y = i;

            if (IsPositionFinal(currentPos))
            {
                return currentPos;
            }
        }

        return Vector3Int.zero;
    }

    private bool IsPositionFinal(Vector3Int position)
    {
        return !IsDownAvailable(position) || IsStuckHorizontal(position);
    }

    private void GiveControls()
    {
        EventManager.RaisedMove.AddListener(MoveHorizontal);
        EventManager.RaisedSwitchSpeed.AddListener(SwitchMoveDownSpeed);
        EventManager.RaisedDropDown.AddListener(DropDown);

        EventManager.GameOver.AddListener(OnGameOver);
    }

    private void RemoveControls()
    {
        EventManager.RaisedMove.RemoveListener(MoveHorizontal);
        EventManager.RaisedSwitchSpeed.RemoveListener(SwitchMoveDownSpeed);
        EventManager.RaisedDropDown.RemoveListener(DropDown);

        EventManager.GameOver.RemoveListener(OnGameOver);
    }

    private void DestroyOrStick()
    {
        EventManager.StoppedMovement?.Invoke();
        RemoveControls();

        if (DestroyNearbySameColors())
        {
            SpawnDestructionEffect();

            // Убираем объект из видимости
            ChangePosition(PositionInt + 120 * Vector3Int.down);

            foreach (var item in DefaultBlockScheme)
            {
                GameManager.UpdateScore();
            }

            EventManager.AllowedDestroyFlying?.Invoke();
        }
        else
        {
            StuckPosition = PositionInt;
            StuckRotation = transform.rotation;

            // Убираем объект из видимости
            ChangePosition(PositionInt + 120 * Vector3Int.down);

            EventManager.Stuck?.Invoke();
        }

        StartCoroutine(ReadyWithDelay());
    }

    private IEnumerator ReadyWithDelay()
    {
        EventManager.DoneDestruction?.Invoke();

        yield return new WaitForSeconds(GameManager.DelayBetweenWaves);

        gameObject.SetActive(false);

        EventManager.ReadyForNextBlock?.Invoke();
    }

    private bool IsStuckHorizontal(Vector3Int positionInt)
    {
        List<Vector3Int> shifts = new List<Vector3Int> { Vector3Int.left, Vector3Int.right,
            Vector3Int.forward, Vector3Int.back };

        foreach (var shift in shifts)
        {
            Vector3Int newBlockPos = positionInt + shift;

            foreach (Vector3Int pos in RotatedBlockScheme)
            {
                Vector3Int newCubePos = newBlockPos + pos;

                if (_mapManager.CubeMap.ContainsKey(newCubePos))
                {
                    var color = _mapManager.CubeMap[newCubePos].material.color;

                    if (color == BlockColor)
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
                    var color = _mapManager.CubeMap[newCubePos].material.color;

                    if (color == BlockColor)
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
        StartCoroutine(MoveDown());
    }

    private bool IsDownAvailable(Vector3Int positionInt)
    {
        if (positionInt.y == 0)
            return false;

        Vector3Int shift = Vector3Int.down;

        Vector3Int newBlockPos = positionInt + shift;

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

        FinalPosition = GetFinalPosition();

        EventManager.ChangedPosition?.Invoke(PositionInt.x, PositionInt.z);
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

            ParticlesAudioPool.Instance.SpawnParticles(newPos, BlockColor);
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

    private void SwitchMoveDownSpeed()
    {
        if (_isSpeeded)
        {
            _isSpeeded = false;
            _moveDownTime = _defaultMoveDownTime;
        }
        else
        {
            _isSpeeded = true;
            _moveDownTime = _speededMoveDownTime;
        }
    }

    private void SlowMoveDown()
    {
        _moveDownTime = _defaultMoveDownTime;
    }

    private void DropDown()
    {
        ChangePosition(FinalPosition);
        RemoveControls();
    }

    public void OnGameOver()
    {
        StopCoroutine(MoveDown());

        EventManager.StoppedMovement?.Invoke();
        RemoveControls();

        StuckPosition = PositionInt;
        StuckRotation = transform.rotation;

        // Убираем объект из видимости
        ChangePosition(PositionInt + 120 * Vector3Int.down);

        EventManager.Stuck?.Invoke();

        gameObject.SetActive(false);
    }
}
