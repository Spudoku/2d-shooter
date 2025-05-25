using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    // bullets can be either hitscan or projectile based.
    // factors such as size are important; based on collider size
    [SerializeField] protected float damage;       // 100 is base hp of player
    [SerializeField] protected float scale;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected float range;
    [SerializeField] protected float width;
    [SerializeField] protected GameObject hitFX;
    protected void Awake()
    {
        layerMask = LayerMask.GetMask(new string[] { "Damageable", "Terrain" ,"Structure"});
    }
    public abstract void Fire(float angleVari, GameObject target);

    public abstract void Fire(float angleVari, Vector3 startDir,GameObject target);

    public abstract void Hit(Vector3 origin,Vector3 hit);

    
}
