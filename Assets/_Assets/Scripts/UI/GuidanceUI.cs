using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidanceUI : MonoBehaviour
{
    public static GuidanceUI Instance { get; private set; } 

    private const string ATTACK_KEY = "guidance_attack"; // Keys to the localization table
    private const string HIDE_KEY = "guidance_hide"; 
    private const string CASSEROLADE_KEY = "guidance_casserolade";
    private const int MAX_MESSAGE_COUNT = 2; //Number of times any message can be displayed on screen

    [SerializeField] private TextMeshProUGUI _guidanceText;
    [SerializeField] private LocalizationManager _localizationManager;

    private readonly Dictionary<string, int> _messageCountDict = new Dictionary<string, int>();
    private string _currentGuidanceDisplayed; // The currently displayed guidance message.
    private float _guidanceResetTimer = 10f; // Time before resetting the guidance message.
    private bool _isCoroutineRunning;
    private string _attackMessage = "Hold [E] to attack";
    private string _hideMessage = "Stay close to hide!";
    private string _casseroladeMessage = "Hold [C]?";


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

        _currentGuidanceDisplayed = null;
        _isCoroutineRunning = false;
    }

    private void Start()
    {
        _localizationManager.OnLocalTableLoaded.AddListener(InitializeMessages);
        Hide();
    }

    private void InitializeMessages()
    {
        _attackMessage = _localizationManager.GetLocalizedString(ATTACK_KEY);
        _hideMessage = _localizationManager.GetLocalizedString(HIDE_KEY);
        _casseroladeMessage = _localizationManager.GetLocalizedString(CASSEROLADE_KEY);

        InitializeMessageDict();
    }

    private void InitializeMessageDict()
    {
        _messageCountDict[_attackMessage] = 0;
        _messageCountDict[_hideMessage] = 0;
        _messageCountDict[_casseroladeMessage] = 0;
    }

    private void ShowGuidance(string message)
    {
        // Check if the same message was not previously displayed to avoid spamming the player.
        bool isMessageCountLessThanMax = _messageCountDict.ContainsKey(message) && _messageCountDict[message] < MAX_MESSAGE_COUNT;
        if (isMessageCountLessThanMax)
        {
            if(_currentGuidanceDisplayed != message)
            {
                _guidanceText.gameObject.SetActive(true);
                _guidanceText.text = message;
                _currentGuidanceDisplayed = message;
                _messageCountDict[message]++; // Increment the message count


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
        _currentGuidanceDisplayed = null;
        _isCoroutineRunning = false;
        yield break;
    }

    private void HideGuidance(string message)
    {
        if (_currentGuidanceDisplayed == message)
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
        ShowGuidance(_hideMessage);
    }

    public void ShowGuidanceAttack()
    {
        ShowGuidance(_attackMessage);
    }

    public void ShowGuidanceCasserolade()
    {
        ShowGuidance(_casseroladeMessage);
    }

    // This method is called by the animation at the end of the Pop animation.
    public void HideEndAnimation()
    {
        _guidanceText.gameObject.SetActive(false);
    }
}
