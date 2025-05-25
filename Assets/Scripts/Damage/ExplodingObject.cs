using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Explosion))]
public class ExplodingObject : DamageableObject
{
    private Explosion explosion;
    private bool exploded;
    private new void Awake()
    {
        exploded = false;
        base.Awake();
        explosion = GetComponent<Explosion>();
    }
    protected override void LethalFX()
    {
        Debug.Log($"Exploding object: exploding from death!");
        if (!exploded)
        {
            exploded=true;
            explosion.Explode(transform.position);
        }
        
        Destroy(gameObject);
    }
}
