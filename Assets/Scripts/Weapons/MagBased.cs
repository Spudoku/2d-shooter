using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class MagBased : Weapon
{
    public bool isAutomatic;
    public int prevAmmoLoaded;
    // Start is called before the first frame update
    protected new void Awake()
    {
        base.Awake();
        CalcFireDelay();
        ammoLoaded = magSize;
    }

    

    public override void Fire()
    {

        Fire(null);
    }

    public void Fire(GameObject target)
    {
        if (target != null)
        {
            Debug.Log($"Firing at {target}!");
        }
        if (delayToNextShot > 0)
        {
            // nothing happens?

        }
        else if (ammoLoaded < 1)
        {
            // empty gun sound

        }
        else
        {
            if (bulletsPerShot > 1)
            {
                if (isFixedPattern)
                {
                    FireInFixedPattern(curSpread, bulletsPerShot);
                }
                else
                {
                    // calculate a 'center direction', then 
                    Vector3 startDir = BulletAngle.GetTrajectory(transform.right, 0f);
                    for (int i = 0; i < bulletsPerShot; i++)
                    {
                        bullet.Fire(curSpread, startDir,target);
                    }
                }


            }
            else
            {
                bullet.Fire(curSpread,target);
            }


            delayToNextShot = fireDelay;
            ammoLoaded--;
            AddSpreadFromShot();
            AudioSource.PlayClipAtPoint(fireClips.GetRandomClip(), transform.position, genericVolume);

        }
    }

    public float GetReloadTime()
    {
        return reloadTime;
    }
    public override void ReloadInit()
    {
        // inital effects

        // manipulate ammo
        prevAmmoLoaded = ammoLoaded;
        ammoCarried += ammoLoaded;
        ammoLoaded = 0;
        isReloading = true;
        reloadTimeTrack = 0;
        Debug.Log("Initialized reload of " + weaponName);
    }
    public override void Reload()
    {
        if (ammoCarried < magSize)
        {
            ammoLoaded = ammoCarried;
            ammoCarried = 0;
        } else
        {
            ammoLoaded = magSize;
            ammoCarried -= magSize;
        }

        // reload SFX, etc
        ResetSpread();
        isReloading = false;
        //Debug.Log("Finished reload of " + weaponName);
    }

    
    public override void SwapTo()
    {
        Debug.Log($"Swapping to {weaponName}");
        gameObject.SetActive(true);
    }
    public override void SwapFrom()
    {
        reloadTimeTrack = 0;
        Debug.Log($"Swapping from {weaponName}");
        gameObject.SetActive(false);
    }

    
    private void FireInFixedPattern(float angleWidth, int count)
    {
        if (count <= 0)
        {
            throw new UnityException("count must be greater than 0");
        }
        Vector3 dir = BulletAngle.RotateVector(transform.right,-angleWidth);
        float rotAmt = (2f * angleWidth) / count;
        for (int i = 0; i < count; i++)
        {
            bullet.Fire(0,dir,null);
            dir = BulletAngle.RotateVector(dir,rotAmt);
        }
    }
}
