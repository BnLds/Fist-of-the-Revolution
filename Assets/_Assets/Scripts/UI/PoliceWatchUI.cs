using UnityEngine;
using System.Collections.Generic;

public class PoliceWatchUI : MonoBehaviour
{
    [SerializeField] private List<StarUI> _stars;

    public void DisplayWatchValue(int watchValue)
    {
        foreach(StarUI star in _stars)
        {
            star.UpdateSprite(watchValue);
        }
    }


}
