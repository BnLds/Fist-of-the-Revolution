using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PoliceFlowfieldsGenerator : MonoBehaviour
{
    //public static PoliceFlowfieldsGenerator Instance { get; private set; }

    private void Awake()
    {
        /*if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        */
    }

    public FlowField CreateNewFlowField(Transform target)
    {
        return GridController.Instance.GenerateFlowField(target);
    }
}