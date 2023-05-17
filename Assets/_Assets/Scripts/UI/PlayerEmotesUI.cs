using UnityEngine;

public class PlayerEmotesUI : IEmotesUI
{
    protected bool _isIdentified;

    protected override void Awake()
    {
        base.Awake();

        _isIdentified = false;
    }

    protected override void PoliceResponseManager_OnPlayerUntracked()
    {
        base.PoliceResponseManager_OnPlayerUntracked();

        _isTracked = false;
        _isFollowed = false;
        HideEmote();
    }

    protected override void PoliceResponseManager_OnPlayerIdentified()
    {
        base.PoliceResponseManager_OnPlayerIdentified();

        _isIdentified = true;
        ShowEmote(EmoteStates.Identified);
    }

    protected override void PoliceResponseManager_OnTrackedList(Transform suspect)
    {
        base.PoliceResponseManager_OnTrackedList(suspect);

        if(suspect == PlayerController.Instance.transform && _isIdentified)
        {
            _isTracked = true;
            ShowEmote(EmoteStates.Identified);
            return;
        }

        if(suspect == PlayerController.Instance.transform && !_isTracked)
        {
            _isTracked = true;
            ShowEmote(EmoteStates.Tracked);
        }
    }

    protected override void PoliceResponseManager_OnFollowed(Transform followedSuspect)
    {
        base.PoliceResponseManager_OnFollowed(followedSuspect);

        if(followedSuspect == PlayerController.Instance.transform && _isIdentified)
        {
            _isTracked = true;
            _isFollowed = true;
            ShowEmote(EmoteStates.Identified);
            return;
        }

        if(followedSuspect == PlayerController.Instance.transform && !_isFollowed)
        {
            _isFollowed = true;
            _isTracked = true;
            ShowEmote(EmoteStates.Followed);
        }
    }

    protected override void PoliceResponseManager_OnPlayerNotIDedAnymore(Transform sender)
    {
        base.PoliceResponseManager_OnPlayerNotIDedAnymore(sender);

        _isIdentified = false;
        HideEmote();

        if(_isFollowed)
        {
            ShowEmote(EmoteStates.Followed);
        }
        else if(_isTracked)
        {
            ShowEmote(EmoteStates.Tracked);
        }
    }

    protected override void PoliceResponseManager_OnSuspectCleared(Transform suspectCleared)
    {
        base.PoliceResponseManager_OnSuspectCleared(suspectCleared);

        if(suspectCleared == PlayerController.Instance.transform)
        {
            _isTracked = false;
            _isFollowed = false;
            HideEmote();
        }
    }
}
