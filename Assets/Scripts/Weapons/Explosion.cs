using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
[RequireComponent(typeof(LineOfSight))]
public class Explosion : MonoBehaviour
{
    [SerializeField] private float exDMG;                               // the damage this explosion deals
    [SerializeField] private float exRAD;                               // the radius of this explosion
    [SerializeField] private bool requireLOS;                           // if true, objects must be 'seen' by the explosion to be damaged
    [SerializeField] private LayerMask lm;
    [SerializeField] private GameObject fxPF;                           // "Effects prefab"
   
    //private Vector2 pos;
    [SerializeField] private AudioClip explodeClip;
    [SerializeField] private LineOfSight los;
    private void Awake()
    {
        InitExplosion();
        los = GetComponent<LineOfSight>();
    }

    public void InitExplosion()
    {
        
        //exDMG = 100f;
        //exRAD = 2f;
        //requireLOS = true;
        lm = LayerMask.GetMask("Terrain", "Damageable");
        //pos  = transform.position;
    }
    /*public void InitExplosion(GameObject go,float dmg,float radius, bool los, Vector2 pos)
    {
        defaultGO = go;
        exDMG = dmg;
        exRAD = radius;
        requireLOS = los;
        this.pos = pos;
    }*/
    public void Explode(Vector2 pos)
    {
        GameObject fx;
        if (fxPF != null)
        {
            fx = Instantiate(fxPF);
        } else
        {
            Debug.LogError("Explosion: fxPF must not be null!",fxPF);
            fx = new GameObject();
            fx.transform.position = pos;
        }
        fx.transform.position = pos;
        fx.transform.localScale = new(exRAD,exRAD,1);
        AudioSource.PlayClipAtPoint(explodeClip, pos);

        Collider2D[] gos = Physics2D.OverlapCircleAll(pos, exRAD,lm);

        foreach (Collider2D g in gos)
        {
            if (!g.Equals(gameObject) && g.TryGetComponent<DamageableObject>(out var DO) )
            {
                if (requireLOS && los.CanSeeGO(g.gameObject, exRAD, lm))
                {
                    DO.ChangeHP(-exDMG);
                    
                } else
                {
                    DO.ChangeHP(-exDMG);
                }
            }
        }
    }
}
