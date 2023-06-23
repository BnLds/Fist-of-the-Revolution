using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ProtestFlowFields : MonoBehaviour
{
    public static ProtestFlowFields Instance { get; private set; }

    [HideInInspector] public UnityEvent OnFlowFieldsCreated;

    private List<Transform> _protestMeetingPoints;
    private Transform _endOfProtest;
    private List<ProtestFlowFieldData> _flowFieldsProtest;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _flowFieldsProtest = new List<ProtestFlowFieldData>();
    }

    private void Start()
    {
        _protestMeetingPoints = ProtestPath.Instance.GetProtestPath();
        _endOfProtest = ProtestPath.Instance.GetEndOfProtest();
        
        CreateProtestFlowFields();
    }

    private void CreateProtestFlowFields()
    {
        for (int i = 0; i < _protestMeetingPoints.Count; i++)
        {
            _flowFieldsProtest.Add(new ProtestFlowFieldData(i, "MeetingPoint: " + i, _protestMeetingPoints[i].position, GridController.Instance.GenerateFlowField(_protestMeetingPoints[i].position)));
        }

        OnFlowFieldsCreated?.Invoke();
    }

    public List<ProtestFlowFieldData> GetFlowFields()
    {
        return _flowFieldsProtest;
    }
}
