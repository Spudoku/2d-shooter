using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HitScan : Bullet
{

    //[SerializeField] private HitscanBulletFX hitscanFX;
    [SerializeField] private Projectile fxBullet;           // this GO must have a ProjectileGO object
    

    public override void Fire(float angleVari, GameObject target)
    {
        Fire(angleVari, transform.right, null);
        
    }

    public override void Fire(float angleVari,Vector3 startDir, GameObject target)
    {
        //float angle = transform.rotation.eulerAngles.z;
        Vector3 trajectory = BulletAngle.GetTrajectory(startDir, angleVari);

        //Debug.DrawRay(transform.position,trajectory * range, Color.red,3.0f, false);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, trajectory, range,layerMask);
        Vector2 endPoint;
        //
        if (hit.collider != null)
        {
            //Debug.Log("HIT " + hit.collider.gameObject.name);
            //Debug.Log("Start point: " + transform.position + "; Hit point: " + hit.point);
            endPoint = hit.point;
            Hit(transform.position, endPoint);

            if (hit.collider.gameObject.TryGetComponent<DamageableObject>(out var hitDO))
            {
                hitDO.ChangeHP(-damage);
            }

        }
        else
        {
            //Debug.Log("Did not hit anything!");
            endPoint = new Ray(transform.position, trajectory).GetPoint(range);
        }
        if (fxBullet != null)
        {
            fxBullet.Fire(angleVari, transform.position, endPoint);
        }
    }

    public override void Hit(Vector3 origin, Vector3 hit)
    {
        if (fxBullet == null)
        {
            
        }
        GameObject fX = Instantiate(hitFX);
        fX.transform.position = hit;

    }
}
