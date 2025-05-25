using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SniperAI : DumbGunman
{
    [SerializeField] private float snipeTurnSpeed;


    [SerializeField] private float fleeSpeed;            // how fast the Sniper runs while panicking

    [SerializeField] private float panicRadius;          // how close the nearest target must be before panicking
    [SerializeField] private Vector2 panicPoint;         // generated when 
    [SerializeField] private bool panicPointInit = false;


    //private float panicLookRot;
    new void Start()
    {
        base.Start();
        //panicLookRot = 0f;
    }
    new void Update()
    {
        target = targetFinder.target;
        if (target == null)
        {

            if (allyLeader != null && followAllyTimeTrack < followAllyTime)
            {
                //Debug.Log($"{myName}: Following {allyLeader}!");
                FollowAI();
            }
            else
            {
                //Debug.Log($"{myName}: wandering!");
                WanderAI();
            }
            if (followAllyTimeTrack > followAllyTime)
            {
                allyLeader = null;
            }
        }
        else
        {
            if (Vector2.Distance(target.transform.position, transform.position) > panicRadius)
            {
                ResetPanicState();
                AttackAI();
            }
            else
            {
                PanicAI();
            }

        }

        CalcAllyMorale();
        CheckIfStuck();

    }
    
private new void WanderAI()
    {
        base.WanderAI();
    }

    private new void AttackAI()
    {
        //Debug.Log(myName + ": I have a target!");
        bool seeCone = false;
        if (hm.lineOfSight.CanSeeGOInCone(target, fovCone, searchRange))
        {
            seeCone = true;
        }
        if (seeCone)
        {
            reactTimer += Time.deltaTime;
        }
        else
        {
            reactTimer += 0.5f * Time.deltaTime;
        }

        wanderTimer = 0;
        followAllyTimeTrack = 0;
        hm.destination = target.transform.position;

        if (reactTimer >= reactionTime)
        {
            hm.RotateTowardsPoint(hm.destination);
            reactTimer = reactionTime;
            if (seeCone)
            {
                AttackTarget();
            }

            /*if (curMorale >= moraleThreshold)
            {
                float dist = Vector2.Distance(transform.position, target.transform.position);
                if (dist > minDistToTarget)
                {
                    hm.MoveTowardsPoint(target.transform.position, speed);
                }

            }
            else
            {
                // back up if able, shoot
                hm.MoveTowardsPoint(target.transform.position, -speed * 0.6f);

            }*/

            // lose morale if hp is low
            if (body.hp < body.maxHP * 0.35f)
            {
                ChangeMorale(moraleLossRate * Time.deltaTime);
            }
        }
        else
        {
            hm.RotateTowardsPoint(hm.destination, turnSpeed * 0.35f);
        }
    }

    private void PanicAI()
    {
        if (!panicPointInit)
        {
            panicPoint = FindPanicPoint();
            if (Vector2.Distance(panicPoint,transform.position) > panicRadius)
            {
                panicPointInit = true;
            }
            
        }
        if (panicPointInit)
        {
            // run away without shooting
            hm.MoveTowardsPoint(panicPoint, fleeSpeed);
        }
        
        



        if (Vector2.Distance(transform.position,panicPoint) < obstacleRange)
        {
            // find new panicPoint if necessary
            panicPointInit = false;
        }
    }

    private void ResetPanicState()
    {
        panicPointInit = false;
    }

    private Vector2 FindPanicPoint()
    {

        // look for a point with the following conditions:
        // 1. must be visible
        // 2. must be at least panicRadius units away from current position
        // 3. must be at least panicRadius unita away from the target causing the panic

        //return transform.position;

        Vector2 findPoint = hm.FindWanderDestination();
        Debug.DrawLine(transform.position,findPoint,Color.red,5f);
        //Debug.DrawLine(target.transform.position,findPoint,Color.blue,5f);
        Debug.Log($"{myName}: panicAI: dot of findpoint-pos ({findPoint - (Vector2)transform.position}) and findpoint-target.pos ({findPoint - (Vector2)target.transform.position}) is: {Vector2.Dot((findPoint - (Vector2)transform.position).normalized,(findPoint - (Vector2)target.transform.position).normalized)}");
        float dot = Vector2.Dot((findPoint - (Vector2)transform.position).normalized, (findPoint - (Vector2)target.transform.position).normalized);
        if (hm.CanReachPoint(findPoint) && Vector2.Distance(transform.position,findPoint) > panicRadius && Vector2.Distance(target.transform.position,findPoint) > panicRadius && dot >= 0f)
        {
            return findPoint;
        } else
        {
            return transform.position;
        }
        
    }

}
