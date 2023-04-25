using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ProtestFlowFields : MonoBehaviour
{
    public static ProtestFlowFields Instance { get; private set; }

    [SerializeField] private List<Transform> protestMeetingPoints;
    [SerializeField] private Transform endOfProtest;

    public UnityEvent OnFlowFieldsCreated;

    private List<ProtestFlowFieldData> flowFieldsProtest;

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

        flowFieldsProtest = new List<ProtestFlowFieldData>();
    }

    private void Start()
    {
        CreateProtestFlowFields();
    }

    private void CreateProtestFlowFields()
    {
        for (int i = 0; i < protestMeetingPoints.Count; i++)
        {
            flowFieldsProtest.Add(new ProtestFlowFieldData(i, "MeetingPoint: " + i, protestMeetingPoints[i].position, GridController.Instance.GenerateFlowField(protestMeetingPoints[i])));
        }

        OnFlowFieldsCreated?.Invoke();
    }

    public List<ProtestFlowFieldData> GetFlowFields()
    {
        return flowFieldsProtest;
    }

    public Transform GetEndOfProtest()
    {
        return endOfProtest;
    }

}
