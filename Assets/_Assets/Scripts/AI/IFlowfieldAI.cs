using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

public class IFlowfieldAI : MonoBehaviour
{
    protected const string PERFORM_DETECTION = "PerformDetection";
    protected const string FOLLOW_PROTEST_PATH = "FollowProtestPath";

    [HideInInspector] public UnityEvent<int> OnProtestPointReached;
    [HideInInspector] public UnityEvent<Vector3> OnMoveDirectionInput;

    [Header("Initialization Parameters")]
    [SerializeField] protected List<SteeringBehaviour> _steeringBehaviours;
    [SerializeField] protected List<Detector> _detectors;
    [SerializeField] protected ProtesterData _protesterData;
    [SerializeField] protected ContextSolver _movementDirectionSolver;

    [Space(5)]
    [Header("Game Balance Parameters")]
    [SerializeField] protected float _meetingPointReachedDistance = 2f;

    [Space(5)]
    [Header("Performance Parameters")]
    [SerializeField] protected float _detectionDelay = .05f;
    [SerializeField] protected float _aiUpdateDelay = .06f;

    [Space(5)]
    [Header("Dev Parameters")]
    [ReadOnly] [SerializeField] protected Vector3 _moveDirectionInput = Vector3.zero;
    [SerializeField] private bool _showFlowFieldGizmo = false;

    private bool _isDataInitialized;
    private bool _isTargetEndOfProtest;

    private void Awake()
    {
        _isDataInitialized = false;
        _isTargetEndOfProtest = false;
    }

    protected virtual void Start()
    {
        try
        {
            ProtestFlowfield_OnFlowFieldsCreated();
        }
        catch(Exception)
        {
            ProtestFlowFields.Instance.OnFlowFieldsCreated.AddListener(ProtestFlowfield_OnFlowFieldsCreated);
        }
    }

    protected virtual void OnDisable()
    {
        ProtestFlowFields.Instance.OnFlowFieldsCreated.RemoveListener(ProtestFlowfield_OnFlowFieldsCreated);
    }

    protected virtual void ProtestFlowfield_OnFlowFieldsCreated()
    {
        _protesterData.FlowFieldsProtest = ProtestFlowFields.Instance.GetFlowFields();
        _protesterData.CurrentFlowFieldIndex = _protesterData.FlowFieldsProtest.IndexOf(_protesterData.FlowFieldsProtest.First(flowfield => flowfield.Index == 0));
        _protesterData.EndOfProtest = ProtestPath.Instance.GetEndOfProtest();

        _isDataInitialized = true;
    }

    protected void PerformDetection()
    {
        foreach(Detector detector in _detectors)
        {
            detector.Detect(_protesterData);
        }
    }

    protected void Update()
    {
        if(_isDataInitialized)
        {
            bool hasReachedTarget = Vector3.Distance(_protesterData.FlowFieldsProtest[_protesterData.CurrentFlowFieldIndex].Target, transform.position) < _meetingPointReachedDistance;
            
            //use the next protest flowfield if the NPC reaches the current meeting point 
            if(_isTargetEndOfProtest && hasReachedTarget)
            {
                //flowfields loop so go back to 1st one when end is reached
                _protesterData.CurrentFlowFieldIndex = 0;
                OnProtestPointReached?.Invoke(_protesterData.CurrentFlowFieldIndex);
            }
            if(hasReachedTarget && _protesterData.CurrentFlowFieldIndex < _protesterData.FlowFieldsProtest.Count - 1)
            {
                //flowfield list is ordered from the first meeting point to last, so it is enough to increment currentFlowfieldIndex by 1 to get the current meeting point
                _protesterData.CurrentFlowFieldIndex++;
                _isTargetEndOfProtest = _protesterData.CurrentFlowFieldIndex == _protesterData.FlowFieldsProtest.Count-1;
                OnProtestPointReached?.Invoke(_protesterData.CurrentFlowFieldIndex);
            }
            //Moving the agent
            OnMoveDirectionInput?.Invoke(_moveDirectionInput);
        }
    }

    protected void FollowProtestPath()
    {
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
    }

    //draw current FlowField info
    
    protected void OnDrawGizmos()
    {
        if(Application.isPlaying && _showFlowFieldGizmo)
        {
            float gridWorldSizeX = 100f;
            float gridWorldSizeY = 100f;
            float nodeRadius = .6f;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSizeX, 1, gridWorldSizeY));
            if(_protesterData.FlowFieldsProtest[_protesterData.CurrentFlowFieldIndex].FlowField != null)
            {
                FlowField currentFlowField = _protesterData.FlowFieldsProtest[_protesterData.CurrentFlowFieldIndex].FlowField;
                if(currentFlowField.Grid != null)
                {
                    foreach(Node node in currentFlowField.Grid)
                    {
                        Gizmos.color = node.Walkable ? Color.green : Color.red;

                        //float t = (float) node.bestCost / 75;
                        //Gizmos.color = Color.Lerp(Color.yellow, Color.magenta, t);
                        //Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeRadius*2 - .1f));

                        Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * (nodeRadius*2 - .1f));
                        //UnityEditor.Handles.Label(node.WorldPosition, node.BestCost.ToString());
                    }
                }
            }
        }
    }
}
