using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour
{
    public enum StarStatus
    {
        EmptyStar,
        FullStar
    }

    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;

    private int layoutIndex;

    private void Awake()
    {
        SetSprite(StarStatus.EmptyStar);
    }

    private void Start()
    {
        layoutIndex = transform.GetSiblingIndex();
    }

    private void SetSprite(StarStatus starStatus)
    {
        switch(starStatus)
        {
            case(StarStatus.EmptyStar):
            {
                GetComponent<Image>().sprite = emptyStar;
                break;
            }

            case(StarStatus.FullStar):
            {
                GetComponent<Image>().sprite = fullStar;
                break;
            }
        }
    }

    public void UpdateSprite(int watchValue)
    {
        if(layoutIndex < watchValue)
        {
            SetSprite(StarStatus.FullStar);
        }
        else
        {
            SetSprite(StarStatus.EmptyStar);
        }
    }



}
