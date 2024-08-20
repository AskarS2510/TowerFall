using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CubeBlock : MonoBehaviour
{
    public List<Vector3Int> DefaultBlockScheme = new List<Vector3Int>(); // Копирует позицию кубов в UnityEditor без домножения на _positionOffset
    public List<Vector3Int> RotatedBlockScheme; // Копирует позицию кубов в UnityEditor без домножения на _positionOffset
    public GameObject BlockStructure; // Основной цветной блок
    public Vector3Int StuckPosition;
    public Quaternion StuckRotation;
    public Vector3Int PositionInt;

    static private float s_speededDownDelay = 0.1f;
    static private float s_defaultDownDelay = 0.1f;
    static private float s_downDelay = s_defaultDownDelay;
    static private float s_speededMoveDownTime = 0.1f;
    static private float s_defaultMoveDownTime = 0.5f;
    static private float s_moveDownTime = s_defaultMoveDownTime;
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

    private void OnDisable()
    {

    }

    void Awake()
    {
        RotatedBlockScheme = new List<Vector3Int>(DefaultBlockScheme);
        _mapManager = _mapManagerObj.GetComponent<MapManager>();

        EventManager.TurnedOnSpeed.AddListener(SpeedUpMoveDown);
        EventManager.TurnedOffSpeed.AddListener(SlowMoveDown);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("entry= " + " other = " + other.name);

        if (_isTimePassed)
        {
            CheckSphereCollisions(other);
        }

        _colliderEntryCounter++;
    }

    void StopMovesStartCollides()
    {
        StopCoroutine(_moveDownCoroutine);

        EventManager.StoppedMovement?.Invoke();
        EventManager.ClickedMove.RemoveListener(MoveHorizontal);

        _isTimePassed = true;

        ModifyColliders(true);

        StartCoroutine(TurnOffColliders());

        StartCoroutine(StickWithDelay());
    }

    void ModifyColliders(bool enabled)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            child.GetComponent<SphereCollider>().enabled = enabled;
        }
    }

    IEnumerator MoveDown()
    {
        yield return new WaitForSeconds(s_moveDownTime);

        while (true)
        {
            ChangePosition(PositionInt - new Vector3Int(0, 1, 0));

            CheckNeighbours();

            yield return new WaitForSeconds(s_moveDownTime);
        }
    }

    IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(s_moveDownTime - s_downDelay);

        StopMovesStartCollides();
    }

    public void StartMoveDown()
    {
        StartCoroutine(_moveDownCoroutine);
    }

    void CheckNeighbours()
    {
        if (PositionInt.y == 0)
        {
            _timeOutCoroutine = TimeOut();

            StartCoroutine(_timeOutCoroutine);

            return;
        }

        List<Vector3Int> shifts = new List<Vector3Int> { new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0 ),
            new Vector3Int(0, 0, -1), new Vector3Int(0, 0, 1 ),
            new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0) };

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
                    if (shift == new Vector3Int(0, -1, 0))
                    {
                        _timeOutCoroutine = TimeOut();

                        StartCoroutine(_timeOutCoroutine);

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

    public void MoveHorizontal(int xIndex, int zIndex)
    {
        Vector3Int shift = new Vector3Int(xIndex, 0, zIndex);
        Vector3Int newPos = PositionInt + shift;

        foreach (Vector3Int pos in RotatedBlockScheme)
        {
            Vector3Int newCubePos = newPos + pos;

            // Если место уже занято каким-либо кубом, то не перемещаем объект
            if (_mapManager.CubeMap.ContainsKey(newCubePos))
            {
                return;
            }

            if (newCubePos.x > _mapManager.CenterMaxIdxX)
            {
                return;
            }

            if (newCubePos.x < -_mapManager.CenterMaxIdxX)
            {
                return;
            }

            if (newCubePos.z > _mapManager.CenterMaxIdxZ)
            {
                return;
            }

            if (newCubePos.z < -_mapManager.CenterMaxIdxZ)
            {
                return;
            }
        }

        // Если все свободно, то перемещаем
        ChangePosition(PositionInt + shift);

        EventManager.ChangedPosition?.Invoke(PositionInt.x, PositionInt.z);

        bool isBelowAvailable = true;

        foreach (Vector3Int pos in RotatedBlockScheme)
        {
            Vector3Int downPos = PositionInt + pos + new Vector3Int(0, -1, 0);

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

        if (_timeOutCoroutine == null)
            CheckNeighbours();        
    }

    Vector3 RoundPosition(Vector3 vector)
    {
        // Округляем позицию вектора до десятых
        float x = vector.x;
        x = Mathf.Round(x * 10.0f) * 0.1f;

        float y = vector.y;
        y = Mathf.Round(y * 10.0f) * 0.1f;

        float z = vector.z;
        z = Mathf.Round(z * 10.0f) * 0.1f;

        return new Vector3(x, y, z);
    }

    void CheckSphereCollisions(Collider other)
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

    IEnumerator DestroyWithDelay(int IDtoDestroy)
    {
        Debug.Log("1st wait destroy");
        yield return new WaitForSeconds(1f);

        EventManager.Destroyed?.Invoke(IDtoDestroy);

        ChangePosition(PositionInt - new Vector3Int(0, 120, 0));

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

    IEnumerator StickWithDelay()
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
        ChangePosition(PositionInt - new Vector3Int(0, 120, 0));

        EventManager.Stuck?.Invoke();

        Debug.Log("stuck invoke + 2nd wait stick");
        yield return new WaitForSeconds(1f);

        Debug.Log("next invoke stick");

        gameObject.SetActive(false);

        EventManager.ReadyForNextBlock?.Invoke();
    }

    IEnumerator TurnOffColliders()
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

    void RotateSchemeFromDefault(int degree)
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

    void SpeedUpMoveDown()
    {
        s_moveDownTime = s_speededMoveDownTime;
        s_downDelay = s_speededDownDelay;
    }
    void SlowMoveDown()
    {
        s_moveDownTime = s_defaultMoveDownTime;
        s_downDelay = s_defaultDownDelay;
    }
}
