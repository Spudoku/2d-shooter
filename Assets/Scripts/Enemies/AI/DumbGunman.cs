using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

// designed for a gunman AI that simply runs and shoots towards the player.
// wanders aimlessly if the player is too far away
[RequireComponent(typeof(EnemyGeneric))]
[RequireComponent(typeof(TargetFinder))]
[RequireComponent(typeof(HumanoidMovement))]
[RequireComponent(typeof(NPCGeneric))]
public class DumbGunman : MonoBehaviour
{
    // npc fields 
    protected NPCGeneric npc;
    // weapons
    [SerializeField] protected MagBased primaryWeapon;

    //protected bool reloadingPrimary = false;
    [SerializeField] protected float reloadWaitTime = 5f;             // how long to wait to initialize reload (while wandering)

    // movement/targeting factors
    [SerializeField] protected float searchRange = 25f;               // radius to search in
    [SerializeField] protected float obstacleRange = 0.5f;
    [SerializeField] protected float minDistToTarget = 2f;              // dumb gunmen will not willingly get closer than this distance to their target.
    [SerializeField] protected float speed;
    [SerializeField] protected float turnSpeed;
    [SerializeField] protected string[] targetTags;                   // the tags to attack
    public string[] allyTags;                                       // the tags to protect/gather around
    protected TargetFinder targetFinder;
    [SerializeField] protected GameObject target;                                      // the current focus of this instances' ire
    [SerializeField] protected GameObject allyLeader;                       // the GO that this Dumb Gunman will follow, provided there is no hostile target
    [SerializeField] protected float followAllyTime = 15f;                   // how long this ally will follow another without 'getting bored'
    [SerializeField] protected float followAllyTimeTrack = 0;                          // how long this ally has followed another
    [SerializeField] protected float fovCone = 45f;                  // the cone in which enemies are detected and targeted.
    protected HumanoidMovement hm;
    [SerializeField] protected float reactionTime = 0.25f;            // delay between spotting an enemy and taking action
    [SerializeField] protected float reactTimer;
    
    
    [SerializeField] protected float timeSinceMoved;
    protected const float TIME_TO_FIND_WANDER_DESTINATION = 5f;       // if this DumbGunman is 'stuck', attempt to find a new wander destination
    protected const float MIN_SPEED_PERC = 0.25f;                       // minimum distance needed to move in order to be not considered 'stuck'
    //protected float actualMIN_SPEED_PERC;

    // 'fear'/morale factors
    [SerializeField] protected float curMorale;
    [SerializeField] protected float startMorale = 25f;           // initial morale
    [SerializeField] protected float maxMorale = 30f;             // maximum morale!
    [SerializeField] protected float moraleThreshold = 15f;       // the required amount
    [SerializeField] protected float courage = 5f;                    // how quickly this enemy can gain/lose morale
    protected float moraleLossRate;

    protected float wanderTimer;
    //protected bool isAttacking;

    // Sound effects
    [SerializeField] protected RandomSFX noticeEnemySFX;

    

    // personal factors
    protected EnemyGeneric body;                                      // EnemyGeneric holds things like health and armor; thus 'body'
    protected readonly string[] possibleTitles = {"Big","Stupid","Strong","Hairy","Sharpeye",
        "Smelly","Tough","Bearded","Jacked","Sergeant","One-eye", "Snotty","Pimply","Goated"};                                         
    protected readonly string[] possibleNames = {"Sarge","Jeff","Benjamin","Jaminben",
        "Gary","John","Bert","Solomon","Harry","Marv","Winston","Marcus","Mark",
        "Viggo","Cassian","Perry","Olson","Olly","Wilson","Tom","Bob","Jack","Sam","Billy", "John Doe"};
    public string myName;

    // Start is called before the first frame update
    protected void Start()
    {
        body = GetComponent<EnemyGeneric>();
        //actualMIN_SPEED_PERC = MIN_SPEED_PERC + transform.localScale.x;
        // init targetFinder
        targetFinder = GetComponent<TargetFinder>();
        targetFinder.searchRange = searchRange;
        targetFinder.targetTags = targetTags;
        targetFinder.allyTags = allyTags;
        // adjusting morale variables
        if (startMorale > maxMorale) {
            startMorale = maxMorale / 2;
        }
        if (moraleThreshold < 0 || moraleThreshold > maxMorale)
        {
            moraleThreshold = maxMorale / 2;
        }
        if (primaryWeapon == null)
        {
            Debug.Log("Warning: Dumb Gunman " + name + " has no weapons!");
        }
        if (courage > maxMorale)
        {
            courage = maxMorale / 2f;
        }
        curMorale = startMorale;

        moraleLossRate = (maxMorale - courage) / (-courage * 2f);                           
        // courage: 5, maxMorale: 30; mLR = -2.5
        // courage: 10, maxMorale: 50; mLR = -2
        wanderTimer = 0f;

        if (myName.Equals(string.Empty))
        {
            myName = GetFullName();
        }
        

        // making sure obstacleRange is outside of collider
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            obstacleRange += collider.size.x;
        } else
        {
            obstacleRange += transform.localScale.x;
        }
        minDistToTarget += obstacleRange;

        // initializing humanoidmovement component
        hm = GetComponent<HumanoidMovement>();
        hm.obstacleRange = obstacleRange;
        hm.maxSpeed = speed;
        hm.turnSpeed = turnSpeed;
        hm.destination = hm.FindWanderDestination();
        hm.fovCone = fovCone;

