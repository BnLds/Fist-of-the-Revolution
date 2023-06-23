using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IEmotesUI : MonoBehaviour
{
    public enum EmoteStates
    {
        Tracked,
        Followed,
        Identified
    }

    [System.Serializable]
    public struct EmoteAllocation
    {
        public EmoteStates EmoteStatus;
        public Sprite EmoteSprite;
        public Color EmoteColor;

        public EmoteAllocation(Sprite emoteSprite, EmoteStates state, Color emoteColor)
        {
            EmoteSprite = emoteSprite;
            EmoteStatus = state;
            EmoteColor = emoteColor;
        }
    }

    [SerializeField] private List<EmoteAllocation> _emotesAllocationCollection;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _defaultSprite;
    private Color _defaultColor = Color.black;

    protected bool _isFollowed;
    protected bool _isTracked;

    protected virtual void Awake()
    {
        _isFollowed = false;
        _isTracked = false;
    }

    private void Start()
    {
        PoliceResponseManager.Instance.OnFollowed.AddListener(PoliceResponseManager_OnFollowed);
        PoliceResponseManager.Instance.OnTracked.AddListener(PoliceResponseManager_OnTrackedList);
        PoliceResponseManager.Instance.OnSuspectCleared.AddListener(PoliceResponseManager_OnSuspectCleared);
        PoliceResponseManager.Instance.OnPlayerIdentified.AddListener(PoliceResponseManager_OnPlayerIdentified);
        PoliceResponseManager.Instance.OnPlayerNotIDedAnymore.AddListener(PoliceResponseManager_OnPlayerNotIDedAnymore);

        HideEmote();
    }

    protected virtual void PoliceResponseManager_OnPlayerNotIDedAnymore(Transform sender) {}
    protected virtual void PoliceResponseManager_OnPlayerIdentified() {}
    protected virtual void PoliceResponseManager_OnSuspectCleared(Transform suspectCleared) {}
    protected virtual void PoliceResponseManager_OnTrackedList(Transform suspect) {}
    protected virtual void PoliceResponseManager_OnFollowed(Transform followedSuspect) {}

    protected void ShowEmote(EmoteStates status)
    {
        EmoteAllocation allocation = _emotesAllocationCollection.First(_ => _.EmoteStatus == status);
        _image.sprite = allocation.EmoteSprite;
        _image.color = allocation.EmoteColor;

        gameObject.SetActive(true);
    }

    protected void HideEmote()
    {
        gameObject.SetActive(false);
    }
}
