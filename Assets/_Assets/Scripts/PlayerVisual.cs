using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private List<SkinSO> _skinSOs;

    private MeshRenderer _meshRenderer;
    private SkinSO _currentSkin;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _currentSkin = _skinSOs[0];
        _meshRenderer.material = _currentSkin.skinMaterial;
    }

    public SkinSO GetSkinSO()
    {
        return _currentSkin;
    }
}
