using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour
{
    public enum StarStatus
    {
        EmptyStar,
        FullStar
    }

    [SerializeField] private Sprite _emptyStar;
    [SerializeField] private Sprite _fullStar;

    private int _layoutIndex;

    private void Awake()
    {
        SetSprite(StarStatus.EmptyStar);
    }

    private void Start()
    {
        _layoutIndex = transform.GetSiblingIndex();
    }

    private void SetSprite(StarStatus starStatus)
    {
        switch(starStatus)
        {
            case(StarStatus.EmptyStar):
            {
                GetComponent<Image>().sprite = _emptyStar;
                break;
            }

            case(StarStatus.FullStar):
            {
                GetComponent<Image>().sprite = _fullStar;
                break;
            }
        }
    }

    public void UpdateSprite(int watchValue)
    {
        if(_layoutIndex < watchValue)
        {
            SetSprite(StarStatus.FullStar);
        }
        else
        {
            SetSprite(StarStatus.EmptyStar);
        }
    }



}
