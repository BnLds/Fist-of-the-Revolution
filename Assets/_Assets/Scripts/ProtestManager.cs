using UnityEngine;
using System.Collections.Generic;
using System;



public class ProtestManager : MonoBehaviour
{
    public static ProtestManager Instance { get; private set; }

    [SerializeField] private List<Transform> protestMeetingPoints;
    [SerializeField] private Transform endOfProtest;

    public event EventHandler OnFlowFieldsCreated;

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

        OnFlowFieldsCreated?.Invoke(this, EventArgs.Empty);
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
