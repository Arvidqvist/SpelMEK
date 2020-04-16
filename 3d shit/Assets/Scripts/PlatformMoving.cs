using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    public float lengthToMove = 15f;
    public Vector3 startposition;
    public bool frozen = false;
    public float timeTillDefrosted;
    public bool tester = false;
    public float freezeTestTime = 5f;
    public Vector3 Direction = new Vector3(0, 1, 0);
    public float platformSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        startposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!frozen)
        {
            transform.position += Direction.normalized * platformSpeed * Time.deltaTime;
        }

        if ((transform.position - startposition).magnitude > lengthToMove)
        {
            Direction = -Direction;
            Debug.Log("turned around now");
            //Debug.Log(transform.position.magnitude + " transform magnitude  "+ startposition.magnitude +" startposition magnitude");
        }

        if (timeTillDefrosted > Time.time && frozen)
        {
            frozen = false;
            tester = false;
        }

        // remove after the freeze function is implemented with player
        // and reomove the other parts connected to tester
        if (tester)
        {
            Freeze(freezeTestTime);
        }
    }
    public void Freeze(float freezeTime)
    {
        timeTillDefrosted = Time.time + freezeTime;
        frozen = true;
    }
}
