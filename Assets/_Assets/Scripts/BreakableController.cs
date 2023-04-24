using UnityEngine;

public class BreakableController : MonoBehaviour
{

    [SerializeField] private int health = 3;
    [SerializeField] private int watchValue = 1;

    public void Damage()
    {
        if(health > 0) health -= 1;
    }

    private void Update()
    {
        if(health == 0) Destroy(gameObject);
    }
}
