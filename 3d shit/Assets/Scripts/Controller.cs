/*
 Olle frid, olfr3472

 Known problems:
    Moving platforms crashes and/or just does not work TT
    Fix camera when hitting a platform
        watch https://www.youtube.com/watch?v=GC6JuH_gWGU for fix

 Unimplemeted features
    State machine
 */

using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private GameController gameController;
    public Settings settings;
    [Header("Speed Settings")]

    [SerializeField]
    [Tooltip("How fast this transform accelerates")]
    private float acceleration = 2;
    [SerializeField]
    [Tooltip("How fast this transform decelerates")]
    private float deceleration = 1;
    [SerializeField]
    [Tooltip("The maximum speed this transform can achieve")]
    private float maxSpeed = 3;
    [SerializeField]
    [Tooltip("The lowest possible speed this transform can achieve before it gets set to zero")]
    private float minimumSpeedCutoff = 0.1f;

    [Header("Friction Settings")]

    [SerializeField]
    [Tooltip("Weird friction 1")]           //TODO: Understand what these frictions/resistances are
    private float staticFriction = 0.45f;
    [SerializeField]
    [Tooltip("Weird friction 2")]           //TODO: Understand what these frictions/resistances are
    private float dynamicFriction = 0.25f;
    [SerializeField]
    [Tooltip("Weird friction 3")]           //TODO: Understand what these frictions/resistances are
    private float airResistance = 0.25f;

    [Header("Jump & Collision Settings")]

    [SerializeField]
    [Tooltip("How much this transform is affected by gravity")]
    private float gravityModifier = 1;
    [SerializeField]
    [Tooltip("How close this transform can get near other transforms")]
    private float skinWidth = 0.03f;
    [SerializeField]
    [Tooltip("How near this transform needs to be to the ground to be able to jump")]
    private float groundCheckDistance = 0.1f;
    [SerializeField]
    [Tooltip("How high this transform can jump")]
    private float jumpHeight = 4;
    [SerializeField]
    [Tooltip("how much power your dash has")]
    private float dashPower = 5;
    [SerializeField]
    [Tooltip("The layer that this transform will collide with")]
    private LayerMask collisionLayer = new LayerMask();
    [SerializeField]
    [Tooltip("The layer that this transform will be able to gravity flip with")]
    private LayerMask cubeLayer = new LayerMask();

    [Header("Camera Settings")]

    [SerializeField]
    [Tooltip("How sensitive the mouse movements are, corresponding to the camera movements")]
    private float mouseSensitivity = 0.5f;
    [SerializeField]
    [Tooltip("How much the camera can look up expressed in degrees")]
    private float maximumCameraAngle = 90;
    [SerializeField]
    [Tooltip("How much the camera can look down expressed in degrees")]
    private float minimumCameraAngle = -90;
    [SerializeField]
    [Tooltip("How far away the camera is from the player, expressed in 3-dimensional space")]
    private Vector3 cameraOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField]
    [Tooltip("Acticates/ deactivates the third person camera")]
    private bool haveThirdPersonCameraActive = true;
    [SerializeField]
    [Tooltip("In which direction this transform's down is, expressed in 3-dimenaional space")]
    private Vector3 gravityVector = Vector3.down;
    [SerializeField]
    [Tooltip("How many times this transform can flip gravity without touching the ground")]
    private float flipTokens = 1;

    [Header("Non-settings")]
    [SerializeField]
    [Tooltip("How fast the transform moves, expressed in 3-dimenaional space")]
    private Vector3 velocity;

    [SerializeField]
    [Tooltip("Where the player should respawn in case of death")]
    //private Transform respawnPoint;

    public Vector3 direction;
    public Vector3 the180FlipVector;
    private float pushForce = 10;

    private float height;
    private float radius;
    private Vector3 center;

    private new CapsuleCollider collider;
    private Transform playerCamera;

    private float cameraRadius;

    float rotationX;
    float rotationY;
    float gravityFlipAngle;
    int doubleJumped = 1;
    bool dashed = false;
    Vector3 gravity;

    Vector3 point1;
    Vector3 point2;

    private List<Transform> gravityBoxSides = new List<Transform>();

    //This is the layer that the gravity will flip to.
    private LayerMask gravityFlipLayer;
    public LayerMask rotateablePlatform;
    public LayerMask checkpointLayer;

    public Vector3 fakeForward = new Vector3();
    public Quaternion targetRotationFor180Flips;
    void Awake()
    {
        settings = GameObject.FindGameObjectWithTag("GameController").GetComponent<Settings>();
        playerCamera = Camera.main.transform;
        collider = GetComponent<CapsuleCollider>();
        height = collider.height;
        center = collider.center;
        radius = collider.radius;
        checkpointLayer = LayerMask.GetMask("CheckPoint");
        cameraRadius = playerCamera.GetComponent<SphereCollider>().radius;

        Transform gravityBoxParent = transform.Find("GravityCubeParent");

        foreach (Transform child in gravityBoxParent)
        {
            gravityBoxSides.Add(child);
        }

        gravityFlipLayer = collisionLayer;
    }

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        targetRotationFor180Flips = transform.rotation;
        if (gameController.newSceneIsLoaded)
        {
            gameController.SetControllerSettingsAtSpawn(this);
            gameController.SetSpawnTransform(transform);
            gameController.newSceneIsLoaded = false;
            //Destroy(gameController.ControllerSettingsAtSpawn.gameObject);
        }
        else
        {
            transform.SetPositionAndRotation(gameController.getSpawnTransform().position, gameController.getSpawnTransform().rotation);
            transform.SetPositionAndRotation(gameController.getSpawnTransform().position, gameController.getSpawnTransform().rotation);
            //gravityVector = gameController.getControllerSpawnSettings().gravityVector;
        }
    }

    void Update()
    {
        MakeSpeed();

        //ControlCamera();

        SetGravity();
        if (transform.up != -gravityVector)
        {
            LerpRotation();
        }

        FreezePower();
        FlippingPower();

        gravity = gravityVector * gravityModifier * Time.deltaTime;
        velocity += gravity;

        Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);

        point1 = transform.position + center + (-gravityVector * ((height / 2) - radius));
        point2 = transform.position + center + (gravityVector * ((height / 2) - radius));

        Vector3 interactDirection = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump(groundCheck);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashed == false)
        {
            velocity += interactDirection.normalized * dashPower;
            dashed = true;
        }
        if (groundCheck.collider != null && doubleJumped != 0 || groundCheck.collider != null && dashed != false)
        {
            doubleJumped = 1;
            dashed = false;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowGravityBoxSides();
        }

        movingPlattform(groundCheck);
        killZone(groundCheck);

        //point1 = transform.position + center + (-gravityVector * ((height / 2) - radius));
        //point2 = transform.position + center + (gravityVector * ((height / 2) - radius));

        //Vector3 interactDirection = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z);

        Physics.CapsuleCast(point1, point2, radius, interactDirection.normalized, out RaycastHit forwardHit, 0.8f, collisionLayer);

        //Debug.Log(interactDirection.normalized + "  <---camer.forward// direction.normalised ----> " + playerCamera.forward);

        if (forwardHit.collider != null)
        {
            Interact(forwardHit);
        }

        Collision();

        velocity *= Mathf.Pow(airResistance, Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }

    private void ShowGravityBoxSides()
    {
        foreach (Transform boxSide in gravityBoxSides)
        {
            if (boxSide.GetComponent<MeshRenderer>().enabled == false)
            {
                boxSide.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                boxSide.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        gravityFlipLayer = (gravityFlipLayer == collisionLayer) ? cubeLayer : collisionLayer;
    }
    private void FlippingPower()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit rayHit = rayCastfunction(collisionLayer);
            if (rayHit.collider != null)
            {
                if (rayHit.collider.TryGetComponent<RotatingPlatform>(out RotatingPlatform rotatingPlatform))
                {
                    rotatingPlatform.Rotate();
                }
            }
        }
    }
    private void FreezePower()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit rayHit = rayCastfunction(collisionLayer);
            if (rayHit.collider != null && rayHit.collider.TryGetComponent<PlatformMoving>(out PlatformMoving platform))
            {
                //if (rayHit.collider.TryGetComponent<PlatformMoving>(out PlatformMoving platform))
                rayHit.collider.GetComponent<PlatformMoving>().SetState(new FreezeState(rayHit.collider.GetComponent<PlatformMoving>()));
            }

        }

    }
    private void SetGravity()
    {
        Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);

        if (groundCheck.collider != null)
        {
            flipTokens = 1;
        }

        //Physics.Raycast(playerCamera.position, playerCamera.transform.forward, out RaycastHit rayHit, 100f, gravityFlipLayer);
        if (Input.GetKeyDown(KeyCode.G) && flipTokens != 0)
        {
            RaycastHit rayHit = rayCastfunction(gravityFlipLayer);

            if (rayHit.collider != null)
            {
                Camera.main.GetComponent<CameraController>().upVectorBeforeFlip = -gravityVector;
                Camera.main.GetComponent<CameraController>().upVectorAfterFlip = rayHit.normal;
                Camera.main.GetComponent<CameraController>().SwitchForwardCameraVector();
                Debug.Log("FakeForward --->   " + Camera.main.GetComponent<CameraController>().fakeForward);
                targetRotationFor180Flips = Quaternion.AngleAxis(180, the180FlipVector) * transform.rotation;
                gravityVector = -rayHit.normal;
                flipTokens--;
            }
        }
    }
    private void LerpRotation()
    {

        if (Camera.main.GetComponent<CameraController>().upVectorBeforeFlip == -Camera.main.GetComponent<CameraController>().upVectorAfterFlip)
        {
            Debug.Log("hell yeah i worked+ fakeforward --->   " + Camera.main.GetComponent<CameraController>().fakeForward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotationFor180Flips, 2f);
        }
        else
        {
            Debug.Log("im an idiot");
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -gravityVector) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 2f);
        }
    }
    private RaycastHit rayCastfunction(LayerMask layersToHit)
    {
        Physics.Raycast(transform.position, playerCamera.transform.forward, out RaycastHit rayHit, 100f, layersToHit);
        return rayHit;
    }
    //kan behöva brytas ut för att kunnna appliceras på plattformar fiender etc
    private void Collision()
    {
        while (true)
        {
            //Vector3 point1 = transform.position + center + (-gravityVector * ((height / 2) - radius));
            //Vector3 point2 = transform.position + center + (gravityVector * ((height / 2) - radius));

            Physics.CapsuleCast(point1, point2, radius, velocity.normalized, out RaycastHit hit, float.MaxValue, collisionLayer  | checkpointLayer);

            //TITTA
            /*Vector3 interactDirection = new Vector3(playerCamera.forward.y, 0, playerCamera.forward.z);
            Physics.CapsuleCast(point1, point2, radius, interactDirection.normalized, out RaycastHit forwardHit, 1f, collisionLayer);
            Debug.Log(interactDirection.normalized + "  <---camer.forward// direction.normalised ----> " + direction.normalized);

            if (forwardHit.collider != null)
            {
                Interact(forwardHit);
                Debug.Log("forwardHit.collider is " + forwardHit.collider.name);
            }*/
            if (hit.collider != null)
            {
                float distance = skinWidth / Vector3.Dot(velocity.normalized, hit.normal) + hit.distance;
                if (hit.collider.TryGetComponent<CheckPoint>(out CheckPoint checkPoint) && distance < skinWidth*2)
                {
                    checkPoint.SetCheckPoint();
                }

            }

            //added so that the collision only works with the collision layer
            if (hit.collider != null )
            {

                float distance = skinWidth / Vector3.Dot(velocity.normalized, hit.normal) + hit.distance;

                if (distance > velocity.magnitude * Time.deltaTime)
                {
                    break;
                }

                if (distance > 0)
                {
                    transform.position += velocity.normalized * distance;
                }

                Vector3 velocityBeforeNormalforce = CalculateNormalForce(velocity, hit.normal);
                velocity += velocityBeforeNormalforce;

                Friction(velocityBeforeNormalforce.magnitude);

                /*
                 * TODO: Fix inheriting moving platform velocity 
                 */
                //MovingPlatform mov = hit.collider.GetComponent<MovingPlatform>();
                //if (mov != null)
                //{
                //    Vector3 velocityProjected = Vector3.ProjectOnPlane(velocity, hit.normal);

                //    Vector3 diff = mov.velocity - velocityProjected;
                //    //Debug.Log(diff + " = " + mov.velocity + " - " + velocityProjected);

                //    if (Mathf.Abs((mov.velocity - velocityProjected).magnitude) < staticFriction)
                //    {
                //        velocity = mov.velocity;
                //        Debug.Log("AAAAAAA");
                //    }
                //    else
                //    {
                //        velocity = diff;
                //        Debug.Log("BBBBBBB");
                //    }
                //}
            }
            else
            {
                break;
            }
        }
    }

    public void Interact(RaycastHit hit)
    {
        if (hit.collider.tag == "Switches" && Input.GetKeyDown(KeyCode.E))
        {
            hit.collider.GetComponent<DoorSwitch>().Activate();
        }
        else if (hit.collider.tag == "Moveable Object" && Input.GetKey(KeyCode.E))
        {
            hit.collider.GetComponent<MoveAbleObjects>().Move(direction * pushForce * Time.deltaTime);
        }
    }

    void ControlCamera()
    {
        rotationX += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        rotationY += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, minimumCameraAngle, maximumCameraAngle);

        Quaternion cameraRotation;

        if (transform.rotation.z != 0)
        {
            cameraRotation = Quaternion.Euler(rotationY, rotationX, 0);
        }
        else
        {
            cameraRotation = Quaternion.Euler(rotationX, rotationY, 0);
        }

        //FUNCTIONAL CAMERA ROTATION IN THE X- AND Y-AXISES
        Quaternion localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.y);

        //Quaternion l = Quaternion.Euler(velocity);

        //Debug.Log(localRotation + ", " + l);

        //WEIRD CAMERA IN THE X- AND Y-AXIS
        //Quaternion localRotation = transform.rotation * xRot * yRot;

        //FUNCTIONAL CAMERA ROTATION IN ALL AXISES
        //Quaternion localRotation = Quaternion.Inverse(transform.rotation) * cameraRotation;

        if (haveThirdPersonCameraActive)
        {
            Vector3 cameraRelationShipVector = cameraRotation * cameraOffset;

            playerCamera.position = transform.position + cameraRelationShipVector;

            playerCamera.LookAt(transform, transform.up);

            Physics.SphereCast(transform.position, cameraRadius, playerCamera.position, out RaycastHit cameraHit, (cameraRelationShipVector.magnitude - skinWidth), collisionLayer);

            //if (cameraHit.collider != null && (cameraRelationShipVector.magnitude - cameraHit.distance) > cameraHit.distance)
            //{
            //    //Debug.Log("cameraHit.collider is " + cameraHit.collider + ", cameraRelationShipVector.magnitude is " + cameraRelationShipVector.magnitude +
            //    //    ", cameraHit.distance is " + cameraHit.distance);
            //    playerCamera.position = cameraHit.point;
            //}

            //TODO: Fix camera not "sticking" to walls
            if (Physics.Raycast(transform.position, playerCamera.transform.position, float.MaxValue, collisionLayer))
            {
                //Debug.Log(cameraRelationShipVector + ", " + (transform.position - playerCamera.transform.position));
            }
        }
        else
        {
            playerCamera.position = transform.position;
        }
    }

    void Friction(float normalMagnitude)
    {
        if (velocity.magnitude < (normalMagnitude * staticFriction))
        {
            velocity.x = 0;
        }
        else
        {
            velocity += -velocity.normalized * (normalMagnitude * dynamicFriction);
        }
    }

    void Jump(RaycastHit groundCheck)
    {
        //Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);

        if (groundCheck.collider != null || doubleJumped < 2)
        {
            velocity += -gravityVector * settings.playerSettings.jumpHeight;
            doubleJumped++;
        }

    }

    Vector3 CalculateNormalForce(Vector3 velocity, Vector3 normal)
    {
        if ((Vector3.Dot(velocity, normal) < 0))
        {
            Vector3 projection = Vector3.Dot(velocity, normal) * normal;
            return -projection;
        }
        return Vector3.zero;
    }

    void movingPlattform(RaycastHit groundcheck)
    {
        if (groundcheck.collider != null)
        {
            if (groundcheck.collider.tag == "Moveable Platform")
            {
                this.transform.SetParent(groundcheck.collider.transform);
                return;
            }
        }
        this.transform.SetParent(null);
    }

    void killZone(RaycastHit groundcheck)
    {
        if (groundcheck.collider != null)
        {
            if (groundcheck.collider.tag == "Killzone")
            {
                gameController.Respawn();
                //this.transform.position = respawnPoint.transform.position;
            }
        }
    }

    void MakeSpeed()
    {
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        direction = playerCamera.transform.rotation * direction;

        //Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);
        //Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, float.MaxValue, collisionLayer);

        if (!haveThirdPersonCameraActive)
        {
            playerCamera.position -= new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        direction = Vector3.ProjectOnPlane(direction, transform.up).normalized;
        if (direction.magnitude > 0)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                the180FlipVector = new Vector3(1f, 0, 0);
            }
            else
            {
                the180FlipVector = new Vector3(0, 0, 1f);
            }
        }
        if (direction.magnitude < 0.1)
        {
            Decelerate();
        }
        else
        {
            Accelerate(direction);
        }
    }

    void Accelerate(Vector3 direction)
    {
        velocity += direction * acceleration * Time.deltaTime;

        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
    }

    void Decelerate()
    {
        Vector3 tempVelocity = velocity;

        tempVelocity -= tempVelocity * deceleration * Time.deltaTime;

        Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);

        if (groundCheck.collider != null)
        {
            velocity = tempVelocity;
        }

        if (velocity.magnitude < minimumSpeedCutoff)
        {
            velocity = new Vector3(0, 0, 0);
        }
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetRadius()
    {
        return radius;
    }

    public LayerMask GetCollisionLayer()
    {
        return collisionLayer;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetGravityVector()
    {
        return gravityVector;
    }

    public void IncreaseFlipTokens()
    {
        flipTokens++;
    }

    public float GetFlipTokens()
    {
        return flipTokens;
    }
}