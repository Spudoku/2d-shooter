using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
//[RequireComponent(typeof(Weapon))]


// Stores weapons, and allows swapping between weapons as they are unlocked
public class WeaponSwapper : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int indexToUnlock = 0;
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private List<Weapon> unlockedWeapons;   
    [SerializeField] private int startIndex;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource swapSound;
    private int count;
    [SerializeField] private int swapIndex;
    
    
    //private int index;

    private int unlockedCount;
    void Start()
    {
        if (indexToUnlock >= weapons.Length)
        {
            indexToUnlock = weapons.Length - 1;
        }
        //UnlockWeapon(0);        // always have 1 weapon unlocked
        
        swapIndex = startIndex;
        weapons[swapIndex].enabled = true;
        foreach (var weapon in weapons)
        {
            if (weapon.gameObject.TryGetComponent<LaserPointer>(out var lp))
            {
                lp.isOn = false;
            }
            weapon.gameObject.SetActive(false);
            //weapon.SwapReloadCancel();
        }

        for (int i = 0; i <= indexToUnlock; i++)
        {
            UnlockWeapon(i);
        }
        ChangeSelectedIndex(0);
        
    }

    // calculates the new selected weapon and swaps to it
    public void ChangeSelectedIndex(int amtToChange)
    {
        count = unlockedWeapons.Count;
        Debug.Log($"Weapon swapper: trying to swap weapon (amtToChange: {amtToChange})");
        if (count == 1)
        {
            //Weapon current = weapons[0];
            swapIndex = 0;
            //Debug.Log($"{current.weaponName} is the only weapon available!");

        } else if (amtToChange != 0) {
            
            // disable all unlocked weapons
            foreach (var weapon in unlockedWeapons)
            {
                if (weapon.gameObject.TryGetComponent<LaserPointer>(out var lp))
                {
                    lp.isOn = false;
                }
                
                weapon.SwapFrom();
            }

            swapIndex += amtToChange;

            // wrapping around to 0 or to highest index
            if (swapIndex >= count)

            {
                swapIndex = 0;
            }
            else if (swapIndex < 0)
            {
                swapIndex = count - 1;
            }

            
        }
        // swapping to the newly selected weapon
        Weapon newWeapon = unlockedWeapons[swapIndex];
        Debug.Log($"Swapped to {newWeapon.weaponName} (swapIndex = {swapIndex})");
        //swapSound.Play();
        //newWeapon.SwapTo();

        newWeapon.SwapTo();
    }
    public Weapon GetSelected()
    {
        Debug.Log($"weapon swapper: getting selected weapon {unlockedWeapons[swapIndex]}");
        return unlockedWeapons[swapIndex];
    }

    // appends the weapon from the weapon list to unlockedWeapons
    public void UnlockWeapon(int index)
    {
        // making sure the weapon isn't already in unlockedWeapons
        if (index < 0)
        {
            Debug.Log($"unlock weapon attempt failed: invalid index {index}");
            return;
        } 
        Weapon weapon = weapons[index];
        if (unlockedWeapons.Contains(weapon))
        {
            Debug.Log($"Weapon index {index} ({weapon.weaponName}) is already unlocked!");
        }
        else
        {
            unlockedWeapons.Add(weapon);
            Debug.Log($"Weapon index {index} ({weapon.weaponName}) is now unlocked!");
        }
    }

    public int IndexOf(string search)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].weaponName.Equals(search))
            {
                return i;
            }
        }
        return -1;
    }
}
