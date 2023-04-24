using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakablesCollectionManager : MonoBehaviour
{
    [SerializeField] private List<BreakableController> breakablesList;

    public UnityEvent<int> OnScoreChange;

    private int score;

    private void Awake()
    {        
        foreach(BreakableController breakable in breakablesList)
        {
            breakable.OnDamagedBreakable.AddListener(Breakable_OnDamagedBreakable);
            breakable.OnDestroyedBreakable.AddListener(Breakable_OnDestroyedBreakable);
        }

        score = 0;
    }

    private void Breakable_OnDestroyedBreakable(int reward, BreakableController sender)
    {
        OnScoreChange?.Invoke(reward);

        breakablesList.Remove(sender);
        sender.OnDamagedBreakable.RemoveAllListeners();
        sender.OnDestroyedBreakable.RemoveAllListeners();
    }

    private void Breakable_OnDamagedBreakable(int scoreGained, Transform breakable)
    {
        score += scoreGained;
        OnScoreChange?.Invoke(scoreGained);
    }

    public List<BreakableController> GetBreakablesList()
    {
        return breakablesList;
    }
}
