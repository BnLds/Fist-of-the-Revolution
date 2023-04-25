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

    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private ProtesterData protesterData;
    [ReadOnly] [SerializeField]  private Vector3 moveDirectionInput = Vector3.zero;
    [SerializeField] private ContextSolver movementDirectionSolver;
    [SerializeField] private float catchDistance = 1f;
    [SerializeField] private float catchAttemptDelay = 1f;
    [SerializeField] private float meetingPointReachedDistance = 2f;
    //performance parameters
    [SerializeField] private float detectionDelay = .05f, aiUpdateDelay = .06f;
    [SerializeField] private ProtesterState currentState = ProtesterState.FollowProtest;
    [SerializeField] private bool showFlowFieldGizmo = false;

    public UnityEvent<Transform> OnCatchAttempt;
    public UnityEvent OnProtestEndReached;
    public UnityEvent<Vector3> OnMoveDirectionInput, OnPointerInput;

    private bool isChasing = false;

    private void Start()
    {
        ProtestFlowFields.Instance.OnFlowFieldsCreated.AddListener(ProtestManager_OnFlowFieldsCreated);
        InvokeRepeating(PERFORM_DETECTION, 0f, detectionDelay);
    }

    private void OnDisable()
    {
        ProtestFlowFields.Instance.OnFlowFieldsCreated.RemoveListener(ProtestManager_OnFlowFieldsCreated);
    }

    private void ProtestManager_OnFlowFieldsCreated()
    {
        protesterData.flowFieldsProtest = ProtestFlowFields.Instance.GetFlowFields();
        protesterData.currentFlowFieldIndex = protesterData.flowFieldsProtest.IndexOf(protesterData.flowFieldsProtest.First(flowfield => flowfield.index == 0));
        protesterData.endOfProtest = ProtestFlowFields.Instance.GetEndOfProtest();

        if(currentState == ProtesterState.FollowProtest) StartCoroutine(FollowProtestPath());
    }

    private void PerformDetection()
    {
        foreach(Detector detector in detectors)
        {
            detector.Detect(protesterData);
        }
    }

    private void Update()
    {
        switch(currentState)
        {
            case ProtesterState.FollowProtest:
            {
                //use the next protest flowfield if the NPC reaches the current meeting point 
                if(Vector3.Distance(protesterData.flowFieldsProtest[protesterData.currentFlowFieldIndex].target, transform.position) < meetingPointReachedDistance && protesterData.currentFlowFieldIndex < protesterData.flowFieldsProtest.Count - 1)
                {
                    protesterData.currentFlowFieldIndex = protesterData.flowFieldsProtest.IndexOf(protesterData.flowFieldsProtest.First(flowfield => flowfield.index == protesterData.currentFlowFieldIndex + 1));
                }
                break;
            }
            case ProtesterState.Chase:
            {
                //NPC movement based on target availability and if chasing is enabled
                if(protesterData.currentTarget != null)
                {
                    //chase the target if enabled
                    if(!isChasing)
                    {
                        OnPointerInput?.Invoke(protesterData.currentTarget.position);
                        isChasing = true;
                        StartCoroutine(ChaseAndCatchTarget());
                    }
                } 
                else if(protesterData.GetTargetsCount() > 0)
                {
                    protesterData.currentTarget = protesterData.targets[0];
                }
                break;
            }
        }
        //Moving the agent
        OnMoveDirectionInput?.Invoke(moveDirectionInput);
    }

    private IEnumerator ChaseAndCatchTarget()
    {
        if(protesterData.currentTarget == null)
        {
            //Stopping logic
            Debug.Log("Stopping");
            moveDirectionInput = Vector3.zero;
            isChasing = false;
            yield return null;
        }
        else
        {
            float distance = Vector3.Distance(protesterData.currentTarget.position, transform.position);
            if(distance < catchDistance)
            {
                //Catch logic
                moveDirectionInput = Vector3.zero;
                Debug.Log("Attempting to catch the target");
                OnCatchAttempt?.Invoke(protesterData.currentTarget);
                yield return new WaitForSeconds(catchAttemptDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
            else
            {
                //chase logic
                moveDirectionInput = movementDirectionSolver.GetChaseDirection(steeringBehaviours, protesterData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
        }
    }

    private IEnumerator FollowProtestPath()
    {
        //Check if protester reached the end of the protest
        float destructionDistance = 1f;
        if(Vector3.Distance(protesterData.endOfProtest.position, transform.position) < destructionDistance)
        {
            //Stopping logic
            Debug.Log("Stopping");
            moveDirectionInput = Vector3.zero;
            protesterData.reachedEndOfProtest = true;
            OnProtestEndReached?.Invoke();
            yield return null;
        }

        if(protesterData.flowFieldsProtest.Count == 0)
        {
            //Stopping logic
            Debug.Log("Stopping, no more protest meeting point");
            moveDirectionInput = Vector3.zero;
        }
        else
        {
            moveDirectionInput = movementDirectionSolver.GetProtestDirection(steeringBehaviours, protesterData);
        }
        yield return new WaitForSeconds(aiUpdateDelay);
        StartCoroutine(FollowProtestPath());
    }

    public ProtesterState GetProtesterState()
    {
        return currentState;
    }


    //draw current FlowField info
    private void OnDrawGizmos()
    {
        if(Application.isPlaying && showFlowFieldGizmo)
        {
            float gridWorldSizeX = 50f;
            float gridWorldSizeY = 50f;

            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSizeX, 1, gridWorldSizeY));
            if(protesterData.flowFieldsProtest[protesterData.currentFlowFieldIndex].flowField != null)
            {
                FlowField currentFlowField = protesterData.flowFieldsProtest[protesterData.currentFlowFieldIndex].flowField;
                if(currentFlowField.grid != null)
                {
                    foreach(Node node in currentFlowField.grid)
                    {
                        //Gizmos.color = node.walkable ? Color.green : Color.red;

                        //float t = (float) node.bestCost / 75;
                        //Gizmos.color = Color.Lerp(Color.yellow, Color.magenta, t);
                        //Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeRadius*2 - .1f));

                        //Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeRadius*2 - .1f));
                        UnityEditor.Handles.Label(node.worldPosition, node.bestCost.ToString());
                    }
                }
            }
        }
        
    }
}
