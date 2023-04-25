using System.Collections;
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
        Watch,
        Block,
        Infiltrate
    }

    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private PoliceData policeData;
    [ReadOnly] [SerializeField]  private Vector3 moveDirectionInput = Vector3.zero;
    [SerializeField] private ContextSolver movementDirectionSolver;
    [SerializeField] private float catchDistance = 1f;
    [SerializeField] private float catchAttemptDelay = 1f;

    //performance parameters
    [SerializeField] private float detectionDelay = .05f, aiUpdateDelay = .06f;
    [SerializeField] private PoliceState currentState = PoliceState.Chase;
    [SerializeField] private bool showGizmo = false;

    public UnityEvent<Transform> OnCatchAttempt;
    public UnityEvent<Vector3> OnMoveDirectionInput, OnPointerInput;

    private bool isChasing = false;

    private void Start()
    {
        InvokeRepeating(PERFORM_DETECTION, 0f, detectionDelay);
    }

    private void OnDisable()
    {

    }

    private void PerformDetection()
    {
        foreach(Detector detector in detectors)
        {
            detector.Detect(policeData);
        }
    }

    private void Update()
    {
        switch(currentState)
        {
            case PoliceState.Idle:
            {
                
                break;
            }
            case PoliceState.Chase:
            {
                //NPC movement based on target availability and if chasing is enabled
                if(policeData.currentTarget != null)
                {
                    //chase the target if enabled
                    if(!isChasing)
                    {
                        OnPointerInput?.Invoke(policeData.currentTarget.position);
                        isChasing = true;
                        StartCoroutine(ChaseAndCatchTarget());
                    }
                } 
                else if(policeData.GetTargetsCount() > 0)
                {
                    policeData.currentTarget = policeData.targets[0];
                }

                break;
            }
            case PoliceState.Watch:
            {
                
                break;
            }
            case PoliceState.Block:
            {
                
                break;
            }
            case PoliceState.Infiltrate:
            {
                
                break;
            }
        }
        //Moving the agent
        OnMoveDirectionInput?.Invoke(moveDirectionInput);
    }

    private IEnumerator ChaseAndCatchTarget()
    {
        if(policeData.currentTarget == null)
        {
            //Stopping logic
            Debug.Log("Stopping");
            moveDirectionInput = Vector3.zero;
            isChasing = false;
            yield return null;
        }
        else
        {
            float distance = Vector3.Distance(policeData.currentTarget.position, transform.position);
            if(distance < catchDistance)
            {
                //Catch logic
                moveDirectionInput = Vector3.zero;
                Debug.Log("Attempting to catch the target");
                OnCatchAttempt?.Invoke(policeData.currentTarget);
                yield return new WaitForSeconds(catchAttemptDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
            else
            {
                //chase logic
                moveDirectionInput = movementDirectionSolver.GetChaseDirection(steeringBehaviours, policeData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
        }
    }

    public PoliceState GetPoliceState()
    {
        return currentState;
    }


    private void OnDrawGizmos()
    {
        if(Application.isPlaying && showGizmo)
        {
                
        }
    }
}
