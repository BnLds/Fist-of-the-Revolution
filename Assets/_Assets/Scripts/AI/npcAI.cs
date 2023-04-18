using System.Collections.Generic;
using UnityEngine;

public class npcAI : MonoBehaviour
{
    private const string PERFORM_DETECTION = "PerformDetection";

    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private AIData aiData;
    [SerializeField] private float detectionDelay = .05f;

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

        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach(SteeringBehaviour behaviour in steeringBehaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, aiData);
        }
    }

}
