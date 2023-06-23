using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PoliceEmotesUI : MonoBehaviour
{
    [System.Serializable]
    public struct EmoteAllocation
    {
        public PoliceUnitSM.PoliceReactions PoliceReaction;
        public Sprite EmoteSprite;
        public Color EmoteColor;

        public EmoteAllocation(Sprite emoteSprite, PoliceUnitSM.PoliceReactions policeReaction, Color emoteColor)
        {
            EmoteSprite = emoteSprite;
            PoliceReaction = policeReaction;
            EmoteColor = emoteColor;
        }
    }

    [SerializeField] private PoliceUnitSM _policeUnitSM;
    [SerializeField] private Image _image;
    [SerializeField] private List<EmoteAllocation> _emotesAllocationCollection;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Color _defaultColor = Color.blue;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _policeUnitSM.OnReact.AddListener(PoliceUnitSM_OnReact);

        ShowDefaultEmote();
    }

    private void PoliceUnitSM_OnReact(PoliceUnitSM.PoliceReactions reaction)
    {
        EnableAnimator();
        EnableAnimatorEvent();

        if(reaction == PoliceUnitSM.PoliceReactions.NoReaction) return;

        try
        {
            switch(reaction)
            {
                /*case(PoliceUnitSM.PoliceReactions.FollowProtest):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.FollowProtest);
                    break;
                }*/
                case(PoliceUnitSM.PoliceReactions.FollowSuspect):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.FollowSuspect);
                    break;
                }
                case(PoliceUnitSM.PoliceReactions.ChasePlayer):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.ChasePlayer);
                    DisableAnimatorEvent();
                    break;
                }
                case(PoliceUnitSM.PoliceReactions.Wander):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.Wander);
                    break;
                }
                case(PoliceUnitSM.PoliceReactions.WatchObject):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.WatchObject);
                    break;
                }
                /*
                case(PoliceUnitSM.PoliceReactions.PlayerIDed):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.PlayerIDed);
                    break;
                }
                */
                case(PoliceUnitSM.PoliceReactions.PlayerUnIDed):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.PlayerUnIDed);
                    break;
                }
                case(PoliceUnitSM.PoliceReactions.PlayerUntracked):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.PlayerUntracked);
                    break;
                }
                case(PoliceUnitSM.PoliceReactions.PlayerDodged):
                {
                    SetEmote(PoliceUnitSM.PoliceReactions.PlayerDodged);
                    break;
                }
            }
        }
        catch(Exception)
        {
            ShowDefaultEmote();
        }
    }

    private void SetEmote(PoliceUnitSM.PoliceReactions reaction)
    {
        EmoteAllocation allocation = _emotesAllocationCollection.First(_ => _.PoliceReaction == reaction);
        _image.sprite = allocation.EmoteSprite;
        _image.color = allocation.EmoteColor;
    }

    private void EnableAnimatorEvent()
    {
        _animator.fireEvents = true;
    }

    private void DisableAnimatorEvent()
    {
        _animator.fireEvents = false;
    }

    private void EnableAnimator()
    {
        _animator.enabled = true;
    }

    private void DisableAnimator()
    {
        _animator.enabled = false;
    }

    //this method is called by the animation at the end of the Pop animation
    private void ShowDefaultEmote()
    {
        _image.sprite = _defaultSprite;

        if(PoliceResponseManager.Instance.IsPlayerIdentified())
        {
            _image.color = Color.red;
        }
        else
        {
            _image.color = _defaultColor;
        }

        DisableAnimator();
    }
}
