using UnityEngine;
using UnityEngine.Events;

public class BreakableController : MonoBehaviour
{
    [SerializeField] private BreakableSO breakableSO;

    private int maxHealth;
    private int health;
    private int watchValue;
    private int maxReward;
    private int remainingRewardValue;
    private bool isOnWatch;

    public UnityEvent<int, BreakableController> OnDestroyedBreakable;
    public UnityEvent<int, Transform> OnDamagedBreakable;

    public UnityEvent<int, Transform> StartWatch;

    private void Awake()
    {
        maxHealth = breakableSO.health;
        health = maxHealth;
        watchValue = breakableSO.watchValue;
        maxReward = breakableSO.reward;
        remainingRewardValue = maxReward;
        isOnWatch = false;
    }

    public void Damage(int damageValue)
    {
        if(health - damageValue > 0)
        {
            health -= damageValue;
            int rewardGranted = Mathf.CeilToInt(maxReward * damageValue / maxHealth);
            remainingRewardValue -= rewardGranted;
            OnDamagedBreakable?.Invoke(rewardGranted, transform);

            if(!isOnWatch)
            {
                isOnWatch = true;
                StartWatch?.Invoke(watchValue, transform);
            }   
        } 
        else
        {
            OnDestroyedBreakable?.Invoke(remainingRewardValue, this);
            Destroy(gameObject);
        }
    }
}
