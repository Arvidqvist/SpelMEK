using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    private Vector3 flipAxis;
    Quaternion targetRotation;
    public bool activated = false;
    public bool test = false;
    // Start is called before the first frame update
    void Start()
    {
        flipAxis = transform.forward;
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (test == true)
        {
            Rotate();
            test = false;
        }
        if (activated == true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 2f);
        }
        if (transform.rotation == targetRotation && activated == true)
        {
            activated = false;
        }
    }
    public void Rotate()
    {
        if (transform.rotation == targetRotation)
        {
            targetRotation = Quaternion.AngleAxis(90, flipAxis) * transform.rotation;
            activated = true;
        }
    }
}
