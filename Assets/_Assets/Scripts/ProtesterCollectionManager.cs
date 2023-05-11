using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProtesterCollectionManager : MonoBehaviour
{
    public static ProtesterCollectionManager Instance {get; private set;}

    [SerializeField] private List<Transform> _protestersCollection;

    [HideInInspector] public UnityEvent<Transform> OnPlayerIDFree;
    [HideInInspector] public UnityEvent OnPlayerTrackFree;


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
    }

    private void Start()
    {
        foreach(Transform protester in _protestersCollection)
        {
            protester.GetComponentInChildren<ProtesterSafeZone>().OnPlayerIDedFree.AddListener(ProtesterSafeZone_OnPlayerIDedFree);
            protester.GetComponentInChildren<ProtesterSafeZone>().OnPlayerTrackedFree.AddListener(ProtesterSafeZone_OnPlayerTrackedFree);

        }
    }

    private void ProtesterSafeZone_OnPlayerTrackedFree()
    {
        OnPlayerTrackFree?.Invoke();
    }

    private void ProtesterSafeZone_OnPlayerIDedFree(Transform sender)
    {
        OnPlayerIDFree?.Invoke(sender);
    }
}