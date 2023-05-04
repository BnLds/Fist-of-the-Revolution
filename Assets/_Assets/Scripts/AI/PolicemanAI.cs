/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class PolicemanAI : MonoBehaviour
{
    private const string PERFORM_DETECTION = "PerformDetection";

    public enum PoliceState
    {
        Chase,
        Idle, 
        LookForTarget,
        Block,
        Infiltrate,
        Protect,
        MoveToWatchedPoint
    }

    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours;
    [SerializeField] private List<Detector> _detectors;
    [SerializeField] private PolicemanData _policemanData;
    [ReadOnly] [SerializeField]  private Vector3 _moveDirectionInput = Vector3.zero;
    [SerializeField] private ContextSolver _movementDirectionSolver;
    [SerializeField] private float _catchDistance = 1f;
    [SerializeField] private float _catchAttemptDelay = 1f;
    [SerializeField] private float _watchReactionRange = 10f;

    //performance parameters
    [SerializeField] private float _detectionDelay = .05f;
    [SerializeField]private float _aiUpdateDelay = .06f;
    [SerializeField] private PoliceState _currentState = PoliceState.Chase;
    [SerializeField] private bool _showGizmo = false;

    public UnityEvent<Transform> OnCatchAttempt;
    public UnityEvent<Vector3> OnMoveDirectionInput, OnPointerInput;

    private bool _isChasing = false;
    private bool _isWalking = false;

    private void Start()
    {
        //InvokeRepeating(PERFORM_DETECTION, 0f, _detectionDelay);
    }

    private void OnDisable()
    {

    }

    private void PerformDetection()
    {
        foreach(Detector detector in _detectors)
        {
            detector.Detect(_policemanData);
        }

        if(PoliceResponseData.WatchPoints != null && PoliceResponseData.WatchPoints.Count != 0)
        {
            foreach(Transform watchPoint in PoliceResponseData.WatchPoints)
            {
                if(Utility.Distance2DBetweenVector3(watchPoint.position, transform.position) <= _watchReactionRange)
                {
                    _policemanData.WatchedObjectsInReactionRange.Add(watchPoint);
                }
            }
        }
        
    }


    private void Update()
    {
        switch(_currentState)
        {
            case PoliceState.Idle:
            {
                //if within reaction range of watched objects, go to protect the closest object
                if(_policemanData.WatchedObjectsInReactionRange.Count != 0)
                {
                    //Get closest watched object
                    Transform reactionPoint = _policemanData.WatchedObjectsInReactionRange.OrderBy(target => Utility.Distance2DBetweenVector3(transform.position, target.position)).FirstOrDefault();
                    _policemanData.CurrentWatchObjectPosition = new Vector3(reactionPoint.position.x, reactionPoint.position.y, reactionPoint.position.z);
                    //Generate flowfield to reaction point
                    //policemanData.currentFlowField = PoliceFlowfieldsGenerator.Instance.CreateNewFlowField(reactionPoint);
                    _policemanData.CurrentFlowField = GridController.Instance.GenerateFlowField(reactionPoint);

                    _currentState = PoliceState.MoveToWatchedPoint;
                }


                break;
            }
            case PoliceState.MoveToWatchedPoint:
            {
                if (!_isWalking)
                    {
                        _isWalking = true;
                        StartCoroutine(GoToWatchPoint());
                    }
                break;
            }
            case PoliceState.Protect:
            {
                //stay within a certain area of a watched object
                //chase any character damaging the object

                if(_policemanData.CurrentTarget == null)
                {
                        _currentState = PoliceState.LookForTarget;
                    }

                break;
            }
            case PoliceState.Chase:
            {
                //chase a specific target
                //NPC movement based on target availability and if chasing is enabled
                if(_policemanData.CurrentTarget != null)
                {
                    //chase the target if enabled
                    if(!_isChasing)
                    {
                        OnPointerInput?.Invoke(_policemanData.CurrentTarget.position);
                        _isChasing = true;
                        StartCoroutine(ChaseAndCatchTarget());
                    }
                } 
                else if(_policemanData.GetTargetsCount() > 0)
                {
                    _policemanData.CurrentTarget = _policemanData.Targets[0];
                }

                break;
            }
            case PoliceState.LookForTarget:
            {
                //look for a lost target by exploring an area around the last known position
                //chase the target if seen
                //return to idle after some time 
                break;
            }
            case PoliceState.Block:
            {
                //block protest if the police can't contain it 
                break;
            }
            case PoliceState.Infiltrate:
            {
                //seek for the player within the protest undercover
                //reveals itself when close enough to player
                //begin chasing
                break;
            }
        }
        //Moving the agent
        OnMoveDirectionInput?.Invoke(_moveDirectionInput);
    }

    private IEnumerator ChaseAndCatchTarget()
    {
        if(_policemanData.CurrentTarget == null)
        {
            //Stopping logic
            Debug.Log("Stopping");
            _moveDirectionInput = Vector3.zero;
            _isChasing = false;
            yield return null;
        }
        else
        {
            float distance = Vector3.Distance(_policemanData.CurrentTarget.position, transform.position);
            if(distance < _catchDistance)
            {
                //Catch logic
                _moveDirectionInput = Vector3.zero;
                Debug.Log("Attempting to catch the target");
                OnCatchAttempt?.Invoke(_policemanData.CurrentTarget);
                yield return new WaitForSeconds(_catchAttemptDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
            else
            {
                //chase logic
                _moveDirectionInput = _movementDirectionSolver.GetContextDirection(_steeringBehaviours, _policemanData);
                yield return new WaitForSeconds(_aiUpdateDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
        }
    }

    private IEnumerator GoToWatchPoint()
    {
        //Check if policeman reached the area to watch
        float reactionPointReachedRange = 2f;
        if(Utility.Distance2DBetweenVector3(_policemanData.CurrentWatchObjectPosition, transform.position) < reactionPointReachedRange)
        {
            //Stopping logic
            _moveDirectionInput = Vector3.zero;
            _policemanData.CurrentFlowField = null;
            _isWalking = false;
            _currentState = PoliceState.Protect;

            yield return null;
        }

        if(_policemanData.CurrentFlowField != null)
        {
            _moveDirectionInput = _movementDirectionSolver.GetContextDirection(_steeringBehaviours, _policemanData);
            yield return new WaitForSeconds(_aiUpdateDelay);
            StartCoroutine(GoToWatchPoint());
        }
    }

    public PoliceState GetPoliceState()
    {
        return _currentState;
    }


    private void OnDrawGizmos()
    {
        if(Application.isPlaying && _showGizmo)
        {
                
        }
    }
}
*/