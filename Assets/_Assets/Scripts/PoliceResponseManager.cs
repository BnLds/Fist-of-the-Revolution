using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoliceResponseManager : MonoBehaviour
{
    public static PoliceResponseManager Instance { get; private set; }

    [SerializeField] private BreakablesCollectionManager _breakablesCollectionManager;
    [SerializeField] private PoliceWatchUI _policeWatchUI;
    [SerializeField] private int[] _watchThresholds = new int[6] {0, 1, 3, 7, 12, 20};

    private List<BreakableController> _breakablesWatched;
    private int _currentWatchValue;
    private int _currentWatchThresholdIndex;

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

        _currentWatchValue = 0;
        _currentWatchThresholdIndex = 0;
    }

    private void Start()
    {
        InitializePoliceResponseData();

        _breakablesWatched = _breakablesCollectionManager.GetBreakablesList();
        foreach(BreakableController breakable in _breakablesWatched)
        {
            breakable.OnDestroyedBreakable.AddListener(Breakable_OnDestroyedBreakable);
            breakable.StartWatch.AddListener(Breakable_StartWatch);
        }
    }

    private void Breakable_StartWatch(int watchValue, Transform sender)
    {
        //add damaged object to list of watched items
        PoliceResponseData.WatchPoints.Add(sender);

        //increase watch value as soon as object is damaged
        _currentWatchValue += watchValue;

        int indexTemp = _watchThresholds.Length - 1;
        //find new watch threshhold
        while(_currentWatchValue < _watchThresholds[indexTemp]) indexTemp--;

        _currentWatchThresholdIndex = indexTemp;
        _policeWatchUI.DisplayWatchValue(_currentWatchThresholdIndex);
    }

    private void Breakable_OnDestroyedBreakable(int remainingWatchValue, BreakableController sender)
    {
        PoliceResponseData.WatchPoints.Remove(sender.transform);

        //remove listeners
        sender.OnDestroyedBreakable.RemoveAllListeners();
        sender.StartWatch.RemoveAllListeners();
    }

    private void InitializePoliceResponseData()
    {
        PoliceResponseData.WatchPoints = new List<Transform>();
        PoliceResponseData.Suspects = new List<Transform>();
        PoliceResponseData.Culprit = null;
    }

}
