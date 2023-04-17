using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;


public class ProtestManager : MonoBehaviour
{
    [SerializeField] private List<Transform> protesters;
    [SerializeField] private List<Transform> protestMeetingPoints;
    [SerializeField] private float moveSpeed = .5f;
    [SerializeField] private float meetingPointReachedDistance = 5f;

    private List<FlowFieldData> flowFieldsProtesters;
    private int currentFlowFieldIndex;

    private void Awake()
    {
        flowFieldsProtesters = new List<FlowFieldData>();
    }

    private void Start()
    {
        CreateProtestFlowFields();
        currentFlowFieldIndex = flowFieldsProtesters.IndexOf(flowFieldsProtesters.First(flowfield => flowfield.index == 0));
    }

    private void Update()
    {
        if(Vector3.Distance(flowFieldsProtesters[currentFlowFieldIndex].target, protesters[0].position) < meetingPointReachedDistance && currentFlowFieldIndex < flowFieldsProtesters.Count - 1)
        {
            currentFlowFieldIndex = flowFieldsProtesters.IndexOf(flowFieldsProtesters.First(flowfield => flowfield.index == currentFlowFieldIndex + 1));
        }
    }

    private void FixedUpdate()
    {
        if(flowFieldsProtesters.Count == 0) return;
        foreach(Transform protester in protesters)
        {
            Node nodeBelow = flowFieldsProtesters[currentFlowFieldIndex].flowField.GetNodeFromWorldPoint(protester.position);
            Vector3 moveDirection = new Vector3(nodeBelow.bestDirection.Vector.x, 0, nodeBelow.bestDirection.Vector.y);
            Rigidbody protesterRB = protester.GetComponent<Rigidbody>();
            protesterRB.velocity = moveDirection * moveSpeed;
        }
    }

    private void CreateProtestFlowFields()
    {
        for (int i = 0; i < protestMeetingPoints.Count; i++)
        {
            flowFieldsProtesters.Add(new FlowFieldData(i, "MeetingPoint: " + i, protestMeetingPoints[i].position, GridController.Instance.GenerateFlowField(protestMeetingPoints[i])));
        }
    }
}
