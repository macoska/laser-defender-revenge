using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    // configuration parameters
    [SerializeField] int damage = 100;
    [SerializeField] bool destroyWhenHit = true;

    public int GetDamage() => damage;

    public void Hit()
    {
        if (destroyWhenHit)
            Destroy(gameObject);
    }

}
