using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BreakableController : MonoBehaviour
{
    [SerializeField] private BreakableSO _breakableSO;
    [SerializeField] private GameObject _destroyedPrefab;
    [SerializeField] private GameObject _halo;
    [SerializeField] private AdsSO _adsSO;
    [SerializeField] private Image _image;

    public bool IsHighPriority { get; private set; }
    public bool WasDestroyed { get; private set; }

    private int _maxHealth;
    private int _health;
    private int _watchValue;
    private int _remainingRewardValue;
    private int _maxReward;
    private bool _isOnWatchList;

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
        _isOnWatchList = false;
        WasDestroyed = false;
    }

    private void Start()
    {
        SelectRandomAd();
        IsHighPriority = _maxReward >= BreakablesCollectionManager.Instance.HighValueObjectThreshold;
    }

    private void SelectRandomAd()
    {
        if(_adsSO == null) return;

        int adsCount = _adsSO._adsList.Count;
        int adNumber = Random.Range(0, adsCount-1);

        _image.sprite = _adsSO._adsList[adNumber];
    }

    public void Damage(int damageValue)
    {
        if(_health - damageValue > 0)
        {
            _health -= damageValue;
            int rewardGranted = Mathf.CeilToInt(_maxReward * damageValue / _maxHealth);
            _remainingRewardValue -= rewardGranted;
            OnDamagedBreakable?.Invoke(rewardGranted, transform);

            if(!_isOnWatchList)
            {
                _isOnWatchList = true;
                StartWatch?.Invoke(_watchValue, transform);
            }   
        } 
        else
        {
            _isOnWatchList = false;
            OnDestroyedBreakable?.Invoke(_remainingRewardValue, this);
            WasDestroyed = true;
            _halo.SetActive(false);
            ShowDestroyed();
            gameObject.SetActive(false);
        }
    }

    private void ShowDestroyed()
    {
        _destroyedPrefab.transform.SetParent(null);
        _destroyedPrefab.SetActive(true);
    }

    public int GetWatchersLimit()
    {
        return _breakableSO.NumberOfCopWatchers;
    }
}
