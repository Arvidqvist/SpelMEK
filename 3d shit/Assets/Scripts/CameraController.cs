using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : CameraStateMachince
{
    private const float Y_MAX_ANGLE = 50f;
    private const float Y_MIN_ANGLE = 0f;
    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;

    public float distance = 20f;
    public float currentX = 0f;
    public float currentY = 0f;
    public float sensitivityX = 4f;
    public float sensitivityY = 4f;
    public Vector2 screenCenter;



    // Start is called before the first frame update
    void Start()
    {
        camTransform = transform;
        cam = Camera.main;
        SetState(new NormalGravityState(this));
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    currentX += Input.GetAxis("Mouse X");
    //    currentY += Input.GetAxis("Mouse Y");
    //    //Debug.Log(Input.mousePosition);
    //}
    //private void LateUpdate()
    //{
    //    Vector3 direction = -lookAt.forward*distance;
    //    Quaternion rotation = Quaternion.Euler(-currentY, -currentX, 0);
    //    camTransform.position = lookAt.position + rotation * direction;
    //    camTransform.LookAt(lookAt.position,lookAt.up);
    //}
}
public abstract class CameraStateMachince : MonoBehaviour
{
    protected BaseCameraState thisState;
    public void SetState(BaseCameraState baseCameraState)
    {
        Debug.Log("tog mig in i statemachinen");
        thisState = baseCameraState;
        thisState.Start();
    }
    private void Update()
    {
        Debug.Log("");
        thisState.CameraMovement();
    }
    private void LateUpdate()
    {
        thisState.CameraPostionUpdate();
    }

}
public abstract class BaseCameraState
{
    protected CameraController thisCameraController;
    protected static Vector3 normalGravity = new Vector3(0, 1, 0);
    protected static Vector3 forwardsGravity = new Vector3(0, 0, 1);
    protected static Vector3 sideGravity = new Vector3(1, 0, 0);

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
        thisCameraController.currentX += Input.GetAxis("Mouse X");
        thisCameraController.currentY += Input.GetAxis("Mouse Y");
        SwithCameraState(thisCameraController.lookAt);
    }
    public virtual Quaternion CameraRotationUpdate()
    {
        return new Quaternion();
    }
    public void SwithCameraState(Transform targetTransform)
    {
        if (targetTransform.up.normalized == normalGravity)
        {
            thisCameraController.SetState(new NormalGravityState(thisCameraController));
        }
        else if (targetTransform.up.normalized == -normalGravity)
        {
            thisCameraController.SetState(new UpsideDownGravityState(thisCameraController));
        }
        else
        {
            thisCameraController.SetState(new SideGravityState(thisCameraController));
        }

        //else if (targetTransform.up.normalized == forwardsGravity)
        //{
        //    thisCameraController.SetState(new ForwardGravityState(thisCameraController));
        //}
        //else if (targetTransform.up.normalized == -forwardsGravity)
        //{
        //    thisCameraController.SetState(new BackwardGravityState(thisCameraController));
        //}
        //else if (targetTransform.up.normalized == sideGravity)
        //{
        //    thisCameraController.SetState(new RightGravityState(thisCameraController));
        //}
        //else if (targetTransform.up.normalized == -sideGravity)
        //{
        //    thisCameraController.SetState(new LeftGravityState(thisCameraController));
        //}
    }

    public void CameraPostionUpdate()
    {
        Debug.Log("player forward ---->  " + thisCameraController.lookAt.forward);
        Vector3 direction = new Vector3(0, 0, -10);
        Vector3 offset = new Vector3(2, -2, 0);
        Quaternion rotation = CameraRotationUpdate();
        thisCameraController.camTransform.position = thisCameraController.lookAt.position + rotation * direction;
        thisCameraController.camTransform.LookAt(thisCameraController.lookAt.position, thisCameraController.lookAt.up);
    }
}

public class SideGravityState : BaseCameraState
{
    public SideGravityState(CameraController cameraController) : base(cameraController)
    {
    }
    public override Quaternion CameraRotationUpdate()
    {
        Vector3 FW = new Vector3(0, 1, 0);
        Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentY, FW);
        rotation = Quaternion.AngleAxis(thisCameraController.currentX, thisCameraController.lookAt.up) * rotation;
        return rotation;
    }
}



public class NormalGravityState : BaseCameraState
{
    public NormalGravityState(CameraController cameraController) : base(cameraController)
    {
    }
    public override Quaternion CameraRotationUpdate()
    {
        Debug.Log("NGS");
        Quaternion rotation = Quaternion.Euler(-thisCameraController.currentY, thisCameraController.currentX, 0f);
        return rotation;
    }

}
public class UpsideDownGravityState : BaseCameraState
{
    public UpsideDownGravityState(CameraController cameraController) : base(cameraController)
    {
    }
    public override Quaternion CameraRotationUpdate()
    {
        Debug.Log("UDGS");
        Quaternion rotation = Quaternion.Euler(thisCameraController.currentY, -thisCameraController.currentX, 0f);
        return rotation;
    }

}


//public class BackwardGravityState : BaseCameraState
//{
//    public BackwardGravityState(CameraController cameraController) : base(cameraController)
//    {

//    }
//    public override Quaternion CameraRotationUpdate()
//    {   
//        //Quaternion rotation = Quaternion.LookRotation(thisCameraController.lookAt.position - thisCameraController.transform.position, thisCameraController.lookAt.up);
//        //Quaternion rotation = Quaternion.Euler(thisCameraController.currentY, 0f, -thisCameraController.currentX);

//        Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentY, thisCameraController.lookAt.forward);
//        rotation = Quaternion.AngleAxis(thisCameraController.currentX, thisCameraController.lookAt.up)*rotation;

//        Debug.Log("BWGS");
//        return rotation;
//    }

//}
//public class ForwardGravityState : BaseCameraState
//{
//    public ForwardGravityState(CameraController cameraController) : base(cameraController)
//    {
//    }

//    public override Quaternion CameraRotationUpdate()
//    {
//        Debug.Log("FWGS");
//        //Quaternion rotation = Quaternion.Euler(-thisCameraController.currentY, 0f, thisCameraController.currentX);

//        Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentY, thisCameraController.lookAt.forward);
//        rotation = Quaternion.AngleAxis(thisCameraController.currentX, thisCameraController.lookAt.up) * rotation;
//        return rotation;
//    }

//}
//public class RightGravityState : BaseCameraState
//{
//    public RightGravityState(CameraController cameraController) : base(cameraController)
//    {
//    }
//    public override Quaternion CameraRotationUpdate()
//    {
//        Debug.Log("RGS");
//        //Quaternion rotation = Quaternion.Euler(thisCameraController.currentX, thisCameraController.currentY, 0f);
//        Vector3 FW = new Vector3(0, 1, 0);
//        Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentY, FW);
//        rotation = Quaternion.AngleAxis(thisCameraController.currentX, thisCameraController.lookAt.up)* rotation;
//        return rotation;
//    }

//}
//public class LeftGravityState : BaseCameraState
//{
//    public LeftGravityState(CameraController cameraController) : base(cameraController)
//    {
//    }
//    public override Quaternion CameraRotationUpdate()
//    {
//        Debug.Log("LGS");
//        //Quaternion rotation = Quaternion.Euler(-thisCameraController.currentX, -thisCameraController.currentY, 0);
//        Vector3 FW = new Vector3(0, 1, 0);
//        Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentY, FW);
//        rotation = Quaternion.AngleAxis(thisCameraController.currentX, thisCameraController.lookAt.up) * rotation;
//        return rotation;
//    }

//}
