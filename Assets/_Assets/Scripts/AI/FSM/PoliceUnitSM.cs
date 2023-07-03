using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;
using System.Collections;

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
        Flee,
        PlayerIDed,
        PlayerDodged,
        PlayerUnIDed,
        PlayerUntracked
    }

    private const string PERFORM_DETECTION = "PerformDetection";

    #region Parameters
    [HideInInspector]public UnityEvent<PoliceReactions> OnReact;
    [HideInInspector] public UnityEvent OnCatchAttempt;
    [HideInInspector] public UnityEvent OnFollowProtestEntry;

    [HideInInspector] public Vector3 MoveDirectionInput = Vector3.zero;
    [HideInInspector] public bool IsTargetLost = false;
    [HideInInspector] public bool EnterFollowProtestState = false;

    [HideInInspector] public Idle IdleState;
    [HideInInspector] public FollowProtest FollowProtestState;
    [HideInInspector] public WatchObject WatchObjectState;
    [HideInInspector] public FollowSuspect FollowSuspectState;
    [HideInInspector] public Wander WanderState;
    [HideInInspector] public ChasePlayer ChasePlayerState;
    [HideInInspector] public Flee FleeState;
    [HideInInspector] public List<BaseState> PoliceStates { get; private set; }

    [Header("Initialization Parameters")]
    [SerializeField] private List<Detector> _detectors;
    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours;
    [SerializeField] private ContextSolver _movementDirectionSolver;
    [field: SerializeField] public PolicemanData PoliceUnitData { get; private set; }
    [SerializeField] private LayerMask[] _blockingViewMasks;

    [Space(5)]
    [Header("Game Balance Parameters")]
    [SerializeField] private float _watchObjectDetectionRange = 15f;
    [field: SerializeField] public float PlayerDetectionRange { get; private set; } = 20f;
    [field: SerializeField] public float CatchDistance { get; private set; } = 1f;
    [field: SerializeField] public float CatchAttemptDelay { get; private set; } = 1f;
    [field: SerializeField] public float WanderDuration { get; private set; } = 10f;
    [field: SerializeField] public float CountdownToWalkMax { get; private set; } = .5f;
    [field: SerializeField] public float CountdownToPauseMax { get; private set; } = 3f;
    [field: SerializeField] public float WanderRandomDistanceMax { get; private set; } = 3f;
    [field: SerializeField] public float CasseroladeEffectRadius { get; private set; } = 5f;
    [field: SerializeField] public float SuspectDetectionRange { get; private set; } = 7f;
    [field: SerializeField] public float ReachedWatchedObjectRange { get; private set; } = 1.5f;


    [Space(5)]
    [Header("Optimization Parameters")]
    [SerializeField] private float _detectionRepeatRate = 0.5f;
    [field: SerializeField] public float DetectionDelay { get; private set; } = 0.5f;

    [Space(5)]
    [Header("Dev Tools")]
    [SerializeField] private bool _isPlayerDetectableByPolice = true;
    [SerializeField] private string _currentState;

    private bool isCasserolading;
    #endregion

    private void Awake()
    {
        isCasserolading = false;

        //States initialization 
        IdleState = new Idle(this);
        FollowProtestState = new FollowProtest(this);
        WatchObjectState = new WatchObject(this);
        FollowSuspectState = new FollowSuspect(this);
        WanderState = new Wander(this);
        ChasePlayerState = new ChasePlayer(this);
        FleeState = new Flee(this);

        PoliceStates = new List<BaseState>() { IdleState, FollowProtestState, WatchObjectState, FollowSuspectState, WanderState, ChasePlayerState, FleeState };
    }

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(PERFORM_DETECTION, 0f, _detectionRepeatRate);

        PlayerController.Instance.OnAttackPerformed.AddListener(PlayerController_OnDamageDone);
        PoliceResponseManager.Instance.OnPlayerUntracked.AddListener(PoliceResponseManager_OnPlayerUntracked);
        PoliceResponseManager.Instance.OnPlayerNotIDedAnymore.AddListener(PoliceResponseManager_OnPlayerNotIDedAnymore);
        PlayerController.Instance.OnStartedCasserolade.AddListener(PlayerController_OnStartedCasserolade);
        PlayerController.Instance.OnStoppedCasserolade.AddListener(PlayerController_OnStoppedCasserolade);


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

        if(EnterFollowProtestState)
        {
            OnFollowProtestEntry?.Invoke();
            EnterFollowProtestState = false;
        }

        //check if cop should be impacted by casserolade effect
        if(isCasserolading)
        {
            bool isWithinCasseroladeDistance = Utility.Distance2DBetweenVector3(transform.position, PlayerController.Instance.transform.position) <= CasseroladeEffectRadius;
            if(isWithinCasseroladeDistance && CurrentState != FleeState)
            {
                ChangeState(FleeState);
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

        UpdateUnitListOfObjectsToProtect();
    }

    private void UpdateUnitListOfObjectsToProtect()
    {
        if (PoliceResponseManager.Instance.GetWatchPointsData() != null && PoliceResponseManager.Instance.GetWatchPointsData().Count != 0)
        {
            foreach (Transform watchPoint in PoliceResponseManager.Instance.GetWatchPointsData().Keys)
            {
                if(PoliceUnitData.CurrentWatchedObject == watchPoint) continue; 

                //check if damaged object is already in the cop's list of objects to protect
                if(PoliceUnitData.ObjectsToProtect.Contains(watchPoint))
                {
                    //check if a watcher can still be added
                    if(PoliceResponseManager.Instance.CanAddWatcherToObject(watchPoint))
                    {
                        //all good, object can remain in list, go to next watchPoint
                        continue;
                    } 
                    else
                    {
                        // max number of watcher reached, remove object from the list
                        PoliceUnitData.ObjectsToProtect.Remove(watchPoint);
                    }
                }

                //check if object on watchList is HighPriority && number of watcher had not been reached
                if(watchPoint.GetComponent<BreakableController>().IsHighPriority && PoliceResponseManager.Instance.CanAddWatcherToObject(watchPoint))
                {
                    //add it to watch list 
                    PoliceUnitData.ObjectsToProtect.Add(watchPoint);
                } 
                //check if object is within protection range && number of watcher had not been reached
                else if (Utility.Distance2DBetweenVector3(watchPoint.position, transform.position) <= _watchObjectDetectionRange && PoliceResponseManager.Instance.CanAddWatcherToObject(watchPoint))
                {
                    PoliceUnitData.ObjectsToProtect.Add(watchPoint);
                }
            }
        }
    }

    private void PlayerController_OnStartedCasserolade()
    {
        isCasserolading = true;
    }

    private void PlayerController_OnStoppedCasserolade()
    {
        isCasserolading = false;
    }

    private void PoliceResponseManager_OnPlayerUntracked()
    {
        if(PoliceUnitData.CurrentTarget == PlayerController.Instance.transform)
        {
            PoliceUnitData.CurrentTarget = null;
            OnReact?.Invoke(PoliceReactions.PlayerUntracked);
        }
    }
    
    private void PoliceResponseManager_OnPlayerNotIDedAnymore(Transform target)
    {
        OnReact?.Invoke(PoliceReactions.PlayerUnIDed);

        if(PoliceUnitData.CurrentTarget == PlayerController.Instance.transform && target != null)
        {
            PoliceUnitData.CurrentTarget = target;
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
            OnCatchAttempt?.Invoke();
        }
        else
        {
            Debug.Log("Player Dodged!");
        }
    }

    public void WaitForEndOfframe()
    {
        StartCoroutine(WaitEndOfFrame());
    }

    private IEnumerator WaitEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
    }

    public bool CanWatchObject(Transform watchedObject)
    {
        return PoliceResponseManager.Instance.CanAddWatcherToObject(watchedObject);
    }

    public void AddWatcher(Transform watchedObject)
    {
        PoliceResponseManager.Instance.AddWatcherToObject(watchedObject);
    }

    public void RemoveWatcher(Transform watchedObject)
    {
        PoliceResponseManager.Instance.RemoveWatcherToObject(watchedObject);
    }

    public void RemoveObjectToProtect(Transform objectToProtect)
    {
        PoliceUnitData.ObjectsToProtect.Remove(objectToProtect);
    }
}
