using System.Collections.Generic;
using UnityEngine;

public class DestroyedBreakable : MonoBehaviour
{
    private const string FLICKER = "Flicker";

    [SerializeField] private List<Transform> _fallingBlocks;
    [SerializeField] private List<Material> _fallingMaterials;


    private float _timeElapsed;
    private bool _isVisible = true;

    private void OnEnable()
    {
        foreach (Material material in _fallingMaterials)
        {
            Color newColor = material.color;
            newColor.a = 1;
            material.color = newColor;
        }

        _timeElapsed = 0f;

        float flickeringStart = 4f;
        InvokeRepeating(FLICKER, flickeringStart, .2f);
    }

    private void Update()
    {
        _timeElapsed += Time.deltaTime;
        float timeLimit = 6f;
        if(_timeElapsed >= timeLimit)
        {
            Disappear();
            CancelInvoke();
            _isVisible = true;
        }
    }

    private void Disappear()
    {
        for (int i = 0; i < _fallingBlocks.Count; i++)
        {
            if(_fallingBlocks[i] != null) Destroy(_fallingBlocks[i].gameObject);
        }
    }

    private void Flicker()
    {
        foreach(Material material in _fallingMaterials)
        {
            Color newColor = new Color();
            newColor = material.color;

            if(_isVisible)
            {
                newColor.a = 0;
                material.color = newColor;
            }
            else
            {
                newColor.a = 1;
                material.color = newColor;
            }
        }
        _isVisible = !_isVisible;
    }
}
