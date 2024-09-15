using UnityEngine;

public class SCS : MonoBehaviour
{

    public Transform Player;
    public float CloudsSpeed;


    private void Start()
    {
        transform.Rotate(Vector3.up, CloudsSpeed * Time.unscaledTime);
    }


    // Update is called once per frame
    private void Update()
    {
        if (!Player)
            return;



        gameObject.transform.position = Player.transform.position;

        transform.Rotate(0, Time.unscaledDeltaTime * CloudsSpeed, 0);
    }




}
