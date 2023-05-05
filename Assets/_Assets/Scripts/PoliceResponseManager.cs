using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PoliceResponseManager : MonoBehaviour
{
    public static PoliceResponseManager Instance { get; private set; }

    [Header("Initialization Parameters")]
    [SerializeField] private BreakablesCollectionManager _breakablesCollectionManager;
    [SerializeField] private PoliceWatchUI _policeWatchUI;
    [SerializeField] private LayerMask suspectMask;

    [Header("Game Balance Parameters")]
    [SerializeField] private int[] _watchThresholds = new int[6] {0, 1, 3, 7, 12, 20};
    [SerializeField] private float suspectDetectionRadius = 10f;

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

    public void UpdateClosestSuspects(Transform sender, BreakableController damagedBreakable)
    {
        List<Collider> allCollidersDetected = Physics.OverlapSphere(damagedBreakable.transform.position, suspectDetectionRadius, suspectMask).OrderBy(target => Utility.Distance2DBetweenVector3(sender.position, target.transform.position)).ToList();

        int numberOfSuspects = 5;
        for (int i = 0; i < (int)Mathf.Min(numberOfSuspects, allCollidersDetected.Count); i++)
        {
            if(allCollidersDetected[i] != null) 
            {
                if(PoliceResponseData.TrackedSuspects.Contains(allCollidersDetected[i].transform))
                {
                    continue;
                }

                if(PoliceResponseData.Suspects.Contains(allCollidersDetected[i].transform))
                {
                    PoliceResponseData.Suspects.Remove(allCollidersDetected[i].transform);
                    PoliceResponseData.TrackedSuspects.Add(allCollidersDetected[i].transform);
                }
                else
                {
                    PoliceResponseData.Suspects.Add(allCollidersDetected[i].transform);
                }
            }
        }
        
        /*
        Debug.Log("suspects List: ");
        foreach(Transform element in PoliceResponseData.Suspects) Debug.Log(element.parent);

        Debug.Log("Tracked suspects List: ");
        foreach(Transform element in PoliceResponseData.TrackedSuspects) Debug.Log(element.parent);
        */
    }

    private void InitializePoliceResponseData()
    {
        PoliceResponseData.WatchPoints = new List<Transform>();
        PoliceResponseData.Suspects = new List<Transform>();
        PoliceResponseData.TrackedSuspects = new List<Transform>();
        PoliceResponseData.IsPlayerIdentified = false;
    }

}
