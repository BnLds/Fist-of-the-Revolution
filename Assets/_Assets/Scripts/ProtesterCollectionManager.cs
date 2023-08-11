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
    
    private int _forwardProtestPointIndex;

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

        _forwardProtestPointIndex = 0;
    }

    private void Start()
    {
        foreach(Transform protester in _protestersCollection)
        {
            protester.GetComponentInChildren<ProtesterSafeZone>().OnPlayerIDedFree.AddListener(ProtesterSafeZone_OnPlayerIDedFree);
            protester.GetComponentInChildren<ProtesterSafeZone>().OnPlayerTrackedFree.AddListener(ProtesterSafeZone_OnPlayerTrackedFree);
            protester.GetComponentInChildren<ProtesterSafeZone>().OnPlayerEnterSafeZone.AddListener(ProtesterSafeZone_OnPlayerEnterSafeZone);
            protester.GetComponent<ProtesterFlowfieldAI>().OnProtestPointReached.AddListener(ProtesterAI_OnProtestPointReached);
        }
    }

    private void ProtesterAI_OnProtestPointReached(int protestPointIndex)
    {
        if(protestPointIndex > _forwardProtestPointIndex)
        {
            _forwardProtestPointIndex = protestPointIndex;
        }
    }

    private void ProtesterSafeZone_OnPlayerEnterSafeZone()
    {
        GuidanceUI.Instance.ShowGuidanceSafeZone();
    }

    private void ProtesterSafeZone_OnPlayerTrackedFree()
    {
        OnPlayerTrackFree?.Invoke();
    }

    private void ProtesterSafeZone_OnPlayerIDedFree(Transform sender)
    {
        GuidanceUI.Instance.ShowGuidanceHidden();
        OnPlayerIDFree?.Invoke(sender);
    }

    public int GetForwardProtestPointIndex()
    {
        return _forwardProtestPointIndex;
    }
}
