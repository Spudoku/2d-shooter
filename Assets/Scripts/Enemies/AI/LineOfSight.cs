using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

// 2d line-of-sight utility class;
public class LineOfSight : MonoBehaviour
{
    public Vector2 lookPoint;
    public Vector2 leftPoint;
    public Vector2 rightPoint;
    public GameObject lookGO;
    public float lookRange;
    public Vector2 normalLeft;
    public Vector2 normalRight;
    public bool isLookingAtTarget;
    public float fovCone;
    public LayerMask layerMask ;
    public LayerMask pathingLM;

    private void Awake()
    {
        layerMask = LayerMask.GetMask(new string[] { "Damageable", "Terrain","Structure" });
        pathingLM = LayerMask.GetMask(new string[] { "Damageable" });
    }
    void Update()
    {
        //Debug.DrawRay(transform.position, transform.right * lookRange, Color.red, 0.1f, false);
        Vector2 originPoint = transform.position + 0.75f * transform.localScale.x * transform.right.normalized;
        RaycastHit2D hit = Physics2D.Raycast(originPoint, transform.right,lookRange,layerMask);
        //Debug.DrawRay(originPoint, transform.right, Color.cyan);
        //Debug.DrawRay(transform.position + 0.75f * transform.localScale.x * Vector2.Normal, transform.forward, Color.red);
        Vector2 normal = originPoint - lookPoint ;
        normalLeft = new Vector2(normal.y, -normal.x);
        normalRight = new Vector2(-normal.y, normal.x);
        //Debug.DrawRay(originPoint, normalLeft,Color.red);
        //Debug.DrawRay(originPoint, normalRight, Color.green);
        if (hit)
        {
            //Debug.Log("Line of sight check (" + gameObject.name + "): hit something at " + hit.point);
            lookPoint = hit.point;
        }
        else
        {
            lookPoint = transform.position + lookRange * transform.right.normalized;
            //Debug.Log("Line of sight check (" + gameObject.name + "): empty raycast!");
        }
        
        if (hit.collider != null)
        {
            lookGO = hit.collider.gameObject;

        } else
        {
            lookGO = null;
        }

        RaycastHit2D hitLeft = Physics2D.Raycast(originPoint, normalLeft, layerMask);
        if (hitLeft)
        {
            leftPoint = hitLeft.point;
        } else
        {
            leftPoint = (Vector2)transform.position + lookRange * normalLeft.normalized;
        }

        RaycastHit2D hitRight = Physics2D.Raycast(originPoint, normalRight, layerMask);
        if (hitLeft)
        {
            rightPoint = hitRight.point;
        }
        else
        {
            rightPoint = (Vector2)transform.position + lookRange * normalRight.normalized;
        }

        //Debug.DrawLine(leftPoint, rightPoint,Color.red,2f);
        //Debug.DrawLine(originPoint, lookPoint, Color.cyan, 5f);
    }

    public bool CanSeeGO(GameObject go, float range)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + 1.1f * transform.localScale.x * (go.transform.position - transform.position).normalized, (go.transform.position - transform.position) + (go.transform.position - transform.position) * transform.localScale.x, range, layerMask);
        if (hit.collider != null && hit.collider.gameObject.Equals(go))
        {
            return true;
        }
        return false;
    }

    public bool CanSeeGO(GameObject go, float range, LayerMask lm)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + 1.1f * transform.localScale.x * (go.transform.position - transform.position).normalized, (go.transform.position - transform.position) + (go.transform.position - transform.position) * transform.localScale.x, range, lm);
        if (hit.collider != null && hit.collider.gameObject.Equals(go))
        {
            return true;
        }
        return false;
    }
    


    // return true if the angle between the current direction (transform.right)
    // and the direction towards go is less than or equal to cone, and can be seen
    public bool CanSeeGOInCone(GameObject go, float cone, float range)
    {
        if (!CanSeeGO(go,range))
        {
            return false;
        }
        //float distance = Vector2.Distance(transform.position, go.transform.position);

        float angle = Vector2.Angle(transform.right,go.transform.position-transform.position);
        //Debug.Log(name + "'s angle from its forward to " + go.name + ": " + angle) ;

        if (angle <= cone)
        {
            return true;
        }
        return false;
    }
    
}
