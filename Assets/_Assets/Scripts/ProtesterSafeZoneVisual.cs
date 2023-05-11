using UnityEngine;

public class ProtesterSafeZoneVisual : MonoBehaviour
{
    [Header("Initialization Parameters")]
    [SerializeField] private ProtesterSafeZone _protesterSafeZone;

    private void Start()
    {
        _protesterSafeZone.OnPlayerEnterSafeZone.AddListener(Show);
        _protesterSafeZone.OnPlayerExitSafeZone.AddListener(Hide);
        _protesterSafeZone.OnPlayerIDedFree.AddListener(Hide);
        _protesterSafeZone.OnPlayerTrackedFree.AddListener(Hide);

        //ensure the localscale displays the radius of the safe zone
        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(1f/parentScale.x, 0, 1f/parentScale.z)  * _protesterSafeZone.GetSafeZoneRadius();

        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
