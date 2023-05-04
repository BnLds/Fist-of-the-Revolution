using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PoliceUnitSM : StateMachine
{
    //private const string PERFORM_DETECTION = "PerformDetection";

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
    [SerializeField] private float _watchReactionRange = 10f;
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
        PoliceResponseManager.Instance.WatchTrigger.AddListener(PerformDetection);
    }

    protected override void Update()
    {
        base.Update();
        if(PoliceUnitData.WatchedObjectsInReactionRange.Count != 0)
        {
            for (int i = 0; i < PoliceUnitData.WatchedObjectsInReactionRange.Count; i++)
            {
                if(PoliceUnitData.WatchedObjectsInReactionRange[i].GetComponent<BreakableController>().IsOnWatch == false && PoliceUnitData.WatchedObjectsInReactionRange.Contains(PoliceUnitData.WatchedObjectsInReactionRange[i]))
                {
                    Transform objectDestroyed = PoliceUnitData.WatchedObjectsInReactionRange[i];
                    PoliceUnitData.WatchedObjectsInReactionRange.Remove(PoliceUnitData.WatchedObjectsInReactionRange[i]);
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
                if (Utility.Distance2DBetweenVector3(watchPoint.position, transform.position) <= _watchReactionRange)
                {
                    PoliceUnitData.WatchedObjectsInReactionRange.Add(watchPoint);
                }
            }
        }
    }

    public Vector3 GetMoveDirectionInput()
    {
        return _movementDirectionSolver.GetContextDirection(_steeringBehaviours, PoliceUnitData);
    }

}
