using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class CubeBlock : MonoBehaviour
{
    public List<Vector3Int> DefaultBlockScheme = new List<Vector3Int>(); // Копирует позицию кубов в UnityEditor без домножения на _positionOffset
    public List<Vector3Int> RotatedBlockScheme; // Копирует позицию кубов в UnityEditor без домножения на _positionOffset
    public GameObject BlockStructure; // Основной цветной блок
    public Vector3Int StuckPosition;
    public Quaternion StuckRotation;
    public Vector3Int PositionInt;

    private static float s_speededDownDelay = 0.1f;
    private static float s_defaultDownDelay = 0.1f;
    private static float s_downDelay = s_defaultDownDelay;
    private static float s_speededMoveDownTime = 0.1f;
    private static float s_defaultMoveDownTime = 0.5f;
    private static float s_moveDownTime = s_defaultMoveDownTime;
    private float _positionOffset = 1.2f;
    private MapManager _mapManager;
    private int _colliderEntryCounter;
    private bool _foundToDestroy;
    private bool _isTimePassed;
    private bool _isGroundBelow;
    private float _lastMoveTime;
    private IEnumerator _moveDownCoroutine;
    private IEnumerator _timeOutCoroutine;
    [SerializeField] private GameObject _mapManagerObj;

    private void OnEnable()
    {
        _colliderEntryCounter = 0;
        _foundToDestroy = false;
        _isTimePassed = false;
        _moveDownCoroutine = MoveDown();
        _timeOutCoroutine = null;
        _isGroundBelow = false;

        ModifyColliders(false);

        EventManager.ClickedMove.AddListener(MoveHorizontal);
    }

    private void Awake()
    {
        RotatedBlockScheme = new List<Vector3Int>(DefaultBlockScheme);
        _mapManager = _mapManagerObj.GetComponent<MapManager>();

        EventManager.TurnedOnSpeed.AddListener(SpeedUpMoveDown);

        EventManager.TurnedOffSpeed.AddListener(SlowMoveDown);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entry= " + " other = " + other.name);

        if (_isTimePassed)
        {
            CheckSphereCollisions(other);
        }

        _colliderEntryCounter++;
    }

    private void StopMovesStartCollides()
    {
        StopCoroutine(_moveDownCoroutine);

        EventManager.StoppedMovement?.Invoke();
        EventManager.ClickedMove.RemoveListener(MoveHorizontal);

        _isTimePassed = true;

        ModifyColliders(true);

        StartCoroutine(TurnOffColliders());

        StartCoroutine(StickWithDelay());
    }

    private void ModifyColliders(bool enabled)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            child.GetComponent<SphereCollider>().enabled = enabled;
        }
    }

    private IEnumerator MoveDown()
    {
        yield return new WaitForSeconds(s_moveDownTime);

        while (true)
        {
            ChangePosition(PositionInt + Vector3Int.down);

            CheckNeighbours();

            yield return new WaitForSeconds(s_moveDownTime);
        }
    }

    private IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(s_moveDownTime - s_downDelay);

        StopMovesStartCollides();
    }

    public void StartMoveDown()
    {
        StartCoroutine(_moveDownCoroutine);
    }

    private void CheckNeighbours()
    {
        if (PositionInt.y == 0)
        {
            StartTimeOut();

            return;
        }

        List<Vector3Int> shifts = new List<Vector3Int> { Vector3Int.left, Vector3Int.right,
            Vector3Int.up, Vector3Int.down,
            Vector3Int.forward, Vector3Int.back };

        foreach (var shift in shifts)
        {
            Vector3Int newPos = PositionInt + shift;

            foreach (Vector3Int pos in RotatedBlockScheme)
            {
                Vector3Int newCubePos = newPos + pos;

                if (_mapManager.CubeMap.ContainsKey(newCubePos))
                {
                    string name = _mapManager.CubeMap[newCubePos].name;
                    string nameWithoutClone = name.Substring(0, name.Length - 7);

                    // Если куб снизу (неважно какого цвета), то останавливаемся
                    if (shift == Vector3Int.down)
                    {
                        StartTimeOut();

                        return;
                    }

                    // Если где-то попался куб того же цвета (но не снизу), то останавливаемся
                    if (nameWithoutClone == BlockStructure.name)
                    {
                        StopMovesStartCollides();

                        return;
                    }
                }
            }
        }
    }

    private void StartTimeOut()
    {
        _timeOutCoroutine = TimeOut();

        StartCoroutine(_timeOutCoroutine);
    }

    public void MoveHorizontal(int xIndex, int zIndex)
    {
        Vector3Int shift = new Vector3Int(xIndex, 0, zIndex);
        Vector3Int newPos = PositionInt + shift;

        foreach (Vector3Int pos in RotatedBlockScheme)
        {
            Vector3Int newCubePos = newPos + pos;

            // Если место уже занято каким-либо кубом, то не перемещаем объект
            if (_mapManager.CubeMap.ContainsKey(newCubePos))
                return;

            if (!IsMapClear(newCubePos))
                return;
        }

        // Если все свободно, то перемещаем
        ChangePosition(PositionInt + shift);

        EventManager.ChangedPosition?.Invoke(PositionInt.x, PositionInt.z);

        CheckIfDownAvailable();

        if (_timeOutCoroutine == null)
            CheckNeighbours();        
    }

    private bool IsMapClear(Vector3Int newCubePos)
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

    private void CheckIfDownAvailable()
    {
        bool isBelowAvailable = true;

        foreach (Vector3Int pos in RotatedBlockScheme)
        {
            Vector3Int downPos = PositionInt + pos + Vector3Int.down;

            if (_mapManager.CubeMap.ContainsKey(downPos))
            {
                isBelowAvailable = false;
            }
        }

        if (PositionInt.y == 0)
            isBelowAvailable = false;

        if (isBelowAvailable && _timeOutCoroutine != null)
        {
            StopCoroutine(_timeOutCoroutine);

            _timeOutCoroutine = null;
        }
    }

    private void CheckSphereCollisions(Collider other)
    {
        Debug.Log("sphere entry counter = " + _colliderEntryCounter + " other = " + other.name);

        if (other.gameObject.name == "Ground")
        {
            return;
        }

        // Сравниваем имена кубов (у первого вычитаем "Clone" из названия)
        // Одинаковые цвета уничтожаются
        // Разные цвета остаются на месте (блок застревает)
        if (other.gameObject.name.Substring(0, other.gameObject.name.Length - 7) == BlockStructure.name)
        {
            _foundToDestroy = true;
            StartCoroutine(DestroyWithDelay(other.gameObject.GetComponent<Cube>().CubeID));
        }
    }

    private IEnumerator DestroyWithDelay(int IDtoDestroy)
    {
        Debug.Log("1st wait destroy");
        yield return new WaitForSeconds(1f);

        EventManager.Destroyed?.Invoke(IDtoDestroy);

        // Убираем объект из видимости
        ChangePosition(PositionInt + 120 * Vector3Int.down);

        foreach (var item in DefaultBlockScheme)
        {
            GameManager.UpdateScore("Inner");
        }

        Debug.Log("Destroy invoke + 2nd wait destroy");
        yield return new WaitForSeconds(1f);

        Debug.Log("Destroy invoke destroy");

        EventManager.AllowedDestroyFlying?.Invoke();

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);

        EventManager.ReadyForNextBlock?.Invoke();
    }

    private IEnumerator StickWithDelay()
    {
        Debug.Log("1st wait stick");
        yield return new WaitForSeconds(1f);

        // Отмена, если нашлось обо что уничтожиться
        if (_foundToDestroy)
        {
            yield break;
        }

        StuckPosition = PositionInt;
        StuckRotation = transform.rotation;

        // Убираем объект из видимости
        ChangePosition(PositionInt + 120 * Vector3Int.down);

        EventManager.Stuck?.Invoke();

        Debug.Log("stuck invoke + 2nd wait stick");
        yield return new WaitForSeconds(1f);

        Debug.Log("next invoke stick");

        gameObject.SetActive(false);

        EventManager.ReadyForNextBlock?.Invoke();
    }

    private IEnumerator TurnOffColliders()
    {
        yield return new WaitForSeconds(2f);

        ModifyColliders(false);
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

    public bool IsSpaceAvailable(Vector3Int position)
    {
        Vector3Int newPos = position;

        foreach (Vector3Int pos in RotatedBlockScheme)
        {
            Vector3Int newCubePos = newPos + pos;

            if (_mapManager.CubeMap.ContainsKey(newCubePos))
            {
                return false;
            }
        }

        return true;
    }

    private void SpeedUpMoveDown()
    {
        s_moveDownTime = s_speededMoveDownTime;
        s_downDelay = s_speededDownDelay;
    }
    private void SlowMoveDown()
    {
        s_moveDownTime = s_defaultMoveDownTime;
        s_downDelay = s_defaultDownDelay;
    }
}