        npc = GetComponent<NPCGeneric>();
    }

    // Update is called once per frame
    protected void Update()
    {
        // weapon choices: equip primary or secondary weapon based on certain factors
        // including health, currently loaded ammo, carried ammo, and proximity of the player
        // also, morale, which is determined by the above and constant(s)

        // basic movement; move towards the player if this enemy can see them and confi.
        // otherwise, wander. 
        
        target = targetFinder.target;
        if (target == null)
        {
            if (allyLeader != null && followAllyTimeTrack < followAllyTime)
            {
                //Debug.Log($"{myName}: Following {allyLeader}!");
                FollowAI();
            } else
            {
                //Debug.Log($"{myName}: wandering!");
                WanderAI();
            }
            if (followAllyTimeTrack > followAllyTime)
            {
                allyLeader = null;
            }
        } else
        {
            AttackAI();
            
        }

        CalcAllyMorale();
        CheckIfStuck();
    }


    protected void WanderAI()
    {
        reactTimer = 0f;
        followAllyTimeTrack = 0;
        // wandering
        //Debug.Log(myName + ": Target is null!");
        // regain morale over time
        wanderTimer += Time.deltaTime;

        if (wanderTimer > 5f && !hm.CanReachPoint(hm.destination))
        {
            //Debug.Log($"{myName}: resetting wander destination to avoid getting stuck!");
            hm.ResetDestination();
        }

        if (body.timeSinceLastHit > DamageableObject.RECENT_HIT_TIME)
        {
            ChangeMorale(courage * Time.deltaTime);
        }
        else
        {
            // if damage is taken while no target is in sight/range, lose morale
            ChangeMorale(moraleLossRate * body.lastHitDmg * Time.deltaTime);
        }

        hm.lineOfSight.lookRange = searchRange;
        //hm.destination = hm.FindWanderDestination();
        hm.Wander();
        // reloading if wandering for long enough?
        if (primaryWeapon != null && !primaryWeapon.isReloading && wanderTimer >= reloadWaitTime && primaryWeapon.ammoLoaded < primaryWeapon.magSize)
        {
            StartCoroutine(ReloadWeapon(primaryWeapon, primaryWeapon.GetReloadTime()));

        }
    }

    protected void AttackAI()
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

            if (curMorale >= moraleThreshold)
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
                
            }

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

    // pre: allyLeader must not be null
    // post: follow allyLeader (but not attack)
    protected void FollowAI()
    {
        followAllyTimeTrack += Time.deltaTime;
        hm.destination = allyLeader.transform.position;
        float dist = Vector2.Distance(transform.position, allyLeader.transform.position);
        if (dist > minDistToTarget)
        {
            Debug.Log($"{myName}: moving to follow leader!");
            hm.MoveTowardsPoint(allyLeader.transform.position, speed);
        }
    }

    public void SetFollowAlly(GameObject leader)
    {
        if ( allyLeader == null)
        {
            allyLeader = leader;
            followAllyTimeTrack = 0f;
        }
        
    }

    // pre: target must be set, and a weapon must be equipped
    protected void AttackTarget()
    {
        if (primaryWeapon.ammoLoaded > 0)
        {
            //Debug.Log(myName + ": attacking " + target.name);
            //reloadingPrimary = false;
            primaryWeapon.Fire();
        } else if (primaryWeapon.ammoCarried > 0) 
        {
            if (!primaryWeapon.isReloading)
            {
                //Debug.Log(myName + ": reloading!");
                StartCoroutine(ReloadWeapon(primaryWeapon, primaryWeapon.GetReloadTime()));
                //reloadingPrimary = true;
            }
            
            
            
            // loses morale while attacking a target and reloading
            ChangeMorale(0.5f * moraleLossRate * Time.deltaTime);
        } else
        {
            // morale loss rate is doubled if no ammo is carried
            //Debug.Log(myName + ": Out of ammo! Cowering in fear!");
            ChangeMorale(moraleLossRate * Time.deltaTime);
        }

    }

    protected void CalcAllyMorale()
    {
        // increase morale
        int allyCount = npc.GetNPCsInRadius(searchRange / 2f, false, true);
        float moraleGain = Mathf.Min(courage, (courage * allyCount) / 3) * Time.deltaTime;
        //Debug.Log(myName + ": gaining " + moraleGain + " morale because of " + allyCount + " nearby allies!");
        ChangeMorale(moraleGain);
    }

    protected void CheckIfStuck()
    {
        // "stuck" logic
        // attempt to find a new destination if 'stuck'
        if (hm.curSpeed < speed * MIN_SPEED_PERC)
        {
            Debug.Log($"{myName} 'stuck logic': i'm not moving much!");
            timeSinceMoved += Time.deltaTime;
            if (timeSinceMoved > TIME_TO_FIND_WANDER_DESTINATION && target == null)
            {
                Debug.Log($"{myName} 'stuck logic': resetting my movement! (its been longer than {TIME_TO_FIND_WANDER_DESTINATION} seconds since I last moved)");

                hm.ResetDestination();
            }
        }
        else
        {
            timeSinceMoved = 0;
        }
    }

    protected void ChangeMorale(float amount)
    {
        curMorale += amount;
        if (curMorale > maxMorale)
        {
            curMorale = maxMorale;
        }
        else if (curMorale < 0)
        {
            curMorale = 0;
        }
    }

    public IEnumerator ReloadWeapon(MagBased weapon, float time)
    {
        weapon.ReloadInit();

        yield return new WaitForSeconds(time);

        weapon.Reload();
    }


    

    // get a random name
    protected string GetFullName()
    {
        return getTitle() + " " + getName();
    }

    protected string getTitle()
    {
        return randItem(possibleTitles);
    }

    protected string getName()
    {
        return randItem(possibleNames);
    }

    protected string randItem(string[] list)
    {
        return list[UnityEngine.Random.Range(0, list.Length)];
    }

    
}
