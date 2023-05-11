using System.Collections.Generic;
using UnityEngine;

public class ProtesterVisual : MonoBehaviour
{
    [Header("Initialization Parameters")]
    [SerializeField] private ProtesterData _protesterData;
    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private List<SkinSO> _skinSOs;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        int randomIndex = Random.Range(0, _skinSOs.Count);
        _meshRenderer.material = _skinSOs[randomIndex].skinMaterial;
        _protesterData.Skin = _skinSOs[randomIndex];
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
