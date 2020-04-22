using UnityEngine;

public class PlatformMoving : MovingPlatformStateMachine
{
    [Tooltip("this gets set to the startposition of your platform Automatically")]
    public Vector3 startPosition;
    [Tooltip("Set this position to the position you want your platform to move to")]
    public Vector3 endPositioon;
    [Tooltip("the position the platform is currently moving towards (dont touch! it sets it self")]
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
        if (PlatformMoving.startPosition.magnitude == 0)
        {
            PlatformMoving.startPosition = PlatformMoving.transform.position;
            PlatformMoving.targetPosition = PlatformMoving.endPositioon;
        }
        
    }
    public override void Movement()
    {
        PlatformMoving.velocity = (PlatformMoving.targetPosition - PlatformMoving.transform.position).normalized *PlatformMoving.platformSpeed *Time.deltaTime;
        PlatformMoving.transform.position = Vector3.MoveTowards(PlatformMoving.transform.position, PlatformMoving.targetPosition, PlatformMoving.platformSpeed * Time.deltaTime);

        if ((PlatformMoving.transform.position - PlatformMoving.targetPosition).magnitude == 0)
        {
            if (PlatformMoving.targetPosition == PlatformMoving.endPositioon)
            {
                PlatformMoving.targetPosition = PlatformMoving.startPosition;
            }
            else if (PlatformMoving.targetPosition == PlatformMoving.startPosition)
            {
                PlatformMoving.targetPosition = PlatformMoving.endPositioon;
            }

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