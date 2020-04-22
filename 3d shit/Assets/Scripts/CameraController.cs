using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : CameraStateMachince
{
    private const float Y_MAX_ANGLE = 50f;
    private const float Y_MIN_ANGLE = 0f;
    public Transform PlayerTransfrom;
    public Transform camTransform;
    public Vector3 fakeForward;

    public Vector3 RotationMultiplierVector = new Vector3(0, 0, -10);
    public float distance = 20f;
    public float currentXMouseInput = 0f;
    public float currentYMouseInput = 0f;
    public float sensitivityX = 4f;
    public float sensitivityY = 4f;
    public Vector2 screenCenter;
    public Vector3 currentforwardvector;
    public Vector3 upVectorBeforeFlip;
    public Vector3 upVectorAfterFlip;

    // Start is called before the first frame update
    void Start()
    {
        upVectorBeforeFlip = PlayerTransfrom.up;
        upVectorAfterFlip = Vector3.zero;
        camTransform = transform;
        fakeForward = PlayerTransfrom.forward;
        currentforwardvector = fakeForward;
        Debug.Log(fakeForward + "  <--- FF");
        SetState(new NormalGravityState(this));
    }
}
public abstract class CameraStateMachince : MonoBehaviour
{
    public BaseCameraState thisState;
    public void SetState(BaseCameraState baseCameraState)
    {
        thisState = baseCameraState;
    }
    private void Update()
    {
        thisState.CameraMovement();
    }
    private void LateUpdate()
    {
        thisState.CameraPositionUpdate();
    }
}

public abstract class BaseCameraState
{
    protected CameraController thisCameraController;
    protected static Vector3 normalGravity = new Vector3(0, 1, 0);
    protected static Vector3 Backwardgravity = new Vector3(0, 0, 1);
    protected static Vector3 LeftGravity = new Vector3(1, 0, 0);
    public BaseCameraState(CameraController cameraController)
    {
        thisCameraController = cameraController;
    }

    public virtual void Start()
    {
        return;
    }

    public void CameraMovement()
    {
        thisCameraController.currentXMouseInput += Input.GetAxis("Mouse X");
        thisCameraController.currentYMouseInput -= Input.GetAxis("Mouse Y");
    }

<<<<<<< HEAD
    public virtual Quaternion CameraRotationUpdate()
    {
        Quaternion rotation = Quaternion.Euler(-thisCameraController.currentYMouseInput, thisCameraController.currentXMouseInput, 0f);
        rotation = thisCameraController.PlayerTransfrom.rotation * rotation;
        return rotation;
    }

    public virtual void CameraPositionUpdate()
=======
    public virtual void CameraPostionUpdate()
>>>>>>> c3566f3ca171f14775a9735b260657152b1a9ffd
    {
        //      Sets the rotation of the camera
        Quaternion rotation = Quaternion.Euler(-thisCameraController.currentYMouseInput, thisCameraController.currentXMouseInput, 0f);
        //      Sets the cameras rotation in relation to the player
        thisCameraController.transform.rotation = thisCameraController.PlayerTransfrom.rotation * rotation;
        //      Sets the camera to be a set distance behind the player
        thisCameraController.camTransform.position = thisCameraController.PlayerTransfrom.position + (thisCameraController.transform.forward * 10f);
        //      Makes the camera look at the player with the up of the player as the up of the camera
        thisCameraController.camTransform.LookAt(thisCameraController.PlayerTransfrom.position, thisCameraController.PlayerTransfrom.up);
    }

    public void SwitchCameraState()
    {
        Debug.Log(thisCameraController.upVectorAfterFlip + "  <-- UPVAF | UPBF --->" + -thisCameraController.upVectorBeforeFlip);
        if (thisCameraController.upVectorAfterFlip == -thisCameraController.upVectorBeforeFlip)
        {
            Debug.Log("am i even running?");
            thisCameraController.SetState(new NormalGravityState(thisCameraController));
            return;
        }
        else
        {
            thisCameraController.fakeForward = thisCameraController.PlayerTransfrom.up;
            return;
        }
    }
}

public class NormalGravityState : BaseCameraState
{
    public NormalGravityState(CameraController cameraController) : base(cameraController)
    {
    }
}