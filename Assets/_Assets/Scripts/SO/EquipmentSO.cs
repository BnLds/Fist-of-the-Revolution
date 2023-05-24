using UnityEngine;

[CreateAssetMenu()]
public class EquipmentSO : ScriptableObject
{
    public string EquipmentName;
    public Transform EquipmentPrefab;
    public Transform DisplayPrefab;
    public int FlatBonusDamage;
    public float IncreasePercentMoveSpeed;
    public float ReductionPercentHideTime;
    public float IncreasePercentAttackRange;
    public float IncreasePercentAttackSpeed;
}
