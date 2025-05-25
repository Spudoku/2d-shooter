using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// sergeants occasionally
public class SergeantGunman : DumbGunman
{
    
    [SerializeField] private float callDist = 10f;
    [SerializeField] private float callInterval = 5f;
    [SerializeField] private RandomSFX callSFX;
    private float callTimeTrack;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        callTimeTrack = Random.Range(0,callInterval);
        
    }

    // Update is called once per frame
    protected new void Update()
    {
        // Sergeants cannot follow other Sergeants.
        callTimeTrack += Time.deltaTime;
        if (callTimeTrack > callInterval)
        {
            CallAllies();
        }
        target = targetFinder.target;
        if (target == null)
        {
            WanderAI();
        }
        else
        {
            AttackAI();

        }

        CalcAllyMorale();
        CheckIfStuck();
    }

    // call all allies that are within line of sight.
    private void CallAllies()
    {
        callTimeTrack = 0;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, callDist);

        if (callSFX != null) {
            AudioSource.PlayClipAtPoint(callSFX.GetRandomClip(),transform.position);
        }
        foreach (var collider in colliders)
        {
            //Debug.Log($"{myName}: testing {collider}");
            if (!collider.gameObject.Equals(gameObject)  && collider.gameObject.TryGetComponent<DumbGunman>(out var dm) && dm.allyTags.Contains(gameObject.tag))
            {

                dm.SetFollowAlly(gameObject) ;
            }
        }
    }
}
