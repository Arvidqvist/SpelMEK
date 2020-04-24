using UnityEngine;

public class PlatformMoving : MovingPlatformStateMachine
{
    [Header("Object Settings (Don't touch! These should set themselves!)")]

    [Tooltip("If it didn't happen automatically, insert the platform here.")]
    public Transform startPositionTransform = null;
    [Tooltip("If it didn't happen automatically, insert the targetPositionObject here.")]
    public GameObject endPositionTransform = null;
    [Tooltip("The position that the platform starts at.")]
    public Vector3 startPosition = new Vector3(0, 0, 0);
    [Tooltip("The position that the platform moves to.")]
    public Vector3 endPosition = new Vector3(0, 0, 0);

    [Tooltip("Current target position (Don't touch! It sets itself!)")]
    public Vector3 targetPosition;
    [Tooltip("click this to test the freeze")]
    public bool frozen = false;
    [Tooltip("The in time of the defreezingEvent of the platform")]
    public float timeOfDefreezing;
    [Tooltip("The Amount of time the platfrom will be frozen when the bool Frozen is triggered")]
    public float freezeTestTime = 5f;
    [Tooltip("The speed of the platform set to whatever you need")]
    public float platformSpeed = 5f;
    [Tooltip("Sets itself and will be used so that the player will inherit the speed of the platform")]
    public Vector3 velocity;

    void Start()
    {
        SetState(new MovingState(this));
    }
}

public abstract class BasePlatformState
{
    protected PlatformMoving PlatformMoving;

    public BasePlatformState(PlatformMoving platformMoving)
    {
        PlatformMoving = platformMoving;
    }
    public virtual void Start()
    {
        return;
    }
    public virtual void Movement()
    {
        return;
    }
    public virtual void Freeze()
    {
        return;
    }
}

public abstract class MovingPlatformStateMachine : MonoBehaviour
{
    protected BasePlatformState thisState;
    public void SetState(BasePlatformState state)
    {
        thisState = state;
        thisState.Start();
    }
    private void Update()
    {
        thisState.Movement();
    }
}

public class MovingState : BasePlatformState
{
    public MovingState(PlatformMoving platformMoving) : base(platformMoving)
    {
    }

    public override void Start()
    {
        foreach (Transform child in PlatformMoving.gameObject.transform)
        {
            if (child.name == "MovingPlatform")
            {
                PlatformMoving.startPositionTransform = child;
                PlatformMoving.startPosition = child.position;
            }

            if (child.name == "EndPosition")
            {
                PlatformMoving.endPositionTransform = child.gameObject;
                PlatformMoving.endPosition = child.position;
            }
        }

        if (PlatformMoving.targetPosition == new Vector3(0, 0, 0))
        {
            PlatformMoving.targetPosition = PlatformMoving.endPosition;
        }
    }

    public override void Movement()
    {
        PlatformMoving.velocity = (PlatformMoving.targetPosition - PlatformMoving.startPositionTransform.transform.position).normalized * PlatformMoving.platformSpeed * Time.deltaTime;
        PlatformMoving.startPositionTransform.transform.position = Vector3.MoveTowards(PlatformMoving.startPositionTransform.transform.position, PlatformMoving.targetPosition, PlatformMoving.platformSpeed * Time.deltaTime);

        if (PlatformMoving.startPositionTransform.transform.position == PlatformMoving.endPosition)
        {
            PlatformMoving.targetPosition = PlatformMoving.startPosition;
        }

        if (PlatformMoving.startPositionTransform.transform.position == PlatformMoving.startPosition)
        {
            PlatformMoving.targetPosition = PlatformMoving.endPosition;
        }

        if (PlatformMoving.frozen)
        {
            PlatformMoving.SetState(new FreezeState(PlatformMoving));
        }
    }
}

public class FreezeState : BasePlatformState
{
    public FreezeState(PlatformMoving platformMoving) : base(platformMoving)
    {
    }
    public override void Start()
    {
        PlatformMoving.timeOfDefreezing = Time.time + PlatformMoving.freezeTestTime;
    }
    public override void Movement()
    {
        if (Time.time > PlatformMoving.timeOfDefreezing)
        {
            PlatformMoving.frozen = false;
            PlatformMoving.SetState(new MovingState(PlatformMoving));
        }
    }
}