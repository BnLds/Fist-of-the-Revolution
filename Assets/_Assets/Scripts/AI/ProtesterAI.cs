using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ProtesterAI : MonoBehaviour
{
    private const string PERFORM_DETECTION = "PerformDetection";

    public enum ProtesterState
    {
        Chase,
        FollowProtest
    }

    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours;
    [SerializeField] private List<Detector> _detectors;
    [SerializeField] private ProtesterData _protesterData;
    [ReadOnly] [SerializeField]  private Vector3 _moveDirectionInput = Vector3.zero;
    [SerializeField] private ContextSolver _movementDirectionSolver;
    [SerializeField] private float _catchDistance = 1f;
    [SerializeField] private float _catchAttemptDelay = 1f;
    [SerializeField] private float _meetingPointReachedDistance = 2f;
    //performance parameters
    [SerializeField] private float _detectionDelay = .05f, _aiUpdateDelay = .06f;
    [SerializeField] private ProtesterState _currentState = ProtesterState.FollowProtest;
    [SerializeField] private bool _showFlowFieldGizmo = false;

    [HideInInspector] public UnityEvent<Transform> OnCatchAttempt;
    [HideInInspector] public UnityEvent OnProtestEndReached;
    [HideInInspector] public UnityEvent<Vector3> OnMoveDirectionInput, OnPointerInput;

    private bool _isChasing = false;

    private void Start()
    {
        ProtestFlowFields.Instance.OnFlowFieldsCreated.AddListener(ProtestManager_OnFlowFieldsCreated);
        InvokeRepeating(PERFORM_DETECTION, 0f, _detectionDelay);
    }

    private void OnDisable()
    {
        ProtestFlowFields.Instance.OnFlowFieldsCreated.RemoveListener(ProtestManager_OnFlowFieldsCreated);
    }

    private void ProtestManager_OnFlowFieldsCreated()
    {
        _protesterData.FlowFieldsProtest = ProtestFlowFields.Instance.GetFlowFields();
        _protesterData.CurrentFlowFieldIndex = _protesterData.FlowFieldsProtest.IndexOf(_protesterData.FlowFieldsProtest.First(flowfield => flowfield.Index == 0));
        _protesterData.EndOfProtest = ProtestFlowFields.Instance.GetEndOfProtest();

        if(_currentState == ProtesterState.FollowProtest) StartCoroutine(FollowProtestPath());
    }

    private void PerformDetection()
    {
        foreach(Detector detector in _detectors)
        {
            detector.Detect(_protesterData);
        }
    }

    private void Update()
    {
        switch(_currentState)
        {
            case ProtesterState.FollowProtest:
            {
                //use the next protest flowfield if the NPC reaches the current meeting point 
                if(Vector3.Distance(_protesterData.FlowFieldsProtest[_protesterData.CurrentFlowFieldIndex].Target, transform.position) < _meetingPointReachedDistance && _protesterData.CurrentFlowFieldIndex < _protesterData.FlowFieldsProtest.Count - 1)
                {
                    _protesterData.CurrentFlowFieldIndex = _protesterData.FlowFieldsProtest.IndexOf(_protesterData.FlowFieldsProtest.First(flowfield => flowfield.Index == _protesterData.CurrentFlowFieldIndex + 1));
                }
                break;
            }
            case ProtesterState.Chase:
            {
                //NPC movement based on target availability and if chasing is enabled
                if(_protesterData.CurrentTarget != null)
                {
                    //chase the target if enabled
                    if(!_isChasing)
                    {
                        OnPointerInput?.Invoke(_protesterData.CurrentTarget.position);
                        _isChasing = true;
                        StartCoroutine(ChaseAndCatchTarget());
                    }
                } 
                else if(_protesterData.GetTargetsCount() > 0)
                {
                    _protesterData.CurrentTarget = _protesterData.Targets[0];
                }
                break;
            }
        }
        //Moving the agent
        OnMoveDirectionInput?.Invoke(_moveDirectionInput);
    }

    private IEnumerator ChaseAndCatchTarget()
    {
        if(_protesterData.CurrentTarget == null)
        {
            //Stopping logic
            Debug.Log("Stopping");
            _moveDirectionInput = Vector3.zero;
            _isChasing = false;
            yield return null;
        }
        else
        {
            float distance = Vector3.Distance(_protesterData.CurrentTarget.position, transform.position);
            if(distance < _catchDistance)
            {
                //Catch logic
                _moveDirectionInput = Vector3.zero;
                Debug.Log("Attempting to catch the target");
                OnCatchAttempt?.Invoke(_protesterData.CurrentTarget);
                yield return new WaitForSeconds(_catchAttemptDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
            else
            {
                //chase logic
                _moveDirectionInput = _movementDirectionSolver.GetContextDirection(_steeringBehaviours, _protesterData);
                yield return new WaitForSeconds(_aiUpdateDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
        }
    }

    private IEnumerator FollowProtestPath()
    {
        //Check if protester reached the end of the protest
        float destructionDistance = 1f;
        if(Vector3.Distance(_protesterData.EndOfProtest.position, transform.position) < destructionDistance)
        {
            //Stopping logic
            _moveDirectionInput = Vector3.zero;
            _protesterData.ReachedEndOfProtest = true;
            OnProtestEndReached?.Invoke();
            yield return null;
        }

        if(_protesterData.FlowFieldsProtest.Count == 0)
        {
            //Stopping logic
            Debug.Log("Stopping, no more protest meeting point");
            _moveDirectionInput = Vector3.zero;
        }
        else
        {
            _moveDirectionInput = _movementDirectionSolver.GetContextDirection(_steeringBehaviours, _protesterData);
        }
        yield return new WaitForSeconds(_aiUpdateDelay);
        StartCoroutine(FollowProtestPath());
    }

    public ProtesterState GetProtesterState()
    {
        return _currentState;
    }


    //draw current FlowField info
    private void OnDrawGizmos()
    {
        if(Application.isPlaying && _showFlowFieldGizmo)
        {
            float gridWorldSizeX = 50f;
            float gridWorldSizeY = 50f;

            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSizeX, 1, gridWorldSizeY));
            if(_protesterData.FlowFieldsProtest[_protesterData.CurrentFlowFieldIndex].FlowField != null)
            {
                FlowField currentFlowField = _protesterData.FlowFieldsProtest[_protesterData.CurrentFlowFieldIndex].FlowField;
                if(currentFlowField.Grid != null)
                {
                    foreach(Node node in currentFlowField.Grid)
                    {
                        //Gizmos.color = node.walkable ? Color.green : Color.red;

                        //float t = (float) node.bestCost / 75;
                        //Gizmos.color = Color.Lerp(Color.yellow, Color.magenta, t);
                        //Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeRadius*2 - .1f));

                        //Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeRadius*2 - .1f));
                        UnityEditor.Handles.Label(node.WorldPosition, node.BestCost.ToString());
                    }
                }
            }
        }
        
    }
}
