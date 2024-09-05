using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRun2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(Vector3.up, 20 * Time.deltaTime);
    }
}
