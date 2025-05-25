using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{
    [SerializeField] private float refillPerc;      // percentage of ammo to refill (on ALL weapons carried by recipient)
    protected override void PickupPU(GameObject go)
    {
        Weapon[] weapons = go.GetComponentsInChildren<Weapon>(true);
        Debug.Log($"GameObject {go} has the following weapons: {weapons}; total {weapons.Length}");
        foreach (var weapon in weapons) {
            weapon.RefillAmmo(refillPerc);
        }
    }

    
}
