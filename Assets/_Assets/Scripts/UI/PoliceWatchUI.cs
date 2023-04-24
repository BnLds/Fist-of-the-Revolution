using UnityEngine;
using System.Collections.Generic;

public class PoliceWatchUI : MonoBehaviour
{
    [SerializeField] private List<StarUI> stars;

    public void DisplayWatchValue(int watchValue)
    {
        foreach(StarUI star in stars)
        {
            star.UpdateSprite(watchValue);
        }
    }


}
