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

    private Camera cam;
    public Vector3 RotationMultiplierVector = new Vector3(0,0,-10);
    public float distance = 20f;
    public float currentXMouseInput = 0f;
    public float currentYMouseInput = 0f;
    public float sensitivityX = 4f;
    public float sensitivityY = 4f;
    public Vector2 screenCenter;
    public Vector3 currentforwardvector;



    // Start is called before the first frame update
    void Start()
    {
        camTransform = transform;
        cam = Camera.main;
        fakeForward = PlayerTransfrom.forward;
        currentforwardvector = fakeForward;
        Debug.Log("if im seen more than once its broken");
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
    public BaseCameraState thisState;
    public void SetState(BaseCameraState baseCameraState)
    {
        thisState = baseCameraState;
        //thisState.Start();
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
        thisCameraController.currentYMouseInput += Input.GetAxis("Mouse Y");
        //thisCameraController.currentYMouseInput = Mathf.Clamp(thisCameraController.currentYMouseInput, -50, 50);

        //SwithCameraState(thisCameraController.lookAt);
    }

    public virtual Quaternion CameraRotationUpdate()
    {
        //Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentYMouseInput, thisCameraController.fakeForward);
        //rotation = Quaternion.AngleAxis(thisCameraController.currentXMouseInput, thisCameraController.PlayerTransfrom.up) * rotation;
        //Quaternion targetRotation = Quaternion.FromToRotation(thisCameraController.fakeForward,thisCameraController.PlayerTransfrom.up)* thisCameraController.PlayerTransfrom.rotation;
        Quaternion rotation = Quaternion.Euler(-thisCameraController.currentYMouseInput, thisCameraController.currentXMouseInput, 0f);
        rotation = thisCameraController.PlayerTransfrom.rotation * rotation;
        return rotation;
    }

    public virtual void CameraPostionUpdate()
    {
        // måste förmodligen på något sätt få med spelarens rotation i själva flippen så att kameran stannar bakom för fram/bak gravitationsflips
        Vector3 offset = new Vector3(0, 0, -10);
        Quaternion rotation = CameraRotationUpdate();
        Debug.Log("Rotationvectormultiplier " + thisCameraController.RotationMultiplierVector);
        //thisCameraController.fakeForward = thisCameraController.PlayerTransfrom.up;

        //Quaternion targetRotation = Quaternion.FromToRotation(thisCameraController.transform.forward, thisCameraController.fakeForward) * thisCameraController.transform.rotation;

        //thisCameraController.transform.rotation = Quaternion.Slerp(thisCameraController.transform.rotation, targetRotation, 5f * Time.deltaTime);

        thisCameraController.currentforwardvector = Vector3.Lerp(thisCameraController.currentforwardvector, thisCameraController.fakeForward, 0.5f * Time.deltaTime);
        Debug.Log(thisCameraController.currentforwardvector + "  <--- CFW");
        //thisCameraController.camTransform.position = thisCameraController.PlayerTransfrom.position + rotation * thisCameraController.RotationMultiplierVector;
        thisCameraController.camTransform.position = thisCameraController.PlayerTransfrom.position + rotation*(thisCameraController.currentforwardvector * 10f);
        thisCameraController.camTransform.LookAt(thisCameraController.PlayerTransfrom.position, thisCameraController.PlayerTransfrom.up);
    }

    public void SwithCameraState()
    {
        Debug.Log("FF before  " + thisCameraController.fakeForward);
        thisCameraController.fakeForward = thisCameraController.PlayerTransfrom.up;
        Debug.Log("FF after  "+thisCameraController.fakeForward);
        //if (targetTransform.up == normalGravity)
        //{
            thisCameraController.SetState(new NormalGravityState(thisCameraController));
            Debug.Log("Normal GSTATE");

        //}
        //else if (targetTransform.up == -normalGravity)
        //{
        //    Debug.Log("Upside down GSTATE");
        //    thisCameraController.SetState(new UpsideDownGravityState(thisCameraController));
        //}
        //else if (targetTransform.up == Backwardgravity || targetTransform.up == -LeftGravity)
        //{
        //    Debug.Log("Right/backward GSTATE");
        //    thisCameraController.SetState(new Right_BackwardGravityState(thisCameraController));
        //}
        //else
        //{
        //    Debug.Log("Left/forward GSTATE");
        //    thisCameraController.SetState(new Left_ForwardGravityState(thisCameraController));
            
        //}
        if (targetTransform.up == normalGravity)
        {
            thisCameraController.SetState(new NormalGravityState(thisCameraController));
            Debug.Log("Normal GSTATE");
            thisCameraController.RotationMultiplierVector = new Vector3(0, 0, -10);
        }
        else if (targetTransform.up == -normalGravity)
        {
            Debug.Log("Upside down GSTATE");
            thisCameraController.SetState(new UpsideDownGravityState(thisCameraController));
            thisCameraController.RotationMultiplierVector = new Vector3(0, 0, -10);
        }
        else if (targetTransform.up == Backwardgravity || targetTransform.up == -LeftGravity)
        {
            Debug.Log("Right/backward GSTATE");
            thisCameraController.SetState(new Right_BackwardGravityState(thisCameraController));
            thisCameraController.RotationMultiplierVector = new Vector3(0, 0, -10);
        }
        else
        {
            Debug.Log("Left/forward GSTATE");
            thisCameraController.RotationMultiplierVector = new Vector3(0, 0, -10);
            thisCameraController.SetState(new Left_ForwardGravityState(thisCameraController));

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
}

public class NormalGravityState : BaseCameraState
{ 
    public virtual void CameraPositionUpdate()
    {
        // måste förmodligen på något sätt få med spelarens rotation i själva flippen så att kameran stannar bakom för fram/bak gravitationsflips
        Vector3 offset = new Vector3(0, 0, -10);
        Quaternion rotation = CameraRotationUpdate();
        Debug.Log("Rotationvectormultiplier " + thisCameraController.RotationMultiplierVector);
        thisCameraController.camTransform.position = thisCameraController.lookAt.position + rotation * thisCameraController.RotationMultiplierVector;
        thisCameraController.camTransform.LookAt(thisCameraController.lookAt.position, thisCameraController.lookAt.up);
    }
}

public class Left_ForwardGravityState : BaseCameraState
{
    public Left_ForwardGravityState(CameraController cameraController) : base(cameraController)
    {
    }
        public NormalGravityState(CameraController cameraController) : base(cameraController)
        {
        }
    public override void Start()
    {
        
    }
    //public override Quaternion CameraRotationUpdate()
    //    {
    //        Quaternion rotation = Quaternion.Euler(-thisCameraController.currentYMouseInput, thisCameraController.currentXMouseInput, 0f);
    //        return rotation;
    //    }

    public override void CameraPositionUpdate()
    {
        Vector3 offset = new Vector3(0, -10, 0);
        Quaternion rotation = CameraRotationUpdate();
        Debug.Log("Rotationvectormultiplier " + thisCameraController.RotationMultiplierVector);
        thisCameraController.camTransform.position = thisCameraController.lookAt.position + rotation * offset;
        thisCameraController.camTransform.LookAt(thisCameraController.lookAt.position, thisCameraController.lookAt.up);
    }
}
public class Right_BackwardGravityState : BaseCameraState
{
    public Right_BackwardGravityState(CameraController cameraController) : base(cameraController)
    {
    }
    public override Quaternion CameraRotationUpdate()
    {
        Vector3 FW = new Vector3(0, -1, 0);
        Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentY, FW);
        rotation = Quaternion.AngleAxis(thisCameraController.currentX, thisCameraController.lookAt.up) * rotation;
        return rotation;
    }
    public override void CameraPositionUpdate()
    {
        Vector3 offset = new Vector3(0, 10, -10);
        Quaternion rotation = CameraRotationUpdate();
        Debug.Log("Rotationvectormultiplier " + thisCameraController.RotationMultiplierVector);
        thisCameraController.camTransform.position = thisCameraController.lookAt.position + rotation * offset;
        thisCameraController.camTransform.LookAt(thisCameraController.lookAt.position, thisCameraController.lookAt.up);
    }
}

//public class Left_ForwardGravityState : BaseCameraState
//{
//    public Left_ForwardGravityState(CameraController cameraController) : base(cameraController)
//    {
//    }
//    public override Quaternion CameraRotationUpdate()
//    {
//        Vector3 FW = new Vector3(0, 1, 0);
//        Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentYMouseInput, thisCameraController.fakeForward);
//        rotation = Quaternion.AngleAxis(thisCameraController.currentXMouseInput, thisCameraController.PlayerTransfrom.up) * rotation;
//        return rotation;
//    }

public class NormalGravityState : BaseCameraState
{
    public NormalGravityState(CameraController cameraController) : base(cameraController)
    {
    }
    public override Quaternion CameraRotationUpdate()
    {
        Quaternion rotation = Quaternion.Euler(-thisCameraController.currentY, thisCameraController.currentX, 0f);
        return rotation;
    }
}

//public class Right_BackwardGravityState : BaseCameraState
//{
//        public Right_BackwardGravityState(CameraController cameraController) : base(cameraController)
//        {
//        }
//        public override Quaternion CameraRotationUpdate()
//        {
//            Vector3 FW = new Vector3(0, -1, 0);
//            Quaternion rotation = Quaternion.AngleAxis(thisCameraController.currentYMouseInput, FW);
//            rotation = Quaternion.AngleAxis(thisCameraController.currentXMouseInput, thisCameraController.PlayerTransfrom.up) * rotation;
//            return rotation;
//        }

//}
//public class UpsideDownGravityState : BaseCameraState
//    {
//        public UpsideDownGravityState(CameraController cameraController) : base(cameraController)
//        {
//        }
//        public override Quaternion CameraRotationUpdate()
//        {
//            Debug.Log("UDGS");
//            Quaternion rotation = Quaternion.Euler(thisCameraController.currentYMouseInput, -thisCameraController.currentXMouseInput, 0f);
//            return rotation;
//        }

//    }


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
