using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponPickup : Pickup
{
    public string weaponToGive;                         // must correllate to a valid weapons weaponName!
    //[SerializeField] private float percAmmoToGive = 100f;
    private void Awake()
    {
        isStealable = false;
    }
    protected override void PickupPU(GameObject go)
    {

        WeaponSwapper ws = go.GetComponentInChildren<WeaponSwapper>();
        if (ws != null)
        {
            ws.UnlockWeapon(ws.IndexOf(weaponToGive));
        }
        
    }
}
