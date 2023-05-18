using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BreakablesCollectionManager : MonoBehaviour
{
    public static BreakablesCollectionManager Instance { get; private set;}

    [SerializeField] private List<BreakableController> _breakablesList;
    public int HighValueObjectThreshold { get; private set; } = 5;


    [HideInInspector] public UnityEvent<int> OnScoreChange;

    private int _score;

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

        foreach(BreakableController breakable in _breakablesList)
        {
            breakable.OnDamagedBreakable.AddListener(Breakable_OnDamagedBreakable);
            breakable.OnDestroyedBreakable.AddListener(Breakable_OnDestroyedBreakable);
        }

        _score = 0;
    }

    private void Breakable_OnDestroyedBreakable(int reward, BreakableController sender)
    {
        OnScoreChange?.Invoke(reward);

        _breakablesList.Remove(sender);
        sender.OnDamagedBreakable.RemoveAllListeners();
        sender.OnDestroyedBreakable.RemoveAllListeners();
    }

    private void Breakable_OnDamagedBreakable(int scoreGained, Transform breakable)
    {
        _score += scoreGained;
        OnScoreChange?.Invoke(scoreGained);
    }

    public List<BreakableController> GetBreakablesList()
    {
        return _breakablesList;
    }
}
