using UnityEngine;
using UnityEngine.Events;

public class BreakableController : MonoBehaviour
{
    [SerializeField] private BreakableSO _breakableSO;

    private int _maxHealth;
    private int _health;
    private int _watchValue;
    private int _maxReward;
    private int _remainingRewardValue;
    public bool IsOnWatchList { get; private set; }

    [HideInInspector] public UnityEvent<int, BreakableController> OnDestroyedBreakable;
    [HideInInspector] public UnityEvent<int, Transform> OnDamagedBreakable;
    [HideInInspector] public UnityEvent<int, Transform> StartWatch;

    private void Awake()
    {
        _maxHealth = _breakableSO.Health;
        _health = _maxHealth;
        _watchValue = _breakableSO.WatchValue;
        _maxReward = _breakableSO.Reward;
        _remainingRewardValue = _maxReward;
        IsOnWatchList = false;
    }

    public void Damage(int damageValue)
    {
        if(_health - damageValue > 0)
        {
            _health -= damageValue;
            int rewardGranted = Mathf.CeilToInt(_maxReward * damageValue / _maxHealth);
            _remainingRewardValue -= rewardGranted;
            OnDamagedBreakable?.Invoke(rewardGranted, transform);

            if(!IsOnWatchList)
            {
                IsOnWatchList = true;
                StartWatch?.Invoke(_watchValue, transform);
            }   
        } 
        else
        {
            IsOnWatchList = false;
            OnDestroyedBreakable?.Invoke(_remainingRewardValue, this);
            Destroy(gameObject);
        }
    }
}
