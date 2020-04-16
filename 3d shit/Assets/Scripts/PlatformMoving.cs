using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoving : MovingPlatformStateMachine
{
    public Vector3 startPosition;
    public Vector3 endPositioon;
    public Vector3 targetPosition;
    public bool frozen = false;
    public string currentState;
    public float timeTillDefrosted;
    public bool tester = false;
    public float freezeTestTime = 5f;
    public float platformSpeed = 5f;
    public float movementTimer = 10f;
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
        Debug.Log("tog mig in i statemachinen");
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
            Debug.Log("tog mig in i Start i movementState Ska bara hända en gång");
            PlatformMoving.startPosition = PlatformMoving.transform.position;
            PlatformMoving.targetPosition = PlatformMoving.endPositioon;
        }
        
    }
    public override void Movement()
    {
        PlatformMoving.velocity = (PlatformMoving.targetPosition - PlatformMoving.transform.position).normalized *PlatformMoving.platformSpeed *Time.deltaTime;
        Debug.Log("Tog mig in i movementState Movement");
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
        Debug.Log("tog mig in i Freeze state");
        PlatformMoving.timeTillDefrosted = Time.time + PlatformMoving.freezeTestTime;
    }
    public override void Movement()
    {
            if (Time.time > PlatformMoving.timeTillDefrosted)
            {
                PlatformMoving.frozen = false;
                PlatformMoving.SetState(new MovingState(PlatformMoving));
            }
    }

}