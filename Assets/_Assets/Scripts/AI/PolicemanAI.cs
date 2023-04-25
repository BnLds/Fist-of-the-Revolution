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
        LookForTarget,
        Block,
        Infiltrate,
        Protect,
        MoveToWatchedPoint
    }

    [SerializeField] private PoliceFlowfieldsGenerator policeFlowfieldsGenerator;

    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private PolicemanData policemanData;
    [ReadOnly] [SerializeField]  private Vector3 moveDirectionInput = Vector3.zero;
    [SerializeField] private ContextSolver movementDirectionSolver;
    [SerializeField] private float catchDistance = 1f;
    [SerializeField] private float catchAttemptDelay = 1f;
    [SerializeField] private float watchReactionRange = 10f;

    //performance parameters
    [SerializeField] private float detectionDelay = .05f, aiUpdateDelay = .06f;
    [SerializeField] private PoliceState currentState = PoliceState.Chase;
    [SerializeField] private bool showGizmo = false;

    public UnityEvent<Transform> OnCatchAttempt;
    public UnityEvent<Vector3> OnMoveDirectionInput, OnPointerInput;

    private bool isChasing = false;
    private bool isWalking = false;

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
            detector.Detect(policemanData);
        }

        if(PoliceResponseData.watchPoints != null && PoliceResponseData.watchPoints.Count != 0)
        {
            foreach(Transform watchPoint in PoliceResponseData.watchPoints)
            {
                if(Utility.Distance2DBetweenVector3(watchPoint.position, transform.position) <= watchReactionRange)
                {
                    policemanData.watchedObjectsInReactionRange.Add(watchPoint);
                }
            }
        }
        
    }

    private void Update()
    {
        Debug.Log(currentState);

        switch(currentState)
        {
            case PoliceState.Idle:
            {
                //if within reaction range of watched objects, go to protect the closest object
                if(policemanData.watchedObjectsInReactionRange.Count != 0)
                {
                    //Get closest watched object
                    Transform reactionPoint = policemanData.watchedObjectsInReactionRange.OrderBy(target => Utility.Distance2DBetweenVector3(transform.position, target.position)).FirstOrDefault();
                    policemanData.currentWatchObjectPosition = new Vector3(reactionPoint.position.x, reactionPoint.position.y, reactionPoint.position.z);
                    //Generate flowfield to reaction point
                    //policemanData.currentFlowField = PoliceFlowfieldsGenerator.Instance.CreateNewFlowField(reactionPoint);
                    policemanData.currentFlowField = policeFlowfieldsGenerator.CreateNewFlowField(reactionPoint);

                    currentState = PoliceState.MoveToWatchedPoint;
                }


                break;
            }
            case PoliceState.MoveToWatchedPoint:
            {
                if (!isWalking)
                    {
                        isWalking = true;
                        StartCoroutine(GoToWatchPoint());
                    }
                break;
            }
            case PoliceState.Protect:
            {
                //stay within a certain area of a watched object
                //chase any character damaging the object

                if(policemanData.currentTarget == null)
                {
                        currentState = PoliceState.LookForTarget;
                    }

                break;
            }
            case PoliceState.Chase:
            {
                //chase a specific target
                //NPC movement based on target availability and if chasing is enabled
                if(policemanData.currentTarget != null)
                {
                    //chase the target if enabled
                    if(!isChasing)
                    {
                        OnPointerInput?.Invoke(policemanData.currentTarget.position);
                        isChasing = true;
                        StartCoroutine(ChaseAndCatchTarget());
                    }
                } 
                else if(policemanData.GetTargetsCount() > 0)
                {
                    policemanData.currentTarget = policemanData.targets[0];
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
        OnMoveDirectionInput?.Invoke(moveDirectionInput);
    }

    private IEnumerator ChaseAndCatchTarget()
    {
        if(policemanData.currentTarget == null)
        {
            //Stopping logic
            Debug.Log("Stopping");
            moveDirectionInput = Vector3.zero;
            isChasing = false;
            yield return null;
        }
        else
        {
            float distance = Vector3.Distance(policemanData.currentTarget.position, transform.position);
            if(distance < catchDistance)
            {
                //Catch logic
                moveDirectionInput = Vector3.zero;
                Debug.Log("Attempting to catch the target");
                OnCatchAttempt?.Invoke(policemanData.currentTarget);
                yield return new WaitForSeconds(catchAttemptDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
            else
            {
                //chase logic
                moveDirectionInput = movementDirectionSolver.GetChaseDirection(steeringBehaviours, policemanData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndCatchTarget());
            }
        }
    }

    private IEnumerator GoToWatchPoint()
    {
        //Check if policeman reached the area to watch
        float reactionPointReachedRange = 2f;
        if(Utility.Distance2DBetweenVector3(policemanData.currentWatchObjectPosition, transform.position) < reactionPointReachedRange)
        {
            //Stopping logic
            moveDirectionInput = Vector3.zero;
            policemanData.currentFlowField = null;
            isWalking = false;
            currentState = PoliceState.Protect;

            yield return null;
        }

        if(policemanData.currentFlowField != null)
        {
            moveDirectionInput = movementDirectionSolver.GetPoliceReactionDirection(steeringBehaviours, policemanData);
            yield return new WaitForSeconds(aiUpdateDelay);
            StartCoroutine(GoToWatchPoint());
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
