using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidanceUI : MonoBehaviour
{
    public static GuidanceUI  Instance { get; private set; }

    private const string ATTACK_MESSAGE = "Hold [E] to attack";
    private const string HIDE_MESSAGE = "Stay close to hide!";

    [SerializeField] private TextMeshProUGUI _guidanceText;

    private string _currentGuidanceDisplayed;

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

        _currentGuidanceDisplayed = null;
    }

    private void ShowGuidance(string message)
    {
        if(_currentGuidanceDisplayed == null)
        {
            _guidanceText.gameObject.SetActive(true);
            _guidanceText.text = message;
            _currentGuidanceDisplayed = message;
        }
    }

    private void HideGuidance(string message)
    {
        if(_currentGuidanceDisplayed == message)
        {
            _guidanceText.gameObject.SetActive(false);
            _currentGuidanceDisplayed = null;
        }
    }

    public void ShowGuidanceSafeZone()
    {
        ShowGuidance(HIDE_MESSAGE);
    }

    public void HideGuidanceSafeZone()
    {
        HideGuidance(HIDE_MESSAGE);
    }

    public void ShowGuidanceAttack()
    {
        ShowGuidance(ATTACK_MESSAGE);
    }

    public void HideGuidanceAttack()
    {
        HideGuidance(ATTACK_MESSAGE);
    }
}
