using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // essential functions: 
    // * store ammo, magazine size
    // * either create a projectile or use hitscan
    // * reloading functions

    // ammunition
     public int magSize;
     public int ammoCarried;
     public int ammoLoaded;
     public int maxAmmo;

    // dps
    [SerializeField] private float fireRate = 1;            // measured in rounds per second
    protected float fireDelay;
    public float delayToNextShot;
    [SerializeField] protected float reloadTime = 1;
    [SerializeField] protected float reloadTimeTrack = 0;
    public bool isReloading = false;

    // bullet and recoil
    [SerializeField] protected Bullet bullet;           // the bullet will control factors such as projectile speed (or hitscan), maybe damage too?

    [SerializeField] protected float minSpread = 0;         // minimum 'spread' of this weapon in degrees
    public float curSpread = 0;         // current 'spread' of the weapon in degrees
    [SerializeField] protected float maxSpread = 1;          // maximum 'spread' of this weapon in degrees
    [SerializeField] protected float deltaSpread = 0.5f;       // change in spread in degrees per shot fired
    [SerializeField] protected float spreadRecover = 0.5f;     // how long it takes to reduce spread from max to min in seconds
    

    [SerializeField] protected int bulletsPerShot = 1;
    //[SerializeField] protected bool isFixedCone = false;          // only set to true for shotguns. If true, multiple bullets will clump together in a fixed cone
    //[SerializeField] protected float coneWidth = 0f;
    [SerializeField] protected bool isFixedPattern = false;

    public float aimRange = 15f;                // how far away from the camera this weapon will allow the user to move their mouse for aiming

    [SerializeField] Vector2 offset;                    // offsets position relative to playerMover object
    

    // effects
    [SerializeField] protected Sprite sprite;
    [SerializeField] protected float genericVolume = 0.5f;
    [SerializeField] protected RandomSFX fireClips;
    [SerializeField] protected RandomSFX reloadClips;
    [SerializeField] protected RandomSFX emptyClips;

    // other
    public string weaponName;

    protected void Awake()
    {
        
        // set default recoil values
        if (minSpread < 0)
        {
            minSpread = 0;
        }
        if (maxSpread < minSpread)
        {
            maxSpread = minSpread + 1f;
        }
        if (deltaSpread <= 0)
        {
            deltaSpread = 1f;
        }
        if (spreadRecover <= 0)
        {
            spreadRecover = 1f;
        }
        curSpread = minSpread;

        if (bulletsPerShot < 1)
        {
            bulletsPerShot = 1;
        }

        ammoLoaded = magSize;
        maxAmmo = ammoCarried;
        if (TryGetComponent<LaserPointer>(out var lp))
        {
            Debug.Log("Laser pointer found!");
            minSpread *= lp.spreadMultiplier;
            maxSpread *= lp.spreadMultiplier;
        }
    }

    public abstract void Fire();

    public abstract void ReloadInit();
    public abstract void Reload();

    /*public abstract void CancelReload();

    public abstract void SwapReloadCancel();
*/
    public abstract void SwapTo();
    public abstract void SwapFrom();
    protected void Update()
    {
        if (isActiveAndEnabled)
        {
            if (isReloading)
            {
                reloadTimeTrack += Time.deltaTime;
                if (reloadTimeTrack > reloadTime)
                {
                    Reload();
                }
            } else
            {
                delayToNextShot -= Time.deltaTime;
                if (delayToNextShot < 0)
                {
                    delayToNextShot = 0;
                }
            }
            
            
            
        } else
        {
            if (delayToNextShot > 0)
            {
                delayToNextShot = fireDelay;
            }
        }

        // update position using offset every frame
        //transform.localPosition = new Vector3(0 + offset.x,0 + offset.y, 0);
        CalcSpread();
    }
    protected void CalcFireDelay()
    {
        fireDelay = 1f / fireRate;
        delayToNextShot = 0f;
    }

    protected void ChangeFireDelay(float change)
    {
        delayToNextShot += change;
    }

    // calculate the spread of this weapon, every frame
    protected void CalcSpread()
    {
        if (curSpread < minSpread)
        {
            ResetSpread();
        } else if (curSpread > maxSpread)
        {
            curSpread = maxSpread;
        }
        curSpread -= ((maxSpread - minSpread) / spreadRecover) * Time.deltaTime;
    }

    protected void AddSpreadFromShot()
    {
        curSpread += deltaSpread;
    }

    protected void ResetSpread()
    {
        curSpread = minSpread;
    }
    public void RefillAmmo(float percent)
    {
        ammoCarried += (int)((maxAmmo * percent) / (100f));
        if (ammoCarried > maxAmmo)
        {
            ammoCarried = maxAmmo;
        }
    }

    public Bullet GetBullet()
    {
        return bullet;
    }
}
