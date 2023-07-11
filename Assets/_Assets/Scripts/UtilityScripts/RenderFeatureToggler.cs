using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
 
[System.Serializable]
public struct RenderFeatureToggle
{
    public ScriptableRendererFeature feature;
    public bool isEnabled;
}
 
[ExecuteAlways]
public class RenderFeatureToggler : MonoBehaviour
{
    [SerializeField]
    private List<RenderFeatureToggle> renderFeatures = new List<RenderFeatureToggle>();
    [SerializeField]
    private UniversalRenderPipelineAsset pipelineAsset;
 
    private void Start()
    {

        PoliceResponseManager.Instance?.OnPlayerIdentified.AddListener(PoliceResponseManager_OnPlayerIdentified);
        PoliceResponseManager.Instance?.OnPlayerNotIDedAnymore.AddListener(PoliceResponseManager_OnPlayerNotIDedAnymore);

        foreach (RenderFeatureToggle toggleObj in renderFeatures)
        {
            toggleObj.feature.SetActive(toggleObj.isEnabled);
        }
    }

    private void OnDisable()
    {
        foreach (RenderFeatureToggle toggleObj in renderFeatures)
        {
            toggleObj.feature.SetActive(false);
        }
    }

    private void PoliceResponseManager_OnPlayerIdentified()
    {
        foreach (RenderFeatureToggle toggleObj in renderFeatures)
        {
            if(toggleObj.feature.name == "Blit")
            {
                toggleObj.feature.SetActive(true);
                return;
            }
        }
    }

    private void PoliceResponseManager_OnPlayerNotIDedAnymore(Transform arg0)
    {
        foreach (RenderFeatureToggle toggleObj in renderFeatures)
        {
            if(toggleObj.feature.name == "Blit")
            {
                toggleObj.feature.SetActive(false);
                return;
            }
        }
    }
}