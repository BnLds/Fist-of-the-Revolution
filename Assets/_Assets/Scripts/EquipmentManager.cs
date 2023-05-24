using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private List<Transform> _equipmentsSkinsList;

    private void Awake()
    {
        foreach(Transform equipment in _equipmentsSkinsList)
        {
            equipment.gameObject.SetActive(false);
        }
    }

    public void Equip(Transform equipmentDisplay)
    {
        foreach(Transform equipment in _equipmentsSkinsList)
        {
            if(equipment.GetComponent<EquipmentData>().GetEquipmentSO().EquipmentName == equipmentDisplay.GetComponent<EquipmentData>().GetEquipmentSO().EquipmentName)
            {
                equipment.gameObject.SetActive(true);
                ApplyEquipmentEffects(equipment.GetComponent<EquipmentData>().GetEquipmentSO());
                break;
            }
        }
    }

    private void ApplyEquipmentEffects(EquipmentSO equipmentSO)
    {
        _playerController.IncreaseFlatAttackDamage(equipmentSO.FlatBonusDamage);
        _playerController.IncreaseAttackRange(equipmentSO.IncreasePercentAttackRange);
        _playerController.IncreaseAttackSpeed(equipmentSO.IncreasePercentAttackSpeed);
        _playerController.IncreaseMoveSpeed(equipmentSO.IncreasePercentMoveSpeed);
        _playerController.ReduceHideTime(equipmentSO.ReductionPercentHideTime);
    }
}
