using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float Y_MAX_ANGLE = 50f;
    private const float Y_MIN_ANGLE = 0f;

    public Transform playerTransform;
    private Settings settings;
    public Vector3 fakeForward;

    public Vector3 RotationMultiplierVector = new Vector3(0, 0, -10);
    public float currentXMouseInput = 0f;
    public float currentYMouseInput = 0f;
    public Vector3 upVectorBeforeFlip;
    public Vector3 upVectorAfterFlip;

    public LayerMask cameraCollisionLayer = new LayerMask();
    public float cameraRadius = 0;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        settings = GameObject.FindGameObjectWithTag("GameController").GetComponent<Settings>();
    }
    void Start()
    {
        upVectorBeforeFlip = playerTransform.up;
        upVectorAfterFlip = Vector3.zero;
        fakeForward = playerTransform.forward;

        cameraRadius = this.GetComponent<SphereCollider>().radius;
    }

    private void Update()
    {
        CameraMovement();
    }
    private void LateUpdate()
    {
        CameraPostionUpdate();
    }
    public void CameraMovement()
    {
        currentXMouseInput += Input.GetAxis("Mouse X") * settings.cameraSettings.MouseSensitivity;
        currentYMouseInput -= Input.GetAxis("Mouse Y") * settings.cameraSettings.MouseSensitivity;
    }

    public virtual void CameraPostionUpdate()
    {
        //      Sets the rotation of the camera
        Quaternion rotation = Quaternion.Euler(-currentYMouseInput, currentXMouseInput, 0f);
        //      Sets the cameras rotation in relation to the player
        transform.rotation = playerTransform.rotation * rotation;
        //      Sets the camera to be a set distance behind the player
        transform.position = playerTransform.position + (transform.forward * settings.cameraSettings.cameraDistance);
        //      Makes the camera look at the player with the up of the player as the up of the camera
        transform.LookAt(playerTransform.position, playerTransform.up);

        //TODO:REEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
        //Camera not going through walls part of the script
        //horribly borked
        /*
        Physics.SphereCast(transform.position,
                           cameraRadius,
                           playerTransform.position,
                           out RaycastHit cameraHit,
                           float.MaxValue,
                           cameraCollisionLayer);
        Debug.Log((playerTransform.position - this.transform.position).magnitude + ", " + cameraHit.distance);

        if (cameraHit.collider != null)
        {
            Debug.Log("cameraHit.collider = " + cameraHit.collider + ", cameraHit.distance = " + cameraHit.distance);
        }

        if ((playerTransform.position - this.transform.position).magnitude > cameraHit.distance)
        {
            Debug.Log("(" + playerTransform.position + "-" + this.transform.position + ").magnitude = " +
                            (playerTransform.position - this.transform.position).magnitude +
                            ", camerahit.distance = " + cameraHit.distance);
        }

        if (cameraHit.collider != null && (playerTransform.position - this.transform.position).magnitude > cameraHit.distance)
        {
            //Debug.Log("cameraHit.point = " + cameraHit.point);
            this.transform.position = cameraHit.point;
        }
        */

    }

    public void SwitchForwardCameraVector()
    {
        if (upVectorAfterFlip != -upVectorBeforeFlip)
        {
            fakeForward = playerTransform.up;
        }
    }
}