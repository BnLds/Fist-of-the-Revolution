using UnityEngine;

public class PolicePlayerDetectedUI : MonoBehaviour
{
    [SerializeField] private PoliceUnitSM _policeUnitSM;

    private void Start()
    {
        _policeUnitSM.OnPlayerIDed.AddListener(Show);

        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    //this method is called by the animation at the end of the Pop animation
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
