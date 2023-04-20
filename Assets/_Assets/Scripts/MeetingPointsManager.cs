using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeetingPointsManager : MonoBehaviour
{
    [SerializeField] private List<Transform> meetingPoints;
    [SerializeField] private npcAI npcai;
    [SerializeField] private SeekBehaviour seekBehaviour;


    [SerializeField] [ReadOnly] private Transform activeMeetingPoint;
    [SerializeField] [ReadOnly] private List<Transform> remainingMeetingPoints;
    [SerializeField] float pointDetection = 4f;

    private void Awake()
    {
        remainingMeetingPoints = new List<Transform>(meetingPoints);
        activeMeetingPoint = meetingPoints[0];
        remainingMeetingPoints.Remove(activeMeetingPoint);
        DisableInactiveMeetingPointsCollider();
    }

    private void Start()
    {
        seekBehaviour.OnTargetReached.AddListener(UpdateTarget);
    }

    private void UpdateTarget()
    {
        if(Vector3.Distance(activeMeetingPoint.position, npcai.transform.position) < pointDetection && remainingMeetingPoints.Count > 0)
        {
            Debug.Log("point reached");
            activeMeetingPoint = remainingMeetingPoints.OrderBy(meetingPoint => Vector3.Distance(npcai.transform.position, meetingPoint.position)).FirstOrDefault();
            activeMeetingPoint.gameObject.GetComponent<Collider>().enabled = true;
            remainingMeetingPoints.Remove(activeMeetingPoint);

            DisableInactiveMeetingPointsCollider();
        }
    }

    private void DisableInactiveMeetingPointsCollider()
    {
        foreach(Transform meetingPoint in meetingPoints)
        {
            if(meetingPoint == activeMeetingPoint) continue;
            meetingPoint.gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
