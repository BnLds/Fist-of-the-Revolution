using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class npcAI : MonoBehaviour
{
    private const string PERFORM_DETECTION = "PerformDetection";

    public enum ProtesterState
    {
        Chase,
        FollowProtest
    }

    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private AIData aiData;
    [ReadOnly] [SerializeField]  private Vector3 moveDirectionInput = Vector3.zero;
    [SerializeField] private ContextSolver movementDirectionSolver;
    [SerializeField] private bool isChasingEnabled = false;
    [SerializeField] private float catchDistance = 1f;
    [SerializeField] private float catchAttemptDelay = 1f;
    [SerializeField] private float meetingPointReachedDistance = 2f;
    //performance parameters
    [SerializeField] private float detectionDelay = .05f, aiUpdateDelay = .06f;

    public UnityEvent<Transform> OnCatchAttempt;
    public UnityEvent OnProtestEndReached;
    public UnityEvent<Vector3> OnMoveDirectionInput, OnPointerInput;

    private bool isChasing = false;
    private ProtesterState currentState;

    private void Awake()
    {
        currentState = ProtesterState.FollowProtest;
    }

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
        aiData.flowFieldsProtest = ProtestFlowFields.Instance.GetFlowFields();
        aiData.currentFlowFieldIndex = aiData.flowFieldsProtest.IndexOf(aiData.flowFieldsProtest.First(flowfield => flowfield.index == 0));
        aiData.endOfProtest = ProtestFlowFields.Instance.GetEndOfProtest();
    }

    private void PerformDetection()
    {
        foreach(Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }

    private void Update()
    {
        switch(currentState)
        {
            case ProtesterState.FollowProtest:
            {
                //Check if protester reached the end of the protest
                float destructionDistance = 1f;
                if(Vector3.Distance(aiData.endOfProtest.position, transform.position) < destructionDistance)
                {
                    //Stopping logic
                    Debug.Log("Stopping");
                    moveDirectionInput = Vector3.zero;
                    OnProtestEndReached?.Invoke();
                    break;
                }

                //use the next protest flowfield if the NPC reaches the current meeting point 
                if(Vector3.Distance(aiData.flowFieldsProtest[aiData.currentFlowFieldIndex].target, transform.position) < meetingPointReachedDistance && aiData.currentFlowFieldIndex < aiData.flowFieldsProtest.Count - 1)
                {
                    aiData.currentFlowFieldIndex = aiData.flowFieldsProtest.IndexOf(aiData.flowFieldsProtest.First(flowfield => flowfield.index == aiData.currentFlowFieldIndex + 1));
                }
                
                if(aiData.flowFieldsProtest.Count == 0)
                {
                    //Stopping logic
                    Debug.Log("Stopping, no more protest meeting point");
                    moveDirectionInput = Vector3.zero;
                }
                else
                {
                    //executes the main logic
                    //get NPC position on grid
                    Node nodeBelow = aiData.flowFieldsProtest[aiData.currentFlowFieldIndex].flowField.GetNodeFromWorldPoint(transform.position);
            
                    //Update the move direction of the player based on its position on the grid
                    Vector3 moveDirectionFlowField = new Vector3(nodeBelow.bestDirection.Vector.x, 0, nodeBelow.bestDirection.Vector.y).normalized;
                    moveDirectionInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
                }
                    break;
            }
            case ProtesterState.Chase:
            {
                //Target chasing behaviour
                if(isChasingEnabled)
                {
                    //NPC movement based on target availability and if chasing is enabled
                    if(aiData.currentTarget != null)
                    {
                        //chase the target if enabled
                        if(!isChasing)
                        {
                            OnPointerInput?.Invoke(aiData.currentTarget.position);
                            isChasing = true;
                            StartCoroutine(ChaseAndCatchTarget());
                        }
                    } 
                    else if(aiData.GetTargetsCount() > 0)
                    {
                        aiData.currentTarget = aiData.targets[0];
                    }
                }
                break;
            }
        }
        //Moving the agent
        OnMoveDirectionInput?.Invoke(moveDirectionInput);
    }

    private IEnumerator ChaseAndCatchTarget()
    {
        if(aiData.currentTarget == null)
        {
            //Stopping logic
            Debug.Log("Stopping");
            moveDirectionInput = Vector3.zero;
            isChasing = false;
            yield return null;
        }
        else
        {
            float distance = Vector3.Distance(aiData.currentTarget.position, transform.position);
            if(distance < catchDistance)
            {
                //Catch logic
                moveDirectionInput = Vector3.zero;
                Debug.Log("Attempting to catch the target");
                OnCatchAttempt?.Invoke(aiData.currentTarget);
                yield return new WaitForSeconds(catchAttemptDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
            else
            {
                //chase logic
                moveDirectionInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
        }
    }

    public ProtesterState GetProtesterState()
    {
        return currentState;
    }
}
