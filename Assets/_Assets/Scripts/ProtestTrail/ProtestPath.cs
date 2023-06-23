using System.Collections.Generic;
using UnityEngine;

public class ProtestPath : MonoBehaviour
{
    public static ProtestPath Instance { get; private set; }
    [SerializeField] private List<Transform> _protestMeetingPoints;
    [SerializeField] private Transform _endOfProtest;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    public List<Transform> GetProtestPath()
    {
        return _protestMeetingPoints;
    }

    public Transform GetEndOfProtest()
    {
        return _endOfProtest;
    }
}
