using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoliceResponseManager : MonoBehaviour
{
    public static PoliceResponseManager Instance { get; private set; }

    [SerializeField] private BreakablesCollectionManager breakablesCollectionManager;
    [SerializeField] PoliceWatchUI policeWatchUI;
    [SerializeField] private int[] watchThresholds = new int[6] {0, 1, 3, 7, 12, 20};

    private List<BreakableController> breakablesWatched;
    private int currentWatchValue;
    private int currentWatchThresholdIndex;

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

        currentWatchValue = 0;
        currentWatchThresholdIndex = 0;
    }

    private void Start()
    {
        InitializePoliceResponseData();

        breakablesWatched = breakablesCollectionManager.GetBreakablesList();
        foreach(BreakableController breakable in breakablesWatched)
        {
            breakable.OnDestroyedBreakable.AddListener(Breakable_OnDestroyedBreakable);
            breakable.StartWatch.AddListener(Breakable_StartWatch);
        }
    }

    private void Breakable_StartWatch(int watchValue, Transform sender)
    {
        //add damaged object to list of watched items
        PoliceResponseData.watchPoints.Add(sender);

        //increase watch value as soon as object is damaged
        currentWatchValue += watchValue;

        int indexTemp = watchThresholds.Length - 1;
        //find new watch threshhold
        while(currentWatchValue < watchThresholds[indexTemp]) indexTemp--;

        currentWatchThresholdIndex = indexTemp;
        policeWatchUI.DisplayWatchValue(currentWatchThresholdIndex);
    }

    private void Breakable_OnDestroyedBreakable(int remainingWatchValue, BreakableController sender)
    {
        PoliceResponseData.watchPoints.Remove(sender.transform);

        //remove listeners
        sender.OnDestroyedBreakable.RemoveAllListeners();
        sender.StartWatch.RemoveAllListeners();
    }

    private void InitializePoliceResponseData()
    {
        PoliceResponseData.watchPoints = new List<Transform>();
    }

}
