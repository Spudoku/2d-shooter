using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Not to be confused with Projectile. This class is to be attached to 
// GameObjects used as bullets.
[RequireComponent(typeof(Collider2D))]
public class ProjectileGO : MonoBehaviour
{
    public float speed;
    public float angle;         // euler angle in degrees; to be rotated around z axis
    // to be passed from the Projectile instance that instantiates this GO
    public Vector3 start;
    public Vector3 end;
    public GameObject target = null;
    public bool followTarget = false;
    public float rotationSpeed = 100f;      // in degrees/second

    private float step;
    public Projectile type;
    public bool isFX;

    private float damage;
    private Explosion explosion;

    protected void Awake()
    {
        explosion = GetComponent<Explosion>();
    }
    public void InitProj(float speed, Vector3 start,Vector3 end, bool isFX, float damage)
    {
        this.speed = speed;
        
        this.start = start;
        this.end = end;
        //this.target = target;
        this.isFX = isFX;
        transform.position = start;
        transform.right = end - start;
        this.damage = damage;
    }
    public void InitProj(float speed, Vector3 start, Vector3 end, float damage)
    {
        this.speed = speed;

        this.start = start;
        this.end = end;
        target = null;
        isFX = false;
        transform.position = start;
        //transform.right = rightDir;
        this.damage = damage;
    }
    //
    protected void Update()
    {
        step = speed * Time.deltaTime;
        if (target != null && followTarget)
        {
            Debug.Log($"Following target {target}");
            // rotate towards the target, then move forward
            end = target.transform.position;
        }
        else if(isFX)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, step);
            if (Vector3.Distance(transform.position, end) < step)
            {
                //Debug.Log("Reached destination!");
                type.Hit(Vector3.zero, Vector3.zero);
                Destroy(gameObject);
            }
        }
        else
        {
            //transform.Translate(transform.forward * step);
            //transform.Translate(transform.right * step);
            //Debug.DrawLine(start, transform.position,Color.magenta,5f) ;
            transform.position = Vector3.MoveTowards(transform.position, end, step);
        }





    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFX)
        {
            //type.Hit(start,transform.position);
            if (collision.gameObject.TryGetComponent<DamageableObject>(out var hitDO))
            {
                hitDO.ChangeHP(-damage);
                Debug.Log(name +$"hit {hitDO} for {damage} damage");
            }
            if (explosion != null)
            {
                Debug.Log($"Projectile: exploding from trigger enter!");
                explosion.Explode(transform.position);
            }
            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject t)
    {
        target = t;
    }
}
