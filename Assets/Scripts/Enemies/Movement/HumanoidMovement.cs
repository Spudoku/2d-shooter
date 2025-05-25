
using System;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(LineOfSight))]
public class HumanoidMovement : MonoBehaviour
{
    public Vector2 destination;
    public float obstacleRange;
    public float maxSpeed;
    public float curSpeed;
    public float turnSpeed;
    public bool isWandering;
    public LineOfSight lineOfSight;
    public bool destinationInitialized = false;
    public float fovCone;

    [SerializeField] private float leftTendency = 0f;                // if leftTendency is above maxLeftTendency/2, turn left. otherwise, turn right
    [SerializeField] private float maxLeftTendency = 20f;
    private bool tendencyIncreasing = true;

    public Vector2 prevPos;

    public void Awake()
    {
        
        lineOfSight = GetComponent<LineOfSight>();
        lineOfSight.fovCone = fovCone;
        destination = FindWanderDestination();
        destinationInitialized = false;
        //destination;
    }
    private void Update()
    {
        if (tendencyIncreasing)
        {
            leftTendency += Time.deltaTime;
        }
        else
        {
            leftTendency -= Time.deltaTime;
        }
        if (leftTendency > maxLeftTendency)
        {
            leftTendency = maxLeftTendency;
            tendencyIncreasing = false;
        } else if (leftTendency < 0f)
        {
            leftTendency = 0f;
            tendencyIncreasing = true;
        }

    }

    private void LateUpdate()
    {
        curSpeed = GetCurSpeed();
        //Debug.Log($"I moved {dist} units this frame! Estimated speed is {dist/Time.deltaTime} units/second");
        prevPos = transform.position;
        
    }
    public void Wander()
    {
        if (!destinationInitialized)
        {
            destination = FindWanderDestination();
            destinationInitialized = true;
        }
        // picking semi-random destinations
        //Debug.Log("Humanoid Movement: distance to destination " + Vector2.Distance(transform.position, destination));
        if (Vector2.Distance(transform.position,destination) > obstacleRange)
        {
            //Debug.Log("Humanoid Movement: moving towards wander destination " + destination);
            
            MoveTowardsPoint(destination,maxSpeed * 0.8f);
        } else
        {
            //Debug.Log("Humanoid Movement: finding new Wander Destination");
            
            destination = FindWanderDestination();
        }
        
        // pick new destination

    }
    

    public void MoveTowardsPoint(Vector2 point, float speed)
    {
        RotateTowardsPoint(point);
        if (IsFacingPoint(point))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, point, step);
        }
    }

    

    public void RotateTowardsPoint(Vector2 point)
    {
        RotateTowardsPoint(point, turnSpeed);
    }
    public void RotateTowardsPoint(Vector2 point, float speed)
    {
        float angle = Mathf.Atan2(point.y - transform.position.y, point.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion pointRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, pointRotation, speed * Time.deltaTime);
    }

    private bool IsFacingPoint(Vector2 point)
    {
        float dot = Vector2.Dot(transform.right, (point - (Vector2)transform.position).normalized);
        if (dot > 0.99f) {
            return true;
        }
        return false;
    }

    public Vector2 FindWanderDestination()
    {
        Vector2 newDestination = transform.position;
        Vector2 originPoint = transform.position + 2f * transform.localScale.x * transform.right.normalized;
        
        //Debug.DrawLine(originPoint,transform.right.normalized * (Vector2.Distance(originPoint, lineOfSight.lookPoint) - transform.localScale.x),Color.red,0.1f);
        //Debug.DrawRay(originPoint, transform.right.normalized * (Vector2.Distance(originPoint, lineOfSight.lookPoint) - transform.localScale.x), Color.blue, 1f);
        //Debug.Log("WanderDestinationCheck: originPoint: " + originPoint + "; patchCheck.collider: " + pathCheck.collider + "; end point: " + (transform.right * (Vector2.Distance(originPoint, lineOfSight.lookPoint) - transform.localScale.x)));
        //Vector2.Distance(transform.position, lineOfSight.lookPoint) > obstacleRange && Vector2.Distance(transform.position, lineOfSight.leftPoint) > obstacleRange && Vector2.Distance(transform.position, lineOfSight.rightPoint) > obstacleRange
        if (Vector2.Distance(transform.position, lineOfSight.lookPoint) > obstacleRange && CanReachPoint(lineOfSight.lookPoint))
        {
            newDestination = lineOfSight.lookPoint;
        } else
        {
            if (leftTendency > maxLeftTendency / 2f)
            {
                transform.Rotate(0, 0, turnSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(0, 0, -turnSpeed * Time.deltaTime);
            }
            
        }
        return newDestination;
    }

    // helper version of below method
    public bool CanReachPoint(Vector2 point)
    {
        Vector2 origin = transform.position + 2f * transform.localScale.x * transform.right.normalized;
        return CanReachPoint(origin, point);
    }

    // uses Physics2D.CircleCast to check the path between origin and destination
    // return true if there are no colliders in the way.
    public bool CanReachPoint(Vector2 origin, Vector2 point)
    {
        RaycastHit2D pathCheck = Physics2D.CircleCast(origin, transform.localScale.x, transform.right.normalized, Vector2.Distance(origin, point) - transform.localScale.x);
        //Debug.DrawLine(origin,transform.right,Color.red,3f);
        //Debug.DrawLine(origin, lineOfSight.lookPoint, Color.magenta, 3f);
        //Debug.DrawRay(origin, transform.right, Color.green, 3f);
        if (pathCheck.collider == null)
        {
            return true;
        }
        return false;
    }

    /*public Vector2 FindNavigablePoint(Vector2 dir, float maxDist)
    {

        return Vector2.zero;
    }*/

    public void ResetDestination()
    {
        destinationInitialized = false;
    }
    public float GetCurSpeed()
    {
        return Vector2.Distance(transform.position, prevPos) / Time.deltaTime;
    }
}
