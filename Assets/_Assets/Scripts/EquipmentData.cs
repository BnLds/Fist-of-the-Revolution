using UnityEngine;

public class EquipmentData : MonoBehaviour
{
    [SerializeField] private EquipmentSO _equipmentSO;

    public EquipmentSO GetEquipmentSO()
    {
        return _equipmentSO;
    }
}
