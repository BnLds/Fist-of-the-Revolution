using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    //target seeking behaviour data 
    public List<Transform> Targets = null;
    public Collider[] Obstacles = null;
    public Transform CurrentTarget;
    public bool IsChasingTarget = false;

    public int GetTargetsCount() => Targets == null ? 0 : Targets.Count;
}
