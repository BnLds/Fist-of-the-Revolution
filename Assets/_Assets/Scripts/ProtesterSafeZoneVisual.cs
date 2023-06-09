using UnityEngine;

public class ProtesterSafeZoneVisual : MonoBehaviour
{
    [Header("Initialization Parameters")]
    [SerializeField] private ProtesterSafeZone _protesterSafeZone;
    [SerializeField] private Transform _protesterParent;

    private void Start()
    {
        _protesterSafeZone.OnPlayerEnterSafeZone.AddListener(Show);
        _protesterSafeZone.OnPlayerExitSafeZone.AddListener(Hide);
        _protesterSafeZone.OnPlayerIDedFree.AddListener((Transform sender) => { Hide(); });
        _protesterSafeZone.OnPlayerTrackedFree.AddListener(Hide);

        //ensure the localscale displays the radius of the safe zone
        Vector3 parentScale = _protesterParent.lossyScale;
        Vector3 localRadius = new Vector3(1f/parentScale.x, 0, 1f/parentScale.z)  * _protesterSafeZone.GetSafeZoneRadius();
        transform.localScale = localRadius*2;
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
