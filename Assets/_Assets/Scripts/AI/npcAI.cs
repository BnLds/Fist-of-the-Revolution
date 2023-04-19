using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class npcAI : MonoBehaviour
{
    private const string PERFORM_DETECTION = "PerformDetection";

    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private AIData aiData;
    [SerializeField] private Vector3 movementInput = Vector3.zero;
    [SerializeField] private ContextSolver movementDirectionSolver;
    [SerializeField] private bool isChasingEnabled = false;
    [SerializeField] private float catchDistance = 1f;
    [SerializeField] private float catchAttemptDelay = 1f;
    //performance parameters
    [SerializeField] private float detectionDelay = .05f, aiUpdateDelay = .06f;

    public UnityEvent OnCatchAttempt;
    public UnityEvent<Vector3> OnMovementInput, OnPointerInput;

    private bool isChasing = false;

    private void Start()
    {
        InvokeRepeating(PERFORM_DETECTION, 0f, detectionDelay);
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
                    StartCoroutine(ChaseAndCatch());
                }
            } 
            else if(aiData.GetTargetsCount() > 0)
            {
                aiData.currentTarget = aiData.targets[0];
            }
        }
        //Moving the agent
        OnMovementInput?.Invoke(movementInput);
    }

    private IEnumerator ChaseAndCatch()
    {
        if(aiData.currentTarget == null)
        {
            //Stopping logic
            Debug.Log("Stopping");
            movementInput = Vector3.zero;
            isChasing = false;
            yield return null;
        }
        else
        {
            float distance = Vector3.Distance(aiData.currentTarget.position, transform.position);
            if(distance < catchDistance)
            {
                //Catch logic
                movementInput = Vector3.zero;
                OnCatchAttempt?.Invoke();
                yield return new WaitForSeconds(catchAttemptDelay);
                StartCoroutine(ChaseAndCatch());
            }
            else
            {
                //chase logic
                movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndCatch());
            }
        }
        
    }
}
