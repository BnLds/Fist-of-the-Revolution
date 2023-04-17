using UnityEngine;

public class ProtesterManager : MonoBehaviour
{
    [SerializeField] private LayerMask floorMask;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        Hide();
    }

    private void Update()
    {
        float floorDetectionDistance = 2f;
        if(Physics.Raycast(transform.position, Vector3.down, floorDetectionDistance, floorMask))
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        meshRenderer.enabled = true;
    }

    private void Hide()
    {
        meshRenderer.enabled = false;
    }
}
