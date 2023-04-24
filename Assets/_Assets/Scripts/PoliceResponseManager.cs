using System.Collections.Generic;
using UnityEngine;

public class PoliceResponseManager : MonoBehaviour
{
    [SerializeField] private List<BreakableController> breakablesWatched;
    [SerializeField] PoliceWatchUI policeWatchUI;

    private int currentWatchValue;
    private int[] watchThresholds;
    private int currentWatchThresholdIndex;

    private void Awake()
    {
        foreach(BreakableController breakable in breakablesWatched)
        {
            breakable.OnDestroyedBreakable.AddListener(IncreaseWatch);
        }

        currentWatchValue = 0;
        watchThresholds = new int[6] {0, 1, 3, 7, 12, 20};
        currentWatchThresholdIndex = 0;
    }

    private void IncreaseWatch(int destroyedValue, Transform sender)
    {
        currentWatchValue += destroyedValue;

        int numberOfFullStars = watchThresholds.Length - 1;
        while(currentWatchValue < watchThresholds[numberOfFullStars]) numberOfFullStars--;

        currentWatchThresholdIndex = numberOfFullStars;
        currentWatchValue = watchThresholds[numberOfFullStars];
        policeWatchUI.DisplayWatchValue(currentWatchThresholdIndex);

        sender.GetComponent<BreakableController>().OnDestroyedBreakable.RemoveListener(IncreaseWatch);
    }
}
