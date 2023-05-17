using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;

public class PoliceUnitSM : StateMachine
{
    public enum PoliceReactions
    {
        NoReaction,
        FollowProtest, 
        WatchObject, 
        FollowSuspect, 
        Wander, 
        ChasePlayer,
        PlayerIDed,
        PlayerDodged,
        PlayerUnIDed,
        PlayerUntracked
    }

    private const string PERFORM_DETECTION = "PerformDetection";

    #region Parameters
    [HideInInspector]public UnityEvent<PoliceReactions> OnReact;
    [HideInInspector] public UnityEvent<Transform> OnObjectDestroyed;

    [HideInInspector] public Vector3 MoveDirectionInput = Vector3.zero;
    [HideInInspector] public bool IsTargetLost = false;

    [HideInInspector] public Idle IdleState;
    [HideInInspector] public FollowProtest FollowProtestState;
    [HideInInspector] public WatchObject WatchObjectState;
    [HideInInspector] public FollowSuspect FollowSuspectState;
    [HideInInspector] public Wander WanderState;
    [HideInInspector] public ChasePlayer ChasePlayerState;


    [Header("Initialization Parameters")]
    [SerializeField] private List<Detector> _detectors;
    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours;
    [SerializeField] private ContextSolver _movementDirectionSolver;
    [field: SerializeField] public PolicemanData PoliceUnitData { get; private set; }
    [SerializeField] private LayerMask[] _blockingViewMasks;

    [Space(5)]
    [Header("Game Balance Parameters")]
    [SerializeField] private float _protectionRange = 10f;
    [field: SerializeField] public float PlayerDetectionRange { get; private set; } = 20f;
    [field: SerializeField] public float CatchDistance { get; private set; } = 1f;
    [field: SerializeField] public float CatchAttemptDelay { get; private set; } = 1f;
    [field: SerializeField] public float WanderDuration { get; private set; } = 10f;
    [field: SerializeField] public float CountdownToWalkMax { get; private set; } = .5f;
    [field: SerializeField] public float CountdownToPauseMax { get; private set; } = 3f;
    [field: SerializeField] public float WanderRandomDistanceMax { get; private set; } = 3f;

    [Space(5)]
    [Header("Optimization Parameters")]
    [SerializeField] private float _detectionRepeatRate = 0.5f;
    [field: SerializeField] public float DetectionDelay { get; private set; } = 0.5f;

    [Space(5)]
    [Header("Dev Tools")]
    [SerializeField] private bool _isPlayerDetectableByPolice = true;
    [SerializeField] private string _currentState;

    [HideInInspector] public List<BaseState> PoliceStates { get; private set; }
    #endregion


    private void Awake()
    {
        //States initialization 
        IdleState = new Idle(this);
        FollowProtestState = new FollowProtest(this);
        WatchObjectState = new WatchObject(this);
        FollowSuspectState = new FollowSuspect(this);
        WanderState = new Wander(this);
        ChasePlayerState = new ChasePlayer(this);

        PoliceStates = new List<BaseState>() { IdleState, FollowProtestState, WatchObjectState, FollowSuspectState, WanderState, ChasePlayerState };
    }

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(PERFORM_DETECTION, 0f, _detectionRepeatRate);

        PlayerController.Instance.OnAttackPerformed.AddListener(PlayerController_OnDamageDone);
        PoliceResponseManager.Instance.OnPlayerUntracked.AddListener(PoliceResponseManager_OnPlayerUntracked);
        PoliceResponseManager.Instance.OnPlayerNotIDedAnymore.AddListener(PoliceResponseManager_OnPlayerNotIDedAnymore);

