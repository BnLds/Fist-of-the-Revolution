using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ProtestFlowFields : MonoBehaviour
{
    public static ProtestFlowFields Instance { get; private set; }

    [SerializeField] private List<Transform> _protestMeetingPoints;
    [SerializeField] private Transform _endOfProtest;

    [HideInInspector] public UnityEvent OnFlowFieldsCreated;

    private List<ProtestFlowFieldData> _flowFieldsProtest;

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

        _flowFieldsProtest = new List<ProtestFlowFieldData>();
    }

    private void Start()
    {
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

    public Transform GetEndOfProtest()
    {
        return _endOfProtest;
    }

}
