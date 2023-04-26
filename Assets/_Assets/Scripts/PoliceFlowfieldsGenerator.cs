using UnityEngine;

public class PoliceFlowfieldsGenerator : MonoBehaviour
{
    public FlowField CreateNewFlowField(Transform target)
    {
        return GridController.Instance.GenerateFlowField(target);
    }
}