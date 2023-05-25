using UnityEngine;

public class ProtesterEmotesUI : IEmotesUI
{
    protected override void PoliceResponseManager_OnSuspectCleared(Transform suspectCleared)
    {
        base.PoliceResponseManager_OnSuspectCleared(suspectCleared);

        if(suspectCleared == transform.parent)
        {
            _isTracked = false;
            _isFollowed = false;
            HideEmote();
        }
    }

    protected override void PoliceResponseManager_OnTrackedList(Transform suspect)
    {
        base.PoliceResponseManager_OnTrackedList(suspect);

        if(suspect == transform.parent && !_isTracked)
        {
            _isTracked = true;
            ShowEmote(EmoteStates.Tracked);
        }
    }

    protected override void PoliceResponseManager_OnFollowed(Transform followedSuspect)
    {
        base.PoliceResponseManager_OnFollowed(followedSuspect);
        if(followedSuspect == transform.parent && !_isFollowed)
        {
            ShowEmote(EmoteStates.Followed);

            _isFollowed = true;
            _isTracked = true;
        }
    }
}
