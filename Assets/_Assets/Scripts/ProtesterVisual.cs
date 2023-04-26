using UnityEngine;

public class ProtesterVisual : MonoBehaviour
{
    [SerializeField] private LayerMask _floorMask;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        float floorDetectionDistance = 2f;
        if(!Physics.Raycast(transform.position, Vector3.down, floorDetectionDistance, _floorMask))
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    

    public void Show()
    {
        _meshRenderer.enabled = true;
    }

    public void Hide()
    {
        _meshRenderer.enabled = false;
    }
}
