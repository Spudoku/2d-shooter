using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] private float hpToRestore;
    [SerializeField] private bool usePercentage;            // if true, restore hp based on percentage
    protected override void PickupPU(GameObject go)
    {
        DamageableObject DO = go.GetComponent<DamageableObject>();
        
        if (usePercentage)
        {
            DO.ChangeHP((DO.maxHP * hpToRestore) / 100f);
        }
        else
        {
            DO.ChangeHP(hpToRestore);
        }
    }

   
}
