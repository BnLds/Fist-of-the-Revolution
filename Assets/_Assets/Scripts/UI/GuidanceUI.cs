using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidanceUI : MonoBehaviour
{
    public static GuidanceUI Instance { get; private set; } 

    private const string ATTACK_MESSAGE = "Hold [E] to attack"; // Attack message to display.
    private const string HIDE_MESSAGE = "Stay close to hide!"; // Hide message to display.
    private const string CASSEROLADE = "Hold [C]?";
    private const int MAX_MESSAGE_COUNT = 2; //Number of times any message can be displayed on screen

    [SerializeField] private TextMeshProUGUI _guidanceText; 

    private readonly Dictionary<string, int> _messageCountDict = new Dictionary<string, int>();
    private string _currentGuidanceDisplayed; // The currently displayed guidance message.
    private float _guidanceResetTimer = 10f; // Time before resetting the guidance message.
    private bool _isCoroutineRunning;
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

        InitializeMessageDict();

        _currentGuidanceDisplayed = null;
        _isCoroutineRunning = false;
    }

    private void Start()
    {
        Hide();
    }

    private void InitializeMessageDict()
    {
        _messageCountDict[ATTACK_MESSAGE] = 0;
        _messageCountDict[HIDE_MESSAGE] = 0;
        _messageCountDict[CASSEROLADE] = 0;
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
        ShowGuidance(HIDE_MESSAGE);
    }

    public void ShowGuidanceAttack()
    {
        ShowGuidance(ATTACK_MESSAGE);
    }

    public void ShowGuidanceCasserolade()
    {
        ShowGuidance(CASSEROLADE);
    }

    // This method is called by the animation at the end of the Pop animation.
    public void HideEndAnimation()
    {
        _guidanceText.gameObject.SetActive(false);
    }
}
