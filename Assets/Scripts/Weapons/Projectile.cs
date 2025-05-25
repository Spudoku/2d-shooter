using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Bullet
{
    public bool isFX;       // if true, does not use its collider, deal damage, etc
    private Vector3 startPoint;
    private Vector3 endPoint;
    private GameObject target;

    [SerializeField] private float speed;       // speed in units per second
    [SerializeField] private GameObject bulletPrefab;

    public override void Fire(float angleVari,GameObject t)
    {
        Fire(angleVari,transform.right,t);
    }

    public override void Fire(float angleVari, Vector3 startDir,GameObject t)
    {

        
        
        GameObject bullet = Instantiate(bulletPrefab);
        ProjectileGO projectileGO = bullet.GetComponent<ProjectileGO>();
        Vector3 trajectory = BulletAngle.GetProjectileTrajectory(startDir, angleVari);
        Debug.DrawLine(transform.position, transform.position + trajectory.normalized * 1000,Color.red,10f);
        Vector3 end = transform.position + trajectory.normalized * 100000f;
        projectileGO.InitProj(speed,transform.position,end,false,damage);
        projectileGO.SetTarget(t);
        //bullet.transform.right += transform.position;
        bullet.transform.right = trajectory.normalized;
        /*if (bullet.TryGetComponent<Explosion>(out var explosion))
        {
            
        }*/

        
    }

    // this overloaded version of Fire is intended only for FX bullets.
    public void Fire(float angleVari, Vector3 start, Vector3 end)
    {
        if (!isFX)
        {
            Fire(angleVari,null);
        } else
        {
            Debug.Log($"Projectile: using overloaded Fire() method");
            GameObject bullet = Instantiate(bulletPrefab);
            ProjectileGO projClass= bullet.GetComponent<ProjectileGO>();
            projClass.InitProj(speed, start, end,isFX,damage);
            projClass.SetTarget(target);
            projClass.type = this;
        }


    }


    public override void Hit(Vector3 origin, Vector3 hit)
    {
        if (!isFX)
        {
            GameObject fX = Instantiate(hitFX);
            fX.transform.position = hit;
        }
        
    }

    // non-FX version
    private void LaunchBullet(Vector3 start, float angle)
    {
        

    }

    private void LaunchBullet(Vector3 start, float angle, float time)
    {

    }

    private GameObject InitBullet() {
        GameObject bullet = Instantiate(bulletPrefab);
        // set rotation, speed, etc

        return bullet;
    }

    public void SetTarget(GameObject t)
    {
        target = t;
    }
    
    public GameObject GetPrefab()
    {
        return bulletPrefab;
    }
}
