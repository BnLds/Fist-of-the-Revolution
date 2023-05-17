using UnityEngine;

public class GuidanceTextUI : MonoBehaviour
{
    [SerializeField] private GuidanceUI _guidanceUI;

    public void HideEndAnimation()
    {
        _guidanceUI.HideEndAnimation();
    }
}
