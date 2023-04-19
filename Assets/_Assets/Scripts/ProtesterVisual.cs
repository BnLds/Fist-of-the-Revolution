using UnityEngine;

public class ProtesterVisual : MonoBehaviour
{
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private bool showVisualOutsideFloor = false;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        float floorDetectionDistance = 2f;
        if(!Physics.Raycast(transform.position, Vector3.down, floorDetectionDistance, floorMask) && !showVisualOutsideFloor)
        {
            Hide();
        }
    }
    

    public void Show()
    {
        meshRenderer.enabled = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
