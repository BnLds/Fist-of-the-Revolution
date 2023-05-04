using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PoliceUnitSM : StateMachine
{
    private const string PERFORM_DETECTION = "PerformDetection";

    [HideInInspector]
    public UnityEvent<Transform> OnObjectDestroyed;

    [HideInInspector]
    public Vector3 MoveDirectionInput = Vector3.zero;

    [HideInInspector] public Idle IdleState;
    [HideInInspector] public FollowProtest FollowProtestState;
    [HideInInspector] public WatchObject WatchObjectState;

    [Header("Initialization Parameters")]
    [SerializeField] private List<Detector> _detectors;
    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours;
    [SerializeField] private ContextSolver _movementDirectionSolver;
    
    public PolicemanData PoliceUnitData;

    [Header("Game Balance Parameters")]
    [SerializeField] private float _protectionRange = 10f;
    [SerializeField] private float _detectionDelay = 0.5f;

    private void Awake()
    {
        //States initialization 
        IdleState = new Idle(this);
        FollowProtestState = new FollowProtest(this);
        WatchObjectState = new WatchObject(this);
    }

    protected override void Start()
    {
        base.Start();
        float repeatRate = 2f;
        InvokeRepeating(PERFORM_DETECTION, 0f, repeatRate);
    }

    protected override void Update()
    {
        base.Update();
        if(PoliceUnitData.ObjectsToProtect.Count != 0)
        {
            for (int i = 0; i < PoliceUnitData.ObjectsToProtect.Count; i++)
            {
                //check if object is not watched (ie undamaged or already destroyed) and if it is in watch list
                if(PoliceUnitData.ObjectsToProtect[i].GetComponent<BreakableController>().IsOnWatchList == false && PoliceUnitData.ObjectsToProtect.Contains(PoliceUnitData.ObjectsToProtect[i]))
                {
                    Transform objectDestroyed = PoliceUnitData.ObjectsToProtect[i];
                    //remove object from the policement watch list and inform the policeman AI the object has been destroyed
                    PoliceUnitData.ObjectsToProtect.Remove(PoliceUnitData.ObjectsToProtect[i]);
                    OnObjectDestroyed?.Invoke(objectDestroyed);
                }        
            }
        }
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }

    /*public void StartBrokenObjectsDetection()
    {
        InvokeRepeating(PERFORM_DETECTION, 0f, _detectionDelay);
    }*/ 

    private void PerformDetection()
    {
        foreach (Detector detector in _detectors)
        {
            detector.Detect(PoliceUnitData);
        }

        if (PoliceResponseData.WatchPoints != null && PoliceResponseData.WatchPoints.Count != 0)
        {
            foreach (Transform watchPoint in PoliceResponseData.WatchPoints)
            {
                //go to next watchPoint if object is already in the list of objects to protect
                if(PoliceUnitData.ObjectsToProtect.Contains(watchPoint)) continue;

                //check if object on watchList is HighPriority
                if(watchPoint.GetComponent<BreakableController>().IsHighPriority)
                {
                    //add it to watch list 
                    PoliceUnitData.ObjectsToProtect.Add(watchPoint);
                } 
                //check if object is within protectionRange
                else if (Utility.Distance2DBetweenVector3(watchPoint.position, transform.position) <= _protectionRange)
                {
                    PoliceUnitData.ObjectsToProtect.Add(watchPoint);
                }
            }
        }
    }

    public Vector3 GetMoveDirectionInput()
    {
        return _movementDirectionSolver.GetContextDirection(_steeringBehaviours, PoliceUnitData);
    }

}
