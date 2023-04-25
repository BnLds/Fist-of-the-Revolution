using System;
using System.Collections.Generic;
using UnityEngine;

public class PoliceResponseManager : MonoBehaviour
{
    [SerializeField] private BreakablesCollectionManager breakablesCollectionManager;
    [SerializeField] PoliceWatchUI policeWatchUI;
    [SerializeField] private int [] watchThresholds = new int[6] {0, 1, 3, 7, 12, 20};

    private List<BreakableController> breakablesWatched;
    private int currentWatchValue;
    private int currentWatchThresholdIndex;

    private void Awake()
    {
        currentWatchValue = 0;
        currentWatchThresholdIndex = 0;
    }

    private void Start()
    {
        breakablesWatched = breakablesCollectionManager.GetBreakablesList();
        foreach(BreakableController breakable in breakablesWatched)
        {
            breakable.OnDestroyedBreakable.AddListener(Breakable_OnDestroyedBreakable);
            breakable.StartWatch.AddListener(Breakable_StartWatch);
        }
    }

    private void Breakable_StartWatch(int watchValue, Transform sender)
    {
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
        //remove listeners
        sender.OnDestroyedBreakable.RemoveAllListeners();
        sender.StartWatch.RemoveAllListeners();
    }

}
