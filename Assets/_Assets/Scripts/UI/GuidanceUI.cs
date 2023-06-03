using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidanceUI : MonoBehaviour
{
    public static GuidanceUI Instance { get; private set; } 

    public enum Guidance
    {
        Attack,
        Hide,
        Casserolade,
        Empty
    }

    private const int MAX_MESSAGE_COUNT = 2; //Number of times any message can be displayed on screen

    [SerializeField] private TextMeshProUGUI _guidanceText;
    [SerializeField] private LocalizationManager _localizationManager;
    [SerializeField] private LocalTableKeySO _guidanceAttackSO;
    [SerializeField] private LocalTableKeySO _guidanceHideSO;
    [SerializeField] private LocalTableKeySO _guidanceCasseroladeSO;


    private readonly Dictionary<Guidance, int> _messageCountDict = new Dictionary<Guidance, int>();
    private Guidance _currentGuidanceKeyDisplayed; // The currently displayed guidance message.
    private float _guidanceResetTimer = 10f; // Time before resetting the guidance message.
    private bool _isCoroutineRunning;
    private string _attackKey;
    private string _hideKey;
    private string _casseroladeKey;
    private string _attackMessage;
    private string _hideMessage;
    private string _casseroladeMessage;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this); 
        }
        else
        {
            Instance = this;
        }

        _currentGuidanceKeyDisplayed = Guidance.Empty;
        _isCoroutineRunning = false;
    }

    private void Start()
    {
        _attackKey = _guidanceAttackSO.LocalizationKey;
        _hideKey = _guidanceHideSO.LocalizationKey;
        _casseroladeKey = _guidanceCasseroladeSO.LocalizationKey;

        _localizationManager.OnLocalTableLoaded.AddListener(InitializeMessages);
        _localizationManager.OnLanguageChanged.AddListener(UpdateMessages);

        Hide();
    }

    private void InitializeMessages()
    {
        _attackMessage = _localizationManager.InitializeLocalizedString(_attackKey);
        _hideMessage = _localizationManager.InitializeLocalizedString(_hideKey);
        _casseroladeMessage = _localizationManager.InitializeLocalizedString(_casseroladeKey);

        InitializeMessageDict();
    }

    private void UpdateMessages()
    {
        _attackMessage = _localizationManager.UpdateLocalizedString(_attackKey);
        _hideMessage = _localizationManager.UpdateLocalizedString(_hideKey);
        _casseroladeMessage = _localizationManager.UpdateLocalizedString(_casseroladeKey);
    }

    private void InitializeMessageDict()
    {
        _messageCountDict[Guidance.Attack] = 0;
        _messageCountDict[Guidance.Hide] = 0;
        _messageCountDict[Guidance.Casserolade] = 0;
    }

    private void ShowGuidance(Guidance key)
    {
        // Check if the same message was not previously displayed to avoid spamming the player.
        bool isMessageCountLessThanMax = _messageCountDict.ContainsKey(key) && _messageCountDict[key] < MAX_MESSAGE_COUNT;
        if (isMessageCountLessThanMax)
        {
            if(_currentGuidanceKeyDisplayed != key)
            {
                _guidanceText.gameObject.SetActive(true);
                switch(key)
                {
                    case(Guidance.Attack):
                        _guidanceText.text = _attackMessage;
                        break;
                    case(Guidance.Hide):
                        _guidanceText.text = _hideMessage;
                        break;
                    case(Guidance.Casserolade):
                        _guidanceText.text = _casseroladeMessage;
                        break;
                }
                _currentGuidanceKeyDisplayed = key;
                _messageCountDict[key]++; // Increment the message count


                StopCoroutine(ResetMessage());
                _isCoroutineRunning = false;
            }
            else if (!_isCoroutineRunning)
            {
                StartCoroutine(ResetMessage());
            }
        }
    }

    private IEnumerator ResetMessage()
    {
        _isCoroutineRunning = true;
        yield return new WaitForSeconds(_guidanceResetTimer);
        _currentGuidanceKeyDisplayed = Guidance.Empty;
        _isCoroutineRunning = false;
        yield break;
    }

    private void HideGuidance(Guidance key)
    {
        if (_currentGuidanceKeyDisplayed == key)
        {
            _guidanceText.gameObject.SetActive(false);
        }
    }

    private void Hide()
    {
        _guidanceText.gameObject.SetActive(false);
    }

    public void ShowGuidanceSafeZone()
    {
        ShowGuidance(Guidance.Hide);
    }

    public void ShowGuidanceAttack()
    {
        ShowGuidance(Guidance.Attack);
    }

    public void ShowGuidanceCasserolade()
    {
        ShowGuidance(Guidance.Casserolade);
    }

    // This method is called by the animation at the end of the Pop animation.
    public void HideEndAnimation()
    {
        _guidanceText.gameObject.SetActive(false);
    }
}
