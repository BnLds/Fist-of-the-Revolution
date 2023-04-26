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
    private bool _isOnWatch;

    public UnityEvent<int, BreakableController> OnDestroyedBreakable;
    public UnityEvent<int, Transform> OnDamagedBreakable;

    public UnityEvent<int, Transform> StartWatch;

    private void Awake()
    {
        _maxHealth = _breakableSO.Health;
        _health = _maxHealth;
        _watchValue = _breakableSO.WatchValue;
        _maxReward = _breakableSO.Reward;
        _remainingRewardValue = _maxReward;
        _isOnWatch = false;
    }

    public void Damage(int damageValue)
    {
        if(_health - damageValue > 0)
        {
            _health -= damageValue;
            int rewardGranted = Mathf.CeilToInt(_maxReward * damageValue / _maxHealth);
            _remainingRewardValue -= rewardGranted;
            OnDamagedBreakable?.Invoke(rewardGranted, transform);

            if(!_isOnWatch)
            {
                _isOnWatch = true;
                StartWatch?.Invoke(_watchValue, transform);
            }   
        } 
        else
        {
            OnDestroyedBreakable?.Invoke(_remainingRewardValue, this);
            Destroy(gameObject);
        }
    }
}
