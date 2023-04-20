using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class ProtestFlowFields : MonoBehaviour
{
    public static ProtestFlowFields Instance { get; private set; }

    [SerializeField] private List<Transform> protestMeetingPoints;
    [SerializeField] private Transform endOfProtest;

    public UnityEvent OnFlowFieldsCreated;

    private List<FlowFieldData> flowFieldsProtest;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        flowFieldsProtest = new List<FlowFieldData>();
    }

    private void Start()
    {
        CreateProtestFlowFields();
    }

    private void CreateProtestFlowFields()
    {
        for (int i = 0; i < protestMeetingPoints.Count; i++)
        {
            flowFieldsProtest.Add(new FlowFieldData(i, "MeetingPoint: " + i, protestMeetingPoints[i].position, GridController.Instance.GenerateFlowField(protestMeetingPoints[i])));
        }

        OnFlowFieldsCreated?.Invoke();
    }

    public List<FlowFieldData> GetFlowFields()
    {
        return flowFieldsProtest;
    }

    public Transform GetEndOfProtest()
    {
        return endOfProtest;
    }

}
