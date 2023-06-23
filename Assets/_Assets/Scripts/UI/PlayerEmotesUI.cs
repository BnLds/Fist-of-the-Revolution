using UnityEngine;

public class PlayerEmotesUI : IEmotesUI
{
    protected bool _isIdentified;

    protected override void Awake()
    {
        base.Awake();

        _isIdentified = false;
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
            ShowEmote(EmoteStates.Identified);
            return;
        }
    }

    protected override void PoliceResponseManager_OnFollowed(Transform followedSuspect)
    {
        base.PoliceResponseManager_OnFollowed(followedSuspect);

        if(followedSuspect == PlayerController.Instance.transform && _isIdentified)
        {
            ShowEmote(EmoteStates.Identified);
            return;
        }
    }

    protected override void PoliceResponseManager_OnPlayerNotIDedAnymore(Transform sender)
    {
        base.PoliceResponseManager_OnPlayerNotIDedAnymore(sender);

        _isIdentified = false;
        HideEmote();
    }
}