        foreach (SteeringBehaviour behaviour in _steeringBehaviours)
        {
            if (behaviour is SeekBehaviour seekBehaviour)
            {
                seekBehaviour.OnTargetLost.AddListener(SeekBehaviour_OnTargetLost);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        _currentState = CurrentState.Name;

        GetComponent<ProtesterAI>().enabled = CurrentState == FollowProtestState;

        //Remove destroyed object from police unit watch list if necessary
        if(PoliceUnitData.ObjectsToProtect.Count != 0)
        {
            for (int i = 0; i < PoliceUnitData.ObjectsToProtect.Count; i++)
            {
                //check if object is not watched (ie undamaged or already destroyed) and if it is in watch list of the police unit
                if(PoliceUnitData.ObjectsToProtect[i] != null && PoliceUnitData.ObjectsToProtect[i].GetComponent<BreakableController>().IsOnWatchList == false && PoliceUnitData.ObjectsToProtect.Contains(PoliceUnitData.ObjectsToProtect[i]))
                {
                    Transform objectDestroyed = PoliceUnitData.ObjectsToProtect[i];
                    //remove object from the policement watch list and inform the policeman AI the object has been destroyed
                    PoliceUnitData.ObjectsToProtect.Remove(PoliceUnitData.ObjectsToProtect[i]);
                    OnObjectDestroyed?.Invoke(objectDestroyed);
                }        
            }
        }
    }

    public override void ChangeState(BaseState newState)
    {
        base.ChangeState(newState);

        PoliceReactions reaction = PoliceReactions.NoReaction;

        switch(newState)
        {
            case(FollowProtest):
            {
                reaction = PoliceReactions.FollowProtest;
                break;
            }
            case(ChasePlayer):
            {
                reaction = PoliceReactions.ChasePlayer;
                break;
            }
            case(FollowSuspect):
            {
                reaction = PoliceReactions.FollowSuspect;
                break;
            }
            case(Wander):
            {
                reaction = PoliceReactions.Wander;
                break;
            }
            case(WatchObject):
            {
                reaction = PoliceReactions.WatchObject;
                break;
            }
        }

        OnReact?.Invoke(reaction);
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }

    private void PerformDetection()
    {
        foreach (Detector detector in _detectors)
        {
            detector.Detect(PoliceUnitData);
        }

        if (PoliceResponseManager.Instance.GetWatchPointsData() != null && PoliceResponseManager.Instance.GetWatchPointsData().Count != 0)
        {
            foreach (Transform watchPoint in PoliceResponseManager.Instance.GetWatchPointsData())
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

    private void PoliceResponseManager_OnPlayerUntracked()
    {
        if(PoliceUnitData.CurrentTarget == PlayerController.Instance.transform)
        {
            PoliceUnitData.CurrentTarget = null;
            OnReact?.Invoke(PoliceReactions.PlayerUntracked);
        }
    }
    
    private void PoliceResponseManager_OnPlayerNotIDedAnymore(Transform protester)
    {
        OnReact?.Invoke(PoliceReactions.PlayerUnIDed);

        if(PoliceUnitData.CurrentTarget == PlayerController.Instance.transform && protester != null)
        {
            PoliceUnitData.CurrentTarget = protester;
        }
    }

    private void PlayerController_OnDamageDone(Transform player)
    {
        if(Utility.Distance2DBetweenVector3(transform.position, player.position) <= PlayerDetectionRange)
        {
            //check if the player is in line of sight and not already IDed
            if (IsPlayerInLineOfSight() && !PoliceResponseManager.Instance.IsPlayerIdentified())
            {
                OnReact?.Invoke(PoliceReactions.PlayerIDed);
                PoliceResponseManager.Instance.SetPlayerToIdentified();

                PoliceResponseManager.Instance.AddFollowedTargetToTrackedList(PlayerController.Instance.transform);
            }
        }
    }

    private void SeekBehaviour_OnTargetLost()
    {
        IsTargetLost = true;
    }

    public Vector3 GetMoveDirectionInput()
    {
        return _movementDirectionSolver.GetContextDirection(_steeringBehaviours, PoliceUnitData);
    }

    public bool IsPlayerInLineOfSight()
    {
        if(_isPlayerDetectableByPolice)
        {
            Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
            bool isPlayerInDetectionRange = Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, PlayerDetectionRange);

            return isPlayerInDetectionRange && _blockingViewMasks.FirstOrDefault(_ => _.value == 1 << hitInfo.collider.gameObject.layer) == 0;
        }
        else
        {
            return false;
        }
    }

    public void AttemptCatchPlayer()
    {
        float attemptValue = Random.Range(0f, 100f);
        float catchThreshold = 80f;
        if(attemptValue <= catchThreshold)
        {
            Debug.Log("Player Caught!");
        }
        else
        {
            Debug.Log("Player Dodged!");
        }
    }

}
