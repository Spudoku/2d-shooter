using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(TargetFinder))]
public class SentryGeneric : MonoBehaviour
{
    [SerializeField] private float searchRange = 30f;       // in units/meters
    [SerializeField] private float turnSpeed = 30f;         // in degrees/second
    [SerializeField] private Weapon weapon;                 // 
    private Bullet bullet;
    [SerializeField] private Transform firePoint;           // 

    [SerializeField] private string[] targetTags;                   // the tags to attack
    private TargetFinder targetFinder;
    [SerializeField]  private GameObject target;
    [SerializeField] private LineOfSight los;

    private int rotateDir = 0;
    private float rotateTimeTrack = 0;
    private const float ROTATE_DIR_TIME = 5f;
    private void Awake()
    {
        los.lookRange = searchRange;
        targetFinder  = GetComponent<TargetFinder>();
        targetFinder.searchRange = searchRange;
        targetFinder.targetTags = targetTags;

        bullet = weapon.GetComponent<Bullet>();
    }

    // Update is called once per frame
    void Update()
    {
        
        target = targetFinder.target;
        if (target != null)
        {
            rotateDir = 0;
            rotateTimeTrack = 0;
            TryToAttackTarget(target);
        } else
        {
            // rotate randomly
            if (rotateDir == 0 || rotateTimeTrack == 0)
            {
                rotateTimeTrack = ROTATE_DIR_TIME;
                rotateDir = RNG.RandBool() ? 1 : -1;
            } else
            {
                rotateTimeTrack -= Time.deltaTime;
                transform.Rotate(0,0,rotateDir * 0.5f * turnSpeed * Time.deltaTime);
            }
        }
    }

    

    private void TryToAttackTarget(GameObject t)
    {
        if (los.lookGO != null && los.lookGO.Equals(t))
        {
            // attack!
            //Debug.Log("Sentry: Attacking!");
            if (weapon != null)
            {
                if (weapon.GetType() == typeof(MagBased))
                {
                    MagBased magBased = (MagBased)weapon;
                    if (!magBased.isReloading)
                    {
                        if (magBased.ammoLoaded >= 1)
                        {
                            //Debug.Log("Sentry: Shooting!");
                            if (bullet is Projectile proj && proj.GetPrefab().GetComponent<ProjectileGO>().followTarget)
                            {
                                proj.SetTarget(t);
                                magBased.Fire(t);
                            }
                            else
                            {
                                magBased.Fire();
                            }

                        }
                        else
                        {
                            //Debug.Log("Sentry: Reloading!");
                            StartCoroutine(ReloadWeapon(magBased, magBased.GetReloadTime()));

                        }
                    }
                   
                    
                }
                
                
            }
        }
        else
        {
            // rotate towards target
            //Debug.Log("Sentry: Rotating!");

            //transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(t.transform.position),turnSpeed * Time.deltaTime);
            float angle = Mathf.Atan2(t.transform.position.y - transform.position.y, t.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    public IEnumerator ReloadWeapon(Weapon weapon, float time)
    {
        weapon.ReloadInit();

        yield return new WaitForSeconds(time);

        weapon.Reload();
    }

}
